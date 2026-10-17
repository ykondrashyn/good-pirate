namespace Boomlagoon.JSON
{
	public class JSONValue
	{
		public global::Boomlagoon.JSON.JSONValueType Type { get; private set; }

		public string Str { get; set; }

		public double Number { get; set; }

		public global::Boomlagoon.JSON.JSONObject Obj { get; set; }

		public global::Boomlagoon.JSON.JSONArray Array { get; set; }

		public bool Boolean { get; set; }

		public global::Boomlagoon.JSON.JSONValue Parent { get; set; }

		public JSONValue(global::Boomlagoon.JSON.JSONValueType type)
		{
			Type = type;
		}

		public JSONValue(string str)
		{
			Type = global::Boomlagoon.JSON.JSONValueType.String;
			Str = str;
		}

		public JSONValue(double number)
		{
			Type = global::Boomlagoon.JSON.JSONValueType.Number;
			Number = number;
		}

		public JSONValue(global::Boomlagoon.JSON.JSONObject obj)
		{
			if (obj == null)
			{
				Type = global::Boomlagoon.JSON.JSONValueType.Null;
				return;
			}
			Type = global::Boomlagoon.JSON.JSONValueType.Object;
			Obj = obj;
		}

		public JSONValue(global::Boomlagoon.JSON.JSONArray array)
		{
			Type = global::Boomlagoon.JSON.JSONValueType.Array;
			Array = array;
		}

		public JSONValue(bool boolean)
		{
			Type = global::Boomlagoon.JSON.JSONValueType.Boolean;
			Boolean = boolean;
		}

		public JSONValue(global::Boomlagoon.JSON.JSONValue value)
		{
			Type = value.Type;
			switch (Type)
			{
			case global::Boomlagoon.JSON.JSONValueType.String:
				Str = value.Str;
				break;
			case global::Boomlagoon.JSON.JSONValueType.Boolean:
				Boolean = value.Boolean;
				break;
			case global::Boomlagoon.JSON.JSONValueType.Number:
				Number = value.Number;
				break;
			case global::Boomlagoon.JSON.JSONValueType.Object:
				if (value.Obj != null)
				{
					Obj = new global::Boomlagoon.JSON.JSONObject(value.Obj);
				}
				break;
			case global::Boomlagoon.JSON.JSONValueType.Array:
				Array = new global::Boomlagoon.JSON.JSONArray(value.Array);
				break;
			}
		}

		public static implicit operator global::Boomlagoon.JSON.JSONValue(string str)
		{
			return new global::Boomlagoon.JSON.JSONValue(str);
		}

		public static implicit operator global::Boomlagoon.JSON.JSONValue(double number)
		{
			return new global::Boomlagoon.JSON.JSONValue(number);
		}

		public static implicit operator global::Boomlagoon.JSON.JSONValue(global::Boomlagoon.JSON.JSONObject obj)
		{
			return new global::Boomlagoon.JSON.JSONValue(obj);
		}

		public static implicit operator global::Boomlagoon.JSON.JSONValue(global::Boomlagoon.JSON.JSONArray array)
		{
			return new global::Boomlagoon.JSON.JSONValue(array);
		}

		public static implicit operator global::Boomlagoon.JSON.JSONValue(bool boolean)
		{
			return new global::Boomlagoon.JSON.JSONValue(boolean);
		}

		public override string ToString()
		{
			switch (Type)
			{
			case global::Boomlagoon.JSON.JSONValueType.Object:
				return Obj.ToString();
			case global::Boomlagoon.JSON.JSONValueType.Array:
				return Array.ToString();
			case global::Boomlagoon.JSON.JSONValueType.Boolean:
				if (!Boolean)
				{
					return "false";
				}
				return "true";
			case global::Boomlagoon.JSON.JSONValueType.Number:
				return Number.ToString();
			case global::Boomlagoon.JSON.JSONValueType.String:
				return "\"" + Str + "\"";
			case global::Boomlagoon.JSON.JSONValueType.Null:
				return "null";
			default:
				return "null";
			}
		}
	}
}
