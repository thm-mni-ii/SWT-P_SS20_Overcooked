// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Fluid"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", Color) = (1,0,0,0)
		_Fill("Fill", Range( 0 , 1)) = 0
		_MaxVertexHeight("MaxVertexHeight", Float) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float3 worldPos;
		};

		uniform float4 _Albedo;
		uniform float _MaxVertexHeight;
		uniform float _Fill;
		uniform float _Cutoff = 0.5;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = _Albedo.rgb;
			o.Alpha = 1;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 _Vector0 = float3(0,1,0);
			float4 transform20 = mul(unity_WorldToObject,float4( _Vector0 , 0.0 ));
			float dotResult18 = dot( float4( _Vector0 , 0.0 ) , transform20 );
			clip( ( ( ase_vertex3Pos.y / _MaxVertexHeight ) < ( _Fill * saturate( dotResult18 ) ) ? 1.0 : 0.0 ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18100
464;253;1399;732;1460.783;118.4626;1.353488;True;False
Node;AmplifyShaderEditor.Vector3Node;19;-1346.695,322.5685;Inherit;False;Constant;_Vector0;Vector 0;4;0;Create;True;0;0;False;0;False;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldToObjectTransfNode;20;-1147.402,557.5859;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;18;-923.0764,540.407;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-857.754,167.0571;Inherit;False;Property;_MaxVertexHeight;MaxVertexHeight;3;0;Create;True;0;0;False;0;False;0;0.345;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-892.2648,310.4759;Inherit;False;Property;_Fill;Fill;2;0;Create;True;0;0;False;0;False;0;0.115;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;25;-779.9259,540.3547;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;16;-689.2416,-3.769527;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;6;-460.165,147.0169;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-528.3948,375.9067;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-387.2856,-63.76131;Inherit;False;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;False;1,0,0,0;1,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Compare;3;-248.5642,218.7069;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;12;-12,-15;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Fluid;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;1;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;20;0;19;0
WireConnection;18;0;19;0
WireConnection;18;1;20;0
WireConnection;25;0;18;0
WireConnection;6;0;16;2
WireConnection;6;1;5;0
WireConnection;26;0;2;0
WireConnection;26;1;25;0
WireConnection;3;0;6;0
WireConnection;3;1;26;0
WireConnection;12;2;1;0
WireConnection;12;10;3;0
ASEEND*/
//CHKSM=103AAFD91A831A72EC1F83EEA8DCA1B3A18BD5FD