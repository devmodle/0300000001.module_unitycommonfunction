using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif			// #if UNITY_EDITOR

//! 유틸리티 함수
public static partial class CFunc {
	#region 클래스 함수
	//! 앱을 종료한다
	public static void QuitApplication(int a_nExitCode = 0) {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.ExitPlaymode();
#else
		Application.Quit(a_nExitCode);
#endif			// #if UNITY_EDITOR
	}

	//! 객체를 탐색한다
	public static GameObject FindObj(string a_oName) {
		CAccess.Assert(a_oName.ExIsValid());
		GameObject oObj = null;

		CFunc.EnumerateScenes((a_stScene) => {
			oObj = (oObj != null) ? oObj : a_stScene.ExFindChild(a_oName);
		});

		return oObj;
	}

	//! 객체를 탐색한다
	public static List<GameObject> FindObjs(string a_oName) {
		CAccess.Assert(a_oName.ExIsValid());
		var oObjList = new List<GameObject>();
		
		CFunc.EnumerateScenes((a_stScene) => {
			var oChildObjList = a_stScene.ExFindChildren(a_oName);

			// 자식 객체가 존재 할 경우
			if(oChildObjList != null) {
				oObjList.AddRange(oChildObjList);
			}
		});
		
		return oObjList;
	}
	
	//! 메세지를 전송한다
	public static void SendMsg(string a_oName, string a_oMsg, object a_oParams) {
		var oObj = CFunc.FindObj(a_oName);
		oObj?.SendMessage(a_oMsg, a_oParams, SendMessageOptions.DontRequireReceiver);
	}

	//! 메세지를 전파한다
	public static void BroadcastMsg(string a_oMsg, object a_oParams) {
		CFunc.EnumerateScenes((a_stScene) => {
			a_stScene.ExBroadcastMsg(a_oMsg, a_oParams);
		});
	}

	//! 씬을 순회한다
	public static void EnumerateScenes(System.Action<Scene> a_oCallback) {
		for(int i = 0; i < SceneManager.sceneCount; ++i) {
			a_oCallback?.Invoke(SceneManager.GetSceneAt(i));
		}
	}

	//! 함수를 지연 호출한다
	public static void LateCallFunc(MonoBehaviour a_oComponent,
		System.Action<MonoBehaviour, object[]> a_oCallback, object[] a_oParams = null) {
		a_oComponent?.StartCoroutine(CFunc.DoLateCallFunc(a_oComponent, a_oCallback, a_oParams));
	}

	//! 함수를 지연 호출한다
	public static void LateCallFunc(MonoBehaviour a_oComponent,
		float a_fDelay, System.Action<MonoBehaviour, object[]> a_oCallback, bool a_bIsRealtime = false, object[] a_oParams = null) {
		a_oComponent?.StartCoroutine(CFunc.DoLateCallFunc(a_oComponent, a_fDelay, a_oCallback, a_bIsRealtime, a_oParams));
	}

	//! 함수를 반복 호출한다
	public static void RepeatCallFunc(MonoBehaviour a_oComponent,
		float a_fDeltaTime, float a_fMaxDeltaTime, System.Func<MonoBehaviour, object[], bool, bool> a_oCallback, bool a_bIsRealtime = false, object[] a_oParams = null) {
		CAccess.Assert(a_oCallback != null);
		a_oComponent?.StartCoroutine(CFunc.DoRepeatCallFunc(a_oComponent, a_fDeltaTime, a_fMaxDeltaTime, a_oCallback, a_bIsRealtime, a_oParams));
	}

	//! 함수를 지연 호출한다
	private static IEnumerator DoLateCallFunc(MonoBehaviour a_oComponent,
		System.Action<MonoBehaviour, object[]> a_oCallback, object[] a_oParams) {
		yield return new WaitForEndOfFrame();
		a_oCallback?.Invoke(a_oComponent, a_oParams);
	}

	//! 함수를 지연 호출한다
	private static IEnumerator DoLateCallFunc(MonoBehaviour a_oComponent,
		float a_fDelay, System.Action<MonoBehaviour, object[]> a_oCallback, bool a_bIsRealtime, object[] a_oParams) {
		yield return CFactory.CreateWaitForSeconds(a_fDelay, a_bIsRealtime);
		a_oCallback?.Invoke(a_oComponent, a_oParams);
	}

	//! 함수를 반복 호출한다
	private static IEnumerator DoRepeatCallFunc(MonoBehaviour a_oComponent,
		float a_fDeltaTime, double a_dblMaxDeltaTime, System.Func<MonoBehaviour, object[], bool, bool> a_oCallback, bool a_bIsRealtime, object[] a_oParams) {
		var stStartTime = System.DateTime.Now;
		System.TimeSpan stDeltaTime;
		
		do {
			yield return CFactory.CreateWaitForSeconds(a_fDeltaTime, a_bIsRealtime);
			stDeltaTime = System.DateTime.Now - stStartTime;
		} while(a_oCallback(a_oComponent, a_oParams, false) && stDeltaTime.TotalSeconds.ExIsLess(a_dblMaxDeltaTime));

		a_oCallback(a_oComponent, a_oParams, true);
	}
	#endregion			// 클래스 함수

	#region 제네릭 클래스 함수
	//! 컴포넌트를 탐색한다
	public static T FindComponent<T>(string a_oName) where T : Component {
		var oObj = CFunc.FindObj(a_oName);
		return oObj?.GetComponentInChildren<T>();
	}

	//! 컴포넌트를 탐색한다
	public static T[] FindComponents<T>(string a_oName) where T : Component {
		var oObj = CFunc.FindObj(a_oName);
		return oObj?.GetComponentsInChildren<T>();
	}
	#endregion			// 제네릭 클래스 함수

	#region 조건부 클래스 함수
#if UNITY_EDITOR
	//! 객체를 선택한다
	public static void SelectObj(GameObject a_oObj, bool a_bIsPing = false) {
		CAccess.Assert(a_oObj != null);
		Selection.activeGameObject = a_oObj;

		// 핑 모드 일 경우
		if(a_bIsPing) {
			EditorGUIUtility.PingObject(a_oObj);
		}
	}

	//! 객체를 선택한다
	public static void SelectObjs(GameObject[] a_oObjs, bool a_bIsPing = false) {
		CAccess.Assert(a_oObjs.ExIsValid());
		Selection.objects = a_oObjs;

		// 핑 모드 일 경우
		if(a_bIsPing) {
			EditorGUIUtility.PingObject(a_oObjs.Last());
		}
	}
#endif			// #if UNITY_EDITOR

#if MSG_PACK_ENABLE
	//! 버전 정보를 생성한다
	public static STVersionInfo MakeDefVersionInfo(string a_oVersion) {
		return new STVersionInfo() {
			m_oVersion = a_oVersion,
			m_oExtraInfoList = new Dictionary<string, string>()
		};
	}
#endif			// #if MSG_PACK_ENABLE
	#endregion			// 조건부 클래스 함수
}
