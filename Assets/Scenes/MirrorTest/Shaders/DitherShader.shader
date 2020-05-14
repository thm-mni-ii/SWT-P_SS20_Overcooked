// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DitherShader"
{
	Properties
	{
		_Albedo("Albedo", Color) = (0.3689712,0.8000001,0.1671744,1)
		_DitherRadius("Dither Radius", Range( 0 , 1)) = 0
		_DitherFactor("Dither Factor", Range( 0 , 1)) = 1
		_DitherTriggerPadding("Dither Trigger Padding", Float) = 0.1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Overlay+0" "IgnoreProjector" = "True" }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float eyeDepth;
			float4 screenPosition;
		};

		uniform float4 _Albedo;
		uniform float3 PlayerViewportPosition;
		uniform float _DitherTriggerPadding;
		uniform float _DitherFactor;
		uniform float _DitherRadius;


		inline float Dither4x4Bayer( int x, int y )
		{
			const float dither[ 16 ] = {
				 1,  9,  3, 11,
				13,  5, 15,  7,
				 4, 12,  2, 10,
				16,  8, 14,  6 };
			int r = y * 4 + x;
			return dither[r] / 16; // same # of instructions as pre-dividing due to compiler magic
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = _Albedo.rgb;
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen25 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither25 = Dither4x4Bayer( fmod(clipScreen25.x, 4), fmod(clipScreen25.y, 4) );
			float2 appendResult69 = (float2(PlayerViewportPosition.x , PlayerViewportPosition.y));
			float2 appendResult74 = (float2(ase_screenPosNorm.x , ase_screenPosNorm.y));
			dither25 = step( dither25, saturate( (0.0 + (distance( appendResult69 , appendResult74 ) - ( _DitherFactor * _DitherRadius )) * (1.0 - 0.0) / (_DitherRadius - ( _DitherFactor * _DitherRadius ))) ) );
			float ifLocalVar70 = 0;
			if( PlayerViewportPosition.z <= ( i.eyeDepth + _DitherTriggerPadding ) )
				ifLocalVar70 = 1.0;
			else
				ifLocalVar70 = dither25;
			o.Alpha = ifLocalVar70;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float1 customPack1 : TEXCOORD1;
				float4 customPack2 : TEXCOORD2;
				float3 worldPos : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.x = customInputData.eyeDepth;
				o.customPack2.xyzw = customInputData.screenPosition;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.eyeDepth = IN.customPack1.x;
				surfIN.screenPosition = IN.customPack2.xyzw;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18000
347;207;1294;739;2289.334;522.5984;2.476853;True;False
Node;AmplifyShaderEditor.CommentaryNode;24;-2566.4,278.8327;Inherit;False;1565.005;722.7902;Camera Dither;11;79;26;78;25;40;39;35;69;74;73;67;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector3Node;67;-2504.58,341.7904;Inherit;False;Global;PlayerViewportPosition;Player Viewport Position;3;0;Create;True;0;0;False;0;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ScreenPosInputsNode;73;-2461.044,565.0561;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;74;-2233.043,567.0561;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;69;-2236.762,371.2592;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-2293.561,881.5874;Inherit;False;Property;_DitherRadius;Dither Radius;2;0;Create;True;0;0;False;0;0;0.04;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-2290.766,692.604;Inherit;False;Property;_DitherFactor;Dither Factor;3;0;Create;True;0;0;False;0;1;0.6;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-1930.772,552.1983;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;35;-2057.758,472.4064;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;39;-1726.906,474.9353;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;40;-1528.616,489.2661;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-967.002,225.9922;Inherit;False;Property;_DitherTriggerPadding;Dither Trigger Padding;4;0;Create;True;0;0;False;0;0.1;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SurfaceDepthNode;77;-985.7866,112.3806;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-695.46,540.259;Inherit;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DitheringNode;25;-1328.357,485.6894;Inherit;False;0;False;3;0;FLOAT;0;False;1;SAMPLER2D;;False;2;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;80;-680.002,206.9922;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;70;-506.4605,319.2591;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;23;-675.7409,-65.76476;Inherit;False;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;0.3689712,0.8000001,0.1671744,1;0.3689712,0.8000001,0.1671744,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;22;-200,21;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;DitherShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;Overlay;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;74;0;73;1
WireConnection;74;1;73;2
WireConnection;69;0;67;1
WireConnection;69;1;67;2
WireConnection;79;0;78;0
WireConnection;79;1;26;0
WireConnection;35;0;69;0
WireConnection;35;1;74;0
WireConnection;39;0;35;0
WireConnection;39;1;79;0
WireConnection;39;2;26;0
WireConnection;40;0;39;0
WireConnection;25;0;40;0
WireConnection;80;0;77;0
WireConnection;80;1;81;0
WireConnection;70;0;67;3
WireConnection;70;1;80;0
WireConnection;70;2;25;0
WireConnection;70;3;71;0
WireConnection;70;4;71;0
WireConnection;22;0;23;0
WireConnection;22;9;70;0
ASEEND*/
//CHKSM=FADF8262B08C768430AAE8E36DEE84F76DBE7088