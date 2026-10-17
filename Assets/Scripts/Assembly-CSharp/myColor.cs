public static class myColor
{
	private const float multiple = 0.00392156f;

	public static global::UnityEngine.Color GetColor(int r, int g, int b, int a = 255)
	{
		return new global::UnityEngine.Color((float)r * 0.00392156f, (float)g * 0.00392156f, (float)b * 0.00392156f, (float)a * 0.00392156f);
	}

	public static global::UnityEngine.Color LerpArray(float t, params global::UnityEngine.Color[] colorArr)
	{
		int num = colorArr.Length;
		t = global::UnityEngine.Mathf.Clamp01(t);
		global::UnityEngine.Color result = global::UnityEngine.Color.white;
		if (num > 2)
		{
			float num2 = 1f / (float)(num - 1);
			float t2 = t % num2 / num2;
			int num3 = 0;
			if (t > 0f && t < 1f)
			{
				num3 = (int)(t / num2);
			}
			else
			{
				if (t >= 1f)
				{
					return colorArr[num - 1];
				}
				if (t <= 0f)
				{
					return colorArr[0];
				}
			}
			if (num3 + 1 < num && num3 >= 0)
			{
				result = global::UnityEngine.Color.Lerp(colorArr[num3], colorArr[num3 + 1], t2);
			}
			return result;
		}
		return global::UnityEngine.Color.white;
	}
}
