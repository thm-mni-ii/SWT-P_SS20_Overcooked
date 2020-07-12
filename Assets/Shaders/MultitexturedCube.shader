// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MultitexturedCube"
{
	Properties
	{
		_Front("Front", 2D) = "white" {}
		_Left("Left", 2D) = "white" {}
		_Top("Top", 2D) = "white" {}
		_Back("Back", 2D) = "white" {}
		_Right("Right", 2D) = "white" {}
		_Bottom("Bottom", 2D) = "white" {}
		_FrontTint("FrontTint", Color) = (0,0,0,0)
		_LeftTint("LeftTint", Color) = (0,0,0,0)
		_TopTint("TopTint", Color) = (0,0,0,0)
		_BackTint("BackTint", Color) = (0,0,0,0)
		_RightTint("RightTint", Color) = (0,0,0,0)
		_BottomTint("BottomTint", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldNormal;
			float2 uv_texcoord;
		};

		uniform float4 _BackTint;
		uniform sampler2D _Back;
		uniform float4 _Back_ST;
		uniform float4 _FrontTint;
		uniform sampler2D _Front;
		uniform float4 _Front_ST;
		uniform float4 _RightTint;
		uniform sampler2D _Right;
		uniform float4 _Right_ST;
		uniform float4 _LeftTint;
		uniform sampler2D _Left;
		uniform float4 _Left_ST;
		uniform float4 _TopTint;
		uniform sampler2D _Top;
		uniform float4 _Top_ST;
		uniform float4 _BottomTint;
		uniform sampler2D _Bottom;
		uniform float4 _Bottom_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float dotResult10 = dot( float3(0,0,1) , ase_vertexNormal );
			float2 uv_Back = i.uv_texcoord * _Back_ST.xy + _Back_ST.zw;
			float4 tex2DNode7 = tex2D( _Back, uv_Back );
			float4 lerpResult46 = lerp( _BackTint , ( tex2DNode7 * _BackTint ) , tex2DNode7.a);
			float2 uv_Front = i.uv_texcoord * _Front_ST.xy + _Front_ST.zw;
			float4 tex2DNode6 = tex2D( _Front, uv_Front );
			float4 lerpResult45 = lerp( _FrontTint , ( tex2DNode6 * _FrontTint ) , tex2DNode6.a);
			float dotResult26 = dot( float3(1,0,0) , ase_vertexNormal );
			float2 uv_Right = i.uv_texcoord * _Right_ST.xy + _Right_ST.zw;
			float4 tex2DNode19 = tex2D( _Right, uv_Right );
			float4 lerpResult47 = lerp( _RightTint , ( tex2DNode19 * _RightTint ) , tex2DNode19.a);
			float2 uv_Left = i.uv_texcoord * _Left_ST.xy + _Left_ST.zw;
			float4 tex2DNode20 = tex2D( _Left, uv_Left );
			float4 lerpResult44 = lerp( _LeftTint , ( tex2DNode20 * _LeftTint ) , tex2DNode20.a);
			float dotResult38 = dot( float3(0,1,0) , ase_vertexNormal );
			float2 uv_Top = i.uv_texcoord * _Top_ST.xy + _Top_ST.zw;
			float4 tex2DNode34 = tex2D( _Top, uv_Top );
			float4 lerpResult48 = lerp( _TopTint , ( tex2DNode34 * _TopTint ) , tex2DNode34.a);
			float2 uv_Bottom = i.uv_texcoord * _Bottom_ST.xy + _Bottom_ST.zw;
			float4 tex2DNode31 = tex2D( _Bottom, uv_Bottom );
			float4 lerpResult49 = lerp( _BottomTint , ( tex2DNode31 * _BottomTint ) , tex2DNode31.a);
			o.Albedo = (  ( dotResult10 - 0.1 > 0.0 ? lerpResult46 : dotResult10 - 0.1 <= 0.0 && dotResult10 + 0.1 >= 0.0 ? 0.0 : lerpResult45 )  +  ( dotResult26 - 0.1 > 0.0 ? lerpResult47 : dotResult26 - 0.1 <= 0.0 && dotResult26 + 0.1 >= 0.0 ? 0.0 : lerpResult44 )  +  ( dotResult38 - 0.1 > 0.0 ? lerpResult48 : dotResult38 - 0.1 <= 0.0 && dotResult38 + 0.1 >= 0.0 ? 0.0 : lerpResult49 )  ).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

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
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
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
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
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
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18200
282;268;1162;625;1875.98;-429.9985;1.65912;True;False
Node;AmplifyShaderEditor.CommentaryNode;2;-1384.904,-669.4136;Inherit;False;1081.997;1032.17;Front/Back;11;14;15;6;7;10;11;45;46;53;56;57;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;18;-1380.63,401.092;Inherit;False;1081.997;1032.17;Left/Right;11;20;21;19;22;26;23;44;47;54;58;59;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;29;-1377.937,1485.731;Inherit;False;1081.997;1032.17;Top/Bottom;10;30;31;32;33;34;38;48;49;60;61;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;14;-1321.24,-394.3906;Inherit;False;Property;_FrontTint;FrontTint;6;0;Create;True;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-1333.48,-601.8309;Inherit;True;Property;_Front;Front;0;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;31;-1320.661,2029.059;Inherit;True;Property;_Bottom;Bottom;5;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;22;-1312.841,1125.36;Inherit;False;Property;_RightTint;RightTint;10;0;Create;True;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;33;-1308.421,1799.598;Inherit;False;Property;_TopTint;TopTint;8;0;Create;True;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;30;-1309.923,2236.77;Inherit;False;Property;_BottomTint;BottomTint;11;0;Create;True;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;21;-1311.339,688.1884;Inherit;False;Property;_LeftTint;LeftTint;7;0;Create;True;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;15;-1322.742,42.78181;Inherit;False;Property;_BackTint;BackTint;9;0;Create;True;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;7;-1333.48,-164.9297;Inherit;True;Property;_Back;Back;3;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;19;-1323.579,917.6491;Inherit;True;Property;_Right;Right;4;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;34;-1320.661,1592.157;Inherit;True;Property;_Top;Top;2;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;20;-1323.579,480.7476;Inherit;True;Property;_Left;Left;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-1008.774,-392.8204;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-1007.585,1826.471;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;8;-2111.521,850.898;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;23;-965.068,1195.718;Inherit;False;Constant;_Vector1;Vector 1;2;0;Create;True;0;0;False;0;False;1,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;11;-980.4418,113.1396;Inherit;False;Constant;_Vector0;Vector 0;2;0;Create;True;0;0;False;0;False;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-994.5844,2207.371;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;32;-962.15,2307.128;Inherit;False;Constant;_Vector4;Vector 4;2;0;Create;True;0;0;False;0;False;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-997.2742,0.8796272;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-1010.185,713.6719;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-976.3846,1080.272;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;46;-847.9357,-119.2309;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;10;-755.2689,171.3089;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;38;-736.9771,2365.298;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;26;-739.8951,1253.888;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;44;-838.9102,565.7302;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;47;-822.1315,959.3513;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;49;-827.3312,2086.451;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;48;-829.9312,1702.952;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;45;-861.8378,-506.2596;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCIf;55;-555.2698,2000.298;Inherit;False;6;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;5;FLOAT;0.1;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCIf;53;-577.5353,-166.6476;Inherit;False;6;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;5;FLOAT;0.1;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCIf;54;-568.8837,931.2947;Inherit;False;6;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;5;FLOAT;0.1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;89.46713,935.944;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;376.7041,887.5815;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;MultitexturedCube;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;56;0;6;0
WireConnection;56;1;14;0
WireConnection;60;0;34;0
WireConnection;60;1;33;0
WireConnection;61;0;31;0
WireConnection;61;1;30;0
WireConnection;57;0;7;0
WireConnection;57;1;15;0
WireConnection;58;0;20;0
WireConnection;58;1;21;0
WireConnection;59;0;19;0
WireConnection;59;1;22;0
WireConnection;46;0;15;0
WireConnection;46;1;57;0
WireConnection;46;2;7;4
WireConnection;10;0;11;0
WireConnection;10;1;8;0
WireConnection;38;0;32;0
WireConnection;38;1;8;0
WireConnection;26;0;23;0
WireConnection;26;1;8;0
WireConnection;44;0;21;0
WireConnection;44;1;58;0
WireConnection;44;2;20;4
WireConnection;47;0;22;0
WireConnection;47;1;59;0
WireConnection;47;2;19;4
WireConnection;49;0;30;0
WireConnection;49;1;61;0
WireConnection;49;2;31;4
WireConnection;48;0;33;0
WireConnection;48;1;60;0
WireConnection;48;2;34;4
WireConnection;45;0;14;0
WireConnection;45;1;56;0
WireConnection;45;2;6;4
WireConnection;55;0;38;0
WireConnection;55;2;48;0
WireConnection;55;4;49;0
WireConnection;53;0;10;0
WireConnection;53;2;46;0
WireConnection;53;4;45;0
WireConnection;54;0;26;0
WireConnection;54;2;47;0
WireConnection;54;4;44;0
WireConnection;28;0;53;0
WireConnection;28;1;54;0
WireConnection;28;2;55;0
WireConnection;0;0;28;0
ASEEND*/
//CHKSM=C16C51AF1881481EAABCDB8EAE6204178A91A6B5