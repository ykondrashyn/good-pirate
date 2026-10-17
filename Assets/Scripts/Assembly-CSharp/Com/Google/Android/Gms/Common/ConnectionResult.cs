namespace Com.Google.Android.Gms.Common
{
	public class ConnectionResult : global::Google.Developers.JavaObjWrapper
	{
		private const string CLASS_NAME = "com/google/android/gms/common/ConnectionResult";

		public static int SUCCESS => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "SUCCESS");

		public static int SERVICE_MISSING => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "SERVICE_MISSING");

		public static int SERVICE_VERSION_UPDATE_REQUIRED => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "SERVICE_VERSION_UPDATE_REQUIRED");

		public static int SERVICE_DISABLED => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "SERVICE_DISABLED");

		public static int SIGN_IN_REQUIRED => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "SIGN_IN_REQUIRED");

		public static int INVALID_ACCOUNT => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "INVALID_ACCOUNT");

		public static int RESOLUTION_REQUIRED => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "RESOLUTION_REQUIRED");

		public static int NETWORK_ERROR => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "NETWORK_ERROR");

		public static int INTERNAL_ERROR => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "INTERNAL_ERROR");

		public static int SERVICE_INVALID => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "SERVICE_INVALID");

		public static int DEVELOPER_ERROR => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "DEVELOPER_ERROR");

		public static int LICENSE_CHECK_FAILED => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "LICENSE_CHECK_FAILED");

		public static int CANCELED => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "CANCELED");

		public static int TIMEOUT => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "TIMEOUT");

		public static int INTERRUPTED => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "INTERRUPTED");

		public static int API_UNAVAILABLE => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "API_UNAVAILABLE");

		public static int SIGN_IN_FAILED => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "SIGN_IN_FAILED");

		public static int SERVICE_UPDATING => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "SERVICE_UPDATING");

		public static int SERVICE_MISSING_PERMISSION => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "SERVICE_MISSING_PERMISSION");

		public static int DRIVE_EXTERNAL_STORAGE_REQUIRED => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "DRIVE_EXTERNAL_STORAGE_REQUIRED");

		public static object CREATOR => global::Google.Developers.JavaObjWrapper.GetStaticObjectField<object>("com/google/android/gms/common/ConnectionResult", "CREATOR", "Landroid/os/Parcelable$Creator;");

		public static string NULL => global::Google.Developers.JavaObjWrapper.GetStaticStringField("com/google/android/gms/common/ConnectionResult", "NULL");

		public static int CONTENTS_FILE_DESCRIPTOR => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "CONTENTS_FILE_DESCRIPTOR");

		public static int PARCELABLE_WRITE_RETURN_VALUE => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/ConnectionResult", "PARCELABLE_WRITE_RETURN_VALUE");

		public ConnectionResult(global::System.IntPtr ptr)
			: base(ptr)
		{
		}

		public ConnectionResult(int arg_int_1, object arg_object_2, string arg_string_3)
		{
			CreateInstance("com/google/android/gms/common/ConnectionResult", arg_int_1, arg_object_2, arg_string_3);
		}

		public ConnectionResult(int arg_int_1, object arg_object_2)
		{
			CreateInstance("com/google/android/gms/common/ConnectionResult", arg_int_1, arg_object_2);
		}

		public ConnectionResult(int arg_int_1)
		{
			CreateInstance("com/google/android/gms/common/ConnectionResult", arg_int_1);
		}

		public bool equals(object arg_object_1)
		{
			return InvokeCall<bool>("equals", "(Ljava/lang/Object;)Z", new object[1] { arg_object_1 });
		}

		public string toString()
		{
			return InvokeCall<string>("toString", "()Ljava/lang/String;", global::System.Array.Empty<object>());
		}

		public int hashCode()
		{
			return InvokeCall<int>("hashCode", "()I", global::System.Array.Empty<object>());
		}

		public int describeContents()
		{
			return InvokeCall<int>("describeContents", "()I", global::System.Array.Empty<object>());
		}

		public object getResolution()
		{
			return InvokeCall<object>("getResolution", "()Landroid/app/PendingIntent;", global::System.Array.Empty<object>());
		}

		public bool hasResolution()
		{
			return InvokeCall<bool>("hasResolution", "()Z", global::System.Array.Empty<object>());
		}

		public void startResolutionForResult(object arg_object_1, int arg_int_2)
		{
			InvokeCallVoid("startResolutionForResult", "(Landroid/app/Activity;I)V", arg_object_1, arg_int_2);
		}

		public void writeToParcel(object arg_object_1, int arg_int_2)
		{
			InvokeCallVoid("writeToParcel", "(Landroid/os/Parcel;I)V", arg_object_1, arg_int_2);
		}

		public int getErrorCode()
		{
			return InvokeCall<int>("getErrorCode", "()I", global::System.Array.Empty<object>());
		}

		public string getErrorMessage()
		{
			return InvokeCall<string>("getErrorMessage", "()Ljava/lang/String;", global::System.Array.Empty<object>());
		}

		public bool isSuccess()
		{
			return InvokeCall<bool>("isSuccess", "()Z", global::System.Array.Empty<object>());
		}
	}
}
