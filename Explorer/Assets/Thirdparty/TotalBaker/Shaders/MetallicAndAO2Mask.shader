/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/MetallicAndAO2Mask"
{
    Properties
    {
        _MetallicMap ("Metallic Map", 2D) = "white" {}
        _AOMap ("AO Map", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Cull Off 
        ZWrite Off 
        ZTest Always
        
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

            sampler2D _MetallicMap;
            sampler2D _AOMap;
            float4 _MetallicMap_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 metallic = tex2D(_MetallicMap, i.uv);
                fixed4 ao = tex2D(_AOMap, i.uv);
                return fixed4(metallic.r, ao.r, 1, metallic.a);
            }
            ENDCG
        }
    }
}
