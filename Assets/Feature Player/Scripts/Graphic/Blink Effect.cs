using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class BlinkEffect : MonoBehaviour
{
    [SerializeField] private Material blinkMaterial;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, blinkMaterial, pass: 0);
    }
}
