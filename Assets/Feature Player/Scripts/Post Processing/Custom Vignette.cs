using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using Vector2Parameter = UnityEngine.Rendering.PostProcessing.Vector2Parameter;

namespace Bed.PostProcessing
{
    [Serializable]
    [PostProcess(typeof(VignetteRenderer), PostProcessEvent.AfterStack, "Custom/Custom Vignette")]
    public sealed class CustomVignette : PostProcessEffectSettings
    {
        public FloatParameter intensity = new FloatParameter() { value = 0.45f };
        public FloatParameter smoothness = new FloatParameter() { value = 0.2f };
        public Vector2Parameter roundness = new Vector2Parameter();
        
        [Range(0.001f, 1.0f)]
        public FloatParameter blink = new FloatParameter() { value = 0.001f };
        public Vector2Parameter center = new Vector2Parameter() { value = new Vector2(0.5f, 0.5f) };
        
        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value && intensity.value > 0f && smoothness.value > 0f && blink.value > 0.0f;
        }
    }

    public sealed class VignetteRenderer : PostProcessEffectRenderer<CustomVignette>
    {
        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Vignette HLSL"));
            settings.blink.value += 1f;
            sheet.properties.SetFloat("_Vignette_Intensity", settings.intensity);
            sheet.properties.SetFloat("_Vignette_Smoothness", settings.smoothness);
            sheet.properties.SetVector("_Vignette_Roundness", settings.roundness);
            sheet.properties.SetFloat("_Vignette_Blink", settings.blink);
            sheet.properties.SetVector("_Vignette_Center", settings.center);
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}
