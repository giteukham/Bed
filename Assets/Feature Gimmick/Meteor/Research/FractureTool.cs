using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FractureTool
{
    public static List<Mesh> FractureMesh(Mesh mesh, int siteCount, int seed)
    {
        NvBlastExtUnity.setSeed(seed);

        var nvMesh = new NvMesh(mesh.vertices, mesh.normals, mesh.uv, mesh.vertexCount, mesh.GetIndices(0), (int) mesh.GetIndexCount(0));
        
        var fractureTool = new NvFractureTool();
        fractureTool.setRemoveIslands(false);
        fractureTool.setSourceMesh(nvMesh);
        
        var sites = new NvVoronoiSitesGenerator(nvMesh);
        sites.uniformlyGenerateSitesInMesh(siteCount);
        fractureTool.voronoiFracturing(0, sites);
        fractureTool.finalizeFracturing();
        
        var meshCount = fractureTool.getChunkCount();
        var meshes = new List<Mesh>(meshCount);
        for (var i = 0; i < meshCount; i++)
        {
            meshes.Add(ExtractChunkMesh(fractureTool, i));
        }
        
        return meshes;
    }
    
    private static Mesh ExtractChunkMesh(NvFractureTool fractureTool, int index)
    {
        var outside = fractureTool.getChunkMesh(index, false);
        var inside = fractureTool.getChunkMesh(index, true);
        var chunkMesh = outside.toUnityMesh();
        
        chunkMesh.subMeshCount = 2;
        chunkMesh.SetIndices(inside.getIndexes(), MeshTopology.Triangles, 1);
        return chunkMesh;
    }
}
