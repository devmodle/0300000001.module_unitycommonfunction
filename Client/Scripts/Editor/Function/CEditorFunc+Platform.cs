using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;

/** 에디터 함수 - 플랫폼 */
public static partial class CEditorFunc {
	#region 클래스 함수
	/** 플랫폼을 변경한다 */
	public static void ChangePlatform(BuildTargetGroup a_eTargetGroup, BuildTarget a_eTarget) {
		EditorUserBuildSettings.SwitchActiveBuildTarget(a_eTargetGroup, a_eTarget);
	}
	#endregion // 클래스 함수
}
#endif // #if UNITY_EDITOR
