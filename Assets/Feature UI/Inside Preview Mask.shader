Shader "CustomRenderTexture/Render Texture"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex("InputTex", 2D) = "white" {}
        _MaskTex("MaskTex", 2D) = "white" {}
     }

     SubShader
     {
        Pass
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            Name "Render Texture"

            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vertex
            #pragma fragment frag
            #pragma target 3.0

            float4      _Color;
            sampler2D   _MainTex;
            sampler2D   _MaskTex;
            float2      _insideOffsetMin;
            float2      _insideOffsetMax;
            float2      _insideSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            v2f vertex(appdata v)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.color = v.color * _Color;
                OUT.texcoord = v.texcoord;
                return OUT;
            }
            
            float4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                // InputTex�� ���� ��������
                float4 inputColor = tex2D(_MainTex, uv);
                
                // MaskTex�� ���İ� ��������
                float maskAlpha = tex2D(_MaskTex, uv).a;

                // InputTex ������ MaskTex ���İ����� ����ŷ
                float4 outputColor = inputColor;
                outputColor.a *= maskAlpha;

                return outputColor;
            }
            ENDCG
        }
    }
}
