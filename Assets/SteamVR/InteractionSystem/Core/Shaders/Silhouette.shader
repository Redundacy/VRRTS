//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Used to show the outline of the object
//
//=============================================================================
// UNITY_SHADER_NO_UPGRADE
Shader "Valve/VR/Silhouette"
{
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	Properties
	{
		g_vOutlineColor("Outline Color", Color) = (.5, .5, .5, 1)
		g_flOutlineWidth("Outline width", Range(.001, 0.03)) = .005
		g_flCornerAdjust("Corner Adjustment", Range(0, 2)) = .5
	}
		HLSLINCLUDE
#pragma target 5.0
#pragma multi_compile_instancing
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		//-------------------------------------------------------------------------------------------------------------------------------------------------------------
		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_DEFINE_INSTANCED_PROP(float4, g_vOutlineColor)
		UNITY_DEFINE_INSTANCED_PROP(float, g_flOutlineWidth)
		UNITY_DEFINE_INSTANCED_PROP(float, g_flCornerAdjust)
		UNITY_INSTANCING_BUFFER_END(Props)

		//-------------------------------------------------------------------------------------------------------------------------------------------------------------
		struct VS_INPUT
	{
		float4 vPositionOs : POSITION;
		float3 vNormalOs : NORMAL;

		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	struct PS_INPUT
	{
		float4 vPositionOs : TEXCOORD0;
		float3 vNormalOs : TEXCOORD1;
		float4 vPositionPs : SV_POSITION;

		UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
	};

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	PS_INPUT MainVs(VS_INPUT i)
	{
		PS_INPUT o;

		UNITY_SETUP_INSTANCE_ID(i);
		ZERO_INITIALIZE(PS_INPUT, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.vPositionOs.xyzw = i.vPositionOs.xyzw;
		o.vNormalOs.xyz = i.vNormalOs.xyz;
		o.vPositionPs = TransformObjectToHClip(i.vPositionOs.xyz);
		return o;
	}

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	PS_INPUT Extrude(PS_INPUT vertex)
	{
		PS_INPUT extruded = vertex;

		// Offset along normal in projection space
		float3 vNormalVs = mul((float3x3)UNITY_MATRIX_IT_MV, vertex.vNormalOs.xyz);
		float2 vOffsetPs = mul((float2x2)UNITY_MATRIX_P, vNormalVs.xy); //TransformViewToProjection( vNormalVs.xy );
		vOffsetPs.xy = normalize(vOffsetPs.xy);

		// Calculate position
		extruded.vPositionPs = TransformObjectToHClip(vertex.vPositionOs.xyz);
		extruded.vPositionPs.xy += vOffsetPs.xy * extruded.vPositionPs.w * UNITY_ACCESS_INSTANCED_PROP(Props, g_flOutlineWidth);

		return extruded;
	}

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	[maxvertexcount(18)]
	void ExtrudeGs(triangle PS_INPUT inputTriangle[3], inout TriangleStream<PS_INPUT> outputStream)
	{
		UNITY_SETUP_INSTANCE_ID(inputTriangle[0])
			UNITY_SETUP_INSTANCE_ID(inputTriangle[1])
			UNITY_SETUP_INSTANCE_ID(inputTriangle[2])

			DEFAULT_UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(inputTriangle[0])
			DEFAULT_UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(inputTriangle[1])
			DEFAULT_UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(inputTriangle[2])

			PS_INPUT inputTriangle0 = inputTriangle[0];
		PS_INPUT inputTriangle1 = inputTriangle[1];
		PS_INPUT inputTriangle2 = inputTriangle[2];

		float3 a = normalize(inputTriangle0.vPositionOs.xyz - inputTriangle1.vPositionOs.xyz);
		float3 b = normalize(inputTriangle1.vPositionOs.xyz - inputTriangle2.vPositionOs.xyz);
		float3 c = normalize(inputTriangle2.vPositionOs.xyz - inputTriangle0.vPositionOs.xyz);

		inputTriangle0.vNormalOs = inputTriangle0.vNormalOs + normalize(a - c) * UNITY_ACCESS_INSTANCED_PROP(Props, g_flCornerAdjust);
		inputTriangle1.vNormalOs = inputTriangle1.vNormalOs + normalize(-a + b) * UNITY_ACCESS_INSTANCED_PROP(Props, g_flCornerAdjust);
		inputTriangle2.vNormalOs = inputTriangle2.vNormalOs + normalize(-b + c) * UNITY_ACCESS_INSTANCED_PROP(Props, g_flCornerAdjust);

		PS_INPUT extrudedTriangle0;
		PS_INPUT extrudedTriangle1;
		PS_INPUT extrudedTriangle2;

		ZERO_INITIALIZE(PS_INPUT, extrudedTriangle0);
		ZERO_INITIALIZE(PS_INPUT, extrudedTriangle0);
		ZERO_INITIALIZE(PS_INPUT, extrudedTriangle0);

		extrudedTriangle0 = Extrude(inputTriangle0);
		extrudedTriangle1 = Extrude(inputTriangle1);
		extrudedTriangle2 = Extrude(inputTriangle2);

		outputStream.Append(inputTriangle0);
		outputStream.Append(extrudedTriangle0);
		outputStream.Append(inputTriangle1);
		outputStream.Append(extrudedTriangle0);
		outputStream.Append(extrudedTriangle1);
		outputStream.Append(inputTriangle1);

		outputStream.Append(inputTriangle1);
		outputStream.Append(extrudedTriangle1);
		outputStream.Append(extrudedTriangle2);
		outputStream.Append(inputTriangle1);
		outputStream.Append(extrudedTriangle2);
		outputStream.Append(inputTriangle2);

		outputStream.Append(inputTriangle2);
		outputStream.Append(extrudedTriangle2);
		outputStream.Append(inputTriangle0);
		outputStream.Append(extrudedTriangle2);
		outputStream.Append(extrudedTriangle0);
		outputStream.Append(inputTriangle0);
	}

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	float4 MainPs(PS_INPUT i) : SV_Target
	{
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(i);

		return UNITY_ACCESS_INSTANCED_PROP(Props, g_vOutlineColor);
	}

		//-------------------------------------------------------------------------------------------------------------------------------------------------------------
		float4 NullPs(PS_INPUT i) : SV_Target
	{
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(i);

		return float4(1.0, 0.0, 1.0, 1.0);
	}
		ENDHLSL
		SubShader
	{
		Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

			Pass
		{
			Tags { "LightMode" = "UniversalForward" }
			ColorMask 0
			Cull Off
			ZWrite Off
			Stencil
			{
				Ref 1
				Comp always
				Pass replace
			}

			HLSLPROGRAM
				#pragma vertex MainVs
				#pragma fragment NullPs

				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			ENDHLSL
		}
			Pass
		{
			Tags { "LightMode" = "UniversalForward" }
			Cull Off
			ZWrite On
			Stencil
			{
				Ref 1
				Comp notequal
				Pass keep
				Fail keep
			}

			HLSLPROGRAM
				#pragma vertex MainVs
				#pragma geometry ExtrudeGs
				#pragma fragment MainPs

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			ENDHLSL
		}
	}

}