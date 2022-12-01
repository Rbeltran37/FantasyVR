/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/AddLights"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_High2LowWpos ("High2LowWpos", 2D) = "white" {}
		_High2LowNormal ("High2LowNormal", 2D) = "white" {}
	}
	SubShader
	{
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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _High2LowWpos;
			sampler2D _High2LowNormal;
			
			int _LightsCount;
			float _LightsValidation[100];  
			float _LightsTypes[100];  
			float4 _LightsPositions[100];  
			float4 _LightsDirections[100];  
			float4 _LightsColors[100];
			float _LightsIntensities[100];
			float _LightsRanges[100];
			float _LightsOuterCosAngles[100];
			float _LightsInnerCosAngles[100];
			float4 _AmbientColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
			    int atLeast1Light = 0; //is there at least one valid light?
			    
			    float4 color = tex2D(_MainTex, i.uv);
			    float3 p = tex2D(_High2LowWpos, i.uv).rgb;
			    float3 normal = tex2D(_High2LowNormal, i.uv).rgb;
				
				float4 addition = fixed4(0,0,0,0); //the lights will add this color to the original one 
				_AmbientColor.a = 1;
                                
                for (int i = 0; i < _LightsCount; i++) {                    
    
                    if(_LightsValidation[i] < 1){
                        continue; //null light
                    }
                    atLeast1Light = 1;
                    if (_LightsTypes[i] == 2) { //point light
                        float3 dir = _LightsPositions[i] - p;
                        float distance = length(dir);
                        dir /= distance;
                        float normalizedDist = distance / _LightsRanges[i];
                        float attenuation = saturate(1.0 / (1.0 + 25.0*normalizedDist*normalizedDist) * saturate((1.0 - normalizedDist) * 5.0));
                        float dotProduct =  max(dot(dir, normal), 0);
                        addition += _LightsColors[i] * color * dotProduct * attenuation * _LightsIntensities[i];
                    }
    
                    else if (_LightsTypes[i] == 1) { //directional light
                        float dotProduct =  max(dot(normal, -_LightsDirections[i]), 0);
                        addition += _LightsColors[i] * color * dotProduct * _LightsIntensities[i];
                    }
                    
                    else if (_LightsTypes[i] == 0) { //spot light
                        float spotAttenuation;
    
                        float3 dir = _LightsPositions[i] - p;
                        float distance = length(dir);
                        float3 normalizedDir = dir / distance;
                        float normalizedDist = distance / _LightsRanges[i];
                        float attenuation = saturate(1.0 / (1.0 + 25.0*normalizedDist*normalizedDist) * saturate((1.0 - normalizedDist) * 5.0));
    
                        // See if point on surface is inside cone of illumination
                        float3 dirFromLightToPoint = normalize(p - _LightsPositions[i]);
                        float spotCosine = dot(_LightsDirections[i], dirFromLightToPoint);

                        float innerCosCutoff = _LightsInnerCosAngles[i];
                        float outerCosCutoff = _LightsOuterCosAngles[i];
                            
                        float epsilon = innerCosCutoff-outerCosCutoff;
                        float smoothFactor = clamp((-outerCosCutoff+spotCosine) / epsilon, 0.0, 1.0);
    
                        //float exponent = 0;
                        if (spotCosine < outerCosCutoff)
                            spotAttenuation = 0.0; // light adds no contribution
                        else
                            //spotAttenuation = Mathf.Pow(spotCosine, exponent);
                            spotAttenuation = 1.0; //not using exponent
                        
                        spotAttenuation *= smoothFactor; 
                        attenuation *= spotAttenuation; 
                        
                        float dotProduct =  max(dot(normalizedDir, normal), 0);
                        
                        addition += _LightsColors[i] * color * attenuation * dotProduct * _LightsIntensities[i];
                    }
                }
                if(atLeast1Light > 0){
                    color *= _AmbientColor;
                    color += addition;
                }
                return color;
			}
			ENDCG
		}
	}
}
