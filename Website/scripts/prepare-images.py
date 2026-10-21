"""Prepare web derivatives without altering canonical Unity captures or artwork.

Run manually with Python and Pillow after changing a source image. CI uses the
committed derivatives, so the website build remains dependency-free.
"""

import hashlib
import argparse
import json
from pathlib import Path

from PIL import Image, ImageOps

ROOT = Path(__file__).resolve().parents[2]
MEDIA = ROOT / "Website/media"
WEB_MAX_SIZE = (2560, 1440)
WEBP_QUALITY = 92
CAPTURE = "Assets/Art/Previews/Gameplay_v0.1.0-demo.2_20261018.jpg"
DEVELOPMENT_CAPTURE = "Assets/Art/Previews/Gameplay_Development_20261019.png"
DEVELOPMENT_ATTRIBUTION = "Creator-supplied development gameplay screenshot, received 2026-10-19; exact build and capture environment unverified; not evidence of a new public release. No generative editing."
SOURCES = {
    "gameplay-development-20261019": DEVELOPMENT_CAPTURE,
    "palette": "Assets/Art/Previews/Palette_20261019.png",
    "menu-background": "Assets/StudioArtPack/Menu/MenuBackground_v1.png",
    "gameplay-demo2": CAPTURE,
    "studio-concept": "Assets/Art/Concepts/CozyGraffitiStudio_Concept.png",
    "paint-blaster-concept": "Assets/StudioArtPack/PaintBlaster/Concepts/PaintBlaster_Concept_v1.png",
    "balloon-concept": "Assets/Art/Concepts/Balloon_Readability_v2.png",
    "surface-patterns-20261019": "Assets/Art/Previews/SurfacePatterns_20261019.png",
    "paint-blaster-model-20261019": "Assets/StudioArtPack/PaintBlaster/ConceptRefinement20261019/Previews/After_Model.png",
    "start-screen-20261019": "Assets/StudioArtPack/InterfaceV2/Previews/Start_AuthorCredit_1080p.png",
    "studio-refresh-20261020": "Assets/StudioArtPack/PaintBlaster/MixedPigment20261020/Previews/After.png",
    "mixed-pigment-model-20261020": "Assets/StudioArtPack/PaintBlaster/MixedPigment20261020/Previews/After_Model.png",
}

PREVIEW_ATTRIBUTIONS = {
    "studio-refresh-20261020": "Native Unity saved-camera scene inspection, 2026-10-20. Plaster walls, mineral floor, side-wall pigments and mixed-color reservoir. UI temporarily hidden; not recorded gameplay or a new public release. No generative image editing.",
    "mixed-pigment-model-20261020": "Native Unity enlarged model inspection, 2026-10-20. Static eight-hue marbled pigment and reservoir calibration marks. Temporary inspection pose, UI hidden; not fluid simulation or a preview of the next projectile color. No generative image editing.",
    "surface-patterns-20261019": "Native Unity asset-sheet preview, 2026-10-19. Eight colors and six patterns; not recorded gameplay or a public release. Existing atlas attribution and recorded generation prompts remain unchanged.",
    "paint-blaster-model-20261019": "Native Unity model-inspection render, 2026-10-19. Temporary enlarged inspection pose, Canvas hidden; not recorded gameplay. Native procedural geometry, no new generative image editing.",
    "start-screen-20261019": "Native Unity interface preview, 2026-10-19. Refined creator nameplate; not a screenshot of the released demo. Original menu-art attribution and prompts remain unchanged.",
}


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--only", nargs="+", choices=sorted(SOURCES), help="Update selected WebP derivatives and their provenance, preserving other media bytes.")
    selected = parser.parse_args().only
    MEDIA.mkdir(exist_ok=True)
    records = []
    for name, relative in SOURCES.items():
        if selected and name not in selected:
            continue
        source = ROOT / relative
        image = Image.open(source).convert("RGB")
        image.thumbnail(WEB_MAX_SIZE, Image.Resampling.LANCZOS, reducing_gap=3.0)
        output = MEDIA / f"{name}.webp"
        image.save(output, "WEBP", quality=WEBP_QUALITY, method=6)
        records.append({"source": relative, "source_sha256": digest(source),
                        "output": output.relative_to(ROOT).as_posix(),
                        "output_sha256": digest(output), "dimensions": list(image.size),
                        "preparation": "RGB, preserve source resolution up to 2560x1440, Lanczos when downscaling; WebP quality 92, method 6"})
        if name in PREVIEW_ATTRIBUTIONS:
            records[-1]["attribution"] = PREVIEW_ATTRIBUTIONS[name]
        elif relative == DEVELOPMENT_CAPTURE:
            records[-1]["attribution"] = DEVELOPMENT_ATTRIBUTION
    if selected:
        provenance = MEDIA / "provenance.json"
        previous = json.loads(provenance.read_text(encoding="utf-8")) if provenance.exists() else []
        outputs = {record["output"] for record in records}
        previous = [record for record in previous if record["output"] not in outputs]
        provenance.write_text(json.dumps(previous + records, indent=2) + "\n", encoding="utf-8")
        for record in records:
            print(record["output"], (ROOT / record["output"]).stat().st_size)
        return
    image = Image.open(ROOT / CAPTURE).convert("RGB")
    image = image.resize((1200, 675), Image.Resampling.LANCZOS)
    output = MEDIA / "gameplay-share.jpg"
    image.save(output, "JPEG", quality=88, optimize=True)
    records.append({"source": CAPTURE, "source_sha256": digest(ROOT / CAPTURE),
                    "output": output.relative_to(ROOT).as_posix(),
                    "output_sha256": digest(output), "dimensions": list(image.size),
                    "preparation": "1200x675 Lanczos; RGB JPEG quality 88, optimized"})
    image = ImageOps.pad(Image.open(ROOT / DEVELOPMENT_CAPTURE).convert("RGB"),
                         (1200, 675), method=Image.Resampling.LANCZOS, color="#fff4dc")
    output = MEDIA / "gameplay-development-share-20261019.jpg"
    image.save(output, "JPEG", quality=88, optimize=True)
    records.append({"source": DEVELOPMENT_CAPTURE,
                    "source_sha256": digest(ROOT / DEVELOPMENT_CAPTURE),
                    "output": output.relative_to(ROOT).as_posix(),
                    "output_sha256": digest(output), "dimensions": list(image.size),
                    "preparation": "Fit entire capture within 1200x675 with centered cream padding, no crop or distortion; Lanczos; RGB JPEG quality 88, optimized",
                    "attribution": DEVELOPMENT_ATTRIBUTION})
    (MEDIA / "provenance.json").write_text(json.dumps(records, indent=2) + "\n", encoding="utf-8")
    for record in records:
        print(record["output"], (ROOT / record["output"]).stat().st_size)


if __name__ == "__main__":
    main()
