Shader "Custom/GraphingPaper" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Unfilled("Unfilled Square", 2D) = "white" {}
		_Filled("Filled Square", 2D) = "white" {}
		_Overlay("Overlay", 2D) = "white" {}
		_Glossiness ("Smoothness", 2D) = "white" {}
		_Res("Resolution", Int) = 32.0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200
		Cull Off
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_Unfilled;
			float2 uv_Overlay;
		};

		sampler2D _Glossiness;
		sampler2D _Unfilled;
		sampler2D _Filled;
		sampler2D _Overlay;
		half _Metallic;
		int _Res;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			float2 graphuv = float2(IN.uv_Unfilled.x * (_Res * 2 + 2), IN.uv_Unfilled.y * (_Res+2)*1.25);
			fixed4 map = tex2D(_Glossiness, IN.uv_MainTex);
			fixed4 albedo = tex2D(_MainTex, float2(IN.uv_MainTex.x, IN.uv_MainTex.y * 1.25 - 0.121));
			fixed4 c = tex2D(_Filled, graphuv/4) * albedo;
			c += tex2D(_Unfilled, graphuv) * (1-albedo);
			c *= tex2D(_Overlay, IN.uv_Overlay);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = map.r;
			o.Alpha = (1-map.g);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
