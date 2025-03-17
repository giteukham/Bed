using UnityEngine;

public class FractureInPlayMode : MonoBehaviour
{
    [Header("Chunk Settings")] 
    [SerializeField, Tooltip("������ ������Ʈ�� ����")]
    private FractureData fractureData;
    public FractureData FractureData { get => fractureData; set => fractureData = value; }
    
    [SerializeField, Tooltip("������ �޽�")]
    private Mesh fractureMesh;
    
    private GameObject chunksParent, chunks;
    
    private void Start()
    {
        GenerateFracture();
    }

    public void GenerateFracture()
    {
        fractureMesh = GetComponent<MeshFilter>().mesh;
        var meshes = FractureTool.CreateFractureMeshes(gameObject, fractureData, fractureMesh);
        FractureTool.CreateFractureGameObjects(gameObject, fractureData, meshes);
    }
}