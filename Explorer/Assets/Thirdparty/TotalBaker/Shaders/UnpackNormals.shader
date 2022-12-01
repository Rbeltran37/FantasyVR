/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/UnpackNormals" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" { }
	}
	SubShader{

		ZTest Always Cull Off ZWrite Off Fog{ Mode Off }

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;

			struct v2f {
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};

			float4 _MainTex_ST;

			inline fixed4 UnpackNormalDXT5nm1(fixed4 packednormal){
				fixed4 normal;
				normal.xy = (packednormal.wy * 2 - 1) * 0.5 + 0.5;
				normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy))) * 0.5 + 0.5;
				normal.w = 1;
				return normal;
			}

			inline fixed4 UnpackNormal1(fixed4 packednormal){
				#if defined(UNITY_NO_DXT5nm)
					packednormal.w = 1;
					packednormal.xyz = (packednormal.xyz * 2 - 1) * 0.5 + 0.5;
					return packednormal;
				#else
					return UnpackNormalDXT5nm1(packednormal);
				#endif
			}


			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR {
				return UnpackNormal1(tex2D(_MainTex, i.uv));
			}

			ENDCG
		}
	}
}