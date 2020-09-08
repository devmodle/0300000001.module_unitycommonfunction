using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using UnityEngine;

#if MSG_PACK_ENABLE
using MessagePack;
#endif			// #if MSG_PACK_ENABLE

//! 기본 함수
public static partial class CFunc {
	#region 클래스 함수
	//! 파일을 복사한다
	public static void CopyFile(string a_oSrcFilepath, string a_oDestFilepath, bool a_bIsOverwrite = true) {
		CAccess.Assert(a_oSrcFilepath.ExIsValid() && a_oDestFilepath.ExIsValid());

		// 파일 복사가 가능 할 경우
		if(File.Exists(a_oSrcFilepath) && (a_bIsOverwrite || !File.Exists(a_oDestFilepath))) {
			var oBytes = CFunc.ReadBytes(a_oSrcFilepath);
			CFunc.WriteBytes(a_oDestFilepath, oBytes);
		}
	}

	//! 파일을 복사한다
	public static void CopyFile(string a_oSrcFilepath, 
		string a_oDestFilepath, string a_oIgnore, System.Text.Encoding a_oEncoding, bool a_bIsOverwrite = true) {
		CAccess.Assert(a_oSrcFilepath.ExIsValid() && a_oDestFilepath.ExIsValid() && a_oIgnore.ExIsValid());

		// 파일 복사가 가능 할 경우
		if(File.Exists(a_oSrcFilepath) && (a_bIsOverwrite || !File.Exists(a_oDestFilepath))) {
			var oStringLines = CFunc.ReadStringLines(a_oSrcFilepath, a_oEncoding);

			// 문자열이 유효 할 경우
			if(oStringLines.ExIsValid()) {
				var oStringBuilder = new System.Text.StringBuilder();

				for(int i = 0; i < oStringLines.Length; ++i) {
					// 문자열 추가가 가능 할 경우
					if(!oStringLines[i].Contains(a_oIgnore)) {
						oStringBuilder.AppendLine(oStringLines[i]);
					}
				}

				CFunc.WriteString(a_oDestFilepath, oStringBuilder.ToString(), a_oEncoding);
			}
		}
	}

	//! 파일을 복사한다
	public static void CopyFile(string a_oSrcFilepath, 
		string a_oDestFilepath, string a_oSearch, string a_oReplace, System.Text.Encoding a_oEncoding, bool a_bIsOverwrite = true) {
		CAccess.Assert(a_oSrcFilepath.ExIsValid() && a_oDestFilepath.ExIsValid() && a_oSearch.ExIsValid() && a_oReplace.ExIsValid());

		// 파일 복사가 가능 할 경우
		if(File.Exists(a_oSrcFilepath) && (a_bIsOverwrite || !File.Exists(a_oDestFilepath))) {
			var oStringLines = CFunc.ReadStringLines(a_oSrcFilepath, a_oEncoding);

			// 문자열이 유효 할 경우
			if(oStringLines.ExIsValid()) {
				var oStringBuilder = new System.Text.StringBuilder();

				for(int i = 0; i < oStringLines.Length; ++i) {
					string oString = !oStringLines[i].ExIsValid() ? string.Empty
						: oStringLines[i].ExGetReplaceString(a_oSearch, a_oReplace, short.MaxValue);
						
					oStringBuilder.AppendLine(oString);
				}

				CFunc.WriteString(a_oDestFilepath, oStringBuilder.ToString(), a_oEncoding);
			}
		}
	}

	//! 디렉토리를 복사한다
	public static void CopyDir(string a_oSrcPath, string a_oDestPath, bool a_bIsOverwrite = true) {
		CAccess.Assert(a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid());

		// 디렉토리가 존재 할 경우
		if(Directory.Exists(a_oSrcPath)) {
			// 디렉토리 복사가 가능 할 경우
			if(a_bIsOverwrite || !Directory.Exists(a_oDestPath)) {
				CAccess.RemoveDirectory(a_oDestPath);
				Directory.CreateDirectory(a_oDestPath);

				var oFiles = Directory.GetFiles(a_oSrcPath);

				for(int i = 0; i < oFiles.Length; ++i) {
					string oFilename = Path.GetFileName(oFiles[i]);
					CFunc.CopyFile(oFiles[i], Path.Combine(a_oDestPath, oFilename), a_bIsOverwrite);
				}
			}

			// 하위 디렉토리를 복사한다 {
			var oDirectories = Directory.GetDirectories(a_oSrcPath);

			for(int i = 0; i < oDirectories.Length; ++i) {
				string oDirectoryName = Path.GetFileNameWithoutExtension(oDirectories[i]);
				CFunc.CopyDir(oDirectories[i], Path.Combine(a_oDestPath, oDirectoryName), a_bIsOverwrite);
			}
			// 하위 디렉토리를 복사한다 }
		}
	}

	//! 바이트를 읽어들인다
	public static byte[] ReadBytes(string a_oFilepath) {
		CAccess.Assert(a_oFilepath.ExIsValid());
		return File.Exists(a_oFilepath) ? File.ReadAllBytes(a_oFilepath) : null;
	}

	//! 보안 바이트를 읽어들인다
	public static byte[] ReadSecurityBytes(string a_oFilepath) {
		var oBytes = CFunc.ReadBytes(a_oFilepath);
		return (oBytes != null) ? System.Convert.FromBase64String(System.Text.Encoding.Default.GetString(oBytes)) : null;
	}

	//! 문자열을 읽어들인다
	public static string ReadString(string a_oFilepath, System.Text.Encoding a_oEncoding) {
		CAccess.Assert(a_oEncoding != null && a_oFilepath.ExIsValid());
		return File.Exists(a_oFilepath) ? File.ReadAllText(a_oFilepath, a_oEncoding) : string.Empty;
	}

	//! 문자열 라인을 읽어들인다
	public static string[] ReadStringLines(string a_oFilepath, System.Text.Encoding a_oEncoding) {
		CAccess.Assert(a_oEncoding != null && a_oFilepath.ExIsValid());
		return File.ReadAllLines(a_oFilepath, a_oEncoding);
	}

	//! 보안 문자열을 읽어들인다
	public static string ReadSecurityString(string a_oFilepath, System.Text.Encoding a_oEncoding) {
		CAccess.Assert(a_oEncoding != null && a_oFilepath.ExIsValid());
		var oBytes = CFunc.ReadSecurityBytes(a_oFilepath);

		return (oBytes != null) ? a_oEncoding.GetString(oBytes) : string.Empty;
	}

	//! 바이트를 기록한다
	public static void WriteBytes(string a_oFilepath,
		byte[] a_oBytes, bool a_bIsAutoCreateDirectory = true, bool a_bIsAutoBackup = false, string a_oBackupDirectoryName = KCDefine.B_EMPTY_STRING) {
		using(var oWStream = CAccess.GetWriteStream(a_oFilepath, a_bIsAutoCreateDirectory, a_bIsAutoBackup, a_oBackupDirectoryName)) {
			CFunc.WriteBytes(oWStream, a_oBytes);
		}
	}

	//! 바이트를 기록한다
	public static void WriteBytes(FileStream a_oWStream, byte[] a_oBytes) {
		CAccess.Assert(a_oBytes.ExIsValid());

		a_oWStream?.Write(a_oBytes, 0, a_oBytes.Length);
		a_oWStream?.Flush(true);
	}

	//! 보안 바이트를 기록한다
	public static void WriteSecurityBytes(string a_oFilepath,
		byte[] a_oBytes, bool a_bIsAutoCreateDirectory = true, bool a_bIsAutoBackup = false, string a_oBackupDirectoryName = KCDefine.B_EMPTY_STRING) {
		using(var oWStream = CAccess.GetWriteStream(a_oFilepath, a_bIsAutoCreateDirectory, a_bIsAutoBackup, a_oBackupDirectoryName)) {
			CFunc.WriteSecurityBytes(oWStream, a_oBytes);
		}
	}

	//! 보안 바이트를 기록한다
	public static void WriteSecurityBytes(FileStream a_oWStream, byte[] a_oBytes) {
		CAccess.Assert(a_oBytes.ExIsValid());
		var oString = System.Convert.ToBase64String(a_oBytes, 0, a_oBytes.Length);

		CFunc.WriteBytes(a_oWStream, System.Text.Encoding.Default.GetBytes(oString));
	}

	//! 문자열을 기록한다
	public static void WriteString(string a_oFilepath,
		string a_oString, System.Text.Encoding a_oEncoding, bool a_bIsAutoCreateDirectory = true, bool a_bIsAutoBackup = false, string a_oBackupDirectoryName = KCDefine.B_EMPTY_STRING) {
		using(var oWStream = CAccess.GetWriteStream(a_oFilepath, a_bIsAutoCreateDirectory, a_bIsAutoBackup, a_oBackupDirectoryName)) {
			CFunc.WriteString(oWStream, a_oString, a_oEncoding);
		}
	}

	//! 문자열을 기록한다
	public static void WriteString(FileStream a_oWStream, string a_oString, System.Text.Encoding a_oEncoding) {
		CAccess.Assert(a_oEncoding != null && a_oString.ExIsValid());
		CFunc.WriteBytes(a_oWStream, a_oEncoding.GetBytes(a_oString));
	}

	//! 보안 문자열을 기록한다
	public static void WriteSecurityString(string a_oFilepath,
		string a_oString, System.Text.Encoding a_oEncoding, bool a_bIsAutoCreateDirectory = true, bool a_bIsAutoBackup = false, string a_oBackupDirectoryName = KCDefine.B_EMPTY_STRING) {
		using(var oWStream = CAccess.GetWriteStream(a_oFilepath, a_bIsAutoCreateDirectory, a_bIsAutoBackup, a_oBackupDirectoryName)) {
			CFunc.WriteSecurityString(oWStream, a_oString, a_oEncoding);
		}
	}

	//! 보안 문자열을 기록한다
	public static void WriteSecurityString(FileStream a_oWStream, string a_oString, System.Text.Encoding a_oEncoding) {
		CAccess.Assert(a_oEncoding != null && a_oString.ExIsValid());
		CFunc.WriteSecurityBytes(a_oWStream, a_oEncoding.GetBytes(a_oString));
	}

	//! URL 을 개방한다
	public static void OpenURL(string a_oURL) {
		CAccess.Assert(a_oURL.ExIsValid());
		Application.OpenURL(a_oURL);
	}

	//! 메일을 전송한다
	public static void SendMail(string a_oRecipient, string a_oTitle, string a_oMsg) {
		CAccess.Assert(a_oTitle != null && a_oMsg != null && a_oRecipient.ExIsValid());

		string oURL = string.Format(KCDefine.B_MAIL_URL_FORMAT,
			a_oRecipient, System.Uri.EscapeUriString(a_oTitle), System.Uri.EscapeUriString(a_oMsg));

		CFunc.OpenURL(oURL);
	}

	//! 로그를 출력한다
	[Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
	public static void ShowLog(string a_oFormat, params object[] a_oParams) {
		CFunc.DoShowLog(string.Format(a_oFormat, a_oParams), LogType.Log);
	}

	//! 로그를 출력한다
	[Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
	public static void ShowLog(string a_oFormat, Color a_stColor, params object[] a_oParams) {
		string oFormat = a_oFormat.ExGetColorFormatString(a_stColor);
		CFunc.DoShowLog(string.Format(oFormat, a_oParams), LogType.Log);
	}

	//! 경고 로그를 출력한다
	public static void ShowLogWarning(string a_oFormat, params object[] a_oParams) {
		CFunc.DoShowLog(string.Format(a_oFormat, a_oParams), LogType.Warning);
	}

	//! 경고 로그를 출력한다
	public static void ShowLogWarning(string a_oFormat, Color a_stColor, params object[] a_oParams) {
		string oFormat = a_oFormat.ExGetColorFormatString(a_stColor);
		CFunc.DoShowLog(string.Format(oFormat, a_oParams), LogType.Warning);
	}

	//! 에러 로그를 출력한다
	public static void ShowLogError(string a_oFormat, params object[] a_oParams) {
		CFunc.DoShowLog(string.Format(a_oFormat, a_oParams), LogType.Error);
	}

	//! 에러 로그를 출력한다
	public static void ShowLogError(string a_oFormat, Color a_stColor, params object[] a_oParams) {
		string oFormat = a_oFormat.ExGetColorFormatString(a_stColor);
		CFunc.DoShowLog(string.Format(oFormat, a_oParams), LogType.Error);
	}

	//! 정수 랜덤 값을 생성한다
	public static int[] MakeIntRandomValues(int a_nMin, int a_nMax, int a_nNumValues) {
		CAccess.Assert(a_nMin <= a_nMax);

		return CFunc.MakeValues<int>(a_nNumValues, (a_nIndex) => {
			return Random.Range(a_nMin, a_nMax + 1);
		});
	}

	//! 실수 랜덤 값을 생성한다
	public static float[] MakeFloatRandomValues(float a_fMin, float a_fMax, int a_nNumValues) {
		CAccess.Assert(a_fMin <= a_fMax);

		return CFunc.MakeValues<float>(a_nNumValues, (a_nIndex) => {
			return Random.Range(a_fMin, a_fMax);
		});
	}

	//! 정수 랜덤 분할 값을 생성한다
	public static int[] MakeIntRandomSplitValues(int a_nValue, int a_nNumValues) {
		CAccess.Assert(a_nNumValues >= 1);
		int nSumValue = 0;

		var oValues = CFunc.MakeValues<int>(a_nNumValues, (a_nIndex) => {
			int nValue = a_nValue / a_nNumValues;
			nSumValue += nValue;

			return (a_nIndex < a_nNumValues - 1) ? nValue : a_nValue - nSumValue;
		});

		for(int i = 0; i < oValues.Length; ++i) {
			int nIndex = Random.Range(0, oValues.Length);

			// 값 보정이 가능 할 경우
			if(oValues[i] > 1 && oValues[nIndex] > 1) {
				oValues[i] += 1;
				oValues[nIndex] -= 1;
			}
		}

		return oValues;
	}

	//! 로그를 출력한다
	private static void DoShowLog(string a_oLog, LogType a_eLogType) {
		// 에러 로그 일 경우
		if(a_eLogType == LogType.Error) {
			UnityEngine.Debug.LogError(a_oLog);
		}
		// 경고 로그 일 경우
		else if(a_eLogType == LogType.Warning) {
			UnityEngine.Debug.LogWarning(a_oLog);
		} else {
			UnityEngine.Debug.Log(a_oLog);
		}
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
		CAccess.Assert(a_oCallback != null && a_nNumValues >= 1);
		var oValues = new T[a_nNumValues];

		for(int i = 0; i < a_nNumValues; ++i) {
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
		var oString = CFunc.ReadSecurityString(a_oFilepath, a_oEncoding);
#else
		var oString = CFunc.ReadString(a_oFilepath, a_oEncoding);
#endif			// #if SECURITY_ENABLE

		CAccess.Assert(oString.ExIsValid());
		return oString.ExJSONStringToObj<T>();
	}

	//! JSON 객체를 기록한다
	public static void WriteJSONObj<T>(string a_oFilepath, 
		T a_oObj, System.Text.Encoding a_oEncoding, bool a_bIsNeedRoot = false, bool a_bIsPretty = false) {
		var oString = a_oObj.ExToJSONString(a_bIsNeedRoot, a_bIsPretty);
		CAccess.Assert(oString.ExIsValid());

#if SECURITY_ENABLE
		CFunc.WriteSecurityString(a_oFilepath, oString, a_oEncoding);
#else
		CFunc.WriteString(a_oFilepath, oString, a_oEncoding);
#endif			// #if SECURITY_ENABLE
	}
	#endregion			// 제네릭 클래스 함수

	#region 조건부 제네릭 클래스 함수
#if MSG_PACK_ENABLE
	//! 메세지 팩 객체를 읽어들인다
	public static T ReadMsgPackObj<T>(string a_oFilepath) {
#if SECURITY_ENABLE
		var oBytes = CFunc.ReadSecurityBytes(a_oFilepath);
#else
		var oBytes = CFunc.ReadBytes(a_oFilepath);
#endif			// #if SECURITY_ENABLE

		CAccess.Assert(oBytes.ExIsValid());
		return MessagePackSerializer.Deserialize<T>(oBytes);
	}

	//! 메세지 팩 JSON 객체를 읽어들인다
	public static T ReadMsgPackJSONObj<T>(string a_oFilepath, System.Text.Encoding a_oEncoding) {
#if SECURITY_ENABLE
		var oString = CFunc.ReadSecurityString(a_oFilepath, a_oEncoding);
#else
		var oString = CFunc.ReadString(a_oFilepath, a_oEncoding);
#endif			// #if SECURITY_ENABLE

		CAccess.Assert(oString.ExIsValid());
		return oString.ExMsgPackJSONStringToObj<T>();
	}

	//! 메세지 팩 객체를 기록한다
	public static void WriteMsgPackObj<T>(string a_oFilepath, T a_oObj) {
		var oBytes = MessagePackSerializer.Serialize<T>(a_oObj);
		CAccess.Assert(oBytes.ExIsValid());

#if SECURITY_ENABLE
		CFunc.WriteSecurityBytes(a_oFilepath, oBytes);
#else
		CFunc.WriteBytes(a_oFilepath, oBytes);
#endif			// #if SECURITY_ENABLE
	}

	//! 메세지 팩 JSON 객체를 기록한다
	public static void WriteMsgPackJSONObj<T>(string a_oFilepath, T a_oObj, System.Text.Encoding a_oEncoding) {
		var oString = a_oObj.ExToMsgPackJSONString();
		CAccess.Assert(oString.ExIsValid());

#if SECURITY_ENABLE
		CFunc.WriteSecurityString(a_oFilepath, oString, a_oEncoding);
#else
		CFunc.WriteString(a_oFilepath, oString, a_oEncoding);
#endif			// #if SECURITY_ENABLE		
	}
#endif			// #if MSG_PACK_ENABLE
	#endregion			// 조건부 제네릭 클래스 함수
}
