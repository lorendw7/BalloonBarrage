Shader "Hidden/BalloonBarrage/RoomPaintAccumulate"
{
    Properties { _PaintPigment("Pigment", Color) = (1,1,1,1) }
    SubShader
    {
        Pass
        {
            Cull Off ZWrite Off ZTest Always
            Blend One OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vertex
            #pragma fragment Fragment
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes { float4 position : POSITION; float3 normal : NORMAL; float2 paintUV : TEXCOORD3; };
            struct Varyings { float4 position : SV_POSITION; float3 world : TEXCOORD0; float3 normal : TEXCOORD1; };
            float4x4 _PaintWorldFromLocal, _PaintNormalToWorld;
            float4 _PaintCenterRadius, _PaintContactNormal, _PaintPigment, _PaintMaskRect;
            float _PaintRotation;
            TEXTURE2D(_PaintMask); SAMPLER(sampler_PaintMask);
            Varyings Vertex(Attributes input)
            {
                Varyings output;
                output.position = float4(input.paintUV * 2.0 - 1.0, 0, 1);
                #if UNITY_UV_STARTS_AT_TOP
                    output.position.y = -output.position.y;
                #endif
                output.world = mul(_PaintWorldFromLocal, input.position).xyz;
                output.normal = normalize(mul((float3x3)_PaintNormalToWorld, input.normal));
                return output;
            }
            half4 Fragment(Varyings input) : SV_Target
            {
                float3 normal = normalize(_PaintContactNormal.xyz);
                float3 delta = input.world - _PaintCenterRadius.xyz;
                float radius = _PaintCenterRadius.w;
                clip(dot(normalize(input.normal), normal) + .06); // Never coat the opposite face.
                clip(radius * .4 - abs(dot(delta, normal)));
                float3 tangent = abs(normal.y) > .92 ? float3(1,0,0) : normalize(cross(float3(0,1,0), normal));
                float3 bitangent = cross(normal, tangent);
                float2 q = float2(dot(delta, tangent), dot(delta, bitangent)) / radius;
                float2 gravityQ = q;
                float sine, cosine; sincos(_PaintRotation, sine, cosine);
                q = float2(q.x*cosine-q.y*sine, q.x*sine+q.y*cosine);
                float2 uv = q * .5 + .5;
                clip(min(min(uv.x, uv.y), min(1-uv.x, 1-uv.y)));
                float mask = SAMPLE_TEXTURE2D(_PaintMask, sampler_PaintMask, _PaintMaskRect.xy + uv * _PaintMaskRect.zw).a;
                // A solid ink body and a narrow soft boundary replace dusty, transparent spots.
                float angle = atan2(q.y, q.x);
                float edge = .66 + .055*sin(angle*5 + _PaintRotation) + .03*sin(angle*9 - _PaintRotation);
                float core = 1 - smoothstep(edge-.045, edge+.025, length(q));
                float drips = 0;
                if (abs(normal.y) < .6)
                {
                    float width = .023 * smoothstep(-.98, -.5, gravityQ.y);
                    drips = (1-smoothstep(width, width+.012, min(abs(gravityQ.x-.19), abs(gravityQ.x+.31)))) *
                        smoothstep(-.98, -.90, gravityQ.y) * (1-smoothstep(-.46, -.36, gravityQ.y));
                }
                half alpha = max(max(smoothstep(.045, .38, mask), core), drips) * _PaintPigment.a;
                return half4(_PaintPigment.rgb * alpha, alpha);
            }
            ENDHLSL
        }
    }
}
