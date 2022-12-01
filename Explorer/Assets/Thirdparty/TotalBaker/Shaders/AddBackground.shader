/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/AddBackground"
{
	Properties
	{
		_Texture ("Texture (RGBA)", 2D) = "white" {}
		_Background ("Background Color", Color) = (0,0,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

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

			sampler2D _Texture;
			float4 _Texture_ST;
			float4 _Background;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _Texture);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float4 col = tex2D(_Texture, i.uv);
			    
				col = lerp(_Background, col, col.a);
				col.a = clamp(_Background.a + col.a, 0, 1);
				return col;
			}
			ENDCG
		}
	}
}
