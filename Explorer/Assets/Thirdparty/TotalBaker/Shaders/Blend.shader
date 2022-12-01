/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

Shader "Hidden/TB/BlendTextures" {
    Properties {
        _Background ("Background (RGBA)", 2D) = "white" {}
        _Foreground ("Foreground (RGBA) ", 2D) = "white" {}
    }
    SubShader {
        Pass {
            SetTexture [_Background] {
                combine texture
            }
            SetTexture [_Foreground] {
                combine texture lerp (texture) previous
            }
        }
    }
}