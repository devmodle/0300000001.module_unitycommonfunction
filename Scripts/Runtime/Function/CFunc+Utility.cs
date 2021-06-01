using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif			// #if UNITY_EDITOR

#if UNITY_ANDROID
using UnityEngine.Android;
#endif			// #if UNITY_ANDROID

//! 유틸리티 함수
public static partial class CFunc {
	#region 클래스 함수
	//! 객체를 탐색한다
	public static GameObject FindObj(string a_oName) {
		CAccess.Assert(a_oName.ExIsValid());
		GameObject oObj = null;

		CFunc.EnumerateScenes((a_stScene) => {
			oObj = a_stScene.ExFindChild(a_oName);
			return oObj == null;
		});

		return oObj;
	}

	//! 객체를 탐색한다
	public static List<GameObject> FindObjs(string a_oName) {
		CAccess.Assert(a_oName.ExIsValid());
		var oObjList = new List<GameObject>();

		CFunc.EnumerateScenes((a_stScene) => {
			var oChildObjList = a_stScene.ExFindChildren(a_oName);
			oObjList.AddRange(oChildObjList);
			
			return true;
		});
		
		return oObjList;
	}

	//! 메세지를 전송한다
	public static void SendMsg(string a_oName, string a_oMsg, object a_oParams) {
		CAccess.Assert(a_oName.ExIsValid() && a_oMsg.ExIsValid());
		var oObj = CFunc.FindObj(a_oName);

		oObj?.SendMessage(a_oMsg, a_oParams, SendMessageOptions.DontRequireReceiver);
	}

	//! 메세지를 전파한다
	public static void BroadcastMsg(string a_oMsg, object a_oParams) {
		CAccess.Assert(a_oMsg.ExIsValid());

		CFunc.EnumerateScenes((a_stScene) => {
			a_stScene.ExBroadcastMsg(a_oMsg, a_oParams);
			return true;
		});
	}

	//! 권한을 요청한다
	public static void RequestPermission(MonoBehaviour a_oComponent, string a_oPermission, System.Action<string, bool> a_oCallback, bool a_bIsRealtime = false) {
		CAccess.Assert(a_oComponent != null && a_oPermission.ExIsValid());

#if UNITY_ANDROID
		// 권한이 유효 할 경우
		if(CAccess.IsEnablePermission(a_oPermission)) {
			a_oCallback?.Invoke(a_oPermission, true);
		} else {
			Permission.RequestUserPermission(a_oPermission);

			float fDeltaTime = KCDefine.U_DELTA_T_PERMISSION_M_REQUEST_CHECK;
			float fMaxDeltaTime = KCDefine.U_MAX_DELTA_T_PERMISSION_M_REQUEST_CHECK;
			
			a_oComponent.ExRepeatCallFunc((a_oSender, a_oParams, a_bIsComplete) => {
				// 요청이 완료 되었을 경우
				if(a_bIsComplete) {
					a_oCallback?.Invoke(a_oPermission, CAccess.IsEnablePermission(a_oPermission));
				}

				return !CAccess.IsEnablePermission(a_oPermission);
			}, fDeltaTime, fMaxDeltaTime, a_bIsRealtime);
		}
#else
		a_oCallback?.Invoke(a_oPermission, false);
#endif			// #if UNITY_ANDROID
	}

	//! 씬을 순회한다
	public static void EnumerateScenes(System.Func<Scene, bool> a_oCallback) {
		CAccess.Assert(a_oCallback != null);

		for(int i = 0; i < SceneManager.sceneCount; ++i) {
			var stScene = SceneManager.GetSceneAt(i);

			// 씬 순회가 불가능 할 경우
			if(!a_oCallback(stScene)) {
				break;
			}
		}
	}
	#endregion			// 클래스 함수

	#region 제네릭 클래스 함수
	//! 컴포넌트를 탐색한다
	public static T FindComponent<T>(string a_oName) where T : Component {
		CAccess.Assert(a_oName.ExIsValid());
		var oObj = CFunc.FindObj(a_oName);

		return oObj?.GetComponentInChildren<T>();
	}

	//! 컴포넌트를 탐색한다
	public static T[] FindComponents<T>(string a_oName) where T : Component {
		CAccess.Assert(a_oName.ExIsValid());
		var oObj = CFunc.FindObj(a_oName);
		
		return oObj?.GetComponentsInChildren<T>();
	}
	#endregion			// 제네릭 클래스 함수

	#region 조건부 클래스 함수
#if UNITY_EDITOR
	//! 객체를 선택한다
	public static void SelObj(GameObject a_oObj, bool a_bIsPing = false) {
		Selection.activeGameObject = a_oObj;

		// 핑 모드 일 경우
		if(a_bIsPing && a_oObj != null) {
			EditorGUIUtility.PingObject(a_oObj);
		}
	}

	//! 객체를 선택한다
	public static void SelObjs(GameObject[] a_oObjs, bool a_bIsPing = false) {
		Selection.objects = a_oObjs;

		// 핑 모드 일 경우
		if(a_bIsPing && a_oObjs.ExIsValid()) {
			EditorGUIUtility.PingObject(a_oObjs[KCDefine.B_VAL_0_INT]);
		}
	}
#endif			// #if UNITY_EDITOR
	#endregion			// 조건부 클래스 함수
}
