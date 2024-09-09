
Shader "Unlit/Blink Effect"
{
    Properties
    {
        _Blink ("Blink", Range(0.3, 1.0)) = 0.7
        _StartBlink ("Start Point", Range(0.0, 1.0)) = 0.0
        _Smoothness ("Smoothness", Range(0.1, 0.9)) = 0.5
        _AspectRatio ("Aspect Ratio", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent" }
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float _Blink;
            float _StartBlink;
            float _AspectRatio;
            float _Smoothness;
            
            fixed4 frag (v2f i) : SV_Target
            {
                float x = (i.uv.x - 0.5) * _AspectRatio;
                float y = (i.uv.y - 0.5) * _AspectRatio;
                float height = lerp(_StartBlink, 1.0, _Blink + (1.0 - _Blink) * dot(x,x));
                float width = dot(y,y);
                float mask = width + height;
                return fixed4(1.0 - smoothstep(0.0, (width + height) * _Smoothness, width + height).xxx, mask);
            }
            ENDCG
        }
    }
}
