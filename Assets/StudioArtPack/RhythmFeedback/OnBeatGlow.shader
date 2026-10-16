Shader "BalloonBarrage/OnBeatGlow"
{
    Properties
    {
        [HDR] _Tint ("Glow Color", Color) = (2.6, 1.8, 0.6, 1)
        _Shape ("Shape: Ring 0, Spark 1", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            Blend SrcAlpha One
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Tint;
                float _Shape;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };
            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.color = input.color;
                output.uv = input.uv;
                return output;
            }
            half4 Frag(Varyings input) : SV_Target
            {
                float2 p = input.uv * 2.0 - 1.0;
                float ring = 1.0 - smoothstep(0.025, 0.14, abs(length(p) - 0.65));
                float core = pow(saturate(1.0 - length(p)), 4.0);
                float streakX = pow(saturate(1.0 - abs(p.x)), 18.0) * pow(saturate(1.0 - abs(p.y)), 2.0);
                float streakY = pow(saturate(1.0 - abs(p.y)), 18.0) * pow(saturate(1.0 - abs(p.x)), 2.0);
                float spark = saturate(core + streakX + streakY);
                float mask = lerp(ring, spark, step(0.5, _Shape));
                return half4(_Tint.rgb * input.color.rgb, mask * _Tint.a * input.color.a);
            }
            ENDHLSL
        }
    }
}
