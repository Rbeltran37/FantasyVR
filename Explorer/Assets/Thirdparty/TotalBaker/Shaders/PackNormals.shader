/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/PackNormals" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" { }
	}
	SubShader{

		ZTest Always Cull Off ZWrite Off Fog{ Mode Off }

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;

			struct v2f {
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};

			float4 _MainTex_ST;

			inline fixed4 PackNormal(fixed4 normal){
				#if defined(UNITY_NO_DXT5nm)
					return normal;
				#else
					normal.a = normal.r;
					normal.r = 0;
					normal.b = 0;
					return normal;
				#endif					
			}

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR {
				return PackNormal(tex2D(_MainTex, i.uv));
			}

			ENDCG
		}
	}
}
