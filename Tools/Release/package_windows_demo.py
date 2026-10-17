"""Package an existing tested Windows build without changing its runtime files."""

import argparse
import hashlib
import json
from pathlib import Path
import re
import zipfile


ROOT = Path(__file__).resolve().parents[2]
REPOSITORY = "https://github.com/lorendw7/BalloonBarrage"


def sha256(data):
    return hashlib.sha256(data).hexdigest()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source", required=True, type=Path)
    parser.add_argument("--output", required=True, type=Path)
    parser.add_argument("--tag", required=True)
    args = parser.parse_args()
    if not re.fullmatch(r"v[0-9]+\.[0-9]+\.[0-9]+-demo\.[0-9]+", args.tag):
        parser.error("Use a versioned demo tag, for example v0.1.0-demo.1")
    source, output = args.source.resolve(), args.output.resolve()
    if output.is_relative_to(source):
        parser.error("The output must be outside the source build")
    if output.suffix.lower() != ".zip":
        parser.error("The output must be a ZIP")
    checksums = output.with_suffix(".sha256")
    if output.exists() or checksums.exists():
        parser.error("Refusing to overwrite an existing package or checksum")
    for required in ["BalloonShooter.exe", "UnityPlayer.dll", "BalloonShooter_Data/boot.config"]:
        if not (source / required).is_file():
            parser.error(f"Missing runtime file: {required}")

    files = {}
    for path in sorted(source.rglob("*")):
        if path.is_symlink():
            parser.error(f"Symbolic links are not supported: {path}")
        relative = path.relative_to(source)
        if any(part.endswith("_DoNotShip") for part in relative.parts):
            continue
        if path.is_file():
            files[relative.as_posix()] = path.read_bytes()
    runtime_manifest = [dict(path=name, bytes=len(data), sha256=sha256(data))
                        for name, data in files.items()]

    def source_link(match):
        label, target = match.groups()
        if target.startswith(("https://", "http://", "#")):
            return match.group(0)
        return f"[{label}]({REPOSITORY}/blob/{args.tag}/{target})"

    credits = re.sub(r"\[([^\]]+)\]\(([^)]+)\)", source_link,
                     (ROOT / "CREDITS.md").read_text(encoding="utf-8"))
    extra = {
        "README.txt": f"""BalloonBarrage {args.tag} - Windows x64 solo development demo

Extract the entire ZIP to a folder, then run BalloonShooter.exe.
Keep BalloonShooter_Data, UnityPlayer.dll and all accompanying runtime files.
Unity and an internet connection are not needed for gameplay.

Mouse: aim. Hold left mouse button: fire. Esc: pause or resume.
Start: begin the song. Resume: continue. Play Again: return to title.
Quit: close the game. Release fire after using menus or resuming.
Goin' Under: about 123.429 seconds, 93 target events.
Ordinary hit: 10 points. On-beat shot hitting its matching target: 20 points.

The creator reports Windows start, shooting, pause/resume, whole-song results,
retry and Quit passed on October 17, 2026. This is creator-reported acceptance,
not an independently operated physical-input test on every machine.

Known limits: keyboard/mouse solo only; English game UI; no song selection,
arbitrary-surface painting or co-op. Sustained 2K/120 FPS, ten retries and a
30-minute exhibition-PC soak are not accepted. This tested build is a
development snapshot, not the final non-development exhibition candidate.

Noncommercial development demo: bundled StepMania music is not commercially
cleared. Do not sell this package. Individual assets retain their own licenses.
Read CREDITS.md, ThirdPartyNotices, and
BalloonShooter_Data/StreamingAssets/Rhythm/StepMania/NOTICE.txt.

Source, exact recorded art prompts and known issues:
{REPOSITORY}/tree/{args.tag}
Feedback: {REPOSITORY}/issues
RuntimeFiles.json records SHA-256 for every unchanged original runtime file.
""".encode(),
        "CREDITS.md": credits.encode(),
        "RuntimeFiles.json": (json.dumps(runtime_manifest, indent=2) + "\n").encode(),
    }
    for name, path in {
        "CC-BY-3.0.txt": ROOT / "Tools/Release/Notices/CC-BY-3.0.txt",
        "CC0-1.0.txt": ROOT / "Tools/Release/Notices/CC0-1.0.txt",
        "Lato-OFL.txt": ROOT / "Assets/StudioArtPack/InterfaceV2/Fonts/OFL.txt",
        "UnityCompanionLicense.md": ROOT / "Assets/StudioArtPack/ConceptVariantsV12/Shaders/UnityCompanionLicense.md",
    }.items():
        extra[f"ThirdPartyNotices/{name}"] = path.read_bytes()
    if files.keys() & extra.keys():
        parser.error("A package notice would overwrite a source runtime file")
    files.update(extra)
    output.parent.mkdir(parents=True, exist_ok=True)
    with zipfile.ZipFile(output, "x", compression=zipfile.ZIP_DEFLATED, compresslevel=9) as archive:
        for name, data in files.items():
            entry = zipfile.ZipInfo(name, date_time=(2026, 10, 17, 0, 0, 0))
            entry.compress_type = zipfile.ZIP_DEFLATED
            archive.writestr(entry, data, compresslevel=9)
    with zipfile.ZipFile(output) as archive:
        assert archive.testzip() is None, "ZIP CRC failure"
        assert len(archive.namelist()) == len(files), "ZIP file count mismatch"
        for name, data in files.items():
            assert sha256(archive.read(name)) == sha256(data), name
    # Detect concurrent build changes rather than publishing a mixed checkpoint.
    for item in runtime_manifest:
        assert sha256((source / item["path"]).read_bytes()) == item["sha256"], item["path"]
    digest = sha256(output.read_bytes())
    checksums.write_text(f"{digest}  {output.name}\n", encoding="utf-8")
    print(json.dumps(dict(archive=str(output), bytes=output.stat().st_size,
                          sha256=digest, runtimeFiles=len(runtime_manifest),
                          files=len(files)), indent=2))


if __name__ == "__main__":
    main()
