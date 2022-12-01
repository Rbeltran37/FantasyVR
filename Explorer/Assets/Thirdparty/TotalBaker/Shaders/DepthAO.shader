/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/AOMap" {
	Properties{
			_DirectionMap("Direction Map", 2D) = "white" { }
			_OriginMap("Origin Map", 2D) = "white" { }
			_SampleRad("Sample Radius", Float) = 0.1
			_Intensity("Intensity", Float) = 1
			_Bias("Bias", Float) = 0.1
			_Background("Color", Color) = (0,0,0,0)
		}

	SubShader{

		ZTest Always Cull Off ZWrite Off Fog{ Mode Off }

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _DirectionMap;
			sampler2D _OriginMap;
			sampler2D _Noise;
			float _SampleRad;
			float _Intensity;
			float _Bias;
			float4 _Background;

			float4 _DirectionMap_ST;
			float4 _OriginMap_ST;
			float4 _Noise_ST;

			struct v2f {
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _OriginMap);
				o.uv.y = 1-o.uv.y;
				return o;
			}

			float4 getPosition(in float2 uv){
				return tex2D(_OriginMap,uv);
			}

			float3 getNormal(in float2 uv){
				return normalize(tex2D(_DirectionMap, uv).xyz * 2.0f - 1.0f);
			}

			float doAmbientOcclusion(in float2 tcoord,in float2 uv, in float3 p, in float3 cnorm){
				float3 diff = getPosition(tcoord + uv) - p;
				const float3 v = normalize(diff);
				const float d = length(diff);
				return max(0.0,dot(cnorm,v)-_Bias)*(1.0/(1.0+d))*_Intensity;
			}

			float4 frag(v2f i) : COLOR{
				const float2 vec[4] = {float2(1,0),float2(-1,0),float2(0,1),float2(0,-1)};
				float4 p = getPosition(i.uv);
				float3 n = getNormal(i.uv);
				float ao = 0.0f;
				float rad = _SampleRad/p.z;
				//**SSAO Calculation**//
				int iterations = 4;
				for (int j = 0; j < iterations; ++j){
					float2 coord1 = reflect(vec[j],float2(1,1))*rad;
					float2 coord2 = float2(coord1.x*0.707 - coord1.y*0.707,
					coord1.x*0.707 + coord1.y*0.707);
					ao += doAmbientOcclusion(i.uv,coord1*0.25, p, n);
					ao += doAmbientOcclusion(i.uv,coord2*0.5, p, n);
					ao += doAmbientOcclusion(i.uv,coord1*0.75, p, n);
					ao += doAmbientOcclusion(i.uv,coord2, p, n);
				}
				ao/=(float)iterations*4.0;
				if(p.a == 0) return _Background; 
				return float4(1-ao,1-ao,1-ao,p.a);
			}
			ENDCG
		}
	}
}