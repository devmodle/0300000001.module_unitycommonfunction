using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

//! 에디터 기본 함수
public static partial class CEditorFunc {
	#region 클래스 함수
	//! 에셋을 로드한다
	public static Object LoadAsset(string a_oFilepath) {
		CAccess.Assert(a_oFilepath.ExIsValid());
		var oAssets = AssetDatabase.LoadAllAssetsAtPath(a_oFilepath);

		return oAssets.ExIsValid() ? oAssets.First() : null;
	}

	//! 알림 팝업을 출력한다
	public static bool ShowAlertPopup(string a_oTitle,
		string a_oMsg, string a_oOKBtnText, string a_oCancelBtnText) {
		if(!a_oCancelBtnText.ExIsValid()) {
			return EditorUtility.DisplayDialog(a_oTitle, a_oMsg, a_oOKBtnText);
		}

		return EditorUtility.DisplayDialog(a_oTitle, a_oMsg, a_oOKBtnText, a_oCancelBtnText);
	}

	//! 에셋 데이터 베이스를 갱신한다
	public static void UpdateAssetDatabaseState() {
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
	
	//! 커맨드 라인을 실행한다
	public static void ExecuteCmdline(string a_oParams) {
		CAccess.Assert(a_oParams.ExIsValid());

		if(CAccess.IsMacPlatform()) {
			CEditorFunc.ExecuteCmdline(KCEditorDefine.B_TOOL_PATH_SHELL,
				string.Format(KCEditorDefine.B_CMDLINE_PARAM_FORMAT_SHELL, a_oParams));
		} else if(CAccess.IsWindowsPlatform()) {
			CEditorFunc.ExecuteCmdline(KCEditorDefine.B_TOOL_PATH_CMD_PROMPT,
				string.Format(KCEditorDefine.B_CMDLINE_PARAM_FORMAT_CMD_PROMPT, a_oParams));
		}
	}

	//! 커맨드 라인을 실행한다
	public static void ExecuteCmdline(string a_oFilepath, string a_oParams) {
		CAccess.Assert(a_oFilepath.ExIsValid() && a_oParams.ExIsValid());

		var oStartInfo = new ProcessStartInfo(a_oFilepath, a_oParams);
		oStartInfo.UseShellExecute = true;

		Process.Start(oStartInfo);
	}
	#endregion			// 클래스 함수

	#region 제네릭 클래스 함수
	//! 에셋을 탐색한다
	public static T FindAsset<T>(string a_oFilter, string[] a_oSearchPaths) where T : Object {
		var oAssets = CEditorFunc.FindAssets<T>(a_oFilter, a_oSearchPaths);
		return oAssets.ExIsValid() ? oAssets.First() : null;
	}

	//! 에셋을 탐색한다
	public static List<T> FindAssets<T>(string a_oFilter, string[] a_oSearchPaths) where T : Object {
		CAccess.Assert(a_oFilter.ExIsValid() && a_oSearchPaths.ExIsValid());

		var oAssetList = new List<T>();
		var oAssetGUIDs = AssetDatabase.FindAssets(a_oFilter, a_oSearchPaths);

		for(int i = 0; i < oAssetGUIDs?.Length; ++i) {
			string oPath = AssetDatabase.GUIDToAssetPath(oAssetGUIDs[i]);
			var oAsset = AssetDatabase.LoadAssetAtPath<T>(oPath);

			if(oAsset != null) {
				oAssetList.Add(oAsset);
			}
		}

		return oAssetList;
	}
	#endregion			// 제네릭 클래스 함수
}
#endif			// #if UNITY_EDITOR
