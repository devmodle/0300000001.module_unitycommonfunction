using System.Collections;
using System.Collections.Generic;
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
		CAccess.Assert(a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid());

		// 파일 복사가 가능 할 경우
		if(File.Exists(a_oSrcPath) && (a_bIsOverwrite || !File.Exists(a_oDestPath))) {
			var oBytes = CFunc.ReadBytes(a_oSrcPath);
			CFunc.WriteBytes(a_oDestPath, oBytes);
		}
	}

	//! 파일을 복사한다
	public static void CopyFile(string a_oSrcPath, string a_oDestPath, string a_oIgnore, bool a_bIsOverwrite = true) {
		CAccess.Assert(a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid());

		// 파일 복사가 가능 할 경우
		if(File.Exists(a_oSrcPath) && (a_bIsOverwrite || !File.Exists(a_oDestPath))) {
			var oStrLines = CFunc.ReadStrLines(a_oSrcPath);
			var oStrBuilder = new System.Text.StringBuilder();

			for(int i = 0; i < oStrLines.Length; ++i) {
				// 문자열 추가가 가능 할 경우
				if(!oStrLines[i].Contains(a_oIgnore)) {
					oStrBuilder.AppendLine(oStrLines[i]);
				}
			}

			CFunc.WriteStr(a_oDestPath, oStrBuilder.ToString());
		}
	}

	//! 파일을 복사한다
	public static void CopyFile(string a_oSrcPath, string a_oDestPath, string a_oTarget, string a_oReplace, bool a_bIsOverwrite = true) {
		CAccess.Assert(a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid());

		// 파일 복사가 가능 할 경우
		if(File.Exists(a_oSrcPath) && (a_bIsOverwrite || !File.Exists(a_oDestPath))) {
			var oStrLines = CFunc.ReadStrLines(a_oSrcPath);
			var oStrBuilder = new System.Text.StringBuilder();

			for(int i = 0; i < oStrLines.Length; ++i) {
				string oStr = oStrLines[i].ExIsValid() ? oStrLines[i].ExGetReplaceStr(a_oTarget, a_oReplace, short.MaxValue) : string.Empty;
				oStrBuilder.AppendLine(oStr);
			}

			CFunc.WriteStr(a_oDestPath, oStrBuilder.ToString());
		}
	}

	//! 디렉토리를 복사한다
	public static void CopyDir(string a_oSrcPath, string a_oDestPath, bool a_bIsOverwrite = true) {
		CAccess.Assert(a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid());

		// 디렉토리 복사가 가능 할 경우
		if(Directory.Exists(a_oSrcPath) && (a_bIsOverwrite || !Directory.Exists(a_oDestPath))) {
			CFactory.RemoveDir(a_oDestPath);

			CFunc.EnumerateDirs(a_oSrcPath, (a_oDirPaths, a_oFilePaths) => {
				for(int i = 0; i < a_oFilePaths.Length; ++i) {
					string oDestFilePath = a_oFilePaths[i].ExGetReplaceStr(a_oSrcPath, a_oDestPath);
					CFunc.CopyFile(a_oFilePaths[i], oDestFilePath, a_bIsOverwrite);
				}

				return true;
			});
		}
	}

	//! 디렉토리를 순회한다
	public static void EnumerateDirs(string a_oDirPath, System.Func<string[], string[], bool> a_oCallback) {
		CAccess.Assert(a_oCallback != null && a_oDirPath.ExIsValid());

		// 디렉토리가 존재 할 경우
		if(Directory.Exists(a_oDirPath)) {
			var oFilePaths = Directory.GetFiles(a_oDirPath);
			var oDirPaths = Directory.GetDirectories(a_oDirPath);

			// 디렉토리 순회가 가능 할 경우
			if(a_oCallback(oDirPaths, oFilePaths)) {
				for(int i = 0; i < oDirPaths.Length; ++i) {
					CFunc.EnumerateDirs(oDirPaths[i], a_oCallback);
				}
			}
		}
	}

	//! 바이트를 읽어들인다
	public static byte[] ReadBytes(string a_oFilePath) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		return File.Exists(a_oFilePath) ? File.ReadAllBytes(a_oFilePath) : null;
	}

	//! 바이트를 읽어들인다
	public static byte[] ReadBytesFromRes(string a_oFilePath) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oTextAsset = Resources.Load<TextAsset>(a_oFilePath);

		return (oTextAsset != null) ? oTextAsset.bytes : null;
	}

	//! 보안 바이트를 읽어들인다
	public static byte[] ReadSecurityBytes(string a_oFilePath) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		
		var oBytes = CFunc.ReadBytes(a_oFilePath);
		string oStr = System.Text.Encoding.Default.GetString(oBytes);

		return (oBytes != null) ? System.Convert.FromBase64String(oStr) : null;
	}

	//! 보안 바이트를 읽어들인다
	public static byte[] ReadSecurityBytesFromRes(string a_oFilePath) {
		CAccess.Assert(a_oFilePath.ExIsValid());

		var oBytes = CFunc.ReadBytesFromRes(a_oFilePath);
		string oStr = System.Text.Encoding.Default.GetString(oBytes);

		return (oBytes != null) ? System.Convert.FromBase64String(oStr) : null;
	}

	//! 문자열을 읽어들인다
	public static string ReadStr(string a_oFilePath) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		return File.Exists(a_oFilePath) ? File.ReadAllText(a_oFilePath, System.Text.Encoding.Default) : string.Empty;
	}

	//! 문자열을 읽어들인다
	public static string ReadStrFromRes(string a_oFilePath) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oTextAsset = Resources.Load<TextAsset>(a_oFilePath);

		return (oTextAsset != null) ? oTextAsset.text : string.Empty;
	}

	//! 보안 문자열을 읽어들인다
	public static string ReadSecurityStr(string a_oFilePath) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oBytes = CFunc.ReadSecurityBytes(a_oFilePath);

		return (oBytes != null) ? System.Text.Encoding.Default.GetString(oBytes) : string.Empty;
	}

	//! 보안 문자열을 읽어드린다
	public static string ReadSecurityStrFromRes(string a_oFilePath) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oBytes = CFunc.ReadSecurityBytesFromRes(a_oFilePath);

		return (oBytes != null) ? System.Text.Encoding.Default.GetString(oBytes) : string.Empty;
	}

	//! 문자열 라인을 읽어들인다
	public static string[] ReadStrLines(string a_oFilePath) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		return File.Exists(a_oFilePath) ? File.ReadAllLines(a_oFilePath, System.Text.Encoding.Default) : null;
	}

	//! 바이트를 기록한다
	public static void WriteBytes(string a_oFilePath, byte[] a_oBytes, bool a_bIsAutoCreateDir = true, bool a_bIsAutoBackup = false, string a_oBackupDirName = KCDefine.B_EMPTY_STR) {
		CAccess.Assert(a_oBytes != null && a_oFilePath.ExIsValid());

		using(var oWStream = CAccess.GetWriteStream(a_oFilePath, a_bIsAutoCreateDir, a_bIsAutoBackup, a_oBackupDirName)) {
			CFunc.WriteBytes(oWStream, a_oBytes);
		}
	}

	//! 바이트를 기록한다
	public static void WriteBytes(FileStream a_oWStream, byte[] a_oBytes, bool a_bIsFlush = true) {
		CAccess.Assert(a_oWStream != null && a_oBytes != null);

		a_oWStream.Write(a_oBytes, KCDefine.B_VAL_0_INT, a_oBytes.Length);
		a_oWStream.Flush(a_bIsFlush);
	}

	//! 보안 바이트를 기록한다
	public static void WriteSecurityBytes(string a_oFilePath, byte[] a_oBytes, bool a_bIsAutoCreateDir = true, bool a_bIsAutoBackup = false, string a_oBackupDirName = KCDefine.B_EMPTY_STR) {
		CAccess.Assert(a_oBytes != null && a_oFilePath.ExIsValid());

		using(var oWStream = CAccess.GetWriteStream(a_oFilePath, a_bIsAutoCreateDir, a_bIsAutoBackup, a_oBackupDirName)) {
			CFunc.WriteSecurityBytes(oWStream, a_oBytes);
		}
	}

	//! 보안 바이트를 기록한다
	public static void WriteSecurityBytes(FileStream a_oWStream, byte[] a_oBytes) {
		CAccess.Assert(a_oWStream != null && a_oBytes != null);
		string oStr = System.Convert.ToBase64String(a_oBytes, KCDefine.B_VAL_0_INT, a_oBytes.Length);

		CFunc.WriteBytes(a_oWStream, System.Text.Encoding.Default.GetBytes(oStr));
	}

	//! 문자열을 기록한다
	public static void WriteStr(string a_oFilePath, string a_oStr, bool a_bIsAutoCreateDir = true, bool a_bIsAutoBackup = false, string a_oBackupDirName = KCDefine.B_EMPTY_STR) {
		CAccess.Assert(a_oStr != null && a_oFilePath.ExIsValid());

		using(var oWStream = CAccess.GetWriteStream(a_oFilePath, a_bIsAutoCreateDir, a_bIsAutoBackup, a_oBackupDirName)) {
			CFunc.WriteStr(oWStream, a_oStr);
		}
	}

	//! 문자열을 기록한다
	public static void WriteStr(FileStream a_oWStream, string a_oStr) {
		CAccess.Assert(a_oWStream != null && a_oStr != null);
		CFunc.WriteBytes(a_oWStream, System.Text.Encoding.Default.GetBytes(a_oStr));
	}

	//! 보안 문자열을 기록한다
	public static void WriteSecurityStr(string a_oFilePath, string a_oStr, bool a_bIsAutoCreateDir = true, bool a_bIsAutoBackup = false, string a_oBackupDirName = KCDefine.B_EMPTY_STR) {
		CAccess.Assert(a_oStr != null && a_oFilePath.ExIsValid());

		using(var oWStream = CAccess.GetWriteStream(a_oFilePath, a_bIsAutoCreateDir, a_bIsAutoBackup, a_oBackupDirName)) {
			CFunc.WriteSecurityStr(oWStream, a_oStr);
		}
	}

	//! 보안 문자열을 기록한다
	public static void WriteSecurityStr(FileStream a_oWStream, string a_oStr) {
		CAccess.Assert(a_oWStream != null && a_oStr != null);
		CFunc.WriteSecurityBytes(a_oWStream, System.Text.Encoding.Default.GetBytes(a_oStr));
	}

	//! URL 을 개방한다
	public static void OpenURL(string a_oURL) {
		CAccess.Assert(a_oURL.ExIsValid());
		Application.OpenURL(a_oURL);
	}

	//! 함수를 호출한다
	public static void Invoke(ref System.Action a_oAction) {
		a_oAction?.Invoke();
		a_oAction = null;
	}
	
	//! 로그를 출력한다
	[Conditional("LOGIC_TEST_ENABLE"), Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
	public static void ShowLog(string a_oLog) {
		CAccess.Assert(a_oLog != null);
		CFunc.DoShowLog(LogType.Log, a_oLog);
	}

	//! 로그를 출력한다
	[Conditional("LOGIC_TEST_ENABLE"), Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
	public static void ShowLog(string a_oLog, Color a_stColor) {
		CAccess.Assert(a_oLog != null);
		CFunc.DoShowLog(LogType.Log, a_oLog.ExGetColorFmtStr(a_stColor));
	}
	
	//! 로그를 출력한다
	[Conditional("LOGIC_TEST_ENABLE"), Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
	public static void ShowLog(string a_oFmt, params object[] a_oParams) {
		CAccess.Assert(a_oFmt != null);
		CFunc.DoShowLog(LogType.Log, string.Format(a_oFmt, a_oParams));
	}

	//! 로그를 출력한다
	[Conditional("LOGIC_TEST_ENABLE"), Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
	public static void ShowLog(string a_oFmt, Color a_stColor, params object[] a_oParams) {
		CAccess.Assert(a_oFmt != null);
		string oLogFmt = a_oFmt.ExGetColorFmtStr(a_stColor);

		CFunc.DoShowLog(LogType.Log, string.Format(oLogFmt, a_oParams));
	}

	//! 경고 로그를 출력한다
	public static void ShowLogWarning(string a_oLog) {
		CFunc.DoShowLog(LogType.Warning, a_oLog);
	}

	//! 경고 로그를 출력한다
	public static void ShowLogWarning(string a_oFmt, params object[] a_oParams) {
		CAccess.Assert(a_oFmt != null);
		CFunc.DoShowLog(LogType.Warning, string.Format(a_oFmt, a_oParams));
	}

	//! 경고 로그를 출력한다
	public static void ShowLogWarning(string a_oFmt, Color a_stColor, params object[] a_oParams) {
		CAccess.Assert(a_oFmt != null);
		string oLogFmt = a_oFmt.ExGetColorFmtStr(a_stColor);

		CFunc.DoShowLog(LogType.Warning, string.Format(oLogFmt, a_oParams));
	}

	//! 에러 로그를 출력한다
	public static void ShowLogError(string a_oLog) {
		CFunc.DoShowLog(LogType.Error, a_oLog);
	}

	//! 에러 로그를 출력한다
	public static void ShowLogError(string a_oFmt, params object[] a_oParams) {
		CAccess.Assert(a_oFmt != null);
		CFunc.DoShowLog(LogType.Error, string.Format(a_oFmt, a_oParams));
	}

	//! 에러 로그를 출력한다
	public static void ShowLogError(string a_oFmt, Color a_stColor, params object[] a_oParams) {
		CAccess.Assert(a_oFmt != null);
		string oLogFmt = a_oFmt.ExGetColorFmtStr(a_stColor);

		CFunc.DoShowLog(LogType.Error, string.Format(oLogFmt, a_oParams));
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

	//! 함수를 호출한다
	public static void Invoke<T1>(ref System.Action<T1> a_oAction, T1 a_tParamsA) {
		a_oAction?.Invoke(a_tParamsA);
		a_oAction = null;
	}

	//! 함수를 호출한다
	public static void Invoke<T1, T2>(ref System.Action<T1, T2> a_oAction, T1 a_tParamsA, T2 a_tParamsB) {
		a_oAction?.Invoke(a_tParamsA, a_tParamsB);
		a_oAction = null;
	}

	//! 함수를 호출한다
	public static void Invoke<T1, T2, T3>(ref System.Action<T1, T2, T3> a_oAction, T1 a_tParamsA, T2 a_tParamsB, T3 a_tParamsC) {
		a_oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC);
		a_oAction = null;
	}

	//! 함수를 호출한다
	public static void Invoke<T1, T2, T3, T4>(ref System.Action<T1, T2, T3, T4> a_oAction, T1 a_tParamsA, T2 a_tParamsB, T3 a_tParamsC, T4 a_tParamsD) {
		a_oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD);
		a_oAction = null;
	}

	//! 함수를 호출한다
	public static void Invoke<T1, T2, T3, T4, T5>(ref System.Action<T1, T2, T3, T4, T5> a_oAction, T1 a_tParamsA, T2 a_tParamsB, T3 a_tParamsC, T4 a_tParamsD, T5 a_tParamsE) {
		a_oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE);
		a_oAction = null;
	}

	//! 함수를 호출한다
	public static void Invoke<T1, T2, T3, T4, T5, T6>(ref System.Action<T1, T2, T3, T4, T5, T6> a_oAction, T1 a_tParamsA, T2 a_tParamsB, T3 a_tParamsC, T4 a_tParamsD, T5 a_tParamsE, T6 a_tParamsF) {
		a_oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF);
		a_oAction = null;
	}

	//! 함수를 호출한다
	public static Result Invoke<Result>(ref System.Func<Result> a_oFunc) {
		CAccess.Assert(a_oFunc != null);
		
		var tResult = a_oFunc.Invoke();
		a_oFunc = null;

		return tResult;
	}

	//! 함수를 호출한다
	public static Result Invoke<T1, Result>(ref System.Func<T1, Result> a_oFunc, T1 a_tParamsA) {
		CAccess.Assert(a_oFunc != null);
		
		var tResult = a_oFunc.Invoke(a_tParamsA);
		a_oFunc = null;

		return tResult;
	}

	//! 함수를 호출한다
	public static Result Invoke<T1, T2, Result>(ref System.Func<T1, T2, Result> a_oFunc, T1 a_tParamsA, T2 a_tParamsB) {
		CAccess.Assert(a_oFunc != null);
		
		var tResult = a_oFunc.Invoke(a_tParamsA, a_tParamsB);
		a_oFunc = null;

		return tResult;
	}

	//! 함수를 호출한다
	public static Result Invoke<T1, T2, T3, Result>(ref System.Func<T1, T2, T3, Result> a_oFunc, T1 a_tParamsA, T2 a_tParamsB, T3 a_tParamsC) {
		CAccess.Assert(a_oFunc != null);
		
		var tResult = a_oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC);
		a_oFunc = null;

		return tResult;
	}

	//! 함수를 호출한다
	public static Result Invoke<T1, T2, T3, T4, Result>(ref System.Func<T1, T2, T3, T4, Result> a_oFunc, T1 a_tParamsA, T2 a_tParamsB, T3 a_tParamsC, T4 a_tParamsD) {
		CAccess.Assert(a_oFunc != null);
		
		var tResult = a_oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD);
		a_oFunc = null;

		return tResult;
	}

	//! 함수를 호출한다
	public static Result Invoke<T1, T2, T3, T4, T5, Result>(ref System.Func<T1, T2, T3, T4, T5, Result> a_oFunc, T1 a_tParamsA, T2 a_tParamsB, T3 a_tParamsC, T4 a_tParamsD, T5 a_tParamsE) {
		CAccess.Assert(a_oFunc != null);
		
		var tResult = a_oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE);
		a_oFunc = null;

		return tResult;
	}

	//! 함수를 호출한다
	public static Result Invoke<T1, T2, T3, T4, T5, T6, Result>(ref System.Func<T1, T2, T3, T4, T5, T6, Result> a_oFunc, T1 a_tParamsA, T2 a_tParamsB, T3 a_tParamsC, T4 a_tParamsD, T5 a_tParamsE, T6 a_tParamsF) {
		CAccess.Assert(a_oFunc != null);
		
		var tResult = a_oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF);
		a_oFunc = null;

		return tResult;
	}
	
	//! JSON 객체를 읽어들인다
	public static T ReadJSONObj<T>(string a_oFilePath, bool a_bIsSecurity = true) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		string oStr = CFunc.ReadStr(a_oFilePath);

#if SECURITY_ENABLE
		// 보안 모드 일 경우
		if(a_bIsSecurity) {
			oStr = CFunc.ReadSecurityStr(a_oFilePath);
		}
#endif			// #if SECURITY_ENABLE

		return oStr.ExJSONStrToObj<T>();
	}

	//! JSON 객체를 읽어들인다
	public static T ReadJSONObjFromRes<T>(string a_oFilePath, bool a_bIsSecurity = true) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		string oStr = CFunc.ReadStrFromRes(a_oFilePath);

#if SECURITY_ENABLE
		// 보안 모드 일 경우
		if(a_bIsSecurity) {
			oStr = CFunc.ReadSecurityStrFromRes(a_oFilePath);
		}
#endif			// #if SECURITY_ENABLE

		return oStr.ExJSONStrToObj<T>();
	}

	//! 메세지 팩 객체를 읽어들인다
	public static T ReadMsgPackObj<T>(string a_oFilePath, bool a_bIsSecurity = true) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oBytes = CFunc.ReadBytes(a_oFilePath);

#if SECURITY_ENABLE
		// 보안 모드 일 경우
		if(a_bIsSecurity) {
			oBytes = CFunc.ReadSecurityBytes(a_oFilePath);
		}
#endif			// #if SECURITY_ENABLE

		return MessagePackSerializer.Deserialize<T>(oBytes);
	}

	//! 메세지 팩 객체를 읽어들인다
	public static T ReadMsgPackObjFromRes<T>(string a_oFilePath, bool a_bIsSecurity = true) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oBytes = CFunc.ReadBytesFromRes(a_oFilePath);

#if SECURITY_ENABLE
		// 보안 모드 일 경우
		if(a_bIsSecurity) {
			oBytes = CFunc.ReadSecurityBytesFromRes(a_oFilePath);
		}
#endif			// #if SECURITY_ENABLE

		return MessagePackSerializer.Deserialize<T>(oBytes);
	}

	//! 메세지 팩 JSON 객체를 읽어들인다
	public static T ReadMsgPackJSONObj<T>(string a_oFilePath, bool a_bIsSecurity = true) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		string oStr = CFunc.ReadStr(a_oFilePath);

#if SECURITY_ENABLE
		// 보안 모드 일 경우
		if(a_bIsSecurity) {
			oStr = CFunc.ReadSecurityStr(a_oFilePath);
		}
#endif			// #if SECURITY_ENABLE

		return oStr.ExMsgPackJSONStrToObj<T>();
	}

	//! 메세지 팩 JSON 객체를 읽어들인다
	public static T ReadMsgPackJSONObjFromRes<T>(string a_oFilePath, bool a_bIsSecurity = true) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		string oStr = CFunc.ReadStrFromRes(a_oFilePath);

#if SECURITY_ENABLE
		// 보안 모드 일 경우
		if(a_bIsSecurity) {
			oStr = CFunc.ReadSecurityStrFromRes(a_oFilePath);
		}
#endif			// #if SECURITY_ENABLE

		return oStr.ExMsgPackJSONStrToObj<T>();
	}
	
	//! JSON 객체를 기록한다
	public static void WriteJSONObj<T>(string a_oFilePath, T a_oObj, bool a_bIsNeedRoot = false, bool a_bIsPretty = false, bool a_bIsAutoBackup = false, bool a_bIsSecurity = true) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		string oStr = a_oObj.ExToJSONStr(a_bIsNeedRoot, a_bIsPretty);

		// 보안 모드 일 경우
		if(a_bIsSecurity) {
#if SECURITY_ENABLE
			CFunc.WriteSecurityStr(a_oFilePath, oStr, true, a_bIsAutoBackup);
#else
			CFunc.WriteStr(a_oFilePath, oStr, true, a_bIsAutoBackup);
#endif			// #if SECURITY_ENABLE
		} else {
			CFunc.WriteStr(a_oFilePath, oStr, true, a_bIsAutoBackup);
		}
	}

	//! 메세지 팩 객체를 기록한다
	public static void WriteMsgPackObj<T>(string a_oFilePath, T a_oObj, bool a_bIsAutoBackup = false, bool a_bIsSecurity = true) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oBytes = MessagePackSerializer.Serialize<T>(a_oObj);

		// 보안 모드 일 경우
		if(a_bIsSecurity) {
#if SECURITY_ENABLE
			CFunc.WriteSecurityBytes(a_oFilePath, oBytes, true, a_bIsAutoBackup);
#else
			CFunc.WriteBytes(a_oFilePath, oBytes, true, a_bIsAutoBackup);
#endif			// #if SECURITY_ENABLE
		} else {
			CFunc.WriteBytes(a_oFilePath, oBytes, true, a_bIsAutoBackup);
		}
	}

	//! 메세지 팩 JSON 객체를 기록한다
	public static void WriteMsgPackJSONObj<T>(string a_oFilePath, T a_oObj, bool a_bIsAutoBackup = false, bool a_bIsSecurity = true) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		string oStr = a_oObj.ExToMsgPackJSONStr();

		// 보안 모드 일 경우
		if(a_bIsSecurity) {
#if SECURITY_ENABLE
			CFunc.WriteSecurityStr(a_oFilePath, oStr, true, a_bIsAutoBackup);
#else
			CFunc.WriteStr(a_oFilePath, oStr, true, a_bIsAutoBackup);
#endif			// #if SECURITY_ENABLE
		} else {
			CFunc.WriteStr(a_oFilePath, oStr, true, a_bIsAutoBackup);
		}
	}
	#endregion			// 제네릭 클래스 함수
}
