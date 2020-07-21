using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif			// #if UNITY_EDITOR

//! 유틸리티 함수
public static partial class CFunc {
	#region 클래스 함수
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
	public static void SelectObj(GameObject a_oObj, bool a_bIsEnablePing = false) {
		CAccess.Assert(a_oObj != null);
		Selection.activeGameObject = a_oObj;

		if(a_bIsEnablePing) {
			EditorGUIUtility.PingObject(a_oObj);
		}
	}
#endif			// #if UNITY_EDITOR

#if MESSAGE_PACK_ENABLE
	//! 버전 정보를 생성한다
	public static STVersionInfo MakeDefVersionInfo(string a_oVersion) {
		return new STVersionInfo() {
			m_oVersion = a_oVersion,
			m_oExtraInfoList = new Dictionary<string, string>()
		};
	}
#endif			// #if MESSAGE_PACK_ENABLE
	#endregion			// 조건부 클래스 함수
}
