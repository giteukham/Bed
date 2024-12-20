// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'glstate_matrix_projection' with 'UNITY_MATRIX_P'

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
            
            #define UNITY_INSTANCING_ENABLED

            struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                uint id : SV_InstanceID;
            };

            struct Particle
            {
                float3 position;
                float4 color;
            };

            StructuredBuffer<Particle> _particles;
            float _particleRadius;

            v2f vert(appdata v, uint id : SV_InstanceID)
            {
            #if defined(UNITY_INSTANCING_ENABLED)
                float3 data = _particles[id].position;
            #endif

                float3 localPosition = v.vertex.xyz * (_particleRadius * 2);
                float3 worldPosition = data.xyz + localPosition;
                
                v2f o;
                o.vertex = mul(UNITY_MATRIX_VP, float4(worldPosition, 1.0f));
                o.id  = id;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = _particles[i.id].color;

                if (_particles[i.id].color.x == 1 && _particles[i.id].color.y == 0 && _particles[i.id].color.z == 0)
                {
                    col = float4(1, 0, 0, 1);
                }
                else if (_particles[i.id].color.x == 0 && _particles[i.id].color.y == 1 && _particles[i.id].color.z == 0)
                {
                    col = float4(0, 1, 0, 1);
                }
                else if (_particles[i.id].color.x == 0 && _particles[i.id].color.y == 0 && _particles[i.id].color.z == 1)
                {
                    col = float4(0, 0, 1, 1);
                }
                else if (_particles[i.id].color.x == 1 && _particles[i.id].color.y == 1 && _particles[i.id].color.z == 0)
                {
                    col = float4(1, 1, 0, 1);
                }
                else if (_particles[i.id].color.x == 1 && _particles[i.id].color.y == 0 && _particles[i.id].color.z == 1)
                {
                    col = float4(1, 0, 1, 1);
                }
                else if (_particles[i.id].color.x == 0 && _particles[i.id].color.y == 1 && _particles[i.id].color.z == 1)
                {
                    col = float4(0, 1, 1, 1);
                }
                else if (_particles[i.id].color.x == 1 && _particles[i.id].color.y == 1 && _particles[i.id].color.z == 1)
                {
                    col = float4(1, 1, 1, 1);
                }
                else if (_particles[i.id].color.x == 0 && _particles[i.id].color.y == 0 && _particles[i.id].color.z == 0)
                {
                    col = float4(0, 0, 0, 1);
                }
                
                return col;
            }
            ENDCG
        }

    }
}