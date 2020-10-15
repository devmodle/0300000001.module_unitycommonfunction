using System.Collections;
using System.Collections.Generic;
using System.IO;
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
	//! 앱을 종료한다
	public static void QuitApplication(int a_nExitCode = KCDefine.B_ZERO_VALUE_INT) {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.ExitPlaymode();
#else
		Application.Quit(a_nExitCode);
#endif			// #if UNITY_EDITOR
	}

	//! 객체를 탐색한다
	public static GameObject FindObj(string a_oName) {
		GameObject oObj = null;

		CFunc.EnumerateScenes((a_stScene) => 
			oObj = (oObj != null) ? oObj : a_stScene.ExFindChild(a_oName));

		return oObj;
	}

	//! 객체를 탐색한다
	public static List<GameObject> FindObjs(string a_oName) {
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
		CFunc.EnumerateScenes((a_stScene) => a_stScene.ExBroadcastMsg(a_oMsg, a_oParams));
	}

	//! 권한을 요청한다
	public static void RequestPermission(MonoBehaviour a_oComponent, 
		string a_oPermission, System.Action<string, bool> a_oCallback) 
	{
		CAccess.Assert(a_oComponent != null && a_oPermission.ExIsValid());

#if UNITY_ANDROID
		// 권한이 유효 할 경우
		if(CAccess.IsEnablePermission(a_oPermission)) {
			a_oCallback?.Invoke(a_oPermission, true);
		} else {
			Permission.RequestUserPermission(a_oPermission);

			float fDeltaTime = KCDefine.U_DELTA_TIME_PERMISSION_M_REQUEST_CHECK;
			float fMaxDeltaTime = KCDefine.U_MAX_DELTA_TIME_PERMISSION_M_REQUEST_CHECK;
			
			a_oComponent.ExRepeatCallFunc(fDeltaTime, fMaxDeltaTime, (a_oSender, a_oParams, a_bIsComplete) => {
				// 요청이 완료 되었을 경우
				if(a_bIsComplete) {
					a_oCallback?.Invoke(a_oPermission, CAccess.IsEnablePermission(a_oPermission));
				}

				return !CAccess.IsEnablePermission(a_oPermission);
			});
		}
#else
		a_oCallback?.Invoke(a_oPermission, false);
#endif			// #if UNITY_ANDROID
	}

	//! 씬을 순회한다
	public static void EnumerateScenes(System.Action<Scene> a_oCallback) {
		for(int i = KCDefine.B_INDEX_START; i < SceneManager.sceneCount; ++i) {
			a_oCallback?.Invoke(SceneManager.GetSceneAt(i));
		}
	}

	//! 버전을 생성한다
	public static STVersion MakeDefVersion(string a_oVersion) {
		return new STVersion() {
			m_oVersion = a_oVersion,
			m_oExtraInfoList = new Dictionary<string, string>()
		};
	}

	//! 지역화 파일 경로를 생성한다
	public static string MakeLocalizeFilepath(string a_oBaseFilepath, string a_oLanguage) {
		var oFilename = Path.GetFileNameWithoutExtension(a_oBaseFilepath);
		var oLocalizeFilename = string.Format(KCDefine.B_FILENAME_FORMAT_LOCALIZE, oFilename, a_oLanguage);

		return a_oBaseFilepath.ExGetReplaceFilenamePath(oLocalizeFilename);
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
		Selection.activeGameObject = a_oObj;

		// 핑 모드 일 경우
		if(a_bIsPing) {
			EditorGUIUtility.PingObject(a_oObj);
		}
	}

	//! 객체를 선택한다
	public static void SelectObjs(GameObject[] a_oObjs, bool a_bIsPing = false) {
		Selection.objects = a_oObjs;

		// 핑 모드 일 경우
		if(a_bIsPing) {
			EditorGUIUtility.PingObject(a_oObjs[KCDefine.B_INDEX_START]);
		}
	}
#endif			// #if UNITY_EDITOR
	#endregion			// 조건부 클래스 함수
}
