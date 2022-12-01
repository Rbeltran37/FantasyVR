/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/CombineNormals" {
	Properties{
		_MainTex("Bump Map (RGB)", 2D) = "white" { }
		_Details("Detail Normals (RGB)", 2D) = "white" { }
	}
	SubShader{

	ZTest Always Cull Off ZWrite Off Fog{ Mode Off }

	Pass{
	CGPROGRAM

	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	sampler2D _Details;

	float4 _MainTex_ST;
	float4 _Details_ST;

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


	fixed3 blendNormals(fixed4 n1, fixed4 n2) {
		n1 = n1.xyzz*fixed4(2, 2, 2, -2) + fixed4(-1, -1, -1, 1);
		n2 = n2 * 2 - 1;
		fixed3 r;
		r.x = dot(n1.zxx, n2.xyz);
		r.y = dot(n1.yzy, n2.xyz);
		r.z = dot(n1.xyw, -n2.xyz);
		return normalize(r);
	}

	fixed4 frag(v2f IN) : COLOR{

		fixed4 n1 = tex2D(_MainTex, IN.uv);
		fixed4 n2 = tex2D(_Details, IN.uv);
		fixed4 normal = fixed4(blendNormals(n1, n2)*0.5 + 0.5, 1.0);
		return normal;
	}
	ENDCG //Shader End
	}

	}
}