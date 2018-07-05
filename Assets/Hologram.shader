Shader "Custom/Hologram"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Scanlines("Scanlines", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100
		Cull Off

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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 worldVertex : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
			};

			sampler2D _MainTex;
			sampler2D _Scanlines;
			float4 _MainTex_ST;
			float4 _Color;
			
			float rand(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
			}

			v2f vert (appdata v)
			{
				v2f o;
				
				
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex);
				o.viewDir = normalize(UnityWorldSpaceViewDir(o.worldVertex.xyz));
				v.vertex.x += (-rand(o.viewDir)/4 + rand(v.vertex) / 4) * (step(0.5, sin(_Time.y * 2.0 + v.vertex.y * 1.0)) * step(0.99, sin(_Time.y * 100 * 0.5)));
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 albedo = tex2D(_MainTex, i.uv);
				fixed4 col = _Color;
				col -= 0.3 * (1-albedo);
				col *= 3;
				col += 0.4;
				col.a = tex2D(_Scanlines, float2(i.uv.x, i.uv.y*64 - _Time.w));
				// apply fog
				return col;
			}
			ENDCG
		}
	}
}
