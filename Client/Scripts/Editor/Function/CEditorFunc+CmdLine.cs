using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using System.Diagnostics;
using UnityEditor;

/** 에디터 함수 - 커맨드 라인 */
public static partial class CEditorFunc {
	#region 클래스 함수
	/** 커맨드 라인을 실행한다 */
	public static void ExecuteCmdLine(string a_oParams, bool a_bIsAsync = true, bool a_bIsAssert = true) {
		CAccess.Assert(!a_bIsAssert || a_oParams.ExIsValid());

		// 매개 변수가 유효 할 경우
		if(a_oParams.ExIsValid()) {
#if UNITY_EDITOR_WIN
			string oParams = string.Format(KCEditorDefine.B_CMD_LINE_PARAMS_FMT_CMD_PROMPT, a_oParams);
			CEditorFunc.ExecuteCmdLine(KCEditorDefine.B_TOOL_P_CMD_PROMPT, oParams, a_bIsAsync, a_bIsAssert);
#else
			string oPath = CEditorAccess.IsAppleMSeries ? 
				KCEditorDefine.B_BUILD_CMD_SILICON_EXPORT_PATH : KCEditorDefine.B_BUILD_CMD_INTEL_EXPORT_PATH;

			string oParams = string.Format(KCDefine.B_TEXT_FMT_2_SEMI_COLON_COMBINE, oPath, a_oParams);
			oParams = string.Format(KCEditorDefine.B_CMD_LINE_PARAMS_FMT_SHELL, oParams);

			CEditorFunc.ExecuteCmdLine(KCEditorDefine.B_TOOL_P_SHELL, oParams, a_bIsAsync, a_bIsAssert);
#endif // #if UNITY_EDITOR_WIN
		}
	}
	#endregion // 클래스 함수
}

/** 에디터 함수 - 커맨드 라인 (Private) */
public static partial class CEditorFunc {
	#region 클래스 함수
	/** 커맨드 라인을 실행한다 */
	private static void ExecuteCmdLine(string a_oFilePath, string a_oParams, bool a_bIsAsync = true, bool a_bIsAssert = true) {
		CFunc.ShowLog($"CEditorFunc.ExecuteCmdLine: {a_oFilePath}, {a_oParams}");
		CAccess.Assert(!a_bIsAssert || (a_oFilePath.ExIsValid() && a_oParams.ExIsValid()));

		// 실행이 불가능 할 경우
		if(!a_oFilePath.ExIsValid() || !a_oParams.ExIsValid()) {
			return;
		}

		var oProcess = Process.Start(CEditorFactory.MakeProcessStartInfo(a_oFilePath, a_oParams));

		// 동기 모드 일 경우
		if(!a_bIsAsync) {
			oProcess?.WaitForExit();
		}
	}
	#endregion // 클래스 함수
}
#endif // #if UNITY_EDITOR
