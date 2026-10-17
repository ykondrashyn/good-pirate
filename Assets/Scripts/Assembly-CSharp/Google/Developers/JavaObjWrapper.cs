namespace Google.Developers
{
	public class JavaObjWrapper
	{
		private global::System.IntPtr raw;

		private global::System.IntPtr cachedRawClass = global::System.IntPtr.Zero;

		public global::System.IntPtr RawObject => raw;

		public virtual global::System.IntPtr RawClass
		{
			get
			{
				if (cachedRawClass == global::System.IntPtr.Zero && raw != global::System.IntPtr.Zero)
				{
					cachedRawClass = global::UnityEngine.AndroidJNI.GetObjectClass(raw);
				}
				return cachedRawClass;
			}
		}

		protected JavaObjWrapper()
		{
		}

		public JavaObjWrapper(string clazzName)
		{
			raw = global::UnityEngine.AndroidJNI.AllocObject(global::UnityEngine.AndroidJNI.FindClass(clazzName));
		}

		public JavaObjWrapper(global::System.IntPtr rawObject)
		{
			raw = rawObject;
		}

		public void CreateInstance(string clazzName, params object[] args)
		{
			if (raw != global::System.IntPtr.Zero)
			{
				throw new global::System.Exception("Java object already set");
			}
			global::System.IntPtr constructorID = global::UnityEngine.AndroidJNIHelper.GetConstructorID(RawClass, args);
			global::UnityEngine.jvalue[] array = ConstructArgArray(args);
			try
			{
				raw = global::UnityEngine.AndroidJNI.NewObject(RawClass, constructorID, array);
			}
			finally
			{
				global::UnityEngine.AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		protected static global::UnityEngine.jvalue[] ConstructArgArray(object[] theArgs)
		{
			object[] array = new object[theArgs.Length];
			for (int i = 0; i < theArgs.Length; i++)
			{
				if (theArgs[i] is global::Google.Developers.JavaObjWrapper)
				{
					array[i] = ((global::Google.Developers.JavaObjWrapper)theArgs[i]).raw;
				}
				else
				{
					array[i] = theArgs[i];
				}
			}
			global::UnityEngine.jvalue[] array2 = global::UnityEngine.AndroidJNIHelper.CreateJNIArgArray(array);
			for (int j = 0; j < theArgs.Length; j++)
			{
				if (theArgs[j] is global::Google.Developers.JavaObjWrapper)
				{
					array2[j].l = ((global::Google.Developers.JavaObjWrapper)theArgs[j]).raw;
				}
				else if (theArgs[j] is global::Google.Developers.JavaInterfaceProxy)
				{
					global::System.IntPtr l = global::UnityEngine.AndroidJNIHelper.CreateJavaProxy((global::UnityEngine.AndroidJavaProxy)theArgs[j]);
					array2[j].l = l;
				}
			}
			if (array2.Length == 1)
			{
				for (int k = 0; k < array2.Length; k++)
				{
					global::UnityEngine.Debug.Log("---- [" + k + "] -- " + array2[k].l.ToString());
				}
			}
			return array2;
		}

		public static T StaticInvokeObjectCall<T>(string type, string name, string sig, params object[] args)
		{
			global::System.IntPtr clazz = global::UnityEngine.AndroidJNI.FindClass(type);
			global::System.IntPtr staticMethodID = global::UnityEngine.AndroidJNI.GetStaticMethodID(clazz, name, sig);
			global::UnityEngine.jvalue[] array = ConstructArgArray(args);
			try
			{
				global::System.IntPtr intPtr = global::UnityEngine.AndroidJNI.CallStaticObjectMethod(clazz, staticMethodID, array);
				global::System.Reflection.ConstructorInfo constructor = typeof(T).GetConstructor(new global::System.Type[1] { intPtr.GetType() });
				if (constructor != null)
				{
					return (T)constructor.Invoke(new object[1] { intPtr });
				}
				if (typeof(T).IsArray)
				{
					return global::UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<T>(intPtr);
				}
				global::UnityEngine.Debug.Log("Trying cast....");
				global::System.Type typeFromHandle = typeof(T);
				return (T)global::System.Runtime.InteropServices.Marshal.PtrToStructure(intPtr, typeFromHandle);
			}
			finally
			{
				global::UnityEngine.AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		public static void StaticInvokeCallVoid(string type, string name, string sig, params object[] args)
		{
			global::System.IntPtr clazz = global::UnityEngine.AndroidJNI.FindClass(type);
			global::System.IntPtr staticMethodID = global::UnityEngine.AndroidJNI.GetStaticMethodID(clazz, name, sig);
			global::UnityEngine.jvalue[] array = ConstructArgArray(args);
			try
			{
				global::UnityEngine.AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}
			finally
			{
				global::UnityEngine.AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		public static T GetStaticObjectField<T>(string clsName, string name, string sig)
		{
			global::System.IntPtr clazz = global::UnityEngine.AndroidJNI.FindClass(clsName);
			global::System.IntPtr staticFieldID = global::UnityEngine.AndroidJNI.GetStaticFieldID(clazz, name, sig);
			global::System.IntPtr staticObjectField = global::UnityEngine.AndroidJNI.GetStaticObjectField(clazz, staticFieldID);
			global::System.Reflection.ConstructorInfo constructor = typeof(T).GetConstructor(new global::System.Type[1] { staticObjectField.GetType() });
			if (constructor != null)
			{
				return (T)constructor.Invoke(new object[1] { staticObjectField });
			}
			global::System.Type typeFromHandle = typeof(T);
			return (T)global::System.Runtime.InteropServices.Marshal.PtrToStructure(staticObjectField, typeFromHandle);
		}

		public static int GetStaticIntField(string clsName, string name)
		{
			global::System.IntPtr clazz = global::UnityEngine.AndroidJNI.FindClass(clsName);
			global::System.IntPtr staticFieldID = global::UnityEngine.AndroidJNI.GetStaticFieldID(clazz, name, "I");
			return global::UnityEngine.AndroidJNI.GetStaticIntField(clazz, staticFieldID);
		}

		public static string GetStaticStringField(string clsName, string name)
		{
			global::System.IntPtr clazz = global::UnityEngine.AndroidJNI.FindClass(clsName);
			global::System.IntPtr staticFieldID = global::UnityEngine.AndroidJNI.GetStaticFieldID(clazz, name, "Ljava/lang/String;");
			return global::UnityEngine.AndroidJNI.GetStaticStringField(clazz, staticFieldID);
		}

		public static float GetStaticFloatField(string clsName, string name)
		{
			global::System.IntPtr clazz = global::UnityEngine.AndroidJNI.FindClass(clsName);
			global::System.IntPtr staticFieldID = global::UnityEngine.AndroidJNI.GetStaticFieldID(clazz, name, "F");
			return global::UnityEngine.AndroidJNI.GetStaticFloatField(clazz, staticFieldID);
		}

		public void InvokeCallVoid(string name, string sig, params object[] args)
		{
			global::System.IntPtr methodID = global::UnityEngine.AndroidJNI.GetMethodID(RawClass, name, sig);
			global::UnityEngine.jvalue[] array = ConstructArgArray(args);
			try
			{
				global::UnityEngine.AndroidJNI.CallVoidMethod(raw, methodID, array);
			}
			finally
			{
				global::UnityEngine.AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		public T InvokeCall<T>(string name, string sig, params object[] args)
		{
			global::System.Type typeFromHandle = typeof(T);
			global::System.IntPtr methodID = global::UnityEngine.AndroidJNI.GetMethodID(RawClass, name, sig);
			if (methodID == global::System.IntPtr.Zero)
			{
				global::UnityEngine.Debug.LogError("Cannot get method for " + name);
				throw new global::System.Exception("Cannot get method for " + name);
			}
			global::UnityEngine.jvalue[] array = ConstructArgArray(args);
			try
			{
				if (typeFromHandle == typeof(bool))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallBooleanMethod(raw, methodID, array);
				}
				if (typeFromHandle == typeof(string))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallStringMethod(raw, methodID, array);
				}
				if (typeFromHandle == typeof(int))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallIntMethod(raw, methodID, array);
				}
				if (typeFromHandle == typeof(float))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallFloatMethod(raw, methodID, array);
				}
				if (typeFromHandle == typeof(double))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallDoubleMethod(raw, methodID, array);
				}
				if (typeFromHandle == typeof(byte))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallByteMethod(raw, methodID, array);
				}
				if (typeFromHandle == typeof(char))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallCharMethod(raw, methodID, array);
				}
				if (typeFromHandle == typeof(long))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallLongMethod(raw, methodID, array);
				}
				if (typeFromHandle == typeof(short))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallShortMethod(raw, methodID, array);
				}
				if (typeFromHandle == typeof(global::System.IntPtr))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallObjectMethod(raw, methodID, array);
				}
				return InvokeObjectCall<T>(name, sig, args);
			}
			finally
			{
				global::UnityEngine.AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		public static T StaticInvokeCall<T>(string type, string name, string sig, params object[] args)
		{
			global::System.Type typeFromHandle = typeof(T);
			global::System.IntPtr clazz = global::UnityEngine.AndroidJNI.FindClass(type);
			global::System.IntPtr staticMethodID = global::UnityEngine.AndroidJNI.GetStaticMethodID(clazz, name, sig);
			global::UnityEngine.jvalue[] array = ConstructArgArray(args);
			try
			{
				if (typeFromHandle == typeof(bool))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallStaticBooleanMethod(clazz, staticMethodID, array);
				}
				if (typeFromHandle == typeof(string))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallStaticStringMethod(clazz, staticMethodID, array);
				}
				if (typeFromHandle == typeof(int))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallStaticIntMethod(clazz, staticMethodID, array);
				}
				if (typeFromHandle == typeof(float))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallStaticFloatMethod(clazz, staticMethodID, array);
				}
				if (typeFromHandle == typeof(double))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallStaticDoubleMethod(clazz, staticMethodID, array);
				}
				if (typeFromHandle == typeof(byte))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallStaticByteMethod(clazz, staticMethodID, array);
				}
				if (typeFromHandle == typeof(char))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallStaticCharMethod(clazz, staticMethodID, array);
				}
				if (typeFromHandle == typeof(long))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallStaticLongMethod(clazz, staticMethodID, array);
				}
				if (typeFromHandle == typeof(short))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallStaticShortMethod(clazz, staticMethodID, array);
				}
				if (typeFromHandle == typeof(global::System.IntPtr))
				{
					return (T)(object)global::UnityEngine.AndroidJNI.CallStaticObjectMethod(clazz, staticMethodID, array);
				}
				return StaticInvokeObjectCall<T>(type, name, sig, args);
			}
			finally
			{
				global::UnityEngine.AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		public T InvokeObjectCall<T>(string name, string sig, params object[] theArgs)
		{
			global::System.IntPtr methodID = global::UnityEngine.AndroidJNI.GetMethodID(RawClass, name, sig);
			global::UnityEngine.jvalue[] array = ConstructArgArray(theArgs);
			try
			{
				global::System.IntPtr intPtr = global::UnityEngine.AndroidJNI.CallObjectMethod(raw, methodID, array);
				if (intPtr.Equals(global::System.IntPtr.Zero))
				{
					return default(T);
				}
				global::System.Reflection.ConstructorInfo constructor = typeof(T).GetConstructor(new global::System.Type[1] { intPtr.GetType() });
				if (constructor != null)
				{
					return (T)constructor.Invoke(new object[1] { intPtr });
				}
				global::System.Type typeFromHandle = typeof(T);
				return (T)global::System.Runtime.InteropServices.Marshal.PtrToStructure(intPtr, typeFromHandle);
			}
			finally
			{
				global::UnityEngine.AndroidJNIHelper.DeleteJNIArgArray(theArgs, array);
			}
		}
	}
}
