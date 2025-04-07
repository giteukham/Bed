using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class BlinkEffect : MonoBehaviour
{
    [SerializeField] private Material blinkMat;
    [SerializeField] private Camera camera;
    private static Material blinkMaterial;
    private Shader shader;
    private CommandBuffer commandBuffer;
    public static float BLINK_START_POINT_INIT = 0.81f;
    
    public static float Blink 
    {
        get
        {
            return blinkMaterial.GetFloat("_Blink");
        }
        set
        {
            blinkMaterial.SetFloat("_Blink", Mathf.Clamp01(value));
        } 
    }

    public static float StartPoint
    {
        get
        {
            return blinkMaterial.GetFloat("_StartBlink");
        }
        set
        {
            blinkMaterial.SetFloat("_StartBlink", value);
        }
    }

    public static float Smoothness
    {
        get
        {
            return blinkMaterial.GetFloat("_Smoothness");
        }
        set
        {
            blinkMaterial.SetFloat("_Smoothness", value);
        }
    }
    
    public static float AspectRatio
    {
        get
        {
            return blinkMaterial.GetFloat("_AspectRatio");
        }
        set
        {
            blinkMaterial.SetFloat("_AspectRatio", value);
        }
    }
    
    /// <summary>
    /// �ӽ� ���۸� ���� ���� ȭ���� ������ �� ��ũ ȿ���� ����
    /// ��� �������� ���� �Ŀ� ����
    /// </summary>
    private void OnEnable()
    {
        blinkMaterial = blinkMat;
        
        int temp = Shader.PropertyToID("_CameraOpaqueTexture");
        commandBuffer = new CommandBuffer();
        commandBuffer.GetTemporaryRT(temp, Screen.width, Screen.height, 0, FilterMode.Bilinear);
        commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, temp);
        commandBuffer.Blit(temp, BuiltinRenderTextureType.CameraTarget, blinkMat);
        commandBuffer.ReleaseTemporaryRT(temp);
        
        camera.AddCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
    }

    private void OnApplicationQuit()
    {
        camera.RemoveCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
        blinkMaterial.SetFloat("_Blink", 0.001f);
    }
}
