
using UnityEngine;

public class FractureCollisionMonitoring : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Collider col;

    private void OnEnable()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        // 원하는 태그
        if (other.gameObject.CompareTag("Gimmick"))
        {
            var chunks = transform.Find("Chunks");
            // 충돌할 때 원래 오브젝트의 MeshRenderer와 Collider를 비활성화함으로써 안 보이게 하고, Chunks를 활성화한다.
            EnableRender(false);
            chunks.gameObject.SetActive(true);
        }
    }
    
    private void EnableRender(bool enable)
    {
        meshRenderer.enabled = enable;
        col.enabled = enable;
    }
}
