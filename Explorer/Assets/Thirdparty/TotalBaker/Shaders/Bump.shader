/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/Bump" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" { }
		_Strength("Strength", Float) = 0.0
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
		float _Strength;

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


	float4 frag(v2f i) : COLOR{  
	          
        float texelSize = _MainTex_TexelSize.x;  
	  
	    float tl = tex2D(_MainTex, i.uv + texelSize * fixed2(-1, -1)).r;     
	    float  l = tex2D(_MainTex, i.uv + texelSize * fixed2(-1,  0)).r;     
        float bl = tex2D(_MainTex, i.uv + texelSize * fixed2(-1,  1)).r;    
	    float  t = tex2D(_MainTex, i.uv + texelSize * fixed2(0, -1)).r;     
        float  b = tex2D(_MainTex, i.uv + texelSize * fixed2(0,  1)).r;     
	    float tr = tex2D(_MainTex, i.uv + texelSize * fixed2(1, -1)).r;     
        float  r = tex2D(_MainTex, i.uv + texelSize * fixed2(1,  0)).r;     
	    float br = tex2D(_MainTex, i.uv + texelSize * fixed2(1,  1)).r;     
  
        float Sx = -tr - 2.0f * r - br + tl + 2.0f * l + bl;  
        float Sy = -bl - 2.0f * b - br + tl + 2.0f * t + tr;  
  
        // Build the normalized normal  
	    float4 N = float4(normalize(float3(Sx, Sy, 1.0/_Strength)), 1.0f);  
 
	    return N * 0.5f + 0.5f;  
	  


	}


		ENDCG //Shader End
	}
	}
}
