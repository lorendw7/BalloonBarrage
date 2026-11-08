"""Publish matching native scene, source-license, flower and grass evidence.

Run only after all Unity manifest sets are final. This verifies input hashes
before replacing the website snapshot; it does not author or edit Unity assets.
"""

import hashlib
import json
import math
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
MEDIA = ROOT / "Website/media"
ART = ROOT / "Assets/Art/OpenWorld"


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def read_json(path):
    return json.loads(path.read_text(encoding="utf-8-sig"))


def project_file(relative):
    path = (ROOT / relative).resolve()
    if ROOT.resolve() not in path.parents or not path.is_file():
        raise ValueError(f"Missing or unsafe project source: {relative}")
    return path


def verified_sources(folder):
    record = read_json(folder / "source-manifest.json")
    for item in record["files"]:
        path = (folder / item["path"]).resolve()
        if folder.resolve() not in path.parents or digest(path) != item["sha256"]:
            raise ValueError(f"Raw source mismatch: {item['path']}")
    return record


def finite_number(record, key):
    value = record[key]
    if not isinstance(value, (int, float)) or not math.isfinite(value) or value < 0:
        raise ValueError(f"Invalid native measurement: {key}")
    return value


def main():
    scene = read_json(ART / "SunnyExpansion20261108/Manifest.json")
    detail_path = ART / "SunnyDetail20261108/Manifest.json"
    detail = read_json(detail_path)
    flower_path = ART / "BlossomPrepared/Manifest.json"
    flowers = read_json(flower_path)
    grass_path = ART / "SunnyGrass20261108/Manifest.json"
    grass = read_json(grass_path)
    scene_sha = digest(project_file(scene["scene"]))
    if any(scene_sha != record["sceneSha256"] for record in [scene,detail,grass]):
        raise ValueError("Scene, detail and grass records must identify the same saved scene")
    source_one = verified_sources(ART / "ThirdParty/KenneyNatureExpansion")
    source_two = verified_sources(ART / "ThirdParty/KenneyCoastalDetails")
    grass_sources = verified_sources(ART / "ThirdParty/LushGrass20261108")
    if digest(project_file(grass["source"])) != grass["sourceSha256"]:
        raise ValueError("Grass geometry source does not match its recorded recipe")
    if digest(project_file(grass["sourceManifest"])) != grass["sourceManifestSha256"]:
        raise ValueError("Grass surface sources do not match the prepared surface record")
    source_models = [[item["path"] for item in source["files"] if item["path"].endswith(".obj")]
                     for source in [source_one, source_two]]
    if len(set(source_models[0] + source_models[1])) != sum(map(len, source_models)):
        raise ValueError("CC0 subsets contain duplicate model names")
    flower_names = [flower["name"] for flower in flowers["flowers"]]
    if len(flower_names) != len(set(flower_names)) or not flower_names:
        raise ValueError("Original flower inventory is empty or duplicated")
    for flower in flowers["flowers"]:
        for key in ["mesh", "prefab"]:
            if digest(project_file(flower[key])) != flower[key + "Sha256"]:
                raise ValueError(f"Original flower {key} mismatch: {flower['name']}")
    captures = scene["previews"] + [capture for capture in detail["previews"]
                                  if Path(capture["path"]).name == "Flowers.png"]
    captures += [capture for capture in grass["previews"] if Path(capture["path"]).name == "Grass.png"]
    if len(captures) != 9 or len({capture["path"] for capture in captures}) != 9:
        raise ValueError("Expected seven complete scene captures and flower/grass close-ups")
    for capture in captures:
        if digest(project_file(capture["path"])) != capture["sha256"]:
            raise ValueError(f"Native capture hash mismatch: {capture['path']}")
    verification = (ART / "SunnyExpansion20261108/Verification.txt").read_text(encoding="utf-8-sig")
    original_samples = re.search(r"original ([\d,]+) support/clearance samples", verification)
    if not original_samples:
        raise ValueError("Original-route probe evidence is missing")
    original_area = finite_number(scene, "originalLandSquareMetres")
    added_area = finite_number(scene, "addedLandSquareMetres")
    record = {
        "schema": "balloonbarrage.website.sunny-coast.v3",
        "date": scene["created"], "timezone": "Asia/Tokyo",
        "evidence_scope": "Native Unity static scene preparation. Not gameplay, human playtest, performance acceptance, or an open-world release. Current downloads remain the studio demo.",
        "scene": {
            "name": "OpenWorldSunnyExpansion", "sha256": scene_sha,
            "unity_version": scene["unityVersion"],
            "measurements": {
                "original_land_m2": original_area, "added_land_m2": added_area,
                "total_land_m2": original_area + added_area,
                "total_land_ratio": finite_number(scene, "totalLandRatio"),
                "added_route_metres": finite_number(scene, "addedRouteMetres"),
            },
            "support_probes": finite_number(scene, "routeSupportSamples"),
            "clearance_probes": finite_number(scene, "routeClearanceSamples"),
            "original_route_probes_each": int(original_samples[1].replace(",", "")),
        },
        "captures": captures,
        "new_cc0_sources": source_one,
        "coastal_detail_sources": source_two,
        "grass_surface_sources": grass_sources,
        "asset_counts": {
            "first_expansion_cc0_models": len(source_models[0]),
            "coastal_detail_cc0_models": len(source_models[1]),
            "combined_expansion_cc0_models": sum(map(len, source_models)),
            "original_flower_models": len(flower_names),
            "scope": "Distinct source models in these two expansion subsets; excludes earlier baseline assets and scene placement counts.",
        },
        "original_flower_models": {
            "count": len(flower_names), "names": flower_names,
            "authoring": "Original procedural 3D Unity mesh geometry authored for this project.",
            "scope": "Native deterministic geometry, not raster image generation; static decorative assets, not growth or gardening gameplay. Project-authored licensing is separate from CC0 imports.",
            "source_manifest_sha256": digest(flower_path), "source_manifest": flowers,
        },
        "refinement": {
            "scene_sha256": detail["sceneSha256"],
            "source_manifest_sha256": digest(detail_path), "source_manifest": detail,
        },
        "grass_refinement": {
            "scene_sha256": grass["sceneSha256"],
            "source_manifest_sha256": digest(grass_path), "source_manifest": grass,
            "original_geometry": grass["originalGeometry"],
            "creative_specification": grass["creativeSpecification"],
            "evidence_scope": "Photographic-style PBR ground plus original native 3D grass blades. Provider describes Procedural with Bitmap Elements; not a photogrammetry claim. Static appearance and spatial LOD inventories are not runtime performance acceptance.",
        },
        "license_note": "The CC0 art sources do not change existing restrictions on the mixed music development bundle. Project-relative source paths identify local evidence and are not public download links. Website/media/provenance.json separately records WebP/share preparation.",
    }
    license_bytes = (ART / "ThirdParty/KenneyNatureExpansion/License.txt").read_bytes()
    if license_bytes != (ART / "ThirdParty/KenneyCoastalDetails/License.txt").read_bytes():
        raise ValueError("CC0 subsets do not carry the same original license")
    # All inputs are verified before either published artifact is replaced.
    MEDIA.mkdir(exist_ok=True)
    (MEDIA / "sunny-provenance.json").write_text(json.dumps(record, indent=2) + "\n", encoding="utf-8")
    (MEDIA / "kenney-nature-license.txt").write_bytes(license_bytes)
    (MEDIA / "ambientcg-grass-CC0.txt").write_bytes((ART / "ThirdParty/LushGrass20261108/CC0-1.0.txt").read_bytes())
    print(f"Verified {len(captures)} captures, {sum(map(len, source_models))} CC0 models and {len(flower_names)} original flowers; scene {scene_sha}.")


if __name__ == "__main__":
    main()
