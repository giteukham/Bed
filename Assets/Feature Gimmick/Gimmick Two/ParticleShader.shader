// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

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
            
            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Particle
            {
                float3 position;
            };

            StructuredBuffer<Particle> _Particles;
            float _ParticleRadius;

            v2f vert(v2f v, uint id : SV_InstanceID)        // TODO: Add the instance ID
            {
                v2f o;
                unity_ObjectToWorld._11_21_31_41 = float4(_ParticleRadius, 0, 0, 0);
				unity_ObjectToWorld._12_22_32_42 = float4(0, _ParticleRadius, 0, 0);
				unity_ObjectToWorld._13_23_33_43 = float4(0, 0, _ParticleRadius, 0);
				unity_ObjectToWorld._14_24_34_44 = float4(_Particles[id].position.xyz, 1);
                
                o.pos = UnityObjectToClipPos(v.pos);
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
