"""Transfer exact release ZIPs by reusing unchanged compressed data from a prior ZIP."""

import argparse
import hashlib
import json
from pathlib import Path
import struct
import zipfile


def digest(path):
    with path.open("rb") as stream:
        return hashlib.file_digest(stream, "sha256").hexdigest()


def data_offset(payload, entry):
    offset = entry.header_offset
    if payload[offset:offset + 4] != b"PK\x03\x04":
        raise ValueError("Invalid ZIP local header")
    name_size, extra_size = struct.unpack_from("<HH", payload, offset + 26)
    return offset + 30 + name_size + extra_size


def create(base, target, output):
    old, new = base.read_bytes(), target.read_bytes()
    literals, segments = bytearray(), []

    def literal(data):
        if data:
            segments.append({"literal": [len(literals), len(data)]})
            literals.extend(data)

    with zipfile.ZipFile(base) as before, zipfile.ZipFile(target) as after:
        entries = {entry.filename: entry for entry in before.infolist()}
        cursor = 0
        for entry in sorted(after.infolist(), key=lambda item: item.header_offset):
            offset = data_offset(new, entry)
            literal(new[cursor:offset])
            end = offset + entry.compress_size
            previous = entries.get(entry.filename)
            previous_offset = data_offset(old, previous) if previous else 0
            if previous and previous.compress_size == entry.compress_size and (
                old[previous_offset:previous_offset + entry.compress_size] == new[offset:end]
            ):
                segments.append({"base": [previous_offset, entry.compress_size]})
            else:
                literal(new[offset:end])
            cursor = end
        literal(new[cursor:])
    recipe = {"format": "BalloonBarrage.ZipTransportDelta.v1", "baseSha256": digest(base),
              "targetSha256": digest(target), "targetBytes": len(new), "segments": segments}
    with zipfile.ZipFile(output, "x", zipfile.ZIP_DEFLATED, compresslevel=9) as archive:
        archive.writestr("recipe.json", json.dumps(recipe, separators=(",", ":")))
        archive.writestr("literals.bin", literals)


def restore(base, patch, output):
    with zipfile.ZipFile(patch) as archive:
        recipe = json.loads(archive.read("recipe.json"))
        literals = archive.read("literals.bin")
    if recipe["format"] != "BalloonBarrage.ZipTransportDelta.v1" or digest(base) != recipe["baseSha256"]:
        raise ValueError("Wrong delta format or base ZIP checksum")
    expected = recipe["targetBytes"]
    if not isinstance(expected, int) or not 0 < expected < 2 ** 31:
        raise ValueError("Invalid target size")
    total, result = 0, hashlib.sha256()
    with base.open("rb") as source, output.open("xb") as destination:
        for segment in recipe["segments"]:
            if len(segment) != 1 or next(iter(segment)) not in ("base", "literal"):
                raise ValueError("Invalid segment")
            kind = next(iter(segment))
            offset, size = segment[kind]
            limit = base.stat().st_size if kind == "base" else len(literals)
            if not all(isinstance(n, int) and n >= 0 for n in (offset, size)) or offset + size > limit:
                raise ValueError("Invalid segment bounds")
            if total + size > expected:
                raise ValueError("Delta exceeds target size")
            if kind == "base":
                source.seek(offset)
            remaining = size
            while remaining:
                count = min(1024 * 1024, remaining)
                data = source.read(count) if kind == "base" else literals[offset:offset + count]
                if len(data) != count:
                    raise ValueError("Truncated delta segment")
                destination.write(data)
                result.update(data)
                offset += count
                remaining -= count
            total += size
    if total != expected or result.hexdigest() != recipe["targetSha256"]:
        raise ValueError("Reconstructed ZIP checksum mismatch; do not upload")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("operation", choices=("create", "restore"))
    parser.add_argument("base", type=Path)
    parser.add_argument("input", type=Path, help="New ZIP for create; transport delta for restore")
    parser.add_argument("output", type=Path, help="New file; existing outputs are refused")
    args = parser.parse_args()
    (create if args.operation == "create" else restore)(args.base, args.input, args.output)
    print(f"{args.output.name}: {args.output.stat().st_size} bytes; SHA-256 {digest(args.output)}")
