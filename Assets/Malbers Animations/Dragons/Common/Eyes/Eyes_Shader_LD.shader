// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MA/Eyes_Shader_LD"
{
	Properties
	{
		[NoScaleOffset]_EyeTexture("Eye Texture", 2D) = "white" {}
		_HUE("HUE", Range( 0 , 1)) = 0
		_Saturation("Saturation", Float) = 2
		_Lightness("Lightness", Float) = 2
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.8
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Metallic("Metallic", Range( 0 , 1)) = 0.3294118
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _HUE;
		uniform float _Saturation;
		uniform sampler2D _EyeTexture;
		uniform float _Lightness;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _Cutoff = 0.5;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_EyeTexture7 = i.uv_texcoord;
			float4 tex2DNode7 = tex2D( _EyeTexture, uv_EyeTexture7 );
			float3 hsvTorgb6 = RGBToHSV( tex2DNode7.rgb );
			float3 hsvTorgb12 = HSVToRGB( float3(_HUE,( _Saturation * hsvTorgb6.y ),( hsvTorgb6.z * _Lightness )) );
			o.Albedo = hsvTorgb12;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
			clip( tex2DNode7.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15301
206;164;1666;825;1339.11;191.2654;1.013698;True;False
Node;AmplifyShaderEditor.CommentaryNode;25;-951.788,-221.5728;Float;False;894.4377;571.3958;Color Change;7;6;18;12;5;16;19;17;;0,0.8758622,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;7;-1353.045,245.8828;Float;True;Property;_EyeTexture;Eye Texture;0;1;[NoScaleOffset];Create;True;0;0;False;0;20f2ef1b6e86e8d4e8e6a93afff05f74;7ec2240c3db649446913287cea18cd7e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;18;-901.788,234.8229;Float;False;Property;_Lightness;Lightness;3;0;Create;True;0;0;False;0;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-898.7881,-165.4877;Float;False;Property;_Saturation;Saturation;2;0;Create;True;0;0;False;0;2;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;6;-916.2473,-69.8185;Float;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;5;-619.1212,-178.0008;Float;False;Property;_HUE;HUE;1;0;Create;True;0;0;False;0;0;0.5269905;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-581.3404,-91.13546;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-613.2172,182.9136;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-8.13538,253.7715;Float;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;False;0;0.8;0.4683335;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;29;-582.6617,414.7899;Float;True;HueChange;-1;;4;5cbe9f9fd918cbc4292c90b25314adb1;0;4;1;FLOAT3;0,0,0;False;9;FLOAT;0;False;10;FLOAT;2;False;11;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.HSVToRGBNode;12;-298.586,46.86703;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;24;41.8048,176.5088;Float;False;Property;_Metallic;Metallic;6;0;Create;True;0;0;False;0;0.3294118;0.3294118;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;363.7807,114.917;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;MA/Eyes_Shader_LD;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;0;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;5;-1;-1;-1;0;0;0;False;0;0;0;False;-1;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;0;7;0
WireConnection;16;0;17;0
WireConnection;16;1;6;2
WireConnection;19;0;6;3
WireConnection;19;1;18;0
WireConnection;29;1;7;0
WireConnection;29;9;5;0
WireConnection;29;10;17;0
WireConnection;29;11;18;0
WireConnection;12;0;5;0
WireConnection;12;1;16;0
WireConnection;12;2;19;0
WireConnection;0;0;12;0
WireConnection;0;3;24;0
WireConnection;0;4;23;0
WireConnection;0;10;7;4
ASEEND*/
//CHKSM=FE9AC4F5F68F5873C74A080D5C1046AB4A5F15A3