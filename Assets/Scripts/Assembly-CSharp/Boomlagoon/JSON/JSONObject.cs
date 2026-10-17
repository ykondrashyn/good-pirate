namespace Boomlagoon.JSON
{
	public class JSONObject : global::System.Collections.Generic.IEnumerable<global::System.Collections.Generic.KeyValuePair<string, global::Boomlagoon.JSON.JSONValue>>, global::System.Collections.IEnumerable
	{
		private enum JSONParsingState
		{
			Object = 0,
			Array = 1,
			EndObject = 2,
			EndArray = 3,
			Key = 4,
			Value = 5,
			KeyValueSeparator = 6,
			ValueSeparator = 7,
			String = 8,
			Number = 9,
			Boolean = 10,
			Null = 11
		}

		private readonly global::System.Collections.Generic.IDictionary<string, global::Boomlagoon.JSON.JSONValue> values = new global::System.Collections.Generic.Dictionary<string, global::Boomlagoon.JSON.JSONValue>();

		private static readonly global::System.Text.RegularExpressions.Regex unicodeRegex = new global::System.Text.RegularExpressions.Regex("\\\\u([0-9a-fA-F]{4})");

		private static readonly byte[] unicodeBytes = new byte[2];

		public global::Boomlagoon.JSON.JSONValue this[string key]
		{
			get
			{
				return GetValue(key);
			}
			set
			{
				values[key] = value;
			}
		}

		public JSONObject()
		{
		}

		public JSONObject(global::Boomlagoon.JSON.JSONObject other)
		{
			values = new global::System.Collections.Generic.Dictionary<string, global::Boomlagoon.JSON.JSONValue>();
			if (other == null)
			{
				return;
			}
			foreach (global::System.Collections.Generic.KeyValuePair<string, global::Boomlagoon.JSON.JSONValue> value in other.values)
			{
				values[value.Key] = new global::Boomlagoon.JSON.JSONValue(value.Value);
			}
		}

		public bool ContainsKey(string key)
		{
			return values.ContainsKey(key);
		}

		public global::Boomlagoon.JSON.JSONValue GetValue(string key)
		{
			values.TryGetValue(key, out var value);
			return value;
		}

		public string GetString(string key)
		{
			global::Boomlagoon.JSON.JSONValue value = GetValue(key);
			if (value == null)
			{
				global::Boomlagoon.JSON.JSONLogger.Error(key + "(string) == null");
				return string.Empty;
			}
			return value.Str;
		}

		public double GetNumber(string key)
		{
			global::Boomlagoon.JSON.JSONValue value = GetValue(key);
			if (value == null)
			{
				global::Boomlagoon.JSON.JSONLogger.Error(key + " == null");
				return double.NaN;
			}
			return value.Number;
		}

		public global::Boomlagoon.JSON.JSONObject GetObject(string key)
		{
			global::Boomlagoon.JSON.JSONValue value = GetValue(key);
			if (value == null)
			{
				global::Boomlagoon.JSON.JSONLogger.Error(key + " == null");
				return null;
			}
			return value.Obj;
		}

		public bool GetBoolean(string key)
		{
			global::Boomlagoon.JSON.JSONValue value = GetValue(key);
			if (value == null)
			{
				global::Boomlagoon.JSON.JSONLogger.Error(key + " == null");
				return false;
			}
			return value.Boolean;
		}

		public global::Boomlagoon.JSON.JSONArray GetArray(string key)
		{
			global::Boomlagoon.JSON.JSONValue value = GetValue(key);
			if (value == null)
			{
				global::Boomlagoon.JSON.JSONLogger.Error(key + " == null");
				return null;
			}
			return value.Array;
		}

		public void Add(string key, global::Boomlagoon.JSON.JSONValue value)
		{
			values[key] = value;
		}

		public void Add(global::System.Collections.Generic.KeyValuePair<string, global::Boomlagoon.JSON.JSONValue> pair)
		{
			values[pair.Key] = pair.Value;
		}

		public static global::Boomlagoon.JSON.JSONObject Parse(string jsonString)
		{
			if (string.IsNullOrEmpty(jsonString))
			{
				return null;
			}
			global::Boomlagoon.JSON.JSONValue jSONValue = null;
			global::System.Collections.Generic.List<string> list = new global::System.Collections.Generic.List<string>();
			global::Boomlagoon.JSON.JSONObject.JSONParsingState jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.Object;
			int num;
			for (num = 0; num < jsonString.Length; num++)
			{
				num = SkipWhitespace(jsonString, num);
				switch (jSONParsingState)
				{
				case global::Boomlagoon.JSON.JSONObject.JSONParsingState.Object:
				{
					if (jsonString[num] != '{')
					{
						return Fail('{', num);
					}
					global::Boomlagoon.JSON.JSONValue jSONValue3 = new global::Boomlagoon.JSON.JSONObject();
					if (jSONValue != null)
					{
						jSONValue3.Parent = jSONValue;
					}
					jSONValue = jSONValue3;
					jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.Key;
					break;
				}
				case global::Boomlagoon.JSON.JSONObject.JSONParsingState.EndObject:
					if (jsonString[num] != '}')
					{
						return Fail('}', num);
					}
					if (jSONValue.Parent == null)
					{
						return jSONValue.Obj;
					}
					switch (jSONValue.Parent.Type)
					{
					case global::Boomlagoon.JSON.JSONValueType.Object:
						jSONValue.Parent.Obj.values[list.Pop()] = new global::Boomlagoon.JSON.JSONValue(jSONValue.Obj);
						break;
					case global::Boomlagoon.JSON.JSONValueType.Array:
						jSONValue.Parent.Array.Add(new global::Boomlagoon.JSON.JSONValue(jSONValue.Obj));
						break;
					default:
						return Fail("valid object", num);
					}
					jSONValue = jSONValue.Parent;
					jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.ValueSeparator;
					break;
				case global::Boomlagoon.JSON.JSONObject.JSONParsingState.Key:
				{
					if (jsonString[num] == '}')
					{
						num--;
						jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.EndObject;
						break;
					}
					string text2 = ParseString(jsonString, ref num);
					if (text2 == null)
					{
						return Fail("key string", num);
					}
					list.Add(text2);
					jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.KeyValueSeparator;
					break;
				}
				case global::Boomlagoon.JSON.JSONObject.JSONParsingState.KeyValueSeparator:
					if (jsonString[num] != ':')
					{
						return Fail(':', num);
					}
					jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.Value;
					break;
				case global::Boomlagoon.JSON.JSONObject.JSONParsingState.ValueSeparator:
					switch (jsonString[num])
					{
					case ',':
						jSONParsingState = ((jSONValue.Type == global::Boomlagoon.JSON.JSONValueType.Object) ? global::Boomlagoon.JSON.JSONObject.JSONParsingState.Key : global::Boomlagoon.JSON.JSONObject.JSONParsingState.Value);
						break;
					case '}':
						jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.EndObject;
						num--;
						break;
					case ']':
						jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.EndArray;
						num--;
						break;
					default:
						return Fail(", } ]", num);
					}
					break;
				case global::Boomlagoon.JSON.JSONObject.JSONParsingState.Value:
				{
					char c = jsonString[num];
					if (c == '"')
					{
						jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.String;
					}
					else if (char.IsDigit(c) || c == '-')
					{
						jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.Number;
					}
					else
					{
						switch (c)
						{
						case '{':
							jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.Object;
							break;
						case '[':
							jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.Array;
							break;
						case ']':
							if (jSONValue.Type == global::Boomlagoon.JSON.JSONValueType.Array)
							{
								jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.EndArray;
								break;
							}
							return Fail("valid array", num);
						case 'f':
						case 't':
							jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.Boolean;
							break;
						case 'n':
							jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.Null;
							break;
						default:
							return Fail("beginning of value", num);
						}
					}
					num--;
					break;
				}
				case global::Boomlagoon.JSON.JSONObject.JSONParsingState.String:
				{
					string text = ParseString(jsonString, ref num);
					if (text == null)
					{
						return Fail("string value", num);
					}
					switch (jSONValue.Type)
					{
					case global::Boomlagoon.JSON.JSONValueType.Object:
						jSONValue.Obj.values[list.Pop()] = new global::Boomlagoon.JSON.JSONValue(text);
						break;
					case global::Boomlagoon.JSON.JSONValueType.Array:
						jSONValue.Array.Add(text);
						break;
					default:
						global::Boomlagoon.JSON.JSONLogger.Error("Fatal error, current JSON value not valid");
						return null;
					}
					jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.ValueSeparator;
					break;
				}
				case global::Boomlagoon.JSON.JSONObject.JSONParsingState.Number:
				{
					double num2 = ParseNumber(jsonString, ref num);
					if (double.IsNaN(num2))
					{
						return Fail("valid number", num);
					}
					switch (jSONValue.Type)
					{
					case global::Boomlagoon.JSON.JSONValueType.Object:
						jSONValue.Obj.values[list.Pop()] = new global::Boomlagoon.JSON.JSONValue(num2);
						break;
					case global::Boomlagoon.JSON.JSONValueType.Array:
						jSONValue.Array.Add(num2);
						break;
					default:
						global::Boomlagoon.JSON.JSONLogger.Error("Fatal error, current JSON value not valid");
						return null;
					}
					jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.ValueSeparator;
					break;
				}
				case global::Boomlagoon.JSON.JSONObject.JSONParsingState.Boolean:
					if (jsonString[num] == 't')
					{
						if (jsonString.Length < num + 4 || jsonString[num + 1] != 'r' || jsonString[num + 2] != 'u' || jsonString[num + 3] != 'e')
						{
							return Fail("true", num);
						}
						switch (jSONValue.Type)
						{
						case global::Boomlagoon.JSON.JSONValueType.Object:
							jSONValue.Obj.values[list.Pop()] = new global::Boomlagoon.JSON.JSONValue(boolean: true);
							break;
						case global::Boomlagoon.JSON.JSONValueType.Array:
							jSONValue.Array.Add(new global::Boomlagoon.JSON.JSONValue(boolean: true));
							break;
						default:
							global::Boomlagoon.JSON.JSONLogger.Error("Fatal error, current JSON value not valid");
							return null;
						}
						num += 3;
					}
					else
					{
						if (jsonString.Length < num + 5 || jsonString[num + 1] != 'a' || jsonString[num + 2] != 'l' || jsonString[num + 3] != 's' || jsonString[num + 4] != 'e')
						{
							return Fail("false", num);
						}
						switch (jSONValue.Type)
						{
						case global::Boomlagoon.JSON.JSONValueType.Object:
							jSONValue.Obj.values[list.Pop()] = new global::Boomlagoon.JSON.JSONValue(boolean: false);
							break;
						case global::Boomlagoon.JSON.JSONValueType.Array:
							jSONValue.Array.Add(new global::Boomlagoon.JSON.JSONValue(boolean: false));
							break;
						default:
							global::Boomlagoon.JSON.JSONLogger.Error("Fatal error, current JSON value not valid");
							return null;
						}
						num += 4;
					}
					jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.ValueSeparator;
					break;
				case global::Boomlagoon.JSON.JSONObject.JSONParsingState.Array:
				{
					if (jsonString[num] != '[')
					{
						return Fail('[', num);
					}
					global::Boomlagoon.JSON.JSONValue jSONValue2 = new global::Boomlagoon.JSON.JSONArray();
					if (jSONValue != null)
					{
						jSONValue2.Parent = jSONValue;
					}
					jSONValue = jSONValue2;
					jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.Value;
					break;
				}
				case global::Boomlagoon.JSON.JSONObject.JSONParsingState.EndArray:
					if (jsonString[num] != ']')
					{
						return Fail(']', num);
					}
					if (jSONValue.Parent == null)
					{
						return jSONValue.Obj;
					}
					switch (jSONValue.Parent.Type)
					{
					case global::Boomlagoon.JSON.JSONValueType.Object:
						jSONValue.Parent.Obj.values[list.Pop()] = new global::Boomlagoon.JSON.JSONValue(jSONValue.Array);
						break;
					case global::Boomlagoon.JSON.JSONValueType.Array:
						jSONValue.Parent.Array.Add(new global::Boomlagoon.JSON.JSONValue(jSONValue.Array));
						break;
					default:
						return Fail("valid object", num);
					}
					jSONValue = jSONValue.Parent;
					jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.ValueSeparator;
					break;
				case global::Boomlagoon.JSON.JSONObject.JSONParsingState.Null:
					if (jsonString[num] == 'n')
					{
						if (jsonString.Length < num + 4 || jsonString[num + 1] != 'u' || jsonString[num + 2] != 'l' || jsonString[num + 3] != 'l')
						{
							return Fail("null", num);
						}
						switch (jSONValue.Type)
						{
						case global::Boomlagoon.JSON.JSONValueType.Object:
							jSONValue.Obj.values[list.Pop()] = new global::Boomlagoon.JSON.JSONValue(global::Boomlagoon.JSON.JSONValueType.Null);
							break;
						case global::Boomlagoon.JSON.JSONValueType.Array:
							jSONValue.Array.Add(new global::Boomlagoon.JSON.JSONValue(global::Boomlagoon.JSON.JSONValueType.Null));
							break;
						default:
							global::Boomlagoon.JSON.JSONLogger.Error("Fatal error, current JSON value not valid");
							return null;
						}
						num += 3;
					}
					jSONParsingState = global::Boomlagoon.JSON.JSONObject.JSONParsingState.ValueSeparator;
					break;
				}
			}
			global::Boomlagoon.JSON.JSONLogger.Error("Unexpected end of string");
			return null;
		}

		private static int SkipWhitespace(string str, int pos)
		{
			while (pos < str.Length && char.IsWhiteSpace(str[pos]))
			{
				pos++;
			}
			return pos;
		}

		private static string ParseString(string str, ref int startPosition)
		{
			if (str[startPosition] != '"' || startPosition + 1 >= str.Length)
			{
				Fail('"', startPosition);
				return null;
			}
			int num = str.IndexOf('"', startPosition + 1);
			if (num <= startPosition)
			{
				Fail('"', startPosition + 1);
				return null;
			}
			while (str[num - 1] == '\\')
			{
				num = str.IndexOf('"', num + 1);
				if (num <= startPosition)
				{
					Fail('"', startPosition + 1);
					return null;
				}
			}
			string text = string.Empty;
			if (num > startPosition + 1)
			{
				text = str.Substring(startPosition + 1, num - startPosition - 1);
			}
			startPosition = num;
			while (true)
			{
				global::System.Text.RegularExpressions.Match match = unicodeRegex.Match(text);
				if (!match.Success)
				{
					break;
				}
				string value = match.Groups[1].Captures[0].Value;
				unicodeBytes[1] = byte.Parse(value.Substring(0, 2), global::System.Globalization.NumberStyles.HexNumber);
				unicodeBytes[0] = byte.Parse(value.Substring(2, 2), global::System.Globalization.NumberStyles.HexNumber);
				value = global::System.Text.Encoding.Unicode.GetString(unicodeBytes);
				text = text.Replace(match.Value, value);
			}
			return text;
		}

		private static double ParseNumber(string str, ref int startPosition)
		{
			if (startPosition >= str.Length || (!char.IsDigit(str[startPosition]) && str[startPosition] != '-'))
			{
				return double.NaN;
			}
			int i;
			for (i = startPosition + 1; i < str.Length && str[i] != ',' && str[i] != ']' && str[i] != '}'; i++)
			{
			}
			if (!double.TryParse(str.Substring(startPosition, i - startPosition), global::System.Globalization.NumberStyles.Float, global::System.Globalization.CultureInfo.InvariantCulture, out var result))
			{
				return double.NaN;
			}
			startPosition = i - 1;
			return result;
		}

		private static global::Boomlagoon.JSON.JSONObject Fail(char expected, int position)
		{
			return Fail(new string(expected, 1), position);
		}

		private static global::Boomlagoon.JSON.JSONObject Fail(string expected, int position)
		{
			global::Boomlagoon.JSON.JSONLogger.Error("Invalid json string, expecting " + expected + " at " + position);
			return null;
		}

		public override string ToString()
		{
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
			stringBuilder.Append('{');
			foreach (global::System.Collections.Generic.KeyValuePair<string, global::Boomlagoon.JSON.JSONValue> value in values)
			{
				stringBuilder.Append("\"" + value.Key + "\"");
				stringBuilder.Append(':');
				stringBuilder.Append(value.Value.ToString());
				stringBuilder.Append(',');
			}
			if (values.Count > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		public global::System.Collections.Generic.IEnumerator<global::System.Collections.Generic.KeyValuePair<string, global::Boomlagoon.JSON.JSONValue>> GetEnumerator()
		{
			return values.GetEnumerator();
		}

		global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
		{
			return values.GetEnumerator();
		}

		public void Clear()
		{
			values.Clear();
		}

		public void Remove(string key)
		{
			if (values.ContainsKey(key))
			{
				values.Remove(key);
			}
		}
	}
}
