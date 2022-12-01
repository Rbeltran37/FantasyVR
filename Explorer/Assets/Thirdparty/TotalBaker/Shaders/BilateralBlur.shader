/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
* Based on: https://www.shadertoy.com/view/4dfGDH
*
=====================================================*/

Shader "Hidden/TB/BilateralBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
            
        //Positional sigma which determines the blur intensity
        _SigmaP ("Intensity", float) = 10.0
        //Range sigma which controls the gaussian curve (higher values will produce results similar to gaussian blur)
        _SigmaR ("Range", float) = 0.1
	}
	SubShader
	{
		ZTest Always Cull Off ZWrite Off Fog{ Mode Off }
        
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _SigmaP;
			float _SigmaR;
			#define KERNEL_SIZE 15
									
			float normpdf(float x, float sigma)
            {
                return 0.39894*exp(-0.5*x*x/(sigma*sigma))/sigma;
            }

            float normpdf3(fixed3 v, float sigma)
            {
                return 0.39894*exp(-0.5*dot(v,v)/(sigma*sigma))/sigma;
            }

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				                   
                //declare stuff
                const int kSize = (KERNEL_SIZE-1)/2;
                float kernel[KERNEL_SIZE];
                fixed4 result = fixed4(0,0,0,1);
                
                //create the 1-D kernel
                float Z = 0.0;
                for (int j = 0; j <= kSize; ++j)
                {
                    kernel[kSize+j] = kernel[kSize-j] = normpdf(float(j), _SigmaP);
                }
                   
                fixed4 cc;
                float factor;
                float bZ = 1.0/normpdf(0.0, _SigmaR);
                //read out the texels
                for (int ii=-kSize; ii <= kSize; ++ii)
                {
                    for (int j=-kSize; j <= kSize; ++j)
                    {
                        cc = tex2D(_MainTex, i.uv+float2(ii,j)*_MainTex_TexelSize.xy);
                        factor = normpdf3(cc-c, _SigmaR)*bZ*kernel[kSize+j]*kernel[kSize+ii];
                        Z += factor;
                        result += factor*cc;        
                    }
                }     
                                         
                fixed4 col = result/Z;                      
                col.a = 1;
                return col;          
			}
			ENDCG
		}
	}
}
