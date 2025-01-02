Shader "Unlit/Water Screen Shader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0,0.5,1,1)
        _TransparentColor ("Transparent Color", Color) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZWrite On
        Cull Off
        ZTest LEqual
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma target 3.0
            #pragma multi_compile

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenpos : TEXCOORD1;
            };
            
            fixed4 _BaseColor;
            fixed4 _TransparentColor;
            sampler2D _CameraDepthTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenpos = ComputeScreenPos(o.vertex);
                UNITY_TRANSFER_DEPTH(o.screenpos);
                return o;
            }

            fixed4 frag (v2f i, bool IsFacing : SV_IsFrontFace) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                fixed3 col = IsFacing ? _BaseColor.rgb : _TransparentColor.rgb;
                half mask = tex2D(_CameraDepthTexture, i.screenpos.xy / i.screenpos.w).r;
                half3 underwaterColor = lerp(col, _BaseColor.rgb, mask);

                return LinearEyeDepth(i.screenpos);
            }
            ENDCG
        }
    }
}
