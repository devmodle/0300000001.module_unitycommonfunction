using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using System.Diagnostics;

/** 로그 함수 */
public static partial class CFunc {
	#region 클래스 변수
	private static Dictionary<LogType, System.Action<string>> m_oLogFuncDict = new Dictionary<LogType, System.Action<string>>() {
		[LogType.Log] = UnityEngine.Debug.Log,
		[LogType.Warning] = UnityEngine.Debug.LogWarning,
		[LogType.Error] = UnityEngine.Debug.LogError
	};
	#endregion // 클래스 변수

	#region 클래스 함수
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
	#endregion // 클래스 함수
}
