namespace UnityQuickSheet
{
	public class ConvertExt
	{
		public static object[] Split(string value)
		{
			string text = value;
			text = new string(global::System.Linq.Enumerable.ToArray(global::System.Linq.Enumerable.Where(text.ToCharArray(), (char ch) => !char.IsWhiteSpace(ch))));
			char[] trimChars = new char[2] { ',', ' ' };
			text = text.TrimEnd(trimChars);
			return text.Split(',');
		}

		public static float[] ToSingleArray(string value)
		{
			return global::System.Linq.Enumerable.ToArray(global::System.Linq.Enumerable.Select(global::System.Linq.Enumerable.Select(Split(value), (object e) => global::System.Convert.ChangeType(e, typeof(float))), (object e) => (float)e));
		}

		public static double[] ToDoubleArray(string value)
		{
			return global::System.Linq.Enumerable.ToArray(global::System.Linq.Enumerable.Select(global::System.Linq.Enumerable.Select(Split(value), (object e) => global::System.Convert.ChangeType(e, typeof(double))), (object e) => (double)e));
		}

		public static short[] ToInt16Array(string value)
		{
			return global::System.Linq.Enumerable.ToArray(global::System.Linq.Enumerable.Select(global::System.Linq.Enumerable.Select(Split(value), (object e) => global::System.Convert.ChangeType(e, typeof(short))), (object e) => (short)e));
		}

		public static int[] ToInt32Array(string value)
		{
			return global::System.Linq.Enumerable.ToArray(global::System.Linq.Enumerable.Select(global::System.Linq.Enumerable.Select(Split(value), (object e) => global::System.Convert.ChangeType(e, typeof(int))), (object e) => (int)e));
		}

		public static long[] ToInt64Array(string value)
		{
			return global::System.Linq.Enumerable.ToArray(global::System.Linq.Enumerable.Select(global::System.Linq.Enumerable.Select(Split(value), (object e) => global::System.Convert.ChangeType(e, typeof(long))), (object e) => (long)e));
		}

		public static string[] ToStringArray(string value)
		{
			return global::System.Linq.Enumerable.ToArray(global::System.Linq.Enumerable.Select(global::System.Linq.Enumerable.Select(Split(value), (object e) => global::System.Convert.ChangeType(e, typeof(string))), (object e) => (string)e));
		}
	}
}
