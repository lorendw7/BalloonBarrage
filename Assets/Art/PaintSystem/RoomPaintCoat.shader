Shader "BalloonBarrage/RoomPaintCoat"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off ZTest LEqual Cull Off
            Offset -1, -1
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            struct Attributes
            {
                float4 position : POSITION; float3 normal : NORMAL; float2 paintUV : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 position : SV_POSITION; float3 world : TEXCOORD0; half3 normal : TEXCOORD1;
                float2 paintUV : TEXCOORD2; half fog : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            TEXTURE2D(_RoomPaintAtlas); SAMPLER(sampler_RoomPaintAtlas);
            float4 _RoomPaintAtlas_TexelSize;
            float _RoomPaintActive;
            Varyings Vertex(Attributes input)
            {
                Varyings output; UNITY_SETUP_INSTANCE_ID(input); UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                VertexPositionInputs position = GetVertexPositionInputs(input.position.xyz);
                output.position = position.positionCS; output.world = position.positionWS;
                output.normal = TransformObjectToWorldNormal(input.normal);
                output.paintUV = input.paintUV; output.fog = ComputeFogFactor(position.positionCS.z);
                return output;
            }
            half4 Fragment(Varyings input) : SV_Target
            {
                clip(_RoomPaintActive - .5);
                half4 ink = SAMPLE_TEXTURE2D(_RoomPaintAtlas, sampler_RoomPaintAtlas, input.paintUV);
                clip(ink.a - .006);
                half3 pigment = ink.rgb / max(ink.a, .006);
                half3 normal = normalize(input.normal);
                Light light = GetMainLight(TransformWorldToShadowCoord(input.world));
                half diffuse = saturate(dot(normal, light.direction));
                half3 illumination = max(SampleSH(normal), .30) + light.color * (.18 + .62 * diffuse * light.shadowAttenuation);
                half3 view = SafeNormalize(GetWorldSpaceViewDir(input.world));
                half gloss = pow(saturate(dot(normal, SafeNormalize(view + light.direction))), 48) * .24;
                half rim = smoothstep(.03, .25, ink.a) * (1-smoothstep(.45, .93, ink.a));
                half wetEdge = saturate(abs(ddx(ink.a)) + abs(ddy(ink.a)));
                half3 color = pigment * min(illumination, 1.35) * (1-rim*.16) + gloss * light.color +
                    wetEdge * .20 * (light.color*.7 + .3);
                return half4(MixFog(color, input.fog), ink.a);
            }
            ENDHLSL
        }
    }
}
