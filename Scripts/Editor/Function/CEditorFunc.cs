using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

//! 에디터 기본 함수
public static partial class CEditorFunc {
	#region 클래스 함수
	//! 에셋을 로드한다
	public static Object LoadAsset(string a_oFilePath) {
		CAccess.Assert(a_oFilePath.ExIsValid());
		var oAssets = AssetDatabase.LoadAllAssetsAtPath(a_oFilePath);

		return oAssets.ExIsValid() ? oAssets[KCDefine.B_VAL_0_INT] : null;
	}

	//! 경고 팝업을 출력한다
	public static bool ShowAlertPopup(string a_oTitle, string a_oMsg, string a_oOKBtnText, string a_oCancelBtnText = KCDefine.B_EMPTY_STR) {
		// 취소 버튼 텍스트가 유효 할 경우
		if(a_oCancelBtnText.ExIsValid()) {
			return EditorUtility.DisplayDialog(a_oTitle, a_oMsg, a_oOKBtnText, a_oCancelBtnText);
		}

		return EditorUtility.DisplayDialog(a_oTitle, a_oMsg, a_oOKBtnText);
	}

	//! 에셋 데이터 베이스를 갱신한다
	public static void UpdateAssetDBState() {
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
	
	//! 커맨드 라인을 실행한다
	public static void ExecuteCmdLine(string a_oParams, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || a_oParams.ExIsValid());

		// 매개 변수가 유효 할 경우
		if(a_oParams.ExIsValid()) {
			// 맥 일 경우
			if(CAccess.IsMac) {
				CEditorFunc.ExecuteCmdLine(KCEditorDefine.B_TOOL_P_SHELL, string.Format(KCEditorDefine.B_CMD_LINE_PARAMS_FMT_SHELL, a_oParams));
			}
			// 윈도우즈 일 경우
			else if(CAccess.IsWnds) {
				CEditorFunc.ExecuteCmdLine(KCEditorDefine.B_TOOL_P_CMD_PROMPT, string.Format(KCEditorDefine.B_CMD_LINE_PARAMS_FMT_CMD_PROMPT, a_oParams));
			}
		}
	}

	//! 커맨드 라인을 실행한다
	public static void ExecuteCmdLine(string a_oFilePath, string a_oParams, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || (a_oFilePath.ExIsValid() && a_oParams.ExIsValid()));
		CFunc.ShowLog($"CEditorFunc.ExecuteCmdLine: {a_oFilePath}, {a_oParams}");

		// 커맨드 라인 실행이 가능 할 경우
		if(a_oFilePath.ExIsValid() && a_oParams.ExIsValid()) {
			var oStartInfo = new ProcessStartInfo(a_oFilePath, a_oParams);
			oStartInfo.UseShellExecute = true;

			Process.Start(oStartInfo);
		}
	}

	//! 플랫폼을 변경한다
	public static void ChangePlatform(BuildTargetGroup a_eTargetGroup, BuildTarget a_eTarget) {
		EditorUserBuildSettings.SwitchActiveBuildTarget(a_eTargetGroup, a_eTarget);
	}
	#endregion			// 클래스 함수

	#region 제네릭 클래스 함수
	//! 에셋을 탐색한다
	public static T FindAsset<T>(string a_oFilePath) where T : Object {
		CAccess.Assert(a_oFilePath.ExIsValid());
		return AssetDatabase.LoadAssetAtPath<T>(a_oFilePath);
	}

	//! 에셋을 탐색한다
	public static T FindAsset<T>(string a_oFilter, string[] a_oSearchPaths) where T : Object {
		var oAssets = CEditorFunc.FindAssets<T>(a_oFilter, a_oSearchPaths);
		return oAssets.ExIsValid() ? oAssets[KCDefine.B_VAL_0_INT] : null;
	}

	//! 에셋을 탐색한다
	public static List<T> FindAssets<T>(string a_oFilter, string[] a_oSearchPaths) where T : Object {
		var oAssetList = new List<T>();
		var oAssetGUIDs = AssetDatabase.FindAssets(a_oFilter, a_oSearchPaths);

		// 에셋 GUID 가 존재 할 경우
		if(oAssetGUIDs.ExIsValid()) {
			for(int i = 0; i < oAssetGUIDs.Length; ++i) {
				string oPath = AssetDatabase.GUIDToAssetPath(oAssetGUIDs[i]);
				var oAsset = AssetDatabase.LoadAssetAtPath<T>(oPath);

				// 에셋이 존재 할 경우
				if(oAsset != null) {
					oAssetList.ExAddVal(oAsset);
				}
			}
		}

		return oAssetList;
	}
	#endregion			// 제네릭 클래스 함수
}
#endif			// #if UNITY_EDITOR
