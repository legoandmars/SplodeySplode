Shader "RogueNoodle/GBPalette"
{
	Properties
	{
		_RenderTexture("RenderTexture", 2D) = "white" {}
		_OverlayRenderTexture("OverlayRenderTexture", 2D) = "white" {}
		_Palette("Palette", 2D) = "white" {}
		_Fade("Fade", Range( 0 , 5)) = 1
		_PaletteMove("Shift Palette", Float) = 0 // only works if palette is repeat instead of clamp.
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Palette;
		uniform sampler2D _RenderTexture;
		uniform sampler2D _OverlayRenderTexture;
		uniform float4 _RenderTexture_ST;
		uniform float _Fade;
		uniform float _PaletteMove;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_RenderTexture = i.uv_texcoord * _RenderTexture_ST.xy + _RenderTexture_ST.zw;
			float lerpResult4 = lerp( tex2D( _RenderTexture, uv_RenderTexture ).r , 0.0 , ( 1.0 - _Fade ));
			float2 appendResult3 = (float2(lerpResult4  + _PaletteMove, lerpResult4));
			float4 overlay = tex2D( _OverlayRenderTexture, uv_RenderTexture);
			// o.Emission = tex2D( _Palette, appendResult3 ).rgb + overlay * overlay.a;
			o.Emission = lerp(tex2D( _Palette, appendResult3 ).rgb, overlay, overlay.a);
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
