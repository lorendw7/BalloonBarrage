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
RESULTS_CAPTURE = "Website/media/results-statistics-20261023.png"
RESULTS_ATTRIBUTION = "Creator-supplied Unity 2K Game-view result screenshot, received 2026-10-23. Shows the saved statistics layout with illustrative play values; not a standalone-package capture. Web derivative crops only the Unity editor chrome. No generative editing."
SOURCES = {
    "gameplay-development-20261019": DEVELOPMENT_CAPTURE,
    "results-statistics-20261023": RESULTS_CAPTURE,
    "palette": "Assets/Art/Previews/Palette_20261019.png",
    "menu-background": "Assets/StudioArtPack/Menu/MenuBackground_v1.png",
    "gameplay-demo2": CAPTURE,
    "studio-concept": "Assets/Art/Concepts/CozyGraffitiStudio_Concept.png",
    "paint-blaster-concept": "Assets/StudioArtPack/PaintBlaster/Concepts/PaintBlaster_Concept_v1.png",
    "balloon-concept": "Assets/Art/Concepts/Balloon_Readability_v2.png",
    "surface-patterns-20261019": "Assets/Art/Previews/SurfacePatterns_20261019.png",
    "paint-blaster-model-20261019": "Assets/StudioArtPack/PaintBlaster/ConceptRefinement20261019/Previews/After_Model.png",
    "start-screen-20261019": "Assets/StudioArtPack/InterfaceV2/Previews/Start_AuthorCredit_1080p.png",
    "start-localized-zh-hans-20261025": "Assets/StudioArtPack/InterfaceV2/Previews/Start_Localized_ZhHans_1440p.png",
    "studio-refresh-20261020": "Assets/StudioArtPack/PaintBlaster/MixedPigment20261020/Previews/After.png",
    "mixed-pigment-model-20261020": "Assets/StudioArtPack/PaintBlaster/MixedPigment20261020/Previews/After_Model.png",
    "paint-spray-vfx-20261025": "Assets/StudioArtPack/PaintSprayStyle20261025/Textures/PaintSprayVfx_Atlas_v3.png",
    "surface-paint-coverage-20261027": "Assets/StudioArtPack/SurfacePaintCoverage20261027/Previews/DenseCoverage_1000Marks.png",
}

PREVIEW_ATTRIBUTIONS = {
    "studio-refresh-20261020": "Native Unity saved-camera scene inspection, 2026-10-20. Plaster walls, mineral floor, side-wall pigments and mixed-color reservoir. UI temporarily hidden; not recorded gameplay or a new public release. No generative image editing.",
    "mixed-pigment-model-20261020": "Native Unity enlarged model inspection, 2026-10-20. Static eight-hue marbled pigment and reservoir calibration marks. Temporary inspection pose, UI hidden; not fluid simulation or a preview of the next projectile color. No generative image editing.",
    "surface-patterns-20261019": "Native Unity asset-sheet preview, 2026-10-19. Eight colors and six patterns; not recorded gameplay or a public release. Existing atlas attribution and recorded generation prompts remain unchanged.",
    "paint-blaster-model-20261019": "Native Unity model-inspection render, 2026-10-19. Temporary enlarged inspection pose, Canvas hidden; not recorded gameplay. Native procedural geometry, no new generative image editing.",
    "start-screen-20261019": "Native Unity interface preview, 2026-10-19. Refined creator nameplate; not a screenshot of the released demo. Original menu-art attribution and prompts remain unchanged.",
    "start-localized-zh-hans-20261025": "Native Unity 2560x1440 Simplified Chinese start-screen render, 2026-10-25. Captured by the project localization preview tool after the fixed Medium font and contrast-color pass; not a standalone-package capture. Original menu-art attribution and prompts remain unchanged. No generative editing.",
    "paint-spray-vfx-20261025": "Original transparent 2-by-2 paint-VFX candidate generated with OpenAI built-in image generation on 2026-10-25. Neutral white artwork is prepared for runtime tinting and is not connected gameplay. Exact prompt, source path and source hash are retained in the canonical asset folder.",
    "surface-paint-coverage-20261027": "Native Unity 6000.0.77f1 automated coverage fixture, 2026-10-27. Nine prepared static surfaces, eight pigments and 1,000 retained marks; not physical gameplay or standalone-package acceptance. The persistent marks use original generated artwork whose exact prompt and source hash remain in the canonical asset folder. No generative editing was applied to this capture.",
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
        preserve_alpha = name == "paint-spray-vfx-20261025"
        image = Image.open(source).convert("RGBA" if preserve_alpha else "RGB")
        if name == "results-statistics-20261023":
            image = image.crop((0, 68, image.width, image.height - 34))
        image.thumbnail(WEB_MAX_SIZE, Image.Resampling.LANCZOS, reducing_gap=3.0)
        output = MEDIA / f"{name}.webp"
        image.save(output, "WEBP", quality=WEBP_QUALITY, method=6)
        records.append({"source": relative, "source_sha256": digest(source),
                        "output": output.relative_to(ROOT).as_posix(),
                        "output_sha256": digest(output), "dimensions": list(image.size),
                        "preparation": (
                            "Crop Unity editor chrome (top 68 px, bottom 34 px); RGB, preserve source resolution up to 2560x1440; WebP quality 92, method 6"
                            if name == "results-statistics-20261023"
                            else (
                                "RGBA with preserved transparency, preserve source resolution up to 2560x1440, Lanczos when downscaling; WebP quality 92, method 6"
                                if preserve_alpha
                                else "RGB, preserve source resolution up to 2560x1440, Lanczos when downscaling; WebP quality 92, method 6"
                            )
                        )})
        if name in PREVIEW_ATTRIBUTIONS:
            records[-1]["attribution"] = PREVIEW_ATTRIBUTIONS[name]
        elif relative == DEVELOPMENT_CAPTURE:
            records[-1]["attribution"] = DEVELOPMENT_ATTRIBUTION
        elif relative == RESULTS_CAPTURE:
            records[-1]["attribution"] = RESULTS_ATTRIBUTION
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
