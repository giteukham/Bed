Shader "Hidden/Custom/Vignette HLSL"
{
    Properties
    {
        _Vignette_Intensity("Intensity", Range(0, 1)) = 0.5
        _Vignette_Smoothness("Smoothness", Range(0, 1)) = 0.5
        _Vignette_Roundness("Roundness", Vector)= (1.0, 1.0, 1.0, 1.0)
        _Vignette_Blink("Blink", Range(0, 1)) = 0
        _Vignette_Center("Center", Vector) = (0.5, 0.5, 0.0, 0.0)
    }
    HLSLINCLUDE
    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        
    float _Vignette_Intensity;
    float _Vignette_Smoothness;
    half2 _Vignette_Roundness;
    float _Vignette_Blink;
    half2 _Vignette_Center;
    ENDHLSL

    SubShader
    {
        Cull OFF ZWrite OFF ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
 			
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag
            
            // From Unity Github
            float4 frag(VaryingsDefault i) : SV_Target
            {
                half4 color = (0.0);
                half2 d = abs(i.texcoord - _Vignette_Center) * _Vignette_Intensity;
                
                d.y *= lerp(0.0, 1.0, exp2(11.0 * log2(_Vignette_Blink)));   // Blink
                d = pow(saturate(d),_Vignette_Roundness); // Roundness
                half vfactor = pow(saturate(1.0 - dot(d, d)), _Vignette_Smoothness);
                color.rgb *= lerp(float3(1,1,1), (1.0).xxx, vfactor);
                color.a = lerp(1.0, color.a, vfactor);
                return color;
            }
            ENDHLSL
        }
    }
}
