using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

#if UNIVERSAL_RENDER_PIPELINE_MODULE_ENABLE
using UnityEngine.Rendering.Universal;
#endif			// #if UNIVERSAL_RENDER_PIPELINE_MODULE_ENABLE

/** 설정 함수 */
public static partial class CFunc {
	#region 클래스 함수
	/** 퀄리티를 설정한다 */
	public static void SetupQuality(EQualityLevel a_eQualityLevel, bool a_bIsEnableExpensiveChange = false) {
		// 퀄리티 레벨을 설정한다 {
		var eQualityLevel = a_eQualityLevel;
		
		// 자동 퀄리티 레벨 일 경우
		if(a_eQualityLevel == EQualityLevel.AUTO) {
#if HIGH_QUALITY_LEVEL_ENABLE
			eQualityLevel = EQualityLevel.HIGH;
#elif ULTRA_QUALITY_LEVEL_ENABLE
			eQualityLevel = EQualityLevel.ULTRA;
#else
			eQualityLevel = EQualityLevel.VERY_LOW;
#endif			// #if HIGH_QUALITY_LEVEL_ENABLE
		}

		QualitySettings.SetQualityLevel((int)eQualityLevel, a_bIsEnableExpensiveChange);

#if UNITY_EDITOR
		QualitySettings.streamingMipmapsActive = false;
		QualitySettings.asyncUploadPersistentBuffer = true;

		QualitySettings.antiAliasing = KCDefine.B_VAL_0_INT;
		QualitySettings.maximumLODLevel = KCDefine.B_VAL_3_INT;

		QualitySettings.lodBias = KCDefine.B_VAL_1_FLT;
		QualitySettings.resolutionScalingFixedDPIFactor = KCDefine.B_VAL_1_FLT;

		QualitySettings.asyncUploadTimeSlice = KCDefine.U_QUALITY_ASYNC_UPLOAD_TIME_SLICE;
		QualitySettings.asyncUploadBufferSize = KCDefine.U_QUALITY_ASYNC_UPLOAD_BUFFER_SIZE;

		QualitySettings.vSyncCount = (int)EVSyncType.NEVER;
		QualitySettings.anisotropicFiltering = (eQualityLevel >= EQualityLevel.HIGH) ? AnisotropicFiltering.Enable : AnisotropicFiltering.Disable;

#if REALTIME_REFLECTION_PROBES_ENABLE
		QualitySettings.realtimeReflectionProbes = true;
#else
		QualitySettings.realtimeReflectionProbes = false;
#endif			// #if REALTIME_REFLECTION_PROBES_ENABLE
#endif			// #if UNITY_EDITOR
		// 퀄리티 레벨을 설정한다 }
		
#if UNITY_EDITOR
		// 렌더링 파이프라인을 설정한다 {
#if UNIVERSAL_RENDER_PIPELINE_MODULE_ENABLE
		var oRenderPipeline = Resources.Load<UniversalRenderPipelineAsset>(KCDefine.U_PIPELINE_P_G_UNIVERSAL_RP_ASSET);
		CAccess.Assert(oRenderPipeline != null);

		CFunc.SetupRenderPipeline(oRenderPipeline);
		QualitySettings.renderPipeline = oRenderPipeline;

		GraphicsSettings.renderPipelineAsset = oRenderPipeline;
		GraphicsSettings.useScriptableRenderPipelineBatching = true;
#else
		QualitySettings.renderPipeline = null;
		
		GraphicsSettings.renderPipelineAsset = null;
		GraphicsSettings.useScriptableRenderPipelineBatching = false;
#endif			// #if UNIVERSAL_RENDER_PIPELINE_MODULE_ENABLE
		// 렌더링 파이프라인을 설정한다 }
#endif			// #if UNITY_EDITOR
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

	#region 조건부 클래스 함수
#if UNIVERSAL_RENDER_PIPELINE_MODULE_ENABLE
	/** 렌더 파이프라인을 설정한다 */
	public static void SetupRenderPipeline(UniversalRenderPipelineAsset a_oRenderPipeline, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || a_oRenderPipeline != null);

		// 렌더 파이프라인이 존재 할 경우
		if(a_oRenderPipeline != null) {
			var oRenderPipelineRendererDataList = new List<UniversalRendererData>() {
				Resources.Load<UniversalRendererData>(KCDefine.U_PIPELINE_P_G_UNIVERSAL_RP_RENDER_DATA),
				Resources.Load<UniversalRendererData>(KCDefine.U_PIPELINE_P_G_UNIVERSAL_RP_SSAO_RENDER_DATA)
			};
			
			for(int i = 0; i < oRenderPipelineRendererDataList.Count; ++i) {
				oRenderPipelineRendererDataList[i].shadowTransparentReceive = false;
				
				oRenderPipelineRendererDataList[i].defaultStencilState = new StencilStateData() {
					overrideStencilState = false
				};
			}

			a_oRenderPipeline.supportsHDR = true;
			a_oRenderPipeline.useSRPBatcher = true;
			a_oRenderPipeline.useAdaptivePerformance = true;
			a_oRenderPipeline.supportsDynamicBatching = true;

			a_oRenderPipeline.supportsCameraDepthTexture = true;
			a_oRenderPipeline.supportsCameraOpaqueTexture = true;

			a_oRenderPipeline.colorGradingMode = ColorGradingMode.HighDynamicRange;
			a_oRenderPipeline.shaderVariantLogLevel = ShaderVariantLogLevel.AllShaders;

			a_oRenderPipeline.renderScale = KCDefine.B_VAL_1_FLT;
			a_oRenderPipeline.shadowDepthBias = KCDefine.B_VAL_1_INT;
			a_oRenderPipeline.shadowNormalBias = KCDefine.B_VAL_1_INT;
			a_oRenderPipeline.shadowCascadeCount = (int)(KCDefine.U_OPTS_UNIVERSAL_RP_SHADOW_CASCADES + KCDefine.B_VAL_1_INT);
			a_oRenderPipeline.colorGradingLutSize = KCDefine.U_SIZE_UNIVERSAL_RP_COLOR_GRADING_LUT;

			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_OPAQUE_DOWN_SAMPLING, KCDefine.U_OPTS_UNIVERSAL_RP_DOWN_SAMPLING);

			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_CASCADE_2_SPLIT, KCDefine.U_PERCENT_UNIVERSAL_RP_CASCADE_2_SPLIT);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_CASCADE_3_SPLIT, KCDefine.U_PERCENT_UNIVERSAL_RP_CASCADE_3_SPLIT);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_CASCADE_4_SPLIT, KCDefine.U_PERCENT_UNIVERSAL_RP_CASCADE_4_SPLIT);

			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_SUPPORTS_SOFT_SHADOW, true);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_SUPPORTS_TERRAIN_HOLES, true);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_MAIN_LIGHT_SUPPORTS_SHADOW, true);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_ADDITIONAL_LIGHT_SUPPORTS_SHADOW, true);

			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_ADDITIONAL_LIGHT_PER_OBJ_LIMIT, KCDefine.U_MAX_NUM_UNIVERSAL_RP_ADDITIONAL_LIGHT_PER_OBJ);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_MAIN_LIGHT_SHADOW_MAP_RESOLUTION, UnityEngine.Rendering.Universal.ShadowResolution._2048);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_ADDITIONAL_LIGHT_SHADOW_MAP_RESOLUTION, UnityEngine.Rendering.Universal.ShadowResolution._512);

#if LIGHT_ENABLE
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_SUPPORTS_MIXED_LIGHTING, true);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_MAIN_LIGHT_RENDERING_MODE, LightRenderingMode.PerPixel);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_ADDITIONAL_LIGHT_RENDERING_MODE, LightRenderingMode.PerPixel);
#else
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_SUPPORTS_MIXED_LIGHTING, false);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_MAIN_LIGHT_RENDERING_MODE, LightRenderingMode.Disabled);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_ADDITIONAL_LIGHT_RENDERING_MODE, LightRenderingMode.Disabled);
#endif			// #if LIGHT_ENABLE

#if MSAA_ENABLE
#if HIGH_QUALITY_LEVEL_ENABLE
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_ANTI_ALIASING, MsaaQuality._4x);
#elif ULTRA_QUALITY_LEVEL_ENABLE
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_ANTI_ALIASING, MsaaQuality._8x);
#else
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_ANTI_ALIASING, MsaaQuality._2x);
#endif			// #if HIGH_QUALITY_LEVEL_ENABLE
#else
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCDefine.U_FIELD_N_UNIVERSAL_RP_ANTI_ALIASING, MsaaQuality.Disabled);
#endif			// #if MSAA_ENABLE
		}
	}
#endif			// #if UNIVERSAL_RENDER_PIPELINE_MODULE_ENABLE
	#endregion			// 조건부 클래스 함수
}
