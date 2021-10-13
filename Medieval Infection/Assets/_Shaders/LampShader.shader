Shader "Lamp/LampShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0

        [HDR] _EmissionColor("Emission color", Color) = (0,0,0,1)
        [HideInInspector]_Centre("Centre",Vector) = (0,0,0,1)
        _TimeOffset("Time offset", Range(0,1)) = 0.0
        _FireNoiseTex("Fire noise sample", 2D) = "gray" {}
        _FireGrowthTex("Fire sudden growths sample", 2D) = "black" {}
    }


    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        uniform sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
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

        //#define TAU 6.28318530718
        //#define SIN01(f_,offset_) ((sin((_Time[1] * f_ - (offset_)) * TAU) + 1.0) * 0.5)

        uniform float _TimeOffset, _Frequency;
        uniform float4 _EmissionColor;
        uniform float3 _Centre;
        uniform sampler2D _FireNoiseTex, _FireGrowthTex;

        #define FIRENOISE_HALFLENGTH 256.0
        #define FIRENOISE_LENGTH (FIRENOISE_HALFLENGTH * FIRENOISE_HALFLENGTH)
        #define FIRENOISE_DURATION 4.0

        #define SAMPLE_FROM_FIRENOISE_LOD(time_) tex2Dlod(_FireNoiseTex, float4((time_) % 1.0, ((time_) % FIRENOISE_HALFLENGTH) / FIRENOISE_HALFLENGTH, 0, 0))
        #define SAMPLE_FROM_FIREGROWTH_LOD(time_) tex2Dlod(_FireGrowthTex, float4((time_) % 1.0, ((time_) % FIRENOISE_HALFLENGTH) / FIRENOISE_HALFLENGTH, 0, 0))
        #define SAMPLE_FROM_FIREGROWTH(time_) tex2D(_FireGrowthTex, float2((time_) % 1.0, ((time_) % FIRENOISE_HALFLENGTH) / FIRENOISE_HALFLENGTH))
        #define GET_RANDOM_0to1_FROM_SAMPLES(t) ((SAMPLE_FROM_FIRENOISE_LOD(randt) + SAMPLE_FROM_FIREGROWTH_LOD(t) * 0.3) / (1.3))

        void vert(inout appdata_full v)
        {
            float t = _Time[1] * (FIRENOISE_HALFLENGTH / FIRENOISE_DURATION) - _TimeOffset * FIRENOISE_LENGTH;
            float randt = t - (((v.vertex.x * v.vertex.y) % 7.0) % 1.0) * FIRENOISE_LENGTH;
            v.vertex.xyz = _Centre + (v.vertex.xyz - _Centre) * (GET_RANDOM_0to1_FROM_SAMPLES(t) * (1.75 - 0.75) + 0.75);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            float t = _Time[1] * (FIRENOISE_HALFLENGTH / FIRENOISE_DURATION) - _TimeOffset * FIRENOISE_LENGTH;
            o.Emission = c.rgb * tex2D(_MainTex, IN.uv_MainTex).a * _EmissionColor *(SAMPLE_FROM_FIREGROWTH(t) * (1.0 - 0.7) + 0.7);
        }
        ENDCG
    }
        FallBack "Diffuse"
}
