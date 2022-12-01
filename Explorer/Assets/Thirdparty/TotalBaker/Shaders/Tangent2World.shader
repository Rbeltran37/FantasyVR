/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/Tangent2World" {
    Properties {
        _TangentNormalMap ("Tangent Normal", 2D) = "bump" {}
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float3x3 tangentToWorld : TEXCOORD1;
            };

            sampler2D _TangentNormalMap;
       
            v2f vert (appdata v)
            {
                v2f o;
             
                //use uv space to output results on the screen
                o.pos = float4(v.texcoord.x*2-1, -(v.texcoord.y*2-1), 1.0, 1.0);
                o.texcoord = v.texcoord;

                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldTangent = UnityObjectToWorldDir(v.tangent);               
                float3 worldBitangent = cross(worldNormal, worldTangent) * v.tangent.w * unity_WorldTransformParams.w;
                
                //build the tangentToWorld rotation matrix and output it
                o.tangentToWorld = transpose(float3x3(worldTangent, worldBitangent, worldNormal));
//                o.tangentToWorld = transpose(float3x3(worldTangent, worldBitangent, worldNormal)); //wrong
                
                return o;
            }
       
            float4 frag (v2f i) : COLOR
            {
                float3 tangentNormal = UnpackNormal(tex2D(_TangentNormalMap, i.texcoord));
                float3 worldNormal = mul(i.tangentToWorld, tangentNormal)*0.5+0.5;
//                float3 worldNormal = mul(tangentNormal, i.tangentToWorld)*0.5+0.5; //wrong
                return float4(worldNormal, 1);
            }
         
            ENDCG
        }
    }
}