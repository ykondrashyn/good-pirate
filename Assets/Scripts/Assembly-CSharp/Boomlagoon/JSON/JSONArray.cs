namespace Boomlagoon.JSON
{
	public class JSONArray : global::System.Collections.Generic.IEnumerable<global::Boomlagoon.JSON.JSONValue>, global::System.Collections.IEnumerable
	{
		private readonly global::System.Collections.Generic.List<global::Boomlagoon.JSON.JSONValue> values = new global::System.Collections.Generic.List<global::Boomlagoon.JSON.JSONValue>();

		public global::Boomlagoon.JSON.JSONValue this[int index]
		{
			get
			{
				return values[index];
			}
			set
			{
				values[index] = value;
			}
		}

		public int Length => values.Count;

		public JSONArray()
		{
		}

		public JSONArray(global::Boomlagoon.JSON.JSONArray array)
		{
			values = new global::System.Collections.Generic.List<global::Boomlagoon.JSON.JSONValue>();
			foreach (global::Boomlagoon.JSON.JSONValue value in array.values)
			{
				values.Add(new global::Boomlagoon.JSON.JSONValue(value));
			}
		}

		public void Add(global::Boomlagoon.JSON.JSONValue value)
		{
			values.Add(value);
		}

		public override string ToString()
		{
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
			stringBuilder.Append('[');
			foreach (global::Boomlagoon.JSON.JSONValue value in values)
			{
				stringBuilder.Append(value.ToString());
				stringBuilder.Append(',');
			}
			if (values.Count > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}

		public global::System.Collections.Generic.IEnumerator<global::Boomlagoon.JSON.JSONValue> GetEnumerator()
		{
			return values.GetEnumerator();
		}

		global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
		{
			return values.GetEnumerator();
		}

		public static global::Boomlagoon.JSON.JSONArray Parse(string jsonString)
		{
			return global::Boomlagoon.JSON.JSONObject.Parse("{ \"array\" :" + jsonString + "}")?.GetValue("array").Array;
		}

		public void Clear()
		{
			values.Clear();
		}

		public void Remove(int index)
		{
			if (index >= 0 && index < values.Count)
			{
				values.RemoveAt(index);
				return;
			}
			global::Boomlagoon.JSON.JSONLogger.Error("index out of range: " + index + " (Expected 0 <= index < " + values.Count + ")");
		}

		public static global::Boomlagoon.JSON.JSONArray operator +(global::Boomlagoon.JSON.JSONArray lhs, global::Boomlagoon.JSON.JSONArray rhs)
		{
			global::Boomlagoon.JSON.JSONArray jSONArray = new global::Boomlagoon.JSON.JSONArray(lhs);
			foreach (global::Boomlagoon.JSON.JSONValue value in rhs.values)
			{
				jSONArray.Add(value);
			}
			return jSONArray;
		}
	}
}
