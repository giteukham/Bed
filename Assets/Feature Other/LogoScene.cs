using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class LogoScene : MonoBehaviour
{
    [SerializeField]
    private Texture2D logo;

    [SerializeField]
    private Camera camera;
    
    private Material textureMaterial;
    private CommandBuffer textureBuffer;

    private void Awake()
    {
        var data = SaveManager.Instance.LoadResolutionSettings();
        Screen.SetResolution(data.ResolutionWidth, data.ResolutionHeight, !data.IsWindowed);
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        BlinkEffect.StartPoint = 0.66f;
        BlinkEffect.Blink = 0f;
        SetupTextureOverlay();
    }
    
    private async UniTaskVoid SetupTextureOverlay()
    {
        if (logo == null)
        {
            Debug.LogError("Overlay texture is not assigned!");
            return;
        }

        if (textureMaterial == null)
        {
            textureMaterial = new Material(Shader.Find("Unlit/Texture"));
            textureMaterial.mainTexture = logo;
        }
    
        // 커맨드 버퍼 생성
        textureBuffer = new CommandBuffer();
        textureBuffer.name = "TextureOverlay";
        
        int tempRT = Shader.PropertyToID("_TempRT");
        textureBuffer.GetTemporaryRT(tempRT, Screen.width, Screen.height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
        textureBuffer.Blit(BuiltinRenderTextureType.CameraTarget, tempRT);
        textureBuffer.Blit(logo, BuiltinRenderTextureType.CameraTarget, textureMaterial);
        textureBuffer.ReleaseTemporaryRT(tempRT);
        
        camera.AddCommandBuffer(CameraEvent.BeforeImageEffects, textureBuffer);

        await UniTask.WhenAny(CloseBlink());
        RemoveLogo();
        SceneManager.LoadScene("Demo");                     // TODO: 나중에 main 씬으로 바꿔야 함.
    }

    private void RemoveLogo()
    {
        camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, textureBuffer);
        BlinkEffect.StartPoint = BlinkEffect.BLINK_START_POINT_INIT;
    }

    private async UniTask CloseBlink()
    {
        var blinkSpeed = 2.2f;
        
        while (BlinkEffect.Blink < PlayerEyeControl.BLINK_VALUE_MAX)
        {
            BlinkEffect.Blink += (Time.deltaTime * blinkSpeed);
            await UniTask.Yield();
        }
    }
}
