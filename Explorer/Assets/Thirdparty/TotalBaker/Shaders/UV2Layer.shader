/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/UV2Layer" {
	Properties{
			_LayerMask("Layer Mask", int) = 0
	}
	SubShader{
		Pass{

			Lighting Off
			Cull Off

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				
				#define MAX_INT 2147483647

				struct v2f {
					float4 pos : SV_POSITION;
					float4 color : COLOR;
				};

				// vertex input: position, UV
				struct appdata {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};
				
				int _LayerMask;

				v2f vert(appdata v){
					v2f o;
					float l = (float)_LayerMask/MAX_INT;
                    o.color = float4(l, l, l, 1);					
					o.pos = float4(v.uv.x * 2.0 - 1.0, v.uv.y * 2.0 - 1.0, 1.0, 1.0);
					return o;		
				}

				float4 frag(v2f i) : SV_Target{ 
					return i.color;
				}
			ENDCG
		}
	}
}