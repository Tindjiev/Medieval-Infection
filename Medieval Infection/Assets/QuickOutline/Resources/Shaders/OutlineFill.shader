//
//  OutlineFill.shader
//  QuickOutline
//
//  Created by Chris Nolet on 2/21/18.
//  Copyright © 2018 Chris Nolet. All rights reserved.
//
//  (Modified for this project)
//

Shader "Custom/Outline Fill" {

    Properties{
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 0

        _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineWidth("Outline Width", Range(0, 10)) = 1
    }

    SubShader{

        Tags {
            "Queue" = "Transparent+110"
            "RenderType" = "Transparent"
            "DisableBatching" = "True"
        }

        Pass {
            Name "Fill"
            Cull Off
            ZTest[_ZTest]
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB

            Stencil {
                Ref 1
                ReadMask 1
                Comp NotEqual
            }


            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            struct appdata {
                float4 vertexPosition : POSITION;
                float3 normal : NORMAL;
                float3 smoothNormal : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 position : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            uniform fixed4 _OutlineColor;
            uniform float _OutlineWidth;

            #define CHOOSE_SMOOTH_NORMAL(vData_) (vData_.smoothNormal != 0 ? vData_.smoothNormal : vData_.normal)
            #define CALC_VIEWNROMAL(normal_) normalize(mul((float3x3)UNITY_MATRIX_IT_MV, normal_))

            v2f vert(appdata input) {

                v2f output;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                //at Zdistance of 1 from camera the outlinewidth should be 0.01 with _OutlineWidth=1
                output.position = UnityViewToClipPos(UnityObjectToViewPos(input.vertexPosition) + CALC_VIEWNROMAL(CHOOSE_SMOOTH_NORMAL(input)) * 0.01 * _OutlineWidth);

                return output;
            }

            fixed4 frag(v2f input) : SV_Target {
                return _OutlineColor;
            }

            ENDCG
        }
    }
}
