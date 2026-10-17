import importlib.util
import json
from pathlib import Path
import tempfile
import unittest
import zipfile

spec = importlib.util.spec_from_file_location("publication", Path(__file__).with_name("publication.py"))
publication = importlib.util.module_from_spec(spec)
spec.loader.exec_module(publication)

class PublicationGateTests(unittest.TestCase):
    def accepted(self):
        return {"source_commit": "a" * 40, "gameplay_interaction_seconds": 1800,
                "browsers": {name: {"status": "passed", "evidence": "captured"} for name in ["chromium", "firefox", "mobile_physical"]},
                "scenarios": {name: {"status": "passed", "evidence": "captured"} for name in publication.REQUIRED},
                "uncaught_errors": 0, "unexpected_application_requests": 0}

    def test_no_physical_browser_is_not_an_accepted_release(self):
        data = self.accepted()
        del data["browsers"]["mobile_physical"]
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "acceptance.json"
            path.write_text(json.dumps(data))
            with self.assertRaisesRegex(ValueError, "mobile_physical"):
                publication.acceptance(path, "a" * 40)

    def test_short_session_cannot_be_accepted(self):
        data = self.accepted()
        data["gameplay_interaction_seconds"] = 1799
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "acceptance.json"
            path.write_text(json.dumps(data))
            with self.assertRaisesRegex(ValueError, "30 minutes"):
                publication.acceptance(path, "a" * 40)

    def test_changed_commit_invalidates_acceptance(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "acceptance.json"
            path.write_text(json.dumps(self.accepted()))
            with self.assertRaisesRegex(ValueError, "different source commit"):
                publication.acceptance(path, "b" * 40)

    def test_archive_cannot_escape_site_directory(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            manifest = {"source_commit": "a" * 40, "tag": publication.TAG, "status": "accepted-preview", "acceptance": self.accepted(), "files": {"../escape": "0" * 64}}
            path = root / "publication.json"
            path.write_text(json.dumps(manifest))
            bundle = root / "bundle.zip"
            with zipfile.ZipFile(bundle, "w") as z:
                z.writestr("../escape", "untrusted")
                z.writestr("publication.json", json.dumps(manifest))
            with self.assertRaisesRegex(ValueError, "Unsafe"):
                publication.verify(bundle, path, "a" * 40, publication.TAG, root / "site")
            self.assertFalse((root / "escape").exists())

    def test_corrupted_game_file_blocks_deployment(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            manifest = {"source_commit": "a" * 40, "tag": publication.TAG, "status": "accepted-preview", "acceptance": self.accepted(), "files": {"index.html": "0" * 64}}
            path = root / "publication.json"
            path.write_text(json.dumps(manifest))
            bundle = root / "bundle.zip"
            with zipfile.ZipFile(bundle, "w") as z:
                z.writestr("index.html", "changed after acceptance")
                z.writestr("publication.json", json.dumps(manifest))
            with self.assertRaisesRegex(ValueError, "checksum mismatch"):
                publication.verify(bundle, path, "a" * 40, publication.TAG, root / "site")

if __name__ == "__main__":
    unittest.main()
