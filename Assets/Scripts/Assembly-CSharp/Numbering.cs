public static class Numbering
{
	private static global::System.Text.StringBuilder sb = new global::System.Text.StringBuilder();

	private static string[] numberingText = new string[27]
	{
		"", "A", "B", "C", "D", "E", "F", "G", "H", "I",
		"J", "K", "L", "N", "M", "O", "P", "Q", "R", "S",
		"T", "U", "V", "W", "X", "Y", "Z"
	};

	public static string Convert(int number)
	{
		return Convert(number.ToString());
	}

	public static string Convert(long number)
	{
		return Convert(number.ToString());
	}

	public static string Convert(BigInteger number)
	{
		return Convert(number.ToString());
	}

	private static string Convert(string str)
	{
		sb.Length = 0;
		int num = (str.Length - 1) / 3;
		if (num >= 1)
		{
			int num2 = str.Length % 3;
			num2 = ((num2 == 0) ? 3 : num2);
			for (int i = 0; i < 3; i++)
			{
				if (i == num2)
				{
					sb.Append(".");
				}
				sb.Append(str[i]);
			}
			sb.Append(numberingText[num]);
			return sb.ToString();
		}
		sb.Append(str);
		sb.Append(numberingText[0]);
		return sb.ToString();
	}
}
