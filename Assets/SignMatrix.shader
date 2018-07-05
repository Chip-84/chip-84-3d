Shader "Custom/SignMatrix"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		_LightTex("Light Texture", 2D) = "white" {}
		_OffTex("Off Texture", 2D) = "white" {}
		_Res("Resolution", Int) = 32.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _LightTex;
			sampler2D _OffTex;
			int _Res;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				int _Wid = _Res * 2;
				fixed4 albedo = tex2D(_MainTex, i.uv);
				float2 matrixUv = float2(i.uv.x * (_Wid + 2), i.uv.y * (_Res + 2));
				// sample the texture
				fixed4 col = tex2D(_LightTex, matrixUv)*2 * albedo;
				col += tex2D(_OffTex, matrixUv) * (1 - albedo);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
