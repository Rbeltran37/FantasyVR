/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/World2Tangent" {
    Properties {
        _WorldNormalMap ("World Normal", 2D) = "bump" {}
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
                float3x3 worldToTangent : TEXCOORD1;
            };

            sampler2D _WorldNormalMap;
       
            v2f vert (appdata v)
            {
                v2f o;
             
                //use uv space to output results on the screen
                o.pos = float4(v.texcoord.x*2-1, -(v.texcoord.y*2-1), 1.0, 1.0);
                o.texcoord = v.texcoord;

                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldTangent = UnityObjectToWorldDir(v.tangent);
               
                float3 worldBitangent = cross(worldNormal, worldTangent) * v.tangent.w * unity_WorldTransformParams.w;
                
                
              
                //build the worldToTangent rotation matrix and output it
                
                //precise method
                half3x3 w2tRotation;
                w2tRotation[0] = worldBitangent.yzx * worldNormal.zxy - worldBitangent.zxy * worldNormal.yzx;
                w2tRotation[1] = worldTangent.zxy * worldNormal.yzx - worldTangent.yzx * worldNormal.zxy;
                w2tRotation[2] = worldTangent.yzx * worldBitangent.zxy - worldTangent.zxy * worldBitangent.yzx;
     
                half det = dot(worldTangent.xyz, w2tRotation[0]);
     
                w2tRotation *= rcp(det);
                o.worldToTangent = w2tRotation;
                
                // approximate method (no visible difference in most cases, but more efficient)
                //o.worldToTangent = float3x3(worldTangent, worldBitangent, worldNormal);
                
                return o;
            }
       
            float4 frag (v2f i) : COLOR
            {
                float4 worldNormal = normalize(tex2D(_WorldNormalMap, i.texcoord) * 2.0 - 1.0); // unpack
                float3 tangentNormal = normalize(mul(i.worldToTangent, worldNormal));
                return float4((tangentNormal * 0.5 + 0.5), worldNormal.a); // pack
            }
         
            ENDCG
        }
    }
}