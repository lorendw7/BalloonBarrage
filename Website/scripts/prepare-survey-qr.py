"""Generate and independently decode the public survey QR. See FESTIVAL_SURVEY.md."""
import hashlib
import json
from pathlib import Path
import subprocess
import sys

import qrcode
from PIL import Image
import zxingcpp

site = Path(__file__).resolve().parents[1]
url = subprocess.check_output(
    ["node", "--input-type=module", "-e",
     "import {getSurveyFormUrl} from './src/survey-config.mjs'; console.log(getSurveyFormUrl());"],
    cwd=site, text=True).strip()
if not url:
    raise SystemExit("Configure a verified public responder URL before generating a QR.")
qr = qrcode.QRCode(error_correction=qrcode.constants.ERROR_CORRECT_M, box_size=12, border=4)
qr.add_data(url)
qr.make(fit=True)
matrix = qr.get_matrix()
size = len(matrix)
path = "".join(f"M{x},{y}h1v1h-1z" for y, row in enumerate(matrix) for x, black in enumerate(row) if black)
svg = (f'<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {size} {size}" '
       f'width="{size * 12}" height="{size * 12}" shape-rendering="crispEdges">'
       f'<rect width="{size}" height="{size}" fill="white"/><path d="{path}" fill="black"/></svg>\n')
directory = site / "media" / "survey"
if "--check" not in sys.argv:
    directory.mkdir(parents=True, exist_ok=True)
    qr.make_image(fill_color="black", back_color="white").save(directory / "survey-qr.png")
    (directory / "survey-qr.svg").write_text(svg, encoding="utf-8")
    manifest = {"url": url, "generator": "qrcode 8.2", "decoder": "zxing-cpp 3.0.0",
                "errorCorrection": "M", "quietZoneModules": 4, "modulesIncludingBorder": size,
                "files": {name: hashlib.sha256((directory / name).read_bytes()).hexdigest()
                          for name in ["survey-qr.svg", "survey-qr.png"]}}
    (directory / "manifest.json").write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
manifest = json.loads((directory / "manifest.json").read_text(encoding="utf-8"))
assert manifest["url"] == url
assert (directory / "survey-qr.svg").read_text(encoding="utf-8") == svg
for name, digest in manifest["files"].items():
    assert hashlib.sha256((directory / name).read_bytes()).hexdigest() == digest
image = Image.open(directory / "survey-qr.png").convert("RGB")
for pixels in [image.width, 260]:
    decoded = zxingcpp.read_barcode(image.resize((pixels, pixels), Image.Resampling.LANCZOS))
    assert decoded and decoded.text == url, f"QR decode failed at {pixels}px"
print(f"Verified public survey QR at {image.width}px and 260px; {size} modules including quiet zone.")
