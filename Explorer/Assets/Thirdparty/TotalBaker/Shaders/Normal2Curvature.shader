/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/Normal2Curvature"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Multiplier ("Multiplier", Range(0,5)) = 1
    }
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
					
			#include "UnityCG.cginc"
			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _Multiplier;
			int _Width;

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			
			float Unity_Blend_Overlay(float Base, float Blend, float Opacity)
            {
                float result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
                float result2 = 2.0 * Base * Blend;
                float zeroOrOne = step(Base, 0.5);
                float Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
                Out = lerp(Base, Out, Opacity);
                return Out;
            }

			float4 frag (v2f i) : SV_Target
			{  
                float4 n = tex2D(_MainTex, i.uv);
                                
                if(n.a <= 0)
                {
                    discard;
                }

                float texelW = _MainTex_TexelSize.x;
				float texelH = _MainTex_TexelSize.y;
                float3 xneg = tex2D(_MainTex, i.uv - float2(texelW,0));
                float xm = xneg.r * _Multiplier;
                float3 xpos = tex2D(_MainTex, i.uv + float2(texelW,0));
                float xp = xpos.r * _Multiplier;
                float3 yneg = tex2D(_MainTex, i.uv - float2(0, texelH));
                float ym = yneg.g * _Multiplier;
                float3 ypos = tex2D(_MainTex, i.uv + float2(0, texelH));
                float yp = ypos.g * _Multiplier;
                float x = clamp(xm-xp+0.5, 0, 1);
                float y = clamp(ym-yp+0.5, 0, 1);
                float curvature = 1-Unity_Blend_Overlay(x,y,1);
                return float4(curvature, curvature, curvature, 1);
            }
			
			ENDCG
		}
	}
}
