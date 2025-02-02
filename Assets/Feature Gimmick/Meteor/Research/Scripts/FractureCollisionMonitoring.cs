
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
        // ���ϴ� �±�
        if (other.gameObject.CompareTag("Gimmick"))
        {
            var chunks = transform.Find("Chunks");
            // �浹�� �� ���� ������Ʈ�� MeshRenderer�� Collider�� ��Ȱ��ȭ�����ν� �� ���̰� �ϰ�, Chunks�� Ȱ��ȭ�Ѵ�.
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
