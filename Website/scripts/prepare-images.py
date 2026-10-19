"""Prepare web derivatives without altering canonical Unity captures or artwork.

Run manually with Python and Pillow after changing a source image. CI uses the
committed derivatives, so the website build remains dependency-free.
"""

import hashlib
import json
from pathlib import Path

from PIL import Image

ROOT = Path(__file__).resolve().parents[2]
MEDIA = ROOT / "Website/media"
CAPTURE = "Assets/Art/Previews/Gameplay_v0.1.0-demo.2_20261018.jpg"
SOURCES = {
    "palette": "Assets/Art/Previews/Palette_20261019.png",
    "menu-background": "Assets/StudioArtPack/Menu/MenuBackground_v1.png",
    "gameplay-demo2": CAPTURE,
    "studio-concept": "Assets/Art/Concepts/CozyGraffitiStudio_Concept.png",
    "paint-blaster-concept": "Assets/StudioArtPack/PaintBlaster/Concepts/PaintBlaster_Concept_v1.png",
    "balloon-concept": "Assets/Art/Concepts/Balloon_Readability_v2.png",
}


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    MEDIA.mkdir(exist_ok=True)
    records = []
    for name, relative in SOURCES.items():
        source = ROOT / relative
        image = Image.open(source).convert("RGB")
        image.thumbnail((1600, 1200), Image.Resampling.LANCZOS)
        output = MEDIA / f"{name}.webp"
        image.save(output, "WEBP", quality=85, method=6)
        records.append({"source": relative, "source_sha256": digest(source),
                        "output": output.relative_to(ROOT).as_posix(),
                        "output_sha256": digest(output), "dimensions": list(image.size),
                        "preparation": "RGB, fit within 1600x1200, Lanczos; WebP quality 85, method 6"})
    image = Image.open(ROOT / CAPTURE).convert("RGB")
    image = image.resize((1200, 675), Image.Resampling.LANCZOS)
    output = MEDIA / "gameplay-share.jpg"
    image.save(output, "JPEG", quality=88, optimize=True)
    records.append({"source": CAPTURE, "source_sha256": digest(ROOT / CAPTURE),
                    "output": output.relative_to(ROOT).as_posix(),
                    "output_sha256": digest(output), "dimensions": list(image.size),
                    "preparation": "1200x675 Lanczos; RGB JPEG quality 88, optimized"})
    (MEDIA / "provenance.json").write_text(json.dumps(records, indent=2) + "\n", encoding="utf-8")
    for record in records:
        print(record["output"], (ROOT / record["output"]).stat().st_size)


if __name__ == "__main__":
    main()
