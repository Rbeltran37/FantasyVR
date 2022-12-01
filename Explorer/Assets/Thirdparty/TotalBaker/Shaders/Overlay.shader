/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/Overlay"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_OverlayTex ("Overlay", 2D) = "white" {}
		_Factor ("Factor", 2D) = "white" {}
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
			sampler2D _OverlayTex;
			float _Factor;

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			float4 Unity_Blend_Overlay(float4 Base, float4 Blend, float Opacity)
            {
                float4 result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
                float4 result2 = 2.0 * Base * Blend;
                float4 zeroOrOne = step(Base, 0.5);
                float4 Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
                Out = lerp(Base, Out, Opacity);
                return Out;
            }
			
			float4 frag (v2f i) : SV_Target
			{  
                float4 main = tex2D(_MainTex, i.uv);   
                float4 overlay = tex2D(_OverlayTex, i.uv);   
                return Unity_Blend_Overlay(main, overlay, _Factor);
            }
			
			ENDCG
		}
	}
}
