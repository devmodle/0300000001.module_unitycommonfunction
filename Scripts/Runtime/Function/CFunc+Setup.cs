using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

/** 설정 함수 */
public static partial class CFunc {
	#region 클래스 함수
	/** 퀄리티를 설정한다 */
	public static void SetupQuality(EQualityLevel a_eQualityLevel, RenderPipelineAsset a_oRenderPipeline, bool a_bIsEnableExpensiveChange = false) {
#if UNITY_EDITOR
		for(int i = (int)EQualityLevel.VERY_LOW; i < (int)EQualityLevel.MAX_VAL; ++i) {
			QualitySettings.SetQualityLevel(i, false);
			QualitySettings.renderPipeline = a_oRenderPipeline;

			QualitySettings.streamingMipmapsActive = false;
			QualitySettings.asyncUploadPersistentBuffer = true;
			QualitySettings.billboardsFaceCameraPosition = true;

			QualitySettings.lodBias = KCDefine.B_VAL_1_FLT;
			QualitySettings.antiAliasing = KCDefine.B_VAL_0_INT;
			QualitySettings.maximumLODLevel = KCDefine.B_VAL_3_INT;
			QualitySettings.resolutionScalingFixedDPIFactor = KCDefine.B_VAL_1_FLT;

			QualitySettings.asyncUploadTimeSlice = KCDefine.U_QUALITY_ASYNC_UPLOAD_TIME_SLICE;
			QualitySettings.asyncUploadBufferSize = KCDefine.U_QUALITY_ASYNC_UPLOAD_BUFFER_SIZE;

			QualitySettings.vSyncCount = (int)EVSyncType.NEVER;
			QualitySettings.skinWeights = ((EQualityLevel)i >= EQualityLevel.HIGH) ? SkinWeights.FourBones : SkinWeights.TwoBones;
			QualitySettings.shadowmaskMode = ((EQualityLevel)i >= EQualityLevel.HIGH) ? ShadowmaskMode.DistanceShadowmask : ShadowmaskMode.Shadowmask;
			QualitySettings.anisotropicFiltering = ((EQualityLevel)i >= EQualityLevel.HIGH) ? AnisotropicFiltering.Enable : AnisotropicFiltering.Disable;

#if REALTIME_REFLECTION_PROBES_ENABLE
			QualitySettings.realtimeReflectionProbes = true;
#else
			QualitySettings.realtimeReflectionProbes = false;
#endif			// #if REALTIME_REFLECTION_PROBES_ENABLE
		}

		GraphicsSettings.videoShadersIncludeMode = VideoShadersIncludeMode.Always;
#endif			// #if UNITY_EDITOR

		GraphicsSettings.renderPipelineAsset = a_oRenderPipeline;
		GraphicsSettings.defaultRenderPipeline = a_oRenderPipeline;

		QualitySettings.SetQualityLevel((int)((a_eQualityLevel == EQualityLevel.AUTO) ? CFunc.AutoQualityLevel : a_eQualityLevel), a_bIsEnableExpensiveChange);
	}
	
	/** 화면 UI 를 설정한다 */
	public static void SetupScreenUIs(GameObject a_oScreenUIs, int a_nSortingOrder, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || a_oScreenUIs != null);

		// 객체가 존재 할 경우
		if(a_oScreenUIs != null) {
			var oCanvas = a_oScreenUIs.GetComponentInChildren<Canvas>();
			oCanvas.sortingOrder = a_nSortingOrder;
			oCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

			var oCanvasScaler = a_oScreenUIs.GetComponentInChildren<CanvasScaler>();
			oCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			oCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
			oCanvasScaler.referenceResolution = KCDefine.B_SCREEN_SIZE.ExTo2D();
			oCanvasScaler.referencePixelsPerUnit = KCDefine.B_UNIT_REF_PIXELS_PER_UNIT;

#if PIXELS_PERFECT_ENABLE
			oCanvas.pixelPerfect = true;
#else
			oCanvas.pixelPerfect = false;
#endif			// #if PIXELS_PERFECT_ENABLE
		}
	}
	#endregion			// 클래스 함수
}
