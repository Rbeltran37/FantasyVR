/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/Blur" {
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
			float4 _MainTex_TexelSize;
			float step_h;
			float step_v;

			struct v2f {
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};

			float4 _MainTex_ST;

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR{

				step_h = _MainTex_TexelSize.x;
				step_v = _MainTex_TexelSize.y;

				float2 offset[9] = {
					float2(-step_h, -step_v), float2(0.0, -step_v), float2(step_h, -step_v),
					float2(-step_h, 0.0),     float2(0.0, 0.0),     float2(step_h, 0.0),
					float2(-step_h, step_v),  float2(0.0, step_v),  float2(step_h, step_v)
				};

				float kernel[9] = {
					0.066508,	0.124875,	0.066508,
					0.124875,	0.234467,	0.124875,
					0.066508,	0.124875,	0.066508
				};

				float4 sum = float4(0.0, 0.0, 0.0, 0.0);

				for (int j = 0; j < 9; j++) {
					float4 tmp = tex2D(_MainTex, i.uv + offset[j]);
					sum += tmp * kernel[j];
				}
				return sum;		
            }
            
			ENDCG
		}

	}
}