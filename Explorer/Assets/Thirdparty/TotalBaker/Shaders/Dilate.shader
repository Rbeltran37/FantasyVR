/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/Dilate"
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
                float4 _MainTex_TexelSize;
    
                fixed4 frag (v2f i) : SV_Target
                {    
                    fixed4 col = tex2D(_MainTex, i.uv);
                    fixed originalA = col.a;
                    
                    //opaque area, return pixel as it is
                    if (col.a > 0) 
                    {
                        return col;
                    }
                    
                    //here we're in the transparent area...                    
                    
                    //get neighbor pixels    
                    fixed4 c0 = tex2D(_MainTex, i.uv + float2(0, _MainTex_TexelSize.y));
                    fixed4 c1 = tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x, 0));
                    fixed4 c2 = tex2D(_MainTex, i.uv - float2(0, _MainTex_TexelSize.y));
                    fixed4 c3 = tex2D(_MainTex, i.uv - float2(_MainTex_TexelSize.x, 0));
                                            
                    float sum = c0.a + c1.a + c2.a + c3.a;	
                                
                    //all neighbors are transparent (their alpha sum is 0), return current transparent pixel
                    if (sum <= 0){ 
                        return col;				
                    }
                    
                    //here, some of the neighbor pixels are opaque
                    
                    //dilate RGB
                    col.rgb = (c0.a * c0.rgb + c1.a * c1.rgb + c2.a * c2.rgb + c3.a * c3.rgb) / sum;     
                    
                    //get maximum alpha     
                    float maxAlpha = max(c0.a, max(c1.a, max(c2.a, c3.a)));
                    
                    col.a = clamp(maxAlpha, 0, 1);
                    return col; 
                }
                
            ENDCG
		}
	}
}
