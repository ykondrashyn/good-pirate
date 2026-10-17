public static class GoodPirate113PortablePreferences
{
	private static readonly string[] TutorialKeys =
	{
		"start", "move_base", "goldbox", "firstbattle", "battlewasperfect",
		"worldmap", "doubletap", "rewardbox", "not used", "crewpopup",
		"upgradepopup", "heal", "battleguide", "secondbattle", "firstisland",
		"openclaimpopup", "winclaimfirst"
	};

	private static readonly int[] PatrolCounts = { 3, 2, 2, 2, 2, 3 };

	public static global::GoodPirate113.Reconstruction.Save.GoodPirate113PreferenceSnapshot Capture()
	{
		var snapshot = new global::GoodPirate113.Reconstruction.Save.GoodPirate113PreferenceSnapshot();
		snapshot.MarkComplete();
		for (int island = 1; island <= 6; island++)
		{
			for (int door = 0; door < 4; door++)
			{
				AddBool(snapshot, $"Cube1_Island_{island}_Door_{door}");
			}
		}
		for (int flag = 0; flag < 40; flag++)
		{
			AddBool(snapshot, $"Flag_{flag}");
			AddInt(snapshot, $"Flag_{flag}_ADS");
			AddString(snapshot, $"COOLTIME_ADSFLAGCOOLTIME{flag}");
		}
		for (int ship = 1; ship < (int)EShipType.END; ship++)
		{
			string shipName = ((EShipType)ship).ToString();
			AddBool(snapshot, $"Ship_{shipName}_IsHave");
			AddBool(snapshot, shipName + "_isNew");
			AddInt(snapshot, $"Ship_{shipName}_Level");
			AddString(snapshot, "COOLTIME_" + shipName);
		}
		for (int island = 0; island < ConstValue.islandCount; island++)
		{
			AddBool(snapshot, $"Cube_Island_{island}");
			AddBool(snapshot, $"Island_Claim_{island}");
		}
		for (int skin = 0; skin < 21; skin++)
		{
			AddBool(snapshot, $"ShipSkin_{skin}");
		}
		for (int world = 1; world <= 6; world++)
		{
			AddBool(snapshot, $"World_{world}_Reward");
			AddBool(snapshot, $"Boss_1_{world}_0");
			for (int patrol = 0; patrol < PatrolCounts[world - 1]; patrol++)
			{
				AddString(snapshot, $"COOLTIME_Patrol_1_{world}_{patrol}");
			}
		}
		foreach (string tutorial in TutorialKeys)
		{
			AddBool(snapshot, "Tutorial_" + tutorial);
		}
		AddString(snapshot, "COOLTIME_KEY_FREEDIA");
		AddString(snapshot, "COOLTIME_GetDouble");
		AddString(snapshot, "COOLTIME_SkillMeteor");
		AddString(snapshot, "COOLTIME_SkillSpark");
		AddString(snapshot, "COOLTIME_SkillWindblast");
		for (int slot = 0; slot < 3; slot++)
		{
			AddString(snapshot, $"COOLTIME_RewardBoxSlot_{slot}");
		}
		for (int sailor = 0; sailor < (int)ESailor.END; sailor++)
		{
			AddString(snapshot, "COOLTIME_SailorExplore_" + ((ESailor)sailor).ToString());
		}
		if (MyInventoryDataManager.Instance != null
			&& MyInventoryDataManager.Instance.myData != null)
		{
			foreach (MyShipInventory ship in MyInventoryDataManager.Instance.myData.myShipDataList)
			{
				if (ship != null && ship.id >= 0 && ship.id <= 1000000)
				{
					AddString(snapshot, "COOLTIME_" + ship.id);
				}
			}
		}
		return snapshot;
	}

	public static bool TryApply(
		global::GoodPirate113.Reconstruction.Save.GoodPirate113PreferenceSnapshot snapshot,
		out string error)
	{
		if (snapshot == null)
		{
			error = "The preference snapshot is missing.";
			return false;
		}
		if (!TryValidate(snapshot, out error))
		{
			return false;
		}
		try
		{
			if (snapshot.IsComplete)
			{
				ClearKnownPreferences(snapshot);
			}

			foreach (var entry in snapshot.BoolValues)
			{
				global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetBool(entry.Key, entry.Value);
			}
			foreach (var entry in snapshot.IntValues)
			{
				if (entry.Key.StartsWith("Ship_") && entry.Key.EndsWith("_Level") && entry.Value == 0)
				{
					global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.DeleteKey(entry.Key);
				}
				else
				{
					global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetInt(entry.Key, entry.Value);
				}
			}
			foreach (var entry in snapshot.StringValues)
			{
				if (entry.Exists)
				{
					global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetString(entry.Key, entry.Value);
				}
				else
				{
					global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.DeleteKey(entry.Key);
				}
			}
			global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.Save();
		}
		catch (global::System.Exception exception)
		{
			error = "The portable preferences could not be applied: " + exception.Message;
			return false;
		}
		error = string.Empty;
		return true;
	}

	public static bool TryValidate(
		global::GoodPirate113.Reconstruction.Save.GoodPirate113PreferenceSnapshot snapshot,
		out string error)
	{
		return global::GoodPirate113.Reconstruction.Save.GoodPirate113PreferencePolicy.TryValidate(
			snapshot,
			out error);
	}

	public static bool TryApplyCurrent(out string error)
	{
		if (!global::GoodPirate113.Reconstruction.Save.GoodPirate113LegacyStateBridge.TryLoadPreferences(out var snapshot))
		{
			error = string.Empty;
			return true;
		}
		return TryApply(snapshot, out error);
	}

	private static void AddBool(
		global::GoodPirate113.Reconstruction.Save.GoodPirate113PreferenceSnapshot snapshot,
		string key)
	{
		if (global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.HasKey(key))
		{
			snapshot.AddBool(key, global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetBool(key, false));
		}
	}

	private static void AddInt(
		global::GoodPirate113.Reconstruction.Save.GoodPirate113PreferenceSnapshot snapshot,
		string key)
	{
		if (global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.HasKey(key))
		{
			snapshot.AddInt(key, global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetInt(key, 0));
		}
	}

	private static void AddString(
		global::GoodPirate113.Reconstruction.Save.GoodPirate113PreferenceSnapshot snapshot,
		string key)
	{
		bool exists = global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.HasKey(key);
		snapshot.AddString(
			key,
			exists ? global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetString(key, "") : "",
			exists);
	}

	private static void ClearKnownPreferences(
		global::GoodPirate113.Reconstruction.Save.GoodPirate113PreferenceSnapshot snapshot)
	{
		for (int island = 1; island <= 6; island++)
		{
			for (int door = 0; door < 4; door++)
			{
				Delete($"Cube1_Island_{island}_Door_{door}");
			}
		}
		for (int flag = 0; flag < 40; flag++)
		{
			Delete($"Flag_{flag}");
			Delete($"Flag_{flag}_ADS");
			Delete($"COOLTIME_ADSFLAGCOOLTIME{flag}");
		}
		for (int ship = 1; ship < (int)EShipType.END; ship++)
		{
			string shipName = ((EShipType)ship).ToString();
			Delete($"Ship_{shipName}_IsHave");
			Delete(shipName + "_isNew");
			Delete($"Ship_{shipName}_Level");
			Delete("COOLTIME_" + shipName);
		}
		for (int island = 0; island < ConstValue.islandCount; island++)
		{
			Delete($"Cube_Island_{island}");
			Delete($"Island_Claim_{island}");
		}
		for (int skin = 0; skin < 21; skin++)
		{
			Delete($"ShipSkin_{skin}");
		}
		for (int world = 1; world <= 6; world++)
		{
			Delete($"World_{world}_Reward");
			Delete($"Boss_1_{world}_0");
			for (int patrol = 0; patrol < PatrolCounts[world - 1]; patrol++)
			{
				Delete($"COOLTIME_Patrol_1_{world}_{patrol}");
			}
		}
		foreach (string tutorial in TutorialKeys)
		{
			Delete("Tutorial_" + tutorial);
		}
		Delete("COOLTIME_KEY_FREEDIA");
		Delete("COOLTIME_GetDouble");
		Delete("COOLTIME_SkillMeteor");
		Delete("COOLTIME_SkillSpark");
		Delete("COOLTIME_SkillWindblast");
		for (int slot = 0; slot < 3; slot++)
		{
			Delete($"COOLTIME_RewardBoxSlot_{slot}");
		}
		for (int sailor = 0; sailor < (int)ESailor.END; sailor++)
		{
			Delete("COOLTIME_SailorExplore_" + ((ESailor)sailor).ToString());
		}
		foreach (var entry in snapshot.StringValues)
		{
			if (entry.Key.StartsWith("COOLTIME_")
				&& global::System.Int32.TryParse(entry.Key.Substring("COOLTIME_".Length), out _))
			{
				Delete(entry.Key);
			}
		}
	}

	private static void Delete(string key)
	{
		if (global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.HasKey(key))
		{
			global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.DeleteKey(key);
		}
	}

	private static bool IsAllowedBoolKey(string key)
	{
		if (key.StartsWith("Cube1_Island_") && key.Contains("_Door_"))
		{
			return true;
		}
		if (key.StartsWith("Flag_") && !key.EndsWith("_ADS"))
		{
			return true;
		}
		if (key.StartsWith("Ship_") && key.EndsWith("_IsHave"))
		{
			return true;
		}
		if (key.EndsWith("_isNew"))
		{
			return true;
		}
		return HasNumberInRange(key, "Cube_Island_", 0, ConstValue.islandCount - 1)
			|| HasNumberInRange(key, "Island_Claim_", 0, ConstValue.islandCount - 1)
			|| HasNumberInRange(key, "ShipSkin_", 0, 20)
			|| HasNumberInRange(key, "World_", 1, 6, "_Reward")
			|| key.StartsWith("Boss_1_")
			|| key.StartsWith("Tutorial_");
	}

	private static bool IsAllowedIntKey(string key)
	{
		return (key.StartsWith("Flag_") && key.EndsWith("_ADS"))
			|| (key.StartsWith("Ship_") && key.EndsWith("_Level"));
	}

	private static bool IsAllowedStringKey(string key)
	{
		if (!key.StartsWith("COOLTIME_"))
		{
			return false;
		}
		string suffix = key.Substring("COOLTIME_".Length);
		if (suffix.Length == 0)
		{
			return false;
		}
		if (int.TryParse(suffix, out int shipId))
		{
			return shipId >= 0 && shipId <= 1000000;
		}
		return suffix == "KEY_FREEDIA"
			|| suffix == "GetDouble"
			|| suffix == "SkillMeteor"
			|| suffix == "SkillSpark"
			|| suffix == "SkillWindblast"
			|| suffix.StartsWith("RewardBoxSlot_")
			|| suffix.StartsWith("ADSFLAGCOOLTIME")
			|| suffix.StartsWith("Patrol_1_")
			|| suffix.StartsWith("SailorExplore_")
			|| global::System.Enum.IsDefined(typeof(EShipType), suffix);
	}

	private static bool HasNumberInRange(
		string key,
		string prefix,
		int minimum,
		int maximum,
		string suffix = "")
	{
		if (!key.StartsWith(prefix) || !key.EndsWith(suffix))
		{
			return false;
		}
		int length = key.Length - prefix.Length - suffix.Length;
		if (length <= 0)
		{
			return false;
		}
		return int.TryParse(key.Substring(prefix.Length, length), out int value)
			&& value >= minimum
			&& value <= maximum;
	}
}
