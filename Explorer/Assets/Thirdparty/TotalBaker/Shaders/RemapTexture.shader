/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/Remap" {
	Properties{
			_MainTex("Main Tex", 2D) = "white" { }
			_UVTex("UV Tex", 2D) = "white" { }
		}

	SubShader{

		ZTest Always Cull Off ZWrite Off Fog{ Mode Off }

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _UVTex;

			float4 _MainTex_ST;
			float4 _UVTex_ST;

			struct v2f {
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			
			fixed4 frag(v2f i) : COLOR{
				fixed4 uvsCol = tex2D(_UVTex, i.uv);
				float2 uvs = float2(1-uvsCol.x, uvsCol.y);
				fixed4 c = tex2D(_MainTex, uvs); 
				return c;
			}
			ENDCG
		}
	}
}