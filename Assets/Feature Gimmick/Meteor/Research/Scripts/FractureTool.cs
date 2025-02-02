using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "FractureData", menuName = "Fracture Data", order = 0)]
[Serializable]
public class FractureData : ScriptableObject
{
    [HideInInspector]
    public GameObject   fracturableObject;
    
    public Material     insideMaterial, outsideMaterial;
    
    [Min(1)]
    public int          breakForce;
    
    [Min(1f)]
    public float        density;
    
    [Range(2, 64)]
    public int          siteCount;
    
    [HideInInspector] 
    public bool         haveIndividualSettings;
    
    public bool         isChunksActive = true;
}

public static class FractureTool
{
    private static NvFractureTool fractureTool = new();
    private static NvVoronoiSitesGenerator sites;
    
    /// <summary>
    /// fratureData를 기반으로 Mesh를 조각내어 반환
    /// </summary>
    public static List<Mesh> CreateFractureMeshes(GameObject originObj, FractureData data, Mesh mesh)
    {
        // Seed 값을 현재 시간에 따라 랜덤으로 설정
        Random.InitState((int)DateTime.Now.Ticks);
        NvBlastExtUnity.setSeed(Random.Range(0, 1000));
        
#if UNITY_EDITOR
        var worldMesh = CreateWorldMesh(originObj, mesh);
        
        // sharedMesh는 center가 Vector3.zero에 있는 거 같아서 Pivot 위치 조정
        for (int i = 0; i < worldMesh.vertices.Length; i++)
        {
            worldMesh.vertices[i] += originObj.transform.position;
        }
        worldMesh.RecalculateBounds();
#else
        var worldMesh = CreateWorldMesh(originObj, mesh);
#endif

        var nvMesh = new NvMesh(
            worldMesh.vertices,
            worldMesh.normals, 
            worldMesh.uv,
            worldMesh.vertexCount, 
            worldMesh.GetIndices(0), 
            (int) worldMesh.GetIndexCount(0));
        
        fractureTool.setRemoveIslands(false);
        fractureTool.setSourceMesh(nvMesh);
        
        sites = new NvVoronoiSitesGenerator(nvMesh);
        sites.uniformlyGenerateSitesInMesh(data.siteCount);
        fractureTool.voronoiFracturing(0, sites);
        fractureTool.finalizeFracturing();
        
        var meshCount = fractureTool.getChunkCount();
        var outsideMeshes = new List<Mesh>(meshCount);
        var insideMeshes = new List<Mesh>(meshCount);
        
        for (var i = 0; i < meshCount; i++)
        {
            var tmp = CreateChunkMesh(fractureTool, i);
            var inside = tmp[1];
            var outside = tmp[0];
            outside.name = $"{originObj.name} Outside Chunk {i}";
            
            outsideMeshes.Add(outside);
            insideMeshes.Add(inside);
        }
        
        // Outside Mesh에 Inside Mesh도 포함되어 있음.
        return outsideMeshes;
    }
    
    private static Mesh[] CreateChunkMesh(NvFractureTool fractureTool, int index)
    {
        var outside = fractureTool.getChunkMesh(index, false);
        var inside = fractureTool.getChunkMesh(index, true);
        var outsideMesh = outside.toUnityMesh();
        var insideMesh = inside.toUnityMesh();
        
        outsideMesh.subMeshCount = 2;
        outsideMesh.SetIndices(inside.getIndexes(), MeshTopology.Triangles, 1);
        
        return new Mesh[] { outsideMesh, insideMesh };
    }
    
    private static Mesh CreateWorldMesh(GameObject originObj, Mesh originMesh)
    {
        var newMesh = new Mesh();
        var vertices = originMesh.vertices;

        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i] = originObj.transform.TransformPoint(vertices[i]);
        }

        newMesh.vertices = vertices;
        newMesh.triangles = originMesh.triangles;
        newMesh.normals = originMesh.normals;
        newMesh.uv = originMesh.uv;
        newMesh.RecalculateBounds();

        return newMesh;
    }
    
    public static void CreateFractureGameObjects(GameObject originObj, FractureData data, List<Mesh> meshes)
    {
        var chunks = new GameObject("Chunks");
        var chunkIndex = 0;
        var chunkCount = fractureTool.getChunkCount();

        originObj.GetOrAddComponent<FractureCollisionMonitoring>();
        
        chunks.transform.SetParent(originObj.transform);
        for (var i = 1; i < chunkCount; i++)
        {
            GameObject chunk = new GameObject($"{originObj.gameObject.name} Chunk {chunkIndex++}");
            chunk.transform.SetParent(chunks.transform);
            
            chunk.AddComponent<MeshFilter>().mesh = meshes[i];
            
            data.insideMaterial = data.insideMaterial == null ? new Material(Shader.Find("Standard")) : data.insideMaterial;
            data.outsideMaterial = data.outsideMaterial == null ? originObj.GetComponent<MeshRenderer>().sharedMaterial : data.outsideMaterial;
            chunk.AddComponent<MeshRenderer>().materials = new Material[] { data.outsideMaterial, data.insideMaterial };
            
            Rigidbody rig = chunk.AddComponent<Rigidbody>();
            rig.mass = VolumeOfMesh(meshes[i]) * data.density;
            
            // 충돌할 때 원래 오브젝트가 받은 힘을 Chunk들이 받도록 설정
#if !UNITY_EDITOR
            rig.velocity = actorRig.velocity;
            rig.angularVelocity = actorRig.angularVelocity;
#endif
            
            MeshCollider col = chunk.AddComponent<MeshCollider>();
            col.sharedMesh = meshes[i];
            col.convex = true;
            
            FixedJoint joint = chunk.AddComponent<FixedJoint>();
            joint.breakForce = data.breakForce;
        }
        
        chunks.SetActive(data.isChunksActive);
    }
    

    /// <summary>
    /// 각 조각의 부피를 계산
    /// </summary>
    // https://discussions.unity.com/t/how-would-one-calculate-a-3d-mesh-volume-in-unity/16895
    private static float VolumeOfMesh(Mesh mesh)
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
