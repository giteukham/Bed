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
    
    public static float Blink 
    {
        get
        {
            return blinkMaterial.GetFloat("_Blink");
        }
        set
        {
            blinkMaterial.SetFloat("_Blink", value);
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
        commandBuffer.GetTemporaryRT(temp, -1, -1, 0, FilterMode.Bilinear);
        commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, temp);
        commandBuffer.Blit(temp, BuiltinRenderTextureType.CurrentActive, blinkMat);
        commandBuffer.ReleaseTemporaryRT(temp);
        
        camera.AddCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
    }

    private void OnApplicationQuit()
    {
        camera.RemoveCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
        blinkMaterial.SetFloat("_Blink", 0.3f);
    }
}
