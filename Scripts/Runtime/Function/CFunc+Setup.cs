using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

/** 설정 함수 */
public static partial class CFunc {
	#region 클래스 함수
	/** 퀄리티를 설정한다 */
	public static void SetupQuality(EQualityLevel a_eQualityLevel, bool a_bIsEnableSetupRenderingPipeline, bool a_bIsEnableExpensiveChange = false) {
#if UNITY_EDITOR
		var oQualityLevelList = new List<EQualityLevel>() {
			EQualityLevel.NORM, EQualityLevel.HIGH, EQualityLevel.ULTRA
		};

		for(int i = 0; i < oQualityLevelList.Count; ++i) {
			QualitySettings.SetQualityLevel((int)oQualityLevelList[i], false);
			QualitySettings.renderPipeline = a_bIsEnableSetupRenderingPipeline ? Resources.Load<RenderPipelineAsset>(CAccess.GetRenderingPipelinePath(oQualityLevelList[i])) : null;

			QualitySettings.softParticles = true;
			QualitySettings.streamingMipmapsActive = false;
			QualitySettings.asyncUploadPersistentBuffer = true;
			QualitySettings.billboardsFaceCameraPosition = true;

			QualitySettings.lodBias = KCDefine.B_VAL_1_FLT;
			QualitySettings.maximumLODLevel = KCDefine.B_VAL_0_INT;
			QualitySettings.resolutionScalingFixedDPIFactor = KCDefine.B_VAL_1_FLT;

			QualitySettings.asyncUploadTimeSlice = KCDefine.U_QUALITY_ASYNC_UPLOAD_TIME_SLICE;
			QualitySettings.asyncUploadBufferSize = KCDefine.U_QUALITY_ASYNC_UPLOAD_BUFFER_SIZE;

			QualitySettings.vSyncCount = (int)EVSyncType.NEVER;
			QualitySettings.skinWeights = SkinWeights.FourBones;
			QualitySettings.shadowCascades = (int)EShadowCascadesOpts.FOUR_CASCADES;
			QualitySettings.shadowProjection = ShadowProjection.StableFit;
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;

			QualitySettings.shadowDistance = KCDefine.U_DISTANCE_CAMERA_FAR_PLANE / KCDefine.B_VAL_5_FLT;
			QualitySettings.shadowCascade2Split = KCEditorDefine.U_EDITOR_OPTS_CASCADE_2_SPLIT_PERCENT;
			QualitySettings.shadowCascade4Split = KCEditorDefine.U_EDITOR_OPTS_CASCADE_4_SPLIT_PERCENT;
			QualitySettings.shadowNearPlaneOffset = KCDefine.U_DISTANCE_CAMERA_NEAR_PLANE;

			switch(oQualityLevelList[i]) {
				case EQualityLevel.HIGH: {
					QualitySettings.shadowmaskMode = ShadowmaskMode.DistanceShadowmask;
					QualitySettings.shadowResolution = ShadowResolution.High;
				} break;
				case EQualityLevel.ULTRA: {
					QualitySettings.shadowmaskMode = ShadowmaskMode.DistanceShadowmask;
					QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
				} break;
				default: {
					QualitySettings.shadowmaskMode = ShadowmaskMode.Shadowmask;
					QualitySettings.shadowResolution = ShadowResolution.Medium;
				} break;
			}

#if MSAA_ENABLE
			switch(oQualityLevelList[i]) {
				case EQualityLevel.HIGH: {
					QualitySettings.antiAliasing = (int)EAntiAliasingLevel.FOUR_SAMPLE;
				} break;
				case EQualityLevel.ULTRA: {
					QualitySettings.antiAliasing = (int)EAntiAliasingLevel.EIGHT_SAMPLE;
				} break;
				default: {
					QualitySettings.antiAliasing = (int)EAntiAliasingLevel.TWO_SAMPLE;
				} break;
			}
#else
			QualitySettings.antiAliasing = (int)EAntiAliasingLevel.DISABLE;
#endif			// #if MSAA_ENABLE

#if LIGHT_ENABLE
			QualitySettings.shadows = ShadowQuality.All;
#else
			QualitySettings.shadows = ShadowQuality.Disable;
#endif			// #if LIGHT_ENABLE

#if REALTIME_REFLECTION_PROBES_ENABLE
			QualitySettings.realtimeReflectionProbes = true;
#else
			QualitySettings.realtimeReflectionProbes = false;
#endif			// #if REALTIME_REFLECTION_PROBES_ENABLE
		}
		
		GraphicsSettings.logWhenShaderIsCompiled = true;
		GraphicsSettings.videoShadersIncludeMode = VideoShadersIncludeMode.Always;
#endif			// #if UNITY_EDITOR

		GraphicsSettings.renderPipelineAsset = a_bIsEnableSetupRenderingPipeline ? Resources.Load<RenderPipelineAsset>(CAccess.GetRenderingPipelinePath(a_eQualityLevel)) : null;
		GraphicsSettings.defaultRenderPipeline = a_bIsEnableSetupRenderingPipeline ? Resources.Load<RenderPipelineAsset>(CAccess.GetRenderingPipelinePath(a_eQualityLevel)) : null;

		QualitySettings.SetQualityLevel((int)a_eQualityLevel, a_bIsEnableExpensiveChange);
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
