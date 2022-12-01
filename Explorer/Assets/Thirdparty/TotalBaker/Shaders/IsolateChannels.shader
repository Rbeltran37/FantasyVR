/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/IsolateChannels"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	
	SubShader
	{
		Cull Off
		Lighting Off
		ZWrite Off

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile _MODE_RGBA _MODE_RGB _MODE_R _MODE_G _MODE_B _MODE_A
			
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			int _R, _G, _B, _A;
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				
				#ifdef _MODE_RGBA
				    return col;
				#endif
				
				#ifdef _MODE_RGB
				    col.a = 1;
                    return col;
                #endif	
                
                #ifdef _MODE_R
				    col.g = 0;
				    col.b = 0;
				    col.a = 1;
                    return col;
                #endif	
                
                #ifdef _MODE_G
				    col.r = 0;
				    col.b = 0;
				    col.a = 1;
                    return col;
                #endif	
                
                #ifdef _MODE_B
				    col.r = 0;
				    col.g = 0;
				    col.a = 1;
                    return col;
                #endif	
                
                #ifdef _MODE_A
				    col.r = col.a;
				    col.g = col.a;
				    col.b = col.a;
				    col.a = 1;
                    return col;
                #endif						
				
			}
		ENDCG
		}
	}
}
