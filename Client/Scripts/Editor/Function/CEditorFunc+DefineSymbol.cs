using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;

/** 에디터 함수 - 전처리기 심볼 */
public static partial class CEditorFunc {
	#region 클래스 함수
	/** 전처리기 심볼을 설정한다 */
	public static void SetupDefineSymbols(Dictionary<BuildTargetGroup, List<string>> a_oDefineSymbolDictContainer, 
		bool a_bIsAssert = true) {

		CAccess.Assert(!a_bIsAssert || a_oDefineSymbolDictContainer.ExIsValid());

		// 설정이 불가능 할 경우
		if(!a_oDefineSymbolDictContainer.ExIsValid()) {
			return;
		}

		foreach(var stKeyVal in a_oDefineSymbolDictContainer) {
			PlayerSettings.SetScriptingDefineSymbolsForGroup(stKeyVal.Key, stKeyVal.Value.ToArray());
		}
	}
	#endregion // 클래스 함수
}
#endif // #if UNITY_EDITOR
