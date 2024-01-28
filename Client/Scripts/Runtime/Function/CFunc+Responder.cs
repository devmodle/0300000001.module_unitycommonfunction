using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/** 응답자 함수 */
public static partial class CFunc {
	#region 제네릭 클래스 함수
	/** 터치 응답자를 설정한다 */
	public static void SetupTouchResponders<K>(List<(K, string, GameObject, GameObject)> a_oKeyInfoList, Vector3 a_stSize, Dictionary<K, GameObject> a_oOutObjDict, bool a_bIsAssert = true) {
		CAccess.Assert(!a_bIsAssert || (a_oKeyInfoList.ExIsValid() && a_oOutObjDict != null));

		// 키 정보가 존재 할 경우
		if(a_oKeyInfoList.ExIsValid() && a_oOutObjDict != null) {
			for(int i = 0; i < a_oKeyInfoList.Count; ++i) {
				var oTouchResponder = a_oKeyInfoList[i].Item3?.ExFindChild(a_oKeyInfoList[i].Item2);

				// 터치 응답자가 존재 할 경우
				if(oTouchResponder != null) {
					a_oOutObjDict.ExReplaceVal(a_oKeyInfoList[i].Item1, oTouchResponder);
				} else {
					a_oOutObjDict.ExReplaceVal(a_oKeyInfoList[i].Item1, CFactory.CreateTouchResponder(a_oKeyInfoList[i].Item2, a_oKeyInfoList[i].Item4, a_oKeyInfoList[i].Item3, a_stSize, Vector3.zero, KCDefine.U_COLOR_TRANSPARENT));
				}

				a_oOutObjDict.GetValueOrDefault(a_oKeyInfoList[i].Item1)?.ExSetRaycastTarget<Image>(true, a_bIsAssert);
			}
		}
	}

	/** 드래그 응답자를 설정한다 */
	public static void SetupDragResponders<K>(List<(K, string, GameObject, GameObject)> a_oKeyInfoList, Vector3 a_stSize, Dictionary<K, GameObject> a_oOutObjDict, bool a_bIsAssert = true) {
		CAccess.Assert(!a_bIsAssert || (a_oKeyInfoList.ExIsValid() && a_oOutObjDict != null));

		// 키 정보가 존재 할 경우
		if(a_oKeyInfoList.ExIsValid() && a_oOutObjDict != null) {
			for(int i = 0; i < a_oKeyInfoList.Count; ++i) {
				var oDragResponder = a_oKeyInfoList[i].Item3?.ExFindChild(a_oKeyInfoList[i].Item2);

				// 드래그 응답자가 존재 할 경우
				if(oDragResponder != null) {
					a_oOutObjDict.ExReplaceVal(a_oKeyInfoList[i].Item1, oDragResponder);
				} else {
					a_oOutObjDict.ExReplaceVal(a_oKeyInfoList[i].Item1, CFactory.CreateDragResponder(a_oKeyInfoList[i].Item2, a_oKeyInfoList[i].Item4, a_oKeyInfoList[i].Item3, a_stSize, Vector3.zero, KCDefine.U_COLOR_TRANSPARENT));
				}

				a_oOutObjDict.GetValueOrDefault(a_oKeyInfoList[i].Item1)?.ExSetRaycastTarget<Image>(true, a_bIsAssert);
			}
		}
	}
	#endregion // 제네릭 클래스 함수
}
