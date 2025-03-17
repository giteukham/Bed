

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
        // ????? ?��?
        if (other.gameObject.CompareTag("Gimmick"))
        {
            var chunks = transform.Find("Chunks");
            // ?��?? ?? ???? ????????? MeshRenderer?? Collider?? ???????????��? ?? ????? ???, Chunks?? ???????.
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
