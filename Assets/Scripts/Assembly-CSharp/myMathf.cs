public static class myMathf
{
	public static float GetAngleForward(global::UnityEngine.Vector3 forward)
	{
		float num = 0f;
		num = global::UnityEngine.Mathf.Acos(global::UnityEngine.Mathf.Clamp(forward.x, -1f, 1f));
		num *= 57.29578f;
		if (forward.z < 0f)
		{
			num *= -1f;
			num = 360f + num;
		}
		return num;
	}

	public static long Lerp(long p1, long p2, double t)
	{
		return (long)((1.0 - t) * (double)p1 + t * (double)p2);
	}
}
