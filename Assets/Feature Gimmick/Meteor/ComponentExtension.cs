
using UnityEngine;

public static class ComponentExtension
{
    public static void SetComponent<T>(this GameObject gameObject, T type) where T : Object
    {
        if (type == null) return;
        
        switch (type)
        {
            case Material material:
                gameObject.GetComponent<Renderer>().material = material;
                break;
            case Mesh mesh:
                gameObject.GetComponent<MeshFilter>().mesh = mesh;
                break;
        }
    }
}
