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

		GraphicsSettings.logWhenShaderIsCompiled = true;
		GraphicsSettings.useScriptableRenderPipelineBatching = false;
		GraphicsSettings.videoShadersIncludeMode = VideoShadersIncludeMode.Always;

#if REALTIME_REFLECTION_PROBES_ENABLE
		CFunc.SetupRenderPipeline(a_oRenderPipeline as UniversalRenderPipelineAsset, false);
#endif			// #if REALTIME_REFLECTION_PROBES_ENABLE
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

	#region 조건부 클래스 함수
#if UNITY_EDITOR && UNIVERSAL_RENDER_PIPELINE_MODULE_ENABLE
	/** 렌더 파이프라인을 설정한다 */
	private static void SetupRenderPipeline(UniversalRenderPipelineAsset a_oRenderPipeline, bool a_bIsEnableAssert = true) {
		CAccess.Assert(!a_bIsEnableAssert || a_oRenderPipeline != null);

		// 렌더 파이프라인이 존재 할 경우
		if(a_oRenderPipeline != null) {
			var oRenderPipelineRendererDataList = new List<UniversalRendererData>() {
				Resources.Load<UniversalRendererData>(KCDefine.U_PIPELINE_P_G_UNIVERSAL_RP_RENDER_DATA),
				Resources.Load<UniversalRendererData>(KCDefine.U_PIPELINE_P_G_UNIVERSAL_RP_SSAO_RENDER_DATA)
			};
			
			for(int i = 0; i < oRenderPipelineRendererDataList.Count; ++i) {
				// 렌더 파이프라인 렌더 데이터가 존재 할 경우
				if(oRenderPipelineRendererDataList[i] != null) {
					oRenderPipelineRendererDataList[i].useNativeRenderPass = false;
					oRenderPipelineRendererDataList[i].shadowTransparentReceive = false;

					oRenderPipelineRendererDataList[i].renderingMode = RenderingMode.Forward;
					oRenderPipelineRendererDataList[i].depthPrimingMode = DepthPrimingMode.Disabled;
					oRenderPipelineRendererDataList[i].intermediateTextureMode = IntermediateTextureMode.Auto;

					oRenderPipelineRendererDataList[i].defaultStencilState = new StencilStateData() {
						overrideStencilState = false
					};
					
// TODO: 해당 기능 안정화 필요 (RenderingMode.Deferred)
// #if DEFERRED_RENDER_ENABLE
// 					oRenderPipelineRendererDataList[i].renderingMode = RenderingMode.Deferred;
// #else
// 					oRenderPipelineRendererDataList[i].renderingMode = RenderingMode.Forward;
// #endif			// #if DEFERRED_RENDER_ENABLE

#if HIGH_QUALITY_LEVEL_ENABLE || ULTRA_QUALITY_LEVEL_ENABLE
					oRenderPipelineRendererDataList[i].accurateGbufferNormals = true;
#else
					oRenderPipelineRendererDataList[i].accurateGbufferNormals = false;
#endif			// #if HIGH_QUALITY_LEVEL_ENABLE || ULTRA_QUALITY_LEVEL_ENABLE
				}
			}

			a_oRenderPipeline.supportsHDR = true;
			a_oRenderPipeline.useSRPBatcher = true;
			a_oRenderPipeline.useAdaptivePerformance = false;
			
			a_oRenderPipeline.supportsDynamicBatching = true;
			a_oRenderPipeline.supportsCameraDepthTexture = true;
			a_oRenderPipeline.supportsCameraOpaqueTexture = true;

			a_oRenderPipeline.colorGradingMode = ColorGradingMode.HighDynamicRange;
			a_oRenderPipeline.shaderVariantLogLevel = ShaderVariantLogLevel.AllShaders;

			a_oRenderPipeline.renderScale = KCDefine.B_VAL_1_FLT;
			a_oRenderPipeline.colorGradingLutSize = sizeof(int) * KCDefine.B_UNIT_BITS_PER_BYTE;

			a_oRenderPipeline.shadowDistance = KCDefine.U_DISTANCE_CAMERA_FAR_PLANE / (float)KCDefine.B_UNIT_DIGITS_PER_HUNDRED;
			a_oRenderPipeline.shadowDepthBias = KCDefine.B_VAL_1_INT;
			a_oRenderPipeline.shadowNormalBias = KCDefine.B_VAL_1_INT;
			a_oRenderPipeline.shadowCascadeCount = (int)(KCEditorDefine.U_OPTS_UNIVERSAL_RP_SHADOW_CASCADES + KCDefine.B_VAL_1_INT);

			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_SUPPORTS_SOFT_SHADOW, true);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_SUPPORTS_TERRAIN_HOLES, true);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_SUPPORTS_MIXED_LIGHTING, true);

			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_MAIN_LIGHT_SUPPORTS_SHADOW, true);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_ADDITIONAL_LIGHT_SUPPORTS_SHADOW, true);

			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_CASCADE_BORDER, KCDefine.B_VAL_2_FLT / KCDefine.B_UNIT_DIGITS_PER_TEN);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_OPAQUE_DOWN_SAMPLING, Downsampling.None);

			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_CASCADE_2_SPLIT, KCEditorDefine.U_PERCENT_UNIVERSAL_RP_CASCADE_2_SPLIT);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_CASCADE_3_SPLIT, KCEditorDefine.U_PERCENT_UNIVERSAL_RP_CASCADE_3_SPLIT);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_CASCADE_4_SPLIT, KCEditorDefine.U_PERCENT_UNIVERSAL_RP_CASCADE_4_SPLIT);

			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_MAIN_LIGHT_RENDERING_MODE, KCEditorDefine.B_OPTS_UNIVERSAL_RP_MAIN_LIGHT_RENDERING_MODE);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_MAIN_LIGHT_SHADOW_MAP_RESOLUTION, KCEditorDefine.B_OPTS_UNIVERSAL_MAIN_LIGHT_SHADOW_MAP_RESOLUTION);

			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_ADDITIONAL_LIGHT_COOKIE_FMT, LightCookieFormat.ColorHDR);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_ADDITIONAL_LIGHT_PER_OBJ_LIMIT, KCEditorDefine.U_OPTS_UNIVERSAL_RP_NUM_ADDITIONAL_LIGHTS_PER_OBJ);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_ADDITIONAL_LIGHT_RENDERING_MODE, KCEditorDefine.B_OPTS_UNIVERSAL_RP_ADDITIONAL_LIGHT_RENDERING_MODE);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_ADDITIONAL_LIGHT_COOKIE_RESOLUTION, KCEditorDefine.B_OPTS_UNIVERSAL_RP_ADDITIONAL_LIGHT_COOKIE_RESOLUTION);
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_ADDITIONAL_LIGHT_SHADOW_MAP_RESOLUTION, KCEditorDefine.B_OPTS_UNIVERSAL_RP_ADDITIONAL_LIGHT_SHADOW_MAP_RESOLUTION);

#if MSAA_ENABLE
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_MSAA_QUALITY, KCEditorDefine.U_OPTS_UNIVERSAL_RP_MSAA_QUALITY);
#else
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_MSAA_QUALITY, MsaaQuality.Disabled);
#endif			// #if MSAA_ENABLE

#if HIGH_QUALITY_LEVEL_ENABLE || ULTRA_QUALITY_LEVEL_ENABLE
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_USE_FAST_SRGB_LINEAR_CONVERSION, false);
#else
			a_oRenderPipeline.ExSetRuntimeFieldVal<UniversalRenderPipelineAsset>(KCEditorDefine.U_FIELD_N_UNIVERSAL_RP_USE_FAST_SRGB_LINEAR_CONVERSION, true);
#endif			// #if HIGH_QUALITY_LEVEL_ENABLE || ULTRA_QUALITY_LEVEL_ENABLE
		}
	}
#endif			// #if UNITY_EDITOR && UNIVERSAL_RENDER_PIPELINE_MODULE_ENABLE
	#endregion			// 조건부 클래스 함수
}
