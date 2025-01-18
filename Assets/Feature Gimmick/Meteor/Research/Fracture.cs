
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fracture : MonoBehaviour
{
    [Header("Chunk Settings")] [SerializeField] [Tooltip("조각 내부 Material")]
    private Material insideMaterial;

    [Tooltip("조각 외부 Material")] private Material outsideMaterial;

    [SerializeField] [Tooltip("밀도를 이용해서 조각 질량을 계산")]
    private float density;

    [SerializeField] private float breakForce;

    [SerializeField] [Tooltip("조각 개수"), Min(2)]
    private int siteCount;

    private Rigidbody actorRig;
    private Mesh actorMesh, worldMesh;
    
    private GameObject chunks;

    public void InitSettings(
        Material insideMaterial,
        float density, 
        float breakForce, 
        int siteCount)
    {
        this.insideMaterial = insideMaterial == null ? new Material(Shader.Find("Standard")) : insideMaterial;
        this.density = density;
        this.breakForce = breakForce;
        this.siteCount = siteCount;
    }

    private void OnEnable()
    {
        actorRig = gameObject.GetComponent<Rigidbody>();
        actorMesh = gameObject.GetComponent<MeshFilter>().mesh;
        outsideMaterial = gameObject.GetComponent<MeshRenderer>().material;
        worldMesh = CreateWorldMesh(actorMesh);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Gimmick"))
        {
            chunks.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void GenerateChunk(GameObject chunksParent)
    {
        chunks = new GameObject($"{gameObject.name}_Chunks");
        chunks.transform.SetParent(chunksParent.transform);
        chunks.SetActive(false);
        
        Random.InitState((int)DateTime.Now.Ticks);
        var meshes = FractureTool.FractureMesh(worldMesh, siteCount, Random.Range(0, 1000));
        var chunkIndex = 0;

        for (int i = 0; i < meshes.Count; i++)
        {
            GameObject chunk = new GameObject($"{gameObject.name}_Chunk_{chunkIndex++}");
            chunk.transform.SetParent(chunks.transform);

            chunk.AddComponent<MeshFilter>().mesh = meshes[i];
            chunk.AddComponent<MeshRenderer>().materials = new Material[] { outsideMaterial, insideMaterial };

            Rigidbody rig = chunk.AddComponent<Rigidbody>();
            rig.mass = VolumeOfMesh(meshes[i]) * density;
            rig.velocity = actorRig.velocity;
            rig.angularVelocity = actorRig.angularVelocity;

            MeshCollider col = chunk.AddComponent<MeshCollider>();
            col.sharedMesh = meshes[i];
            col.convex = true;

            FixedJoint joint = chunk.AddComponent<FixedJoint>();
            joint.connectedBody = actorRig;
            joint.breakForce = breakForce;
        }
    }

    private Mesh CreateWorldMesh(Mesh originMesh)
    {
        var newMesh = new Mesh();
        var vertices = originMesh.vertices;

        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i] = transform.TransformPoint(vertices[i]);
        }

        newMesh.vertices = vertices;
        newMesh.triangles = originMesh.triangles;
        newMesh.normals = originMesh.normals;
        newMesh.uv = originMesh.uv;
        newMesh.RecalculateBounds();

        return newMesh;
    }

    // https://discussions.unity.com/t/how-would-one-calculate-a-3d-mesh-volume-in-unity/16895
    private float VolumeOfMesh(Mesh mesh)
    {
        float volume = 0;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];
            volume += SignedVolumeOfTriangle(p1, p2, p3);
        }

        return Mathf.Abs(volume);
    }

    private static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float v321 = p3.x * p2.y * p1.z;
        float v231 = p2.x * p3.y * p1.z;
        float v312 = p3.x * p1.y * p2.z;
        float v132 = p1.x * p3.y * p2.z;
        float v213 = p2.x * p1.y * p3.z;
        float v123 = p1.x * p2.y * p3.z;

        return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
    }
}