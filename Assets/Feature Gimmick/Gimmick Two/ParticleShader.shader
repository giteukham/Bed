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
            #pragma target 4.5

            struct appdata
            {
                float4 pos : POSITION;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            StructuredBuffer<float3> _Positions;

            v2f vert(appdata v, uint id : SV_InstanceID)        // TODO: Add the instance ID
            {
                float3 position = _Positions[id];
                v2f o;
                
                o.pos = UnityObjectToClipPos(v.pos + float4(position, 0));
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
