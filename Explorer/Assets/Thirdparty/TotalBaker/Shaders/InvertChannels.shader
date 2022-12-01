/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/InvertChannels" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" { }
		[MaterialToggle] _R ("Invert R", Int) = 0
		[MaterialToggle] _G ("Invert G", Int) = 0
		[MaterialToggle] _B ("Invert B", Int) = 0
	}
	SubShader{

		ZTest Always Cull Off ZWrite Off Fog{ Mode Off }				
		//Tags {"Queue"="Transparent"}		
		//Blend SrcAlpha OneMinusSrcAlpha 

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			int _R, _G, _B;

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

			float4 frag(v2f i) : COLOR {
				float4 c = tex2D(_MainTex, i.uv);
				if(_R!=0) c.r = 1-c.r;
				if(_G!=0) c.g = 1-c.g;
				if(_B!=0) c.b = 1-c.b;
				return c;
			}

			ENDCG
		}
	}
}