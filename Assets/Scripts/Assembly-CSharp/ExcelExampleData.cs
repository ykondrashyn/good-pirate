[global::System.Serializable]
public class ExcelExampleData
{
	[global::UnityEngine.SerializeField]
	private int id;

	[global::UnityEngine.SerializeField]
	private string name;

	[global::UnityEngine.SerializeField]
	private float strength;

	[global::UnityEngine.SerializeField]
	private Difficulty difficulty;

	public int Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	public float Strength
	{
		get
		{
			return strength;
		}
		set
		{
			strength = value;
		}
	}

	public Difficulty DIFFICULTY
	{
		get
		{
			return difficulty;
		}
		set
		{
			difficulty = value;
		}
	}
}
