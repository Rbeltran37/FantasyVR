/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/Erode" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" { }
		_Background("Background", Color) = (0,0,0,1)
		_MaxSteps("Max Steps", Float) = 0.0
	}
	SubShader{

		ZTest Always Cull Off ZWrite Off Fog{ Mode Off }

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma target 4.0

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _MaxSteps;
			float4 _Background;

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
			
			half OffsetEqualsBackground(float4 sample){
                if(sample.r == _Background.r || sample.g == _Background.g || sample.b == _Background.b || sample.a == _Background.a){
                    return 1;
                }
                return 0;
            }

			float4 frag(v2f input) : COLOR{

			float texelSize = _MainTex_TexelSize.x;				
			float4 mainSample = tex2D(_MainTex, input.uv);
			if (OffsetEqualsBackground(mainSample)) return mainSample;

			float4 offsetsample = float4(0,0,0,0);
			int off = 0;
				int i = 0;
				[unroll(20)]
				while (i < _MaxSteps) {					

					offsetsample = tex2D(_MainTex, input.uv + texelSize * fixed2(1, 0) * i);
					if (OffsetEqualsBackground(offsetsample)) {
						off = 1;
						break;
					}
					offsetsample = tex2D(_MainTex, input.uv + texelSize * fixed2(-1, 0) * i);
					if (OffsetEqualsBackground(offsetsample)) {
						off = 1;
						break;
					}
					offsetsample = tex2D(_MainTex, input.uv + texelSize * fixed2(1, 1) * i);
					if (OffsetEqualsBackground(offsetsample)) {
						off = 1;
						break;
					}
					offsetsample = tex2D(_MainTex, input.uv + texelSize * fixed2(-1, -1) * i);
					if (OffsetEqualsBackground(offsetsample)) {
						off = 1;
						break;
					}
					offsetsample = tex2D(_MainTex, input.uv + texelSize * fixed2(-1, 1) * i);
					if (OffsetEqualsBackground(offsetsample)) {
						off = 1;
						break;
					}
					offsetsample = tex2D(_MainTex, input.uv + texelSize * fixed2(0, 1) * i);
					if (OffsetEqualsBackground(offsetsample)) {
						off = 1;
						break;
					}
					offsetsample = tex2D(_MainTex, input.uv + texelSize * fixed2(0, -1) * i);
					if (OffsetEqualsBackground(offsetsample)) {
						off = 1;
						break;
					}
					offsetsample = tex2D(_MainTex, input.uv + texelSize * fixed2(1, -1) * i);
					if (OffsetEqualsBackground(offsetsample)) {
						off = 1;
						break;
					}
					i++;					
				}
				if (off == 1) return offsetsample;
				return mainSample;
			}
			ENDCG
		}
	}
}



