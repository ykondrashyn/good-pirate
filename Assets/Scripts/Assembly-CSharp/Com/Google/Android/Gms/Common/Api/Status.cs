namespace Com.Google.Android.Gms.Common.Api
{
	public class Status : global::Google.Developers.JavaObjWrapper, global::Com.Google.Android.Gms.Common.Api.Result
	{
		private const string CLASS_NAME = "com/google/android/gms/common/api/Status";

		public static object CREATOR => global::Google.Developers.JavaObjWrapper.GetStaticObjectField<object>("com/google/android/gms/common/api/Status", "CREATOR", "Landroid/os/Parcelable$Creator;");

		public static string NULL => global::Google.Developers.JavaObjWrapper.GetStaticStringField("com/google/android/gms/common/api/Status", "NULL");

		public static int CONTENTS_FILE_DESCRIPTOR => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/api/Status", "CONTENTS_FILE_DESCRIPTOR");

		public static int PARCELABLE_WRITE_RETURN_VALUE => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/api/Status", "PARCELABLE_WRITE_RETURN_VALUE");

		public Status(global::System.IntPtr ptr)
			: base(ptr)
		{
		}

		public Status(int arg_int_1, string arg_string_2, object arg_object_3)
		{
			CreateInstance("com/google/android/gms/common/api/Status", arg_int_1, arg_string_2, arg_object_3);
		}

		public Status(int arg_int_1, string arg_string_2)
		{
			CreateInstance("com/google/android/gms/common/api/Status", arg_int_1, arg_string_2);
		}

		public Status(int arg_int_1)
		{
			CreateInstance("com/google/android/gms/common/api/Status", arg_int_1);
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

		public bool isInterrupted()
		{
			return InvokeCall<bool>("isInterrupted", "()Z", global::System.Array.Empty<object>());
		}

		public global::Com.Google.Android.Gms.Common.Api.Status getStatus()
		{
			return InvokeCall<global::Com.Google.Android.Gms.Common.Api.Status>("getStatus", "()Lcom/google/android/gms/common/api/Status;", global::System.Array.Empty<object>());
		}

		public bool isCanceled()
		{
			return InvokeCall<bool>("isCanceled", "()Z", global::System.Array.Empty<object>());
		}

		public int describeContents()
		{
			return InvokeCall<int>("describeContents", "()I", global::System.Array.Empty<object>());
		}

		public object getResolution()
		{
			return InvokeCall<object>("getResolution", "()Landroid/app/PendingIntent;", global::System.Array.Empty<object>());
		}

		public int getStatusCode()
		{
			return InvokeCall<int>("getStatusCode", "()I", global::System.Array.Empty<object>());
		}

		public string getStatusMessage()
		{
			return InvokeCall<string>("getStatusMessage", "()Ljava/lang/String;", global::System.Array.Empty<object>());
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

		public bool isSuccess()
		{
			return InvokeCall<bool>("isSuccess", "()Z", global::System.Array.Empty<object>());
		}
	}
}
