Shader "Custom/NoRender"
{
    SubShader
    {
        Tags{ "Queue" = "Geometry-1" }
        Lighting Off
        Cull Off
        ZTest Always
        ColorMask 0
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 vert() : SV_POSITION
            {
                return 0;
            }

            fixed4 frag(fixed4 pos : SV_POSITION) : SV_Target
            {
                return 0;
            }
            ENDCG
        }
    }
}
