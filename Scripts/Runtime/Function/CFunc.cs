using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using MessagePack;

//! 기본 함수
public static partial class CFunc {
	#region 클래스 변수
	private static Dictionary<LogType, System.Action<string>> m_oLogList = new Dictionary<LogType, System.Action<string>>() {
		[LogType.Log] = UnityEngine.Debug.Log,
		[LogType.Warning] = UnityEngine.Debug.LogWarning,
		[LogType.Error] = UnityEngine.Debug.LogError
	};
	#endregion			// 클래스 변수

	#region 클래스 함수
	//! 파일을 복사한다
	public static void CopyFile(string a_oSrcPath, string a_oDestPath, bool a_bIsOverwrite = true) {
		// 파일 복사가 가능 할 경우
		if(File.Exists(a_oSrcPath) && (a_bIsOverwrite || !File.Exists(a_oDestPath))) {
			var oBytes = CFunc.ReadBytes(a_oSrcPath);
			CFunc.WriteBytes(a_oDestPath, oBytes);
		}
	}

	//! 파일을 복사한다
	public static void CopyFile(string a_oSrcPath, 
		string a_oDestPath, string a_oIgnore, System.Text.Encoding a_oEncoding, bool a_bIsOverwrite = true) 
	{
		// 파일 복사가 가능 할 경우
		if(File.Exists(a_oSrcPath) && (a_bIsOverwrite || !File.Exists(a_oDestPath))) {
			var oStringLines = CFunc.ReadStringLines(a_oSrcPath, a_oEncoding);

			// 문자열이 유효 할 경우
			if(oStringLines.ExIsValid()) {
				var oStringBuilder = new System.Text.StringBuilder();

				for(int i = KCDefine.B_VALUE_INT_0; i < oStringLines.Length; ++i) {
					// 문자열 추가가 가능 할 경우
					if(!oStringLines[i].Contains(a_oIgnore)) {
						oStringBuilder.AppendLine(oStringLines[i]);
					}
				}

				CFunc.WriteString(a_oDestPath, oStringBuilder.ToString(), a_oEncoding);
			}
		}
	}

	//! 파일을 복사한다
	public static void CopyFile(string a_oSrcPath, 
		string a_oDestPath, string a_oSearch, string a_oReplace, System.Text.Encoding a_oEncoding, bool a_bIsOverwrite = true) 
	{
		// 파일 복사가 가능 할 경우
		if(File.Exists(a_oSrcPath) && (a_bIsOverwrite || !File.Exists(a_oDestPath))) {
			var oStringLines = CFunc.ReadStringLines(a_oSrcPath, a_oEncoding);

			// 문자열이 유효 할 경우
			if(oStringLines.ExIsValid()) {
				var oStringBuilder = new System.Text.StringBuilder();

				for(int i = KCDefine.B_VALUE_INT_0; i < oStringLines.Length; ++i) {
					string oString = !oStringLines[i].ExIsValid() ? 
						string.Empty : oStringLines[i].ExGetReplaceString(a_oSearch, a_oReplace, short.MaxValue);
						
					oStringBuilder.AppendLine(oString);
				}

				CFunc.WriteString(a_oDestPath, oStringBuilder.ToString(), a_oEncoding);
			}
		}
	}

	//! 디렉토리를 복사한다
	public static void CopyDir(string a_oSrcPath, string a_oDestPath, bool a_bIsOverwrite = true) {
		// 디렉토리 복사가 가능 할 경우
		if(Directory.Exists(a_oSrcPath) && (a_bIsOverwrite || !Directory.Exists(a_oDestPath))) {
			CAccess.RemoveDirectory(a_oDestPath);

			CFunc.EnumerateDirs(a_oSrcPath, (a_oFilepaths, a_oDirpaths) => {
				for(int i = KCDefine.B_VALUE_INT_0; i < a_oFilepaths.Length; ++i) {
					string oDestFilepath = a_oFilepaths[i].ExGetReplaceString(a_oSrcPath, a_oDestPath);
					CFunc.CopyFile(a_oFilepaths[i], oDestFilepath, a_bIsOverwrite);
				}
			});
		}
	}

	//! 디렉토리를 순회한다
	public static void EnumerateDirs(string a_oDirpath, System.Action<string[], string[]> a_oCallback) {
		// 디렉토리가 존재 할 경우
		if(Directory.Exists(a_oDirpath)) {
			var oFilepaths = Directory.GetFiles(a_oDirpath);
			var oDirpaths = Directory.GetDirectories(a_oDirpath);

			a_oCallback?.Invoke(oFilepaths, oDirpaths);

			for(int i = KCDefine.B_VALUE_INT_0; i < oDirpaths.Length; ++i) {
				CFunc.EnumerateDirs(oDirpaths[i], a_oCallback);
			}
		}
	}

	//! 바이트를 읽어들인다
	public static byte[] ReadBytes(string a_oFilepath) {
		return File.Exists(a_oFilepath) ? File.ReadAllBytes(a_oFilepath) : null;
	}

	//! 보안 바이트를 읽어들인다
	public static byte[] ReadSecurityBytes(string a_oFilepath) {
		var oBytes = CFunc.ReadBytes(a_oFilepath);

		return (oBytes != null) ? 
			System.Convert.FromBase64String(System.Text.Encoding.Default.GetString(oBytes)) : null;
	}

	//! 문자열을 읽어들인다
	public static string ReadString(string a_oFilepath, System.Text.Encoding a_oEncoding) {
		return File.Exists(a_oFilepath) ? File.ReadAllText(a_oFilepath, a_oEncoding) : string.Empty;
	}

	//! 문자열 라인을 읽어들인다
	public static string[] ReadStringLines(string a_oFilepath, System.Text.Encoding a_oEncoding) {
		return File.ReadAllLines(a_oFilepath, a_oEncoding);
	}

	//! 보안 문자열을 읽어들인다
	public static string ReadSecurityString(string a_oFilepath, System.Text.Encoding a_oEncoding) {
		var oBytes = CFunc.ReadSecurityBytes(a_oFilepath);
		return (oBytes != null) ? a_oEncoding.GetString(oBytes) : string.Empty;
	}

	//! 바이트를 기록한다
	public static void WriteBytes(string a_oFilepath,
		byte[] a_oBytes, bool a_bIsAutoCreateDirectory = true, bool a_bIsAutoBackup = false, string a_oBackupDirectoryName = KCDefine.B_EMPTY_STRING) 
	{
		using(var oWStream = CAccess.GetWriteStream(a_oFilepath, 
			a_bIsAutoCreateDirectory, a_bIsAutoBackup, a_oBackupDirectoryName)) 
		{
			CFunc.WriteBytes(oWStream, a_oBytes);
		}
	}

	//! 바이트를 기록한다
	public static void WriteBytes(FileStream a_oWStream, byte[] a_oBytes) {
		a_oWStream.Write(a_oBytes, KCDefine.B_VALUE_INT_0, a_oBytes.Length);
		a_oWStream.Flush(true);
	}

	//! 보안 바이트를 기록한다
	public static void WriteSecurityBytes(string a_oFilepath,
		byte[] a_oBytes, bool a_bIsAutoCreateDirectory = true, bool a_bIsAutoBackup = false, string a_oBackupDirectoryName = KCDefine.B_EMPTY_STRING) 
	{
		using(var oWStream = CAccess.GetWriteStream(a_oFilepath, 
			a_bIsAutoCreateDirectory, a_bIsAutoBackup, a_oBackupDirectoryName)) 
		{
			CFunc.WriteSecurityBytes(oWStream, a_oBytes);
		}
	}

	//! 보안 바이트를 기록한다
	public static void WriteSecurityBytes(FileStream a_oWStream, byte[] a_oBytes) {
		string oString = System.Convert.ToBase64String(a_oBytes, KCDefine.B_VALUE_INT_0, a_oBytes.Length);
		CFunc.WriteBytes(a_oWStream, System.Text.Encoding.Default.GetBytes(oString));
	}

	//! 문자열을 기록한다
	public static void WriteString(string a_oFilepath,
		string a_oString, System.Text.Encoding a_oEncoding, bool a_bIsAutoCreateDirectory = true, bool a_bIsAutoBackup = false, string a_oBackupDirectoryName = KCDefine.B_EMPTY_STRING) 
	{
		using(var oWStream = CAccess.GetWriteStream(a_oFilepath, 
			a_bIsAutoCreateDirectory, a_bIsAutoBackup, a_oBackupDirectoryName)) 
		{
			CFunc.WriteString(oWStream, a_oString, a_oEncoding);
		}
	}

	//! 문자열을 기록한다
	public static void WriteString(FileStream a_oWStream, 
		string a_oString, System.Text.Encoding a_oEncoding) 
	{
		CFunc.WriteBytes(a_oWStream, a_oEncoding.GetBytes(a_oString));
	}

	//! 보안 문자열을 기록한다
	public static void WriteSecurityString(string a_oFilepath,
		string a_oString, System.Text.Encoding a_oEncoding, bool a_bIsAutoCreateDirectory = true, bool a_bIsAutoBackup = false, string a_oBackupDirectoryName = KCDefine.B_EMPTY_STRING) 
	{
		using(var oWStream = CAccess.GetWriteStream(a_oFilepath, a_bIsAutoCreateDirectory, a_bIsAutoBackup, a_oBackupDirectoryName)) {
			CFunc.WriteSecurityString(oWStream, a_oString, a_oEncoding);
		}
	}

	//! 보안 문자열을 기록한다
	public static void WriteSecurityString(FileStream a_oWStream, string a_oString, System.Text.Encoding a_oEncoding) {
		CFunc.WriteSecurityBytes(a_oWStream, a_oEncoding.GetBytes(a_oString));
	}

	//! URL 을 개방한다
	public static void OpenURL(string a_oURL) {
		CAccess.Assert(a_oURL.ExIsValid());
		Application.OpenURL(a_oURL);
	}
	
	//! 로그를 출력한다
	[Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
	public static void ShowLog(string a_oFormat, params object[] a_oParams) {
		CFunc.DoShowLog(LogType.Log, string.Format(a_oFormat, a_oParams));
	}

	//! 로그를 출력한다
	[Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
	public static void ShowLog(string a_oFormat, Color a_stColor, params object[] a_oParams) {
		string oFormat = a_oFormat.ExGetColorFormatString(a_stColor);
		CFunc.DoShowLog(LogType.Log, string.Format(oFormat, a_oParams));
	}

	//! 경고 로그를 출력한다
	public static void ShowLogWarning(string a_oFormat, params object[] a_oParams) {
		CFunc.DoShowLog(LogType.Warning, string.Format(a_oFormat, a_oParams));
	}

	//! 경고 로그를 출력한다
	public static void ShowLogWarning(string a_oFormat, Color a_stColor, params object[] a_oParams) {
		string oFormat = a_oFormat.ExGetColorFormatString(a_stColor);
		CFunc.DoShowLog(LogType.Warning, string.Format(oFormat, a_oParams));
	}

	//! 에러 로그를 출력한다
	public static void ShowLogError(string a_oFormat, params object[] a_oParams) {
		CFunc.DoShowLog(LogType.Error, string.Format(a_oFormat, a_oParams));
	}

	//! 에러 로그를 출력한다
	public static void ShowLogError(string a_oFormat, Color a_stColor, params object[] a_oParams) {
		string oFormat = a_oFormat.ExGetColorFormatString(a_stColor);
		CFunc.DoShowLog(LogType.Error, string.Format(oFormat, a_oParams));
	}

	//! 정수 랜덤 값을 생성한다
	public static int[] MakeIntRandomValues(int a_nMin, int a_nMax, int a_nNumValues) {
		return CFunc.MakeValues<int>(a_nNumValues, (a_nIndex) => Random.Range(a_nMin, a_nMax + KCDefine.B_VALUE_INT_1));
	}

	//! 실수 랜덤 값을 생성한다
	public static float[] MakeFloatRandomValues(float a_fMin, float a_fMax, int a_nNumValues) {
		return CFunc.MakeValues<float>(a_nNumValues, (a_nIndex) => Random.Range(a_fMin, a_fMax));
	}

	//! 정수 랜덤 분할 값을 생성한다
	public static int[] MakeIntRandomSplitValues(int a_nValue, int a_nNumValues) {
		int nSumValue = KCDefine.B_VALUE_INT_0;

		var oValues = CFunc.MakeValues<int>(a_nNumValues, (a_nIndex) => {
			int nValue = a_nValue / a_nNumValues;
			nSumValue += nValue;

			return (a_nIndex < a_nNumValues - KCDefine.B_VALUE_INT_1) ? nValue : a_nValue - nSumValue;
		});

		for(int i = KCDefine.B_VALUE_INT_0; i < oValues.Length; ++i) {
			int nIndex = Random.Range(KCDefine.B_VALUE_INT_0, oValues.Length);

			// 값 보정이 가능 할 경우
			if(oValues[i] > KCDefine.B_VALUE_INT_1 && oValues[nIndex] > KCDefine.B_VALUE_INT_1) {
				oValues[i] += KCDefine.B_VALUE_INT_1;
				oValues[nIndex] -= KCDefine.B_VALUE_INT_1;
			}
		}

		return oValues;
	}

	//! 로그를 출력한다
	private static void DoShowLog(LogType a_eLogType, string a_oLog) {
		CAccess.Assert(CFunc.m_oLogList.ContainsKey(a_eLogType));
		CFunc.m_oLogList[a_eLogType](a_oLog);
	}
	#endregion			// 클래스 함수

	#region 제네릭 클래스 함수
	//! 값을 교환한다
	public static void Swap<T>(ref T a_tLhs, ref T a_tRhs) {
		T tTemp = a_tLhs;
		a_tLhs = a_tRhs;
		a_tRhs = tTemp;
	}
	
	//! 값을 생성한다
	public static T[] MakeValues<T>(int a_nNumValues, System.Func<int, T> a_oCallback) {
		var oValues = new T[a_nNumValues];

		for(int i = KCDefine.B_VALUE_INT_0; i < a_nNumValues; ++i) {
			oValues[i] = a_oCallback.Invoke(i);
		}

		return oValues;
	}

	//! 섞인 값을 생성한다
	public static T[] MakeShuffleValues<T>(int a_nNumValues, System.Func<int, T> a_oCallback) {
		var oValues = CFunc.MakeValues<T>(a_nNumValues, a_oCallback);
		oValues.ExShuffle();

		return oValues;
	}

	//! JSON 객체를 읽어들인다
	public static T ReadJSONObj<T>(string a_oFilepath, System.Text.Encoding a_oEncoding) {
#if SECURITY_ENABLE
		string oString = CFunc.ReadSecurityString(a_oFilepath, a_oEncoding);
#else
		string oString = CFunc.ReadString(a_oFilepath, a_oEncoding);
#endif			// #if SECURITY_ENABLE

		return oString.ExJSONStringToObj<T>();
	}

	//! 메세지 팩 객체를 읽어들인다
	public static T ReadMsgPackObj<T>(string a_oFilepath) {
#if SECURITY_ENABLE
		var oBytes = CFunc.ReadSecurityBytes(a_oFilepath);
#else
		var oBytes = CFunc.ReadBytes(a_oFilepath);
#endif			// #if SECURITY_ENABLE

		return MessagePackSerializer.Deserialize<T>(oBytes);
	}

	//! 메세지 팩 JSON 객체를 읽어들인다
	public static T ReadMsgPackJSONObj<T>(string a_oFilepath, System.Text.Encoding a_oEncoding) {
#if SECURITY_ENABLE
		string oString = CFunc.ReadSecurityString(a_oFilepath, a_oEncoding);
#else
		string oString = CFunc.ReadString(a_oFilepath, a_oEncoding);
#endif			// #if SECURITY_ENABLE

		return oString.ExMsgPackJSONStringToObj<T>();
	}
	
	//! JSON 객체를 기록한다
	public static void WriteJSONObj<T>(string a_oFilepath, 
		T a_oObj, System.Text.Encoding a_oEncoding, bool a_bIsNeedRoot = false, bool a_bIsPretty = false) 
	{
		string oString = a_oObj.ExToJSONString(a_bIsNeedRoot, a_bIsPretty);

#if SECURITY_ENABLE
		CFunc.WriteSecurityString(a_oFilepath, oString, a_oEncoding);
#else
		CFunc.WriteString(a_oFilepath, oString, a_oEncoding);
#endif			// #if SECURITY_ENABLE
	}

	//! 메세지 팩 객체를 기록한다
	public static void WriteMsgPackObj<T>(string a_oFilepath, T a_oObj) {
		var oBytes = MessagePackSerializer.Serialize<T>(a_oObj);

#if SECURITY_ENABLE
		CFunc.WriteSecurityBytes(a_oFilepath, oBytes);
#else
		CFunc.WriteBytes(a_oFilepath, oBytes);
#endif			// #if SECURITY_ENABLE
	}

	//! 메세지 팩 JSON 객체를 기록한다
	public static void WriteMsgPackJSONObj<T>(string a_oFilepath, 
		T a_oObj, System.Text.Encoding a_oEncoding) 
	{
		string oString = a_oObj.ExToMsgPackJSONString();
		
#if SECURITY_ENABLE
		CFunc.WriteSecurityString(a_oFilepath, oString, a_oEncoding);
#else
		CFunc.WriteString(a_oFilepath, oString, a_oEncoding);
#endif			// #if SECURITY_ENABLE		
	}
	#endregion			// 제네릭 클래스 함수
}
