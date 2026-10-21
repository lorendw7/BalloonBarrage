"""Package an existing desktop build without changing its runtime files."""

from __future__ import annotations

import argparse
import hashlib
import json
import os
from pathlib import Path
import stat
import zipfile


ROOT = Path(__file__).resolve().parents[2]


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def archive_name(path: Path, source: Path) -> str:
    return path.relative_to(source).as_posix()


def write_file(archive: zipfile.ZipFile, path: Path, name: str) -> None:
    info = zipfile.ZipInfo.from_file(path, name)
    if os.name != "nt" and path.stat().st_mode & stat.S_IXUSR:
        info.external_attr = (path.stat().st_mode & 0xFFFF) << 16
    with path.open("rb") as handle:
        archive.writestr(info, handle.read(), compress_type=zipfile.ZIP_DEFLATED, compresslevel=9)


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--source", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--tag", required=True)
    parser.add_argument("--platform", choices=("windows", "macos"), required=True)
    parser.add_argument("--release-info", type=Path)
    args = parser.parse_args()

    source = args.source.resolve()
    output = args.output.resolve()
    if not source.is_dir():
        parser.error(f"Build directory does not exist: {source}")
    if output.exists() or output.with_suffix(output.suffix + ".sha256").exists():
        parser.error("Refusing to overwrite an existing package or checksum")

    if args.platform == "windows":
        required = [source / "BalloonShooter.exe", source / "BalloonShooter_Data", source / "UnityPlayer.dll"]
        launch = "Extract the complete ZIP and run BalloonShooter.exe."
        limitations = "Windows x64 keyboard-and-mouse build."
    else:
        required = [source / "BalloonBarrage.app", source / "BalloonBarrage.app/Contents/MacOS/BalloonBarrage"]
        launch = "Extract the complete ZIP, then open BalloonBarrage.app."
        limitations = ("Unsigned and not notarized. macOS may require Control-click > Open. "
                       "This candidate still requires testing on Intel and Apple-silicon Macs.")
    missing = [str(path) for path in required if not path.exists()]
    if missing:
        parser.error("Missing runtime paths: " + ", ".join(missing))

    info = {}
    if args.release_info:
        info = json.loads(args.release_info.read_text(encoding="utf-8-sig"))

    readme = f"""BalloonBarrage {args.tag} - {args.platform} noncommercial development demo

{launch}
Mouse: aim. Hold left mouse button: fire. Esc: pause or resume.

{limitations}
{info.get('verification', 'This package has structural checks only; physical playtesting is not recorded.')}

Noncommercial development demo: bundled StepMania music is not commercially cleared.
Do not sell this package. Read CREDITS.md and the bundled StepMania NOTICE.txt.
"""

    runtime_files = []
    files = sorted(
        path
        for path in source.rglob("*")
        if path.is_file()
        and not any(
            "DoNotShip" in part or "BackUpThisFolder" in part
            for part in path.relative_to(source).parts
        )
    )
    for path in files:
        runtime_files.append({"path": archive_name(path, source), "bytes": path.stat().st_size, "sha256": sha256(path)})

    output.parent.mkdir(parents=True, exist_ok=True)
    with zipfile.ZipFile(output, "x", allowZip64=True) as archive:
        for path in files:
            write_file(archive, path, archive_name(path, source))
        archive.writestr("README.txt", readme)
        archive.write(ROOT / "CREDITS.md", "CREDITS.md", zipfile.ZIP_DEFLATED)
        for notice in ("CC-BY-3.0.txt", "CC0-1.0.txt"):
            archive.write(ROOT / "Tools/Release/Notices" / notice, f"ThirdPartyNotices/{notice}", zipfile.ZIP_DEFLATED)
        archive.writestr("RuntimeFiles.json", json.dumps(runtime_files, ensure_ascii=False, indent=2) + "\n")

    digest = sha256(output)
    checksum = output.with_suffix(output.suffix + ".sha256")
    checksum.write_text(f"{digest}  {output.name}\n", encoding="ascii")
    print(f"Packaged {len(files)} runtime files: {output}")
    print(f"SHA-256: {digest}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
