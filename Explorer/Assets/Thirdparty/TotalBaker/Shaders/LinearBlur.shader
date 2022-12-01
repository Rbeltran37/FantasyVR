/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/LinearBlur" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" { }
		_Offset("Offset", Float) = 1.3
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
	float _Offset;


	struct v2f {
		float4  pos : SV_POSITION;
		float2  uv : TEXCOORD0;
	};

	float4 _MainTex_ST;
	float4 _MainTex_ST_TexelSize;

	v2f vert(appdata_base v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		return o;
	}

	fixed4 frag(v2f IN) : COLOR{
		//original: 1.3846153846, 3.2307692308
		float3 offset = float3(0.0, _Offset/_MainTex_TexelSize.z, _Offset*4.5/_MainTex_TexelSize.z);
		float3 weight = float3(0.1135135135*1.128, 0.158108108*1.128, 0.03513513515*1.128);

		float4 col;
		float w;
		float o;

		col = tex2D(_MainTex, float2(IN.uv)) * weight.x;
		for (int i = 1; i<3; i++) {
			if (i == 1) {
				w = weight.y;
				o = offset.y;
			}
			if (i == 2) {
				w = weight.z;
				o = offset.z;
			}
			float2 yy1 = float2(IN.uv) + float2(0.0, o);
			float2 yy2 = float2(IN.uv) - float2(0.0, o);
			//if (yy1.y < 1 && yy1.y > 0) col += tex2D(_MainTex, yy1)* w;
			//if (yy2.y < 1 && yy2.y > 0) col += tex2D(_MainTex, yy2)* w;
			col += tex2D(_MainTex, yy1)* w;
			col += tex2D(_MainTex, yy2)* w;

			yy1 = float2(IN.uv) + float2(o, 0.0);
			yy2 = float2(IN.uv) - float2(o, 0.0);
			col += tex2D(_MainTex, yy1)* w;
			col += tex2D(_MainTex, yy2)* w;
			
		}
		return col;
	}

	ENDCG //Shader End
	}

	}
}