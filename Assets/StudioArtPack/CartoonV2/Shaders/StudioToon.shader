Shader "BalloonStudio/SoftToon"
{
    Properties
    {
        _BaseColor("Paint Color", Color) = (1,1,1,1)
        _ShadeColor("Shadow Tint", Color) = (0.70,0.75,0.91,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            half4 _ShadeColor;
            CBUFFER_END
            struct Attributes { float4 positionOS: POSITION; float3 normalOS: NORMAL; };
            struct Varyings { float4 positionCS: SV_POSITION; half3 normalWS: TEXCOORD0; };
            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }
            half4 Frag(Varyings input): SV_Target
            {
                Light sun = GetMainLight();
                half3 paintLight = normalize(sun.direction + half3(0, 1.2h, -0.4h));
                half facing = dot(normalize(input.normalWS), paintLight);
                half band = facing > 0.45h ? 1.0h : (facing > -0.15h ? 0.92h : 0.80h);
                half3 paint = _BaseColor.rgb * lerp(_ShadeColor.rgb, half3(1,1,1), band);
                return half4(paint * band, 1);
            }
            ENDHLSL
        }
    }
}
