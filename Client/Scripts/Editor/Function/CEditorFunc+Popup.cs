using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;

/** 에디터 함수 - 팝업 */
public static partial class CEditorFunc {
	#region 클래스 함수
	/** 경고 팝업을 출력한다 */
	public static bool ShowOKAlertPopup(string a_oTitle, string a_oMsg) {
		return CEditorFunc.ShowAlertPopup(a_oTitle, a_oMsg, KCEditorDefine.B_TEXT_OK, string.Empty);
	}

	/** 경고 팝업을 출력한다 */
	public static bool ShowOKCancelAlertPopup(string a_oTitle, string a_oMsg) {
		return CEditorFunc.ShowAlertPopup(a_oTitle, a_oMsg, KCEditorDefine.B_TEXT_OK, KCEditorDefine.B_TEXT_CANCEL);
	}
	#endregion // 클래스 함수
}

/** 에디터 함수 - 팝업 (Private) */
public static partial class CEditorFunc {
	#region 클래스 함수
	/** 경고 팝업을 출력한다 */
	private static bool ShowAlertPopup(string a_oTitle, string a_oMsg, string a_oOKBtnText, string a_oCancelBtnText) {
		// 취소 버튼 텍스트가 유효 할 경우
		if(a_oCancelBtnText.ExIsValid()) {
			return EditorUtility.DisplayDialog(a_oTitle, a_oMsg, a_oOKBtnText, a_oCancelBtnText);
		}

		return EditorUtility.DisplayDialog(a_oTitle, a_oMsg, a_oOKBtnText);
	}
	#endregion // 클래스 함수
}
#endif // #if UNITY_EDITOR
