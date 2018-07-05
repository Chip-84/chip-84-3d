Shader "Custom/Television_frag"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		_Scanlines("Scanlines", 2D) = "black" {}
		_RGBTex("RGB Texture", 2D) = "black" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Blend One One

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
			sampler2D _Scanlines;
			sampler2D _RGBTex;
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
				// sample the texture
				fixed4 albedo = tex2D(_MainTex, i.uv);
				fixed4 albedo_shift = tex2D(_MainTex, float2(i.uv.x - 0.004, i.uv.y));
				fixed4 col = albedo + tex2D(_RGBTex, float2(i.uv.x*2, i.uv.y + _Time.x*9)*10)/4;
				col += albedo_shift/6 + fixed4(0.15,0,0,0)*albedo_shift;
				col += tex2D(_Scanlines, i.uv * 128);
				col -= 0.1;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
