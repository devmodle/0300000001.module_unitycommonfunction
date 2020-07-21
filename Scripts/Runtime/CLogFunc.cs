using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! 기본 로그 함수
public static partial class CLogFunc {
	#region 클래스 함수
	//! 어플리케이션 시작 로그를 전송한다
	public static void SendAppLaunchLog() {
#if FLURRY_ENABLE && FLURRY_ANALYTICS_ENABLE
		CFlurryManager.Instance.SendLog(KCDefine.LOG_NAME_APP_LAUNCH, null);
#endif			// #if FLURRY_ENABLE && FLURRY_ANALYTICS_ENABLE

#if FACEBOOK_ENABLE && FACEBOOK_ANALYTICS_ENABLE
		CFacebookManager.Instance.SendLog(KCDefine.LOG_NAME_APP_LAUNCH, null);
#endif			// #if FACEBOOK_ENABLE && FACEBOOK_ANALYTICS_ENABLE

#if FIREBASE_ENABLE && FIREBASE_ANALYTICS_ENABLE
		CFirebaseManager.Instance.SendLog(KCDefine.LOG_NAME_APP_LAUNCH, KCDefine.LOG_KEY_USER_INFO, null);
#endif			// #if FIREBASE_ENABLE && FIREBASE_ANALYTICS_ENABLE

#if UNITY_SERVICE_ENABLE && UNITY_SERVICE_ANALYTICS_ENABLE
		CUnityServiceManager.Instance.SendLog(KCDefine.LOG_NAME_APP_LAUNCH, null);
#endif			// #if UNITY_SERVICE_ENABLE && UNITY_SERVICE_ANALYTICS_ENABLE
	}
	#endregion			// 클래스 함수
}
