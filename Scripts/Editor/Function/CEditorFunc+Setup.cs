using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

//! 에디터 설정 함수
public static partial class CEditorFunc {
	#region 클래스 함수
	//! 전처리기 심볼을 설정한다
	public static void SetupDefineSymbols(Dictionary<BuildTargetGroup, List<string>> a_oDefineSymbolListContainer) {
		CAccess.Assert(a_oDefineSymbolListContainer.ExIsValid());
		
		foreach(var stKeyValue in a_oDefineSymbolListContainer) {
			var oDefineSymbolList = new List<string>();

			for(int i = 0; i < stKeyValue.Value.Count; ++i) {
				oDefineSymbolList.ExAddValue(stKeyValue.Value[i]);
			}

			PlayerSettings.SetScriptingDefineSymbolsForGroup(stKeyValue.Key, oDefineSymbolList.ExToString(KCEditorDefine.B_TOKEN_DEFINE_SYMBOL));
		}
	}
	#endregion			// 클래스 함수
}
#endif			// #if UNITY_EDITOR
