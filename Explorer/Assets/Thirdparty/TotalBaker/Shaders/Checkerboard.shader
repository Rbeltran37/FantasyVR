/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/Checkerboard"{
	Properties{
		_Density("Density", Float) = 50
		_Color1("Color1", Color) = (0.4,0.4,0.4,1)
		_Color2("Color2", Color) = (0.6,0.6,0.6,1)
	}
		SubShader{
			Pass{
				CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag
				float _Density;
				fixed4 _Color1;
				fixed4 _Color2;

			#include "UnityCG.cginc"

			float checker(float u, float v){
				float fmodResult = (floor(_Density * u) + floor(_Density * v)) % 2.0;

				if (fmodResult < 1.0) {
					return _Color1;
				}
				return _Color2;
			}

			fixed4 frag(v2f_img i) : SV_Target{
				float c = checker(i.uv.x, i.uv.y);
				return fixed4(c,c,c,1);
			}

		ENDCG
		}
	}
}