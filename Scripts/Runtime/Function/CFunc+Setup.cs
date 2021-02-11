using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

#if UNIVERSAL_PIPELINE_MODULE_ENABLE
using UnityEngine.Rendering.Universal;
#endif			// #if UNIVERSAL_PIPELINE_MODULE_ENABLE

//! 설정 함수
public static partial class CFunc {
	#region 클래스 함수
	//! 퀄리티를 설정한다
	public static void SetupQuality(EQualityLevel a_eQualityLevel, bool a_bIsApplyExpensiveChange = false) {
		// 퀄리티 레벨을 설정한다 {
		var eQualityLevel = a_eQualityLevel;
		
		// 자동 퀄리티 레벨 일 경우
		if(a_eQualityLevel == EQualityLevel.AUTO) {
#if ULTRA_QUALITY_LEVEL_ENABLE
			eQualityLevel = EQualityLevel.ULTRA;
#elif HIGH_QUALITY_LEVEL_ENABLE
			eQualityLevel = EQualityLevel.HIGH;
#else
			eQualityLevel = EQualityLevel.VERY_LOW;
#endif			// #if ULTRA_QUALITY_LEVEL_ENABLE
		}

#if UNITY_EDITOR
		QualitySettings.antiAliasing = KCDefine.U_QUALITY_ANTI_ALIASING;
		QualitySettings.maximumLODLevel = KCDefine.U_QUALITY_MAX_LOD_LEVEL;
		QualitySettings.asyncUploadTimeSlice = KCDefine.U_QUALITY_ASYNC_UPLOAD_TIME_SLICE;
		QualitySettings.asyncUploadBufferSize = KCDefine.U_QUALITY_ASYNC_UPLOAD_BUFFER_SIZE;
		QualitySettings.asyncUploadPersistentBuffer = KCDefine.U_QUALITY_ASYNC_UPLOAD_PERSISTENT_BUFFER;
		QualitySettings.resolutionScalingFixedDPIFactor = KCDefine.U_QUALITY_RESOLUTION_SCALE_FIXED_DPI_FACTOR;

		QualitySettings.vSyncCount = (eQualityLevel >= EQualityLevel.HIGH) ? (int)EVSyncType.EVERY : (int)EVSyncType.NEVER;
		QualitySettings.anisotropicFiltering = (eQualityLevel >= EQualityLevel.HIGH) ? AnisotropicFiltering.Enable : AnisotropicFiltering.Disable;
#endif			// #if UNITY_EDITOR

		QualitySettings.SetQualityLevel((int)eQualityLevel, a_bIsApplyExpensiveChange);
		// 퀄리티 레벨을 설정한다 }
		
#if UNITY_EDITOR
		// 렌더링 파이프라인을 설정한다 {			
#if UNIVERSAL_PIPELINE_MODULE_ENABLE
		var oRenderPipeline = Resources.Load<UniversalRenderPipelineAsset>(KCDefine.U_PIPELINE_P_G_UNIVERSAL_RP_ASSET);

		var oRenderPipelineRenderDatas = new ForwardRendererData[] {
			Resources.Load<ForwardRendererData>(KCDefine.U_PIPELINE_P_G_UNIVERSAL_RP_RENDER_DATA),
			Resources.Load<ForwardRendererData>(KCDefine.U_PIPELINE_P_G_UNIVERSAL_RP_SSAO_RENDER_DATA)
		};

		// 렌더 파이프라인이 존재 할 경우
		if(oRenderPipeline != null) {
			for(int i = 0; i < oRenderPipelineRenderDatas.Length; ++i) {
				var oRenderPipelineRenderData = oRenderPipelineRenderDatas[i];
				oRenderPipelineRenderData.shadowTransparentReceive = false;

				oRenderPipelineRenderData.defaultStencilState = new StencilStateData() {
					overrideStencilState = false
				};
			}

			oRenderPipeline.supportsHDR = KCDefine.U_OPTS_UNIVERSAL_RP_SUPPORTS_HDR;
			oRenderPipeline.shaderVariantLogLevel = ShaderVariantLogLevel.AllShaders;

			oRenderPipeline.renderScale = KCDefine.U_SCALE_UNIVERSAL_RP_RENDERING;
			oRenderPipeline.colorGradingMode = KCDefine.U_OPTS_UNIVERSAL_RP_COLOR_GRADING_MODE;
			oRenderPipeline.colorGradingLutSize = KCDefine.U_SIZE_UNIVERSAL_RP_COLOR_GRADING_LUT;

			oRenderPipeline.shadowDepthBias = KCDefine.B_VALUE_INT_1;
			oRenderPipeline.shadowNormalBias = KCDefine.B_VALUE_INT_1;
			oRenderPipeline.shadowCascadeCount = (int)(KCDefine.U_OPTS_UNIVERSAL_RP_SHADOW_CASCADES + KCDefine.B_VALUE_INT_1);

			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_ANTI_ALIASING, MsaaQuality.Disabled);
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_OPAQUE_DOWN_SAMPLING, KCDefine.U_OPTS_UNIVERSAL_RP_DOWN_SAMPLING);

			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_CASCADE_2_SPLIT, KCDefine.U_PERCENT_UNIVERSAL_RP_CASCADE_2_SPLIT);
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_CASCADE_3_SPLIT, KCDefine.U_PERCENT_UNIVERSAL_RP_CASCADE_3_SPLIT);
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_CASCADE_4_SPLIT, KCDefine.U_PERCENT_UNIVERSAL_RP_CASCADE_4_SPLIT);

			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_SUPPORTS_TERRAIN_HOLES, true);
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_ADDITIONAL_LIGHT_PER_OBJ_LIMIT, KCDefine.U_MAX_NUM_UNIVERSAL_RP_ADDITIONAL_LIGHT_PER_OBJ);
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_MAIN_LIGHT_SHADOW_MAP_RESOLUTION, UnityEngine.Rendering.Universal.ShadowResolution._2048);
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_ADDITIONAL_LIGHT_SHADOW_MAP_RESOLUTION, UnityEngine.Rendering.Universal.ShadowResolution._512);

#if !MODE_2D_ENABLE
			oRenderPipeline.supportsCameraDepthTexture = true;
			oRenderPipeline.supportsCameraOpaqueTexture = true;
#else
			oRenderPipeline.supportsCameraDepthTexture = false;
			oRenderPipeline.supportsCameraOpaqueTexture = false;
#endif			// #if !MODE_2D_ENABLE

#if DYNAMIC_BATCHING_ENABLE
			oRenderPipeline.useSRPBatcher = true;
			oRenderPipeline.supportsDynamicBatching = true;
#else
			oRenderPipeline.useSRPBatcher = false;
			oRenderPipeline.supportsDynamicBatching = false;
#endif			// #if DYNAMIC_BATCHING_ENABLE

#if LIGHT_ENABLE
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_SUPPORTS_MIXED_LIGHTING, true);
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_MAIN_LIGHT_RENDERING_MODE, LightRenderingMode.PerPixel);
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_ADDITIONAL_LIGHT_RENDERING_MODE, LightRenderingMode.PerPixel);
#else
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_SUPPORTS_MIXED_LIGHTING, false);
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_MAIN_LIGHT_RENDERING_MODE, LightRenderingMode.Disabled);
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_ADDITIONAL_LIGHT_RENDERING_MODE, LightRenderingMode.Disabled);
#endif			// #if LIGHT_ENABLE

#if LIGHT_ENABLE && SHADOW_ENABLE
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_MAIN_LIGHT_SUPPORTS_SHADOW, true);
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_ADDITIONAL_LIGHT_SUPPORTS_SHADOW, true);

#if SOFT_SHADOW_ENABLE
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_SUPPORTS_SOFT_SHADOW, true);
#else
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_SUPPORTS_SOFT_SHADOW, false);
#endif			// #if SOFT_SHADOW_ENABLE
#else
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_MAIN_LIGHT_SUPPORTS_SHADOW, false);
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_ADDITIONAL_LIGHT_SUPPORTS_SHADOW, false);
			oRenderPipeline.ExSetRuntimeFieldValue<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_SUPPORTS_SOFT_SHADOW, false);
#endif			// #if LIGHT_ENABLE && SHADOW_ENABLE
		}

		QualitySettings.renderPipeline = oRenderPipeline;
		GraphicsSettings.renderPipelineAsset = oRenderPipeline;
#else
		QualitySettings.renderPipeline = null;
		GraphicsSettings.renderPipelineAsset = null;
#endif			// #if UNIVERSAL_PIPELINE_MODULE_ENABLE
		// 렌더링 파이프라인을 설정한다 }
#endif			// #if UNITY_EDITOR
	}
	
	//! 스크린 UI 를 설정한다
	public static void SetupScreenUI(GameObject a_oScreenUI, int a_nSortingOrder) {
		CAccess.Assert(a_oScreenUI != null);
		
		var oCanvas = a_oScreenUI.GetComponentInChildren<Canvas>();
		oCanvas.sortingOrder = a_nSortingOrder;
		oCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

		var oCanvasScaler = a_oScreenUI.GetComponentInChildren<CanvasScaler>();
		oCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		oCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
		oCanvasScaler.referenceResolution = KCDefine.B_SCREEN_SIZE;
		oCanvasScaler.referencePixelsPerUnit = KCDefine.B_REF_PIXELS_UNIT;

#if PIXELS_PERFECT_ENABLE
		oCanvas.pixelPerfect = true;
#else
		oCanvas.pixelPerfect = false;
#endif			// #if PIXELS_PERFECT_ENABLE
	}
	#endregion			// 클래스 함수
}
