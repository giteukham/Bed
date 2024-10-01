Shader "Custom/ParticleShader"
{
    Properties
    {

    }
    SubShader
    {
        Pass
        {
            Tags { "RenderType"="Opaque" }

            CGPROGRAM
            #pragma vertex vert;
            #pragma fragment frag;
            #pragma multi_compile_instancing
            #pragma target 4.5
            #include "UnityCG.cginc"
            
            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Particle
            {
                float3 position;
                float4 color;
            };

            StructuredBuffer<Particle> _particles;
            float _particleRadius;

            v2f vert(appdata_full v, uint id : SV_InstanceID)
            {
                v2f o;
            #if SHADER_TARGET >= 45
                float4 data = float4(_particles[id].position, 1.0);
            #else
                float4 data = 0;
            #endif

                float3 localPosition = v.vertex.xyz * _particleRadius;      // 스케일 적용
                float3 worldPosition = data.xyz + localPosition;            // 위치 적용

                o.pos = mul(UNITY_MATRIX_VP, float4(worldPosition, 1.0f));
                o.uv  = v.texcoord;
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(float3(1.0, 1.0, 1.0), 1.0);
            }
            ENDCG
        }

    }
}