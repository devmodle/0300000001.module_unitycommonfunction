using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using MessagePack;

/** 기본 함수 */
public static partial class CFunc {
	#region 클래스 변수
	private static Dictionary<LogType, System.Action<string>> m_oLogFuncDict = new Dictionary<LogType, System.Action<string>>() {
		[LogType.Log] = UnityEngine.Debug.Log, [LogType.Warning] = UnityEngine.Debug.LogWarning, [LogType.Error] = UnityEngine.Debug.LogError
	};
	#endregion			// 클래스 변수

	#region 클래스 함수
	/** 파일을 복사한다 */
	public static void CopyFile(string a_oSrcPath, string a_oDestPath, bool a_bIsOverwrite = true, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || (a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid()));
		bool bIsValid = a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid();

		// 파일 복사가 가능 할 경우
		if((bIsValid && File.Exists(a_oSrcPath)) && (a_bIsOverwrite || !File.Exists(a_oDestPath))) {
			var oBytes = CFunc.ReadBytes(a_oSrcPath);
			CFunc.WriteBytes(a_oDestPath, oBytes);
		}
	}

	/** 파일을 복사한다 */
	public static void CopyFile(string a_oSrcPath, string a_oDestPath, string a_oIgnoreToken, System.Text.Encoding a_oEncoding = null, bool a_bIsOverwrite = true, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || (a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid()));
		bool bIsValid = a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid();

		// 파일 복사가 가능 할 경우
		if((bIsValid && File.Exists(a_oSrcPath)) && (a_bIsOverwrite || !File.Exists(a_oDestPath))) {
			var oStrBuilder = new System.Text.StringBuilder();
			var oStrLineList = CFunc.ReadStrLines(a_oSrcPath, a_oEncoding ?? System.Text.Encoding.Default);

			for(int i = 0; i < oStrLineList.Count; ++i) {
				// 문자열이 유효 할 경우
				if(oStrLineList[i] != null && !oStrLineList[i].Contains(a_oIgnoreToken)) {
					oStrBuilder.AppendLine(oStrLineList[i]);
				}
			}

			CFunc.WriteStr(a_oDestPath, oStrBuilder.ToString(), a_oEncoding ?? System.Text.Encoding.Default);
		}
	}

	/** 파일을 복사한다 */
	public static void CopyFile(string a_oSrcPath, string a_oDestPath, string a_oTarget, string a_oReplace, System.Text.Encoding a_oEncoding = null, bool a_bIsOverwrite = true, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || (a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid()));
		bool bIsValid = a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid();

		// 파일 복사가 가능 할 경우
		if((bIsValid && File.Exists(a_oSrcPath)) && (a_bIsOverwrite || !File.Exists(a_oDestPath))) {
			var oStrBuilder = new System.Text.StringBuilder();
			var oStrLineList = CFunc.ReadStrLines(a_oSrcPath, a_oEncoding ?? System.Text.Encoding.Default);

			for(int i = 0; i < oStrLineList.Count; ++i) {
				// 문자열이 유효 할 경우
				if(oStrLineList[i] != null) {
					oStrBuilder.AppendLine(oStrLineList[i].ExGetReplaceStr(a_oTarget, a_oReplace, short.MaxValue));
				}
			}

			CFunc.WriteStr(a_oDestPath, oStrBuilder.ToString(), a_oEncoding ?? System.Text.Encoding.Default);
		}
	}

	/** 디렉토리를 복사한다 */
	public static void CopyDir(string a_oSrcPath, string a_oDestPath, bool a_bIsOverwrite = true, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || (a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid()));
		bool bIsValid = a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid();

		// 디렉토리 복사가 가능 할 경우
		if((bIsValid && Directory.Exists(a_oSrcPath)) && (a_bIsOverwrite || !Directory.Exists(a_oDestPath))) {
			CFactory.RemoveDir(a_oDestPath);

			CFunc.EnumerateDirs(a_oSrcPath, (a_oDirPathList, a_oFilePathList) => {
				for(int i = 0; i < a_oFilePathList.Count; ++i) {
					string oDestFilePath = a_oFilePathList[i].ExGetReplaceStr(a_oSrcPath, a_oDestPath);
					CFunc.CopyFile(a_oFilePathList[i], oDestFilePath, a_bIsOverwrite);
				}

				return true;
			});
		}
	}

	/** 디렉토리를 순회한다 */
	public static void EnumerateDirs(string a_oDirPath, System.Func<List<string>, List<string>, bool> a_oCallback, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || (a_oCallback != null && a_oDirPath.ExIsValid()));
		bool bIsValid = a_oCallback != null && a_oDirPath.ExIsValid();

		// 디렉토리가 존재 할 경우
		if(bIsValid && Directory.Exists(a_oDirPath)) {
			var oDirPaths = Directory.GetDirectories(a_oDirPath);
			
			// 디렉토리 순회가 가능 할 경우
			if(a_oCallback(oDirPaths.ToList(), Directory.GetFiles(a_oDirPath).ToList())) {
				for(int i = 0; i < oDirPaths.Length; ++i) {
					CFunc.EnumerateDirs(oDirPaths[i], a_oCallback);
				}
			}
		}
	}

	/** 바이트를 읽어들인다 */
	public static byte[] ReadBytes(string a_oFilePath) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		return File.Exists(a_oFilePath) ? File.ReadAllBytes(a_oFilePath) : null;
	}

	/** 바이트를 읽어들인다 */
	public static byte[] ReadBytesFromRes(string a_oFilePath) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oTextAsset = Resources.Load<TextAsset>(a_oFilePath);

		return (oTextAsset != null) ? oTextAsset.bytes : null;
	}

	/** 보안 바이트를 읽어들인다 */
	public static byte[] ReadSecurityBytes(string a_oFilePath, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oBytes = CFunc.ReadBytes(a_oFilePath);

		return (oBytes != null) ? System.Convert.FromBase64String((a_oEncoding ?? System.Text.Encoding.Default).GetString(oBytes)) : null;
	}

	/** 보안 바이트를 읽어들인다 */
	public static byte[] ReadSecurityBytesFromRes(string a_oFilePath, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oBytes = CFunc.ReadBytesFromRes(a_oFilePath);

		return (oBytes != null) ? System.Convert.FromBase64String((a_oEncoding ?? System.Text.Encoding.Default).GetString(oBytes)) : null;
	}

	/** 문자열을 읽어들인다 */
	public static string ReadStr(string a_oFilePath, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		return File.Exists(a_oFilePath) ? File.ReadAllText(a_oFilePath, a_oEncoding ?? System.Text.Encoding.Default) : string.Empty;
	}

	/** 문자열을 읽어들인다 */
	public static string ReadStrFromRes(string a_oFilePath) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oTextAsset = Resources.Load<TextAsset>(a_oFilePath);

		return (oTextAsset != null) ? oTextAsset.text : string.Empty;
	}

	/** 보안 문자열을 읽어들인다 */
	public static string ReadSecurityStr(string a_oFilePath, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oBytes = CFunc.ReadSecurityBytes(a_oFilePath, a_oEncoding ?? System.Text.Encoding.Default);

		return (oBytes != null) ? (a_oEncoding ?? System.Text.Encoding.Default).GetString(oBytes) : string.Empty;
	}

	/** 보안 문자열을 읽어들인다 */
	public static string ReadSecurityStrFromRes(string a_oFilePath, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oBytes = CFunc.ReadSecurityBytesFromRes(a_oFilePath, a_oEncoding ?? System.Text.Encoding.Default);

		return (oBytes != null) ? (a_oEncoding ?? System.Text.Encoding.Default).GetString(oBytes) : string.Empty;
	}

	/** 문자열 라인을 읽어들인다 */
	public static List<string> ReadStrLines(string a_oFilePath, System.Text.Encoding a_oEncoding = null) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		return File.Exists(a_oFilePath) ? File.ReadAllLines(a_oFilePath, a_oEncoding ?? System.Text.Encoding.Default).ToList() : null;
	}

	/** 바이트를 기록한다 */
	public static void WriteBytes(string a_oFilePath, byte[] a_oBytes, bool a_bIsAutoCreateDir = true, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || (a_oBytes != null && a_oFilePath.ExIsValid()));

		// 기록이 가능 할 경우
		if(a_oBytes != null && a_oFilePath.ExIsValid()) {
			using(var oWStream = CAccess.GetWriteStream(a_oFilePath, a_bIsAutoCreateDir)) {
				CFunc.WriteBytes(oWStream, a_oBytes, true, a_bIsEnableAssert);
			}
		}
	}

	/** 바이트를 기록한다 */
	public static void WriteBytes(FileStream a_oWStream, byte[] a_oBytes, bool a_bIsFlush = true, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || (a_oWStream != null && a_oBytes != null));

		// 스트림이 존재 할 경우
		if(a_oWStream != null && a_oBytes != null) {
			a_oWStream.Write(a_oBytes, KCDefine.B_VAL_0_INT, a_oBytes.Length);
			a_oWStream.Flush(a_bIsFlush);
		}
	}

	/** 보안 바이트를 기록한다 */
	public static void WriteSecurityBytes(string a_oFilePath, byte[] a_oBytes, System.Text.Encoding a_oEncoding = null, bool a_bIsAutoCreateDir = true, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || (a_oBytes != null && a_oFilePath.ExIsValid()));

		// 기록이 가능 할 경우
		if(a_oBytes != null && a_oFilePath.ExIsValid()) {
			using(var oWStream = CAccess.GetWriteStream(a_oFilePath, a_bIsAutoCreateDir)) {
				CFunc.WriteSecurityBytes(oWStream, a_oBytes, a_oEncoding ?? System.Text.Encoding.Default, a_bIsEnableAssert);
			}
		}
	}

	/** 보안 바이트를 기록한다 */
	public static void WriteSecurityBytes(FileStream a_oWStream, byte[] a_oBytes, System.Text.Encoding a_oEncoding = null, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || (a_oWStream != null && a_oBytes != null));

		// 스트림이 존재 할 경우
		if(a_oWStream != null && a_oBytes != null) {
			string oStr = System.Convert.ToBase64String(a_oBytes, KCDefine.B_VAL_0_INT, a_oBytes.Length);
			CFunc.WriteBytes(a_oWStream, (a_oEncoding ?? System.Text.Encoding.Default).GetBytes(oStr), true, a_bIsEnableAssert);
		}
	}

	/** 문자열을 기록한다 */
	public static void WriteStr(string a_oFilePath, string a_oStr, System.Text.Encoding a_oEncoding = null, bool a_bIsAutoCreateDir = true, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || (a_oStr != null && a_oFilePath.ExIsValid()));

		// 기록이 가능 할 경우
		if(a_oStr != null && a_oFilePath.ExIsValid()) {
			using(var oWStream = CAccess.GetWriteStream(a_oFilePath, a_bIsAutoCreateDir)) {
				CFunc.WriteStr(oWStream, a_oStr, a_oEncoding ?? System.Text.Encoding.Default, a_bIsEnableAssert);
			}
		}
	}

	/** 문자열을 기록한다 */
	public static void WriteStr(FileStream a_oWStream, string a_oStr, System.Text.Encoding a_oEncoding = null, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || (a_oWStream != null && a_oStr != null));

		// 스트림이 존재 할 경우
		if(a_oWStream != null && a_oStr != null) {
			CFunc.WriteBytes(a_oWStream, (a_oEncoding ?? System.Text.Encoding.Default).GetBytes(a_oStr), true, a_bIsEnableAssert);
		}
	}

	/** 보안 문자열을 기록한다 */
	public static void WriteSecurityStr(string a_oFilePath, string a_oStr, System.Text.Encoding a_oEncoding = null, bool a_bIsAutoCreateDir = true, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || (a_oStr != null && a_oFilePath.ExIsValid()));

		// 기록이 가능 할 경우
		if(a_oStr != null && a_oFilePath.ExIsValid()) {
			using(var oWStream = CAccess.GetWriteStream(a_oFilePath, a_bIsAutoCreateDir)) {
				CFunc.WriteSecurityStr(oWStream, a_oStr, a_oEncoding ?? System.Text.Encoding.Default, a_bIsEnableAssert);
			}
		}
	}

	/** 보안 문자열을 기록한다 */
	public static void WriteSecurityStr(FileStream a_oWStream, string a_oStr, System.Text.Encoding a_oEncoding = null, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || (a_oWStream != null && a_oStr != null));

		// 스트림이 존재 할 경우
		if(a_oWStream != null && a_oStr != null) {
			a_oEncoding = a_oEncoding ?? System.Text.Encoding.Default;
			CFunc.WriteSecurityBytes(a_oWStream, a_oEncoding.GetBytes(a_oStr), a_oEncoding, a_bIsEnableAssert);
		}
	}
	
	/** 함수를 호출한다 */
	public static void Invoke(ref System.Action a_rAction) {
		var oAction = a_rAction;

		try {
			a_rAction = null;
		} finally {
			oAction?.Invoke();
		}
	}

	/** 값을 교환한다 */
	public static void LessCorrectSwap(ref float a_rfLhs, ref float a_rfRhs) {
		// 보정이 필요 할 경우
		if(a_rfLhs.ExIsGreate(a_rfRhs)) {
			CFunc.Swap(ref a_rfLhs, ref a_rfRhs);
		}
	}

	/** 값을 교환한다 */
	public static void LessCorrectSwap(ref double a_rdblLhs, ref double a_rdblRhs) {
		// 보정이 필요 할 경우
		if(a_rdblLhs.ExIsGreate(a_rdblRhs)) {
			CFunc.Swap(ref a_rdblLhs, ref a_rdblRhs);
		}
	}

	/** 값을 교환한다 */
	public static void GreateCorrectSwap(ref float a_rfLhs, ref float a_rfRhs) {
		// 보정이 필요 할 경우
		if(a_rfLhs.ExIsLess(a_rfRhs)) {
			CFunc.Swap(ref a_rfLhs, ref a_rfRhs);
		}
	}

	/** 값을 교환한다 */
	public static void GreateCorrectSwap(ref double a_rdblLhs, ref double a_rdblRhs) {
		// 보정이 필요 할 경우
		if(a_rdblLhs.ExIsLess(a_rdblRhs)) {
			CFunc.Swap(ref a_rdblLhs, ref a_rdblRhs);
		}
	}
	
	/** 로그를 출력한다 */
	[Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
	public static void ShowLog(string a_oLog) {
		CAccess.Assert(a_oLog != null);
		CFunc.DoShowLog(LogType.Log, a_oLog);
	}

	/** 로그를 출력한다 */
	[Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
	public static void ShowLog(string a_oLog, Color a_stColor) {
		CAccess.Assert(a_oLog != null);
		CFunc.DoShowLog(LogType.Log, a_oLog.ExGetColorFmtStr(a_stColor));
	}
	
	/** 경고 로그를 출력한다 */
	[Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
	public static void ShowLogWarning(string a_oLog) {
		CFunc.DoShowLog(LogType.Warning, a_oLog);
	}

	/** 경고 로그를 출력한다 */
	[Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
	public static void ShowLogWarning(string a_oLog, Color a_stColor) {
		CAccess.Assert(a_oLog != null);
		CFunc.DoShowLog(LogType.Warning, a_oLog.ExGetColorFmtStr(a_stColor));
	}
	
	/** 에러 로그를 출력한다 */
	[Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
	public static void ShowLogError(string a_oLog) {
		CFunc.DoShowLog(LogType.Error, a_oLog);
	}

	/** 에러 로그를 출력한다 */
	[Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
	public static void ShowLogError(string a_oLog, Color a_stColor) {
		CAccess.Assert(a_oLog != null);
		CFunc.DoShowLog(LogType.Error, a_oLog.ExGetColorFmtStr(a_stColor));
	}

	/** 로그를 출력한다 */
	private static void DoShowLog(LogType a_eLogType, string a_oLog) {
		bool bIsValid = CFunc.m_oLogFuncDict.TryGetValue(a_eLogType, out System.Action<string> oLogFunc);
		CAccess.Assert(bIsValid);
		
		oLogFunc?.Invoke(a_oLog);
	}
	#endregion			// 클래스 함수

	#region 제네릭 클래스 함수
	/** 값을 교환한다 */
	public static void Swap<T>(ref T a_rtLhs, ref T a_rtRhs) {
		T tTemp = a_rtLhs; a_rtLhs = a_rtRhs; a_rtRhs = tTemp;
	}

	/** 값을 교환한다 */
	public static void LessCorrectSwap<T>(ref T a_rtLhs, ref T a_rtRhs) where T : System.IComparable<T> {
		// 보정이 필요 할 경우
		if(a_rtLhs.CompareTo(a_rtRhs) > KCDefine.B_COMPARE_EQUALS) {
			CFunc.Swap(ref a_rtLhs, ref a_rtRhs);
		}
	}

	/** 값을 교환한다 */
	public static void GreateCorrectSwap<T>(ref T a_rtLhs, ref T a_rtRhs) where T : System.IComparable<T> {
		// 보정이 필요 할 경우
		if(a_rtLhs.CompareTo(a_rtRhs) < KCDefine.B_COMPARE_EQUALS) {
			CFunc.Swap(ref a_rtLhs, ref a_rtRhs);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<TA>(ref System.Action<TA> a_rAction, TA a_tParamsA) {
		var oAction = a_rAction;

		try {
			a_rAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<TA, TB>(ref System.Action<TA, TB> a_rAction, TA a_tParamsA, TB a_tParamsB) {
		var oAction = a_rAction;

		try {
			a_rAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA, a_tParamsB);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<TA, TB, TC>(ref System.Action<TA, TB, TC> a_rAction, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC) {
		var oAction = a_rAction;

		try {
			a_rAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<TA, TB, TC, TD>(ref System.Action<TA, TB, TC, TD> a_rAction, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD) {
		var oAction = a_rAction;

		try {
			a_rAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<TA, TB, TC, TD, TE>(ref System.Action<TA, TB, TC, TD, TE> a_rAction, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE) {
		var oAction = a_rAction;

		try {
			a_rAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<TA, TB, TC, TD, TE, TF>(ref System.Action<TA, TB, TC, TD, TE, TF> a_rAction, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE, TF a_tParamsF) {
		var oAction = a_rAction;

		try {
			a_rAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<TA, TB, TC, TD, TE, TF, TG>(ref System.Action<TA, TB, TC, TD, TE, TF, TG> a_rAction, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE, TF a_tParamsF, TG a_tParamsG) {
		var oAction = a_rAction;

		try {
			a_rAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF, a_tParamsG);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<TA, TB, TC, TD, TE, TF, TG, TH>(ref System.Action<TA, TB, TC, TD, TE, TF, TG, TH> a_rAction, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE, TF a_tParamsF, TG a_tParamsG, TH a_tParamsH) {
		var oAction = a_rAction;

		try {
			a_rAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF, a_tParamsG, a_tParamsH);
		}
	}

	/** 함수를 호출한다 */
	public static void Invoke<TA, TB, TC, TD, TE, TF, TG, TH, TI>(ref System.Action<TA, TB, TC, TD, TE, TF, TG, TH, TI> a_rAction, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE, TF a_tParamsF, TG a_tParamsG, TH a_tParamsH, TI a_tParamsI) {
		var oAction = a_rAction;

		try {
			a_rAction = null;
		} finally {
			oAction?.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF, a_tParamsG, a_tParamsH, a_tParamsI);
		}
	}

	/** 함수를 호출한다 */
	public static Result Invoke<Result>(ref System.Func<Result> a_rFunc) {
		CAccess.Assert(a_rFunc != null);
		var oFunc = a_rFunc;

		try {
			a_rFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke();
	}

	/** 함수를 호출한다 */
	public static Result Invoke<TA, Result>(ref System.Func<TA, Result> a_rFunc, TA a_tParamsA) {
		CAccess.Assert(a_rFunc != null);
		var oFunc = a_rFunc;

		try {
			a_rFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<TA, TB, Result>(ref System.Func<TA, TB, Result> a_rFunc, TA a_tParamsA, TB a_tParamsB) {
		CAccess.Assert(a_rFunc != null);
		var oFunc = a_rFunc;

		try {
			a_rFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA, a_tParamsB);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<TA, TB, TC, Result>(ref System.Func<TA, TB, TC, Result> a_rFunc, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC) {
		CAccess.Assert(a_rFunc != null);
		var oFunc = a_rFunc;

		try {
			a_rFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<TA, TB, TC, TD, Result>(ref System.Func<TA, TB, TC, TD, Result> a_rFunc, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD) {
		CAccess.Assert(a_rFunc != null);
		var oFunc = a_rFunc;

		try {
			a_rFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<TA, TB, TC, TD, TE, Result>(ref System.Func<TA, TB, TC, TD, TE, Result> a_rFunc, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE) {
		CAccess.Assert(a_rFunc != null);
		var oFunc = a_rFunc;

		try {
			a_rFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<TA, TB, TC, TD, TE, TF, Result>(ref System.Func<TA, TB, TC, TD, TE, TF, Result> a_rFunc, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE, TF a_tParamsF) {
		CAccess.Assert(a_rFunc != null);
		var oFunc = a_rFunc;

		try {
			a_rFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<TA, TB, TC, TD, TE, TF, TG, Result>(ref System.Func<TA, TB, TC, TD, TE, TF, TG, Result> a_rFunc, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE, TF a_tParamsF, TG a_tParamsG) {
		CAccess.Assert(a_rFunc != null);
		var oFunc = a_rFunc;

		try {
			a_rFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF, a_tParamsG);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<TA, TB, TC, TD, TE, TF, TG, TH, Result>(ref System.Func<TA, TB, TC, TD, TE, TF, TG, TH, Result> a_rFunc, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE, TF a_tParamsF, TG a_tParamsG, TH a_tParamsH) {
		CAccess.Assert(a_rFunc != null);
		var oFunc = a_rFunc;

		try {
			a_rFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF, a_tParamsG, a_tParamsH);
	}

	/** 함수를 호출한다 */
	public static Result Invoke<TA, TB, TC, TD, TE, TF, TG, TH, TI, Result>(ref System.Func<TA, TB, TC, TD, TE, TF, TG, TH, TI, Result> a_rFunc, TA a_tParamsA, TB a_tParamsB, TC a_tParamsC, TD a_tParamsD, TE a_tParamsE, TF a_tParamsF, TG a_tParamsG, TH a_tParamsH, TI a_tParamsI) {
		CAccess.Assert(a_rFunc != null);
		var oFunc = a_rFunc;

		try {
			a_rFunc = null;
		} finally {
			// Do Something
		}

		return oFunc.Invoke(a_tParamsA, a_tParamsB, a_tParamsC, a_tParamsD, a_tParamsE, a_tParamsF, a_tParamsG, a_tParamsH, a_tParamsI);
	}

	/** 메세지 팩 객체를 읽어들인다 */
	public static T ReadMsgPackObj<T>(string a_oFilePath, System.Text.Encoding a_oEncoding = null, bool a_bIsSecurity = true) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oBytes = a_bIsSecurity ? CFunc.ReadSecurityBytes(a_oFilePath, a_oEncoding ?? System.Text.Encoding.Default) : CFunc.ReadBytes(a_oFilePath);

		return MessagePackSerializer.Deserialize<T>(oBytes);
	}

	/** 메세지 팩 객체를 읽어들인다 */
	public static T ReadMsgPackObjFromRes<T>(string a_oFilePath, System.Text.Encoding a_oEncoding = null, bool a_bIsSecurity = true) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oBytes = a_bIsSecurity ? CFunc.ReadSecurityBytesFromRes(a_oFilePath, a_oEncoding ?? System.Text.Encoding.Default) : CFunc.ReadBytesFromRes(a_oFilePath);

		return MessagePackSerializer.Deserialize<T>(oBytes);
	}

	/** 메세지 팩 JSON 객체를 읽어들인다 */
	public static T ReadMsgPackJSONObj<T>(string a_oFilePath, System.Text.Encoding a_oEncoding = null, bool a_bIsSecurity = true) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		string oStr = a_bIsSecurity ? CFunc.ReadSecurityStr(a_oFilePath, a_oEncoding ?? System.Text.Encoding.Default) : CFunc.ReadStr(a_oFilePath, a_oEncoding ?? System.Text.Encoding.Default);

		return oStr.ExMsgPackJSONStrToObj<T>();
	}
	
	/** 메세지 팩 JSON 객체를 읽어들인다 */
	public static T ReadMsgPackJSONObjFromRes<T>(string a_oFilePath, System.Text.Encoding a_oEncoding = null, bool a_bIsSecurity = true) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		string oStr = a_bIsSecurity ? CFunc.ReadSecurityStrFromRes(a_oFilePath, a_oEncoding ?? System.Text.Encoding.Default) : CFunc.ReadStrFromRes(a_oFilePath);

		return oStr.ExMsgPackJSONStrToObj<T>();
	}

	/** 메세지 팩 객체를 기록한다 */
	public static void WriteMsgPackObj<T>(string a_oFilePath, T a_oObj, System.Text.Encoding a_oEncoding = null, bool a_bIsSecurity = true, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || a_oFilePath.ExIsValid());

		// 경로가 유효 할 경우
		if(a_oFilePath.ExIsValid()) {
			var oBytes = MessagePackSerializer.Serialize<T>(a_oObj);

			// 보안 모드 일 경우
			if(a_bIsSecurity) {
				CFunc.WriteSecurityBytes(a_oFilePath, oBytes, a_oEncoding ?? System.Text.Encoding.Default, true, a_bIsEnableAssert);
			} else {
				CFunc.WriteBytes(a_oFilePath, oBytes, true, a_bIsEnableAssert);
			}
		}
	}

	/** 메세지 팩 JSON 객체를 기록한다 */
	public static void WriteMsgPackJSONObj<T>(string a_oFilePath, T a_oObj, System.Text.Encoding a_oEncoding = null, bool a_bIsSecurity = true, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || a_oFilePath.ExIsValid());

		// 경로가 유효 할 경우
		if(a_oFilePath.ExIsValid()) {
			string oStr = a_oObj.ExToMsgPackJSONStr();
			
			// 보안 모드 일 경우
			if(a_bIsSecurity) {
				CFunc.WriteSecurityStr(a_oFilePath, oStr, a_oEncoding ?? System.Text.Encoding.Default, true, a_bIsEnableAssert);
			} else {
				CFunc.WriteStr(a_oFilePath, oStr, a_oEncoding ?? System.Text.Encoding.Default, true, a_bIsEnableAssert);
			}
		}
	}
	#endregion			// 제네릭 클래스 함수

	#region 조건부 제네릭 클래스 함수
#if NEWTON_SOFT_JSON_MODULE_ENABLE
	/** JSON 객체를 읽어들인다 */
	public static T ReadJSONObj<T>(string a_oFilePath, System.Text.Encoding a_oEncoding = null, bool a_bIsSecurity = true) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		string oStr = a_bIsSecurity ? CFunc.ReadSecurityStr(a_oFilePath, a_oEncoding ?? System.Text.Encoding.Default) : CFunc.ReadStr(a_oFilePath, a_oEncoding ?? System.Text.Encoding.Default);

		return oStr.ExJSONStrToObj<T>();
	}

	/** JSON 객체를 읽어들인다 */
	public static T ReadJSONObjFromRes<T>(string a_oFilePath, System.Text.Encoding a_oEncoding = null, bool a_bIsSecurity = true) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		string oStr = a_bIsSecurity ? CFunc.ReadSecurityStrFromRes(a_oFilePath, a_oEncoding ?? System.Text.Encoding.Default) : CFunc.ReadStrFromRes(a_oFilePath);

		return oStr.ExJSONStrToObj<T>();
	}
	
	/** JSON 객체를 기록한다 */
	public static void WriteJSONObj<T>(string a_oFilePath, T a_oObj, System.Text.Encoding a_oEncoding = null, bool a_bIsNeedRoot = false, bool a_bIsPretty = false, bool a_bIsSecurity = true, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || a_oFilePath.ExIsValid());

		// 경로가 유효 할 경우
		if(a_oFilePath.ExIsValid()) {
			string oStr = a_oObj.ExToJSONStr(a_bIsNeedRoot, a_bIsPretty);

			// 보안 모드 일 경우
			if(a_bIsSecurity) {
				CFunc.WriteSecurityStr(a_oFilePath, oStr, a_oEncoding ?? System.Text.Encoding.Default, true, a_bIsEnableAssert);
			} else {
				CFunc.WriteStr(a_oFilePath, oStr, a_oEncoding ?? System.Text.Encoding.Default, true, a_bIsEnableAssert);
			}
		}
	}
#endif			// #if NEWTON_SOFT_JSON_MODULE_ENABLE
	#endregion			// 조건부 제네릭 클래스 함수
}
