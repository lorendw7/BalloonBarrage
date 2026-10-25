"""Focused tests for the cross-platform desktop package writer."""

from __future__ import annotations

import subprocess
import sys
import tempfile
from pathlib import Path
import unittest
import zipfile


ROOT = Path(__file__).resolve().parents[2]
PACKAGER = ROOT / "Tools/Release/package_desktop_demo.py"


class DesktopPackageTests(unittest.TestCase):
    def test_windows_built_macos_binary_is_executable_in_zip(self) -> None:
        with tempfile.TemporaryDirectory(dir=ROOT) as temporary:
            workspace = Path(temporary)
            source = workspace / "macos"
            executable = source / "BalloonBarrage.app/Contents/MacOS/BalloonBarrage"
            executable.parent.mkdir(parents=True)
            executable.write_bytes(b"\xca\xfe\xba\xbe" + b"universal-macho")
            (source / "BalloonBarrage.app/Contents/Info.plist").write_text(
                "<plist></plist>\n", encoding="utf-8"
            )
            output = workspace / "BalloonBarrage-macOS-Universal-vtest.zip"

            completed = subprocess.run(
                [
                    sys.executable,
                    str(PACKAGER),
                    "--source",
                    str(source),
                    "--output",
                    str(output),
                    "--tag",
                    "vtest",
                    "--platform",
                    "macos",
                ],
                cwd=ROOT,
                check=False,
                capture_output=True,
                text=True,
            )

            self.assertEqual(completed.returncode, 0, completed.stderr)
            self.assertTrue(output.with_suffix(".zip.sha256").is_file())
            with zipfile.ZipFile(output) as archive:
                info = archive.getinfo("BalloonBarrage.app/Contents/MacOS/BalloonBarrage")
                self.assertEqual(info.create_system, 3)
                self.assertEqual((info.external_attr >> 16) & 0o111, 0o111)
                self.assertIn("ThirdPartyNotices/MPlus1C-OFL.txt", archive.namelist())
                self.assertIn("ThirdPartyNotices/MPlus1C-Provenance.json", archive.namelist())
                self.assertIn("ThirdPartyNotices/NotoSansSC-OFL.txt", archive.namelist())
                self.assertIn("ThirdPartyNotices/NotoSansSC-Provenance.json", archive.namelist())

    def test_refuses_to_overwrite_existing_package(self) -> None:
        with tempfile.TemporaryDirectory(dir=ROOT) as temporary:
            workspace = Path(temporary)
            source = workspace / "windows"
            source.mkdir()
            (source / "BalloonShooter.exe").write_bytes(b"exe")
            (source / "BalloonShooter_Data").mkdir()
            (source / "UnityPlayer.dll").write_bytes(b"dll")
            output = workspace / "candidate.zip"
            output.write_bytes(b"existing")

            completed = subprocess.run(
                [
                    sys.executable,
                    str(PACKAGER),
                    "--source",
                    str(source),
                    "--output",
                    str(output),
                    "--tag",
                    "vtest",
                    "--platform",
                    "windows",
                ],
                cwd=ROOT,
                check=False,
                capture_output=True,
                text=True,
            )

            self.assertNotEqual(completed.returncode, 0)
            self.assertEqual(output.read_bytes(), b"existing")


if __name__ == "__main__":
    unittest.main()
