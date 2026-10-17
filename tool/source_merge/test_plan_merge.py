import tempfile
import unittest
from pathlib import Path

from plan_merge import make_plan, top_level_types


class SourceMergeTests(unittest.TestCase):
    def test_discovers_namespaced_top_level_type(self):
        with tempfile.TemporaryDirectory() as temporary:
            source = Path(temporary) / "Ship.cs"
            source.write_text(
                'namespace Game { public class Ship { private class Nested {} } }',
                encoding="utf-8",
            )
            self.assertEqual(("Game.Ship",), top_level_types(source))

    def test_ignores_types_in_comments_and_strings(self):
        with tempfile.TemporaryDirectory() as temporary:
            source = Path(temporary) / "Ship.cs"
            source.write_text(
                '// class Fake {}\npublic class Ship { string value = "class AlsoFake"; }',
                encoding="utf-8",
            )
            self.assertEqual(("Ship",), top_level_types(source))

    def test_requires_destination_meta_before_replacement(self):
        with tempfile.TemporaryDirectory() as temporary:
            root = Path(temporary)
            donor = root / "donor"
            destination = root / "destination"
            donor.mkdir()
            destination.mkdir()
            (donor / "Ship.cs").write_text("public class Ship { void Sail() {} }", encoding="utf-8")
            (destination / "Ship.cs").write_text("public class Ship {}", encoding="utf-8")
            rows = make_plan(donor, destination)
            self.assertEqual("review", rows[0].action)
            self.assertEqual("destination_meta_missing", rows[0].reason)

    def test_exact_type_sets_are_replaceable(self):
        with tempfile.TemporaryDirectory() as temporary:
            root = Path(temporary)
            donor = root / "donor"
            destination = root / "destination"
            donor.mkdir()
            destination.mkdir()
            (donor / "Ship.cs").write_text("public class Ship { void Sail() {} }", encoding="utf-8")
            (destination / "Ship.cs").write_text("public class Ship {}", encoding="utf-8")
            (destination / "Ship.cs.meta").write_text("guid: abc", encoding="utf-8")
            rows = make_plan(donor, destination)
            self.assertEqual("replace", rows[0].action)
            self.assertEqual("exact_top_level_type_set", rows[0].reason)


if __name__ == "__main__":
    unittest.main()

