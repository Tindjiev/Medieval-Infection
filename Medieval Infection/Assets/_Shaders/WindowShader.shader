Shader "Window/WindowShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0

        [HDR] _EmissionColor("Emission color", Color) = (0,0,0,1)
        _Frequency("Frequency", Range(0,20)) = 1.597
        _TimeOffset("Time offset", Range(0,1)) = 0.0
        [HideInInspector]_Centre("Centre",Vector) = (0,0,0,1)
        [HideInInspector]_DistanceSq("Distance Squared", Float) = 3.0
    }


    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        uniform sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        uniform half _Glossiness;
        uniform half _Metallic;
        uniform fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        #define TAU 6.28318530718
        #define SIN01(f_,offset_) ((sin((_Time[1] * f_ - (offset_)) * TAU) + 1.0) * 0.5)

        uniform float _Frequency, _TimeOffset, _DistanceSq;
        uniform float3 _EmissionColor;
        uniform float3 _Centre;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            #define INVERSE_SQ saturate(_DistanceSq / dot(IN.worldPos - _Centre, IN.worldPos - _Centre))
            #define SIN (SIN01(_Frequency, _TimeOffset) * (1.2 - 1.1) + 1.1)

            o.Emission = c.rgb * tex2D(_MainTex, IN.uv_MainTex).a * _EmissionColor * SIN * INVERSE_SQ;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
