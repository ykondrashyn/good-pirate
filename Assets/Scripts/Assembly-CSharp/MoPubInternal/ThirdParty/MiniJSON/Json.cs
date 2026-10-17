namespace MoPubInternal.ThirdParty.MiniJSON
{
	public static class Json
	{
		private sealed class Parser : global::System.IDisposable
		{
			private enum TOKEN
			{
				NONE = 0,
				CURLY_OPEN = 1,
				CURLY_CLOSE = 2,
				SQUARED_OPEN = 3,
				SQUARED_CLOSE = 4,
				COLON = 5,
				COMMA = 6,
				STRING = 7,
				NUMBER = 8,
				TRUE = 9,
				FALSE = 10,
				NULL = 11
			}

			private const string WORD_BREAK = "{}[],:\"";

			private global::System.IO.StringReader json;

			private char PeekChar => global::System.Convert.ToChar(json.Peek());

			private char NextChar => global::System.Convert.ToChar(json.Read());

			private string NextWord
			{
				get
				{
					global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
					while (!IsWordBreak(PeekChar))
					{
						stringBuilder.Append(NextChar);
						if (json.Peek() == -1)
						{
							break;
						}
					}
					return stringBuilder.ToString();
				}
			}

			private global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN NextToken
			{
				get
				{
					EatWhitespace();
					if (json.Peek() == -1)
					{
						return global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.NONE;
					}
					switch (PeekChar)
					{
					case '{':
						return global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.CURLY_OPEN;
					case '}':
						json.Read();
						return global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.CURLY_CLOSE;
					case '[':
						return global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.SQUARED_OPEN;
					case ']':
						json.Read();
						return global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.SQUARED_CLOSE;
					case ',':
						json.Read();
						return global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.COMMA;
					case '"':
						return global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.STRING;
					case ':':
						return global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.COLON;
					case '-':
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						return global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.NUMBER;
					default:
						switch (NextWord)
						{
						case "false":
							return global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.FALSE;
						case "true":
							return global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.TRUE;
						case "null":
							return global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.NULL;
						default:
							return global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.NONE;
						}
					}
				}
			}

			public static bool IsWordBreak(char c)
			{
				if (!char.IsWhiteSpace(c))
				{
					return "{}[],:\"".IndexOf(c) != -1;
				}
				return true;
			}

			private Parser(string jsonString)
			{
				json = new global::System.IO.StringReader(jsonString);
			}

			public static object Parse(string jsonString)
			{
				using (global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser parser = new global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser(jsonString))
				{
					return parser.ParseValue();
				}
			}

			public void Dispose()
			{
				json.Dispose();
				json = null;
			}

			private global::System.Collections.Generic.Dictionary<string, object> ParseObject()
			{
				global::System.Collections.Generic.Dictionary<string, object> dictionary = new global::System.Collections.Generic.Dictionary<string, object>();
				json.Read();
				while (true)
				{
					switch (NextToken)
					{
					case global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.COMMA:
						continue;
					case global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.NONE:
						return null;
					case global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.CURLY_CLOSE:
						return dictionary;
					}
					string text = ParseString();
					if (text == null)
					{
						return null;
					}
					if (NextToken != global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.COLON)
					{
						return null;
					}
					json.Read();
					dictionary[text] = ParseValue();
				}
			}

			private global::System.Collections.Generic.List<object> ParseArray()
			{
				global::System.Collections.Generic.List<object> list = new global::System.Collections.Generic.List<object>();
				json.Read();
				bool flag = true;
				while (flag)
				{
					global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN nextToken = NextToken;
					switch (nextToken)
					{
					case global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.NONE:
						return null;
					case global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.SQUARED_CLOSE:
						flag = false;
						break;
					default:
					{
						object item = ParseByToken(nextToken);
						list.Add(item);
						break;
					}
					case global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.COMMA:
						break;
					}
				}
				return list;
			}

			private object ParseValue()
			{
				global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN nextToken = NextToken;
				return ParseByToken(nextToken);
			}

			private object ParseByToken(global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN token)
			{
				switch (token)
				{
				case global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.STRING:
					return ParseString();
				case global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.NUMBER:
					return ParseNumber();
				case global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.CURLY_OPEN:
					return ParseObject();
				case global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.SQUARED_OPEN:
					return ParseArray();
				case global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.TRUE:
					return true;
				case global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.FALSE:
					return false;
				case global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.TOKEN.NULL:
					return null;
				default:
					return null;
				}
			}

			private string ParseString()
			{
				global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
				json.Read();
				bool flag = true;
				while (flag)
				{
					if (json.Peek() == -1)
					{
						flag = false;
						break;
					}
					char nextChar = NextChar;
					switch (nextChar)
					{
					case '"':
						flag = false;
						break;
					case '\\':
						if (json.Peek() == -1)
						{
							flag = false;
							break;
						}
						nextChar = NextChar;
						switch (nextChar)
						{
						case '"':
						case '/':
						case '\\':
							stringBuilder.Append(nextChar);
							break;
						case 'b':
							stringBuilder.Append('\b');
							break;
						case 'f':
							stringBuilder.Append('\f');
							break;
						case 'n':
							stringBuilder.Append('\n');
							break;
						case 'r':
							stringBuilder.Append('\r');
							break;
						case 't':
							stringBuilder.Append('\t');
							break;
						case 'u':
						{
							char[] array = new char[4];
							for (int i = 0; i < 4; i++)
							{
								array[i] = NextChar;
							}
							stringBuilder.Append((char)global::System.Convert.ToInt32(new string(array), 16));
							break;
						}
						}
						break;
					default:
						stringBuilder.Append(nextChar);
						break;
					}
				}
				return stringBuilder.ToString();
			}

			private object ParseNumber()
			{
				string nextWord = NextWord;
				if (nextWord.IndexOf('.') == -1)
				{
					long.TryParse(nextWord, out var result);
					return result;
				}
				double.TryParse(nextWord, out var result2);
				return result2;
			}

			private void EatWhitespace()
			{
				while (char.IsWhiteSpace(PeekChar))
				{
					json.Read();
					if (json.Peek() == -1)
					{
						break;
					}
				}
			}
		}

		private sealed class Serializer
		{
			private global::System.Text.StringBuilder builder;

			private Serializer()
			{
				builder = new global::System.Text.StringBuilder();
			}

			public static string Serialize(object obj)
			{
				global::MoPubInternal.ThirdParty.MiniJSON.Json.Serializer serializer = new global::MoPubInternal.ThirdParty.MiniJSON.Json.Serializer();
				serializer.SerializeValue(obj);
				return serializer.builder.ToString();
			}

			private void SerializeValue(object value)
			{
				if (value == null)
				{
					builder.Append("null");
				}
				else if (value is string str)
				{
					SerializeString(str);
				}
				else if (value is bool)
				{
					builder.Append(((bool)value) ? "true" : "false");
				}
				else if (value is global::System.Collections.IList anArray)
				{
					SerializeArray(anArray);
				}
				else if (value is global::System.Collections.IDictionary obj)
				{
					SerializeObject(obj);
				}
				else if (value is char)
				{
					SerializeString(new string((char)value, 1));
				}
				else
				{
					SerializeOther(value);
				}
			}

			private void SerializeObject(global::System.Collections.IDictionary obj)
			{
				bool flag = true;
				builder.Append('{');
				foreach (object key in obj.Keys)
				{
					if (!flag)
					{
						builder.Append(',');
					}
					SerializeString(key.ToString());
					builder.Append(':');
					SerializeValue(obj[key]);
					flag = false;
				}
				builder.Append('}');
			}

			private void SerializeArray(global::System.Collections.IList anArray)
			{
				builder.Append('[');
				bool flag = true;
				foreach (object item in anArray)
				{
					if (!flag)
					{
						builder.Append(',');
					}
					SerializeValue(item);
					flag = false;
				}
				builder.Append(']');
			}

			private void SerializeString(string str)
			{
				builder.Append('"');
				char[] array = str.ToCharArray();
				foreach (char c in array)
				{
					switch (c)
					{
					case '"':
						builder.Append("\\\"");
						continue;
					case '\\':
						builder.Append("\\\\");
						continue;
					case '\b':
						builder.Append("\\b");
						continue;
					case '\f':
						builder.Append("\\f");
						continue;
					case '\n':
						builder.Append("\\n");
						continue;
					case '\r':
						builder.Append("\\r");
						continue;
					case '\t':
						builder.Append("\\t");
						continue;
					}
					int num = global::System.Convert.ToInt32(c);
					if (num >= 32 && num <= 126)
					{
						builder.Append(c);
						continue;
					}
					builder.Append("\\u");
					builder.Append(num.ToString("x4"));
				}
				builder.Append('"');
			}

			private void SerializeOther(object value)
			{
				if (value is float)
				{
					builder.Append(((float)value).ToString("R"));
				}
				else if (value is int || value is uint || value is long || value is sbyte || value is byte || value is short || value is ushort || value is ulong)
				{
					builder.Append(value);
				}
				else if (value is double || value is decimal)
				{
					builder.Append(global::System.Convert.ToDouble(value).ToString("R"));
				}
				else
				{
					SerializeString(value.ToString());
				}
			}
		}

		public static object Deserialize(string json)
		{
			if (json == null)
			{
				return null;
			}
			return global::MoPubInternal.ThirdParty.MiniJSON.Json.Parser.Parse(json);
		}

		public static string Serialize(object obj)
		{
			return global::MoPubInternal.ThirdParty.MiniJSON.Json.Serializer.Serialize(obj);
		}
	}
}
