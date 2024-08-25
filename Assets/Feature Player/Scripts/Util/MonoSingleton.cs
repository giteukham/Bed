using System;
using Unity.VisualScripting;
using UnityEngine;
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static object lockObj = new object();
    public static bool isDestoryed { get; private set; }
    
    public static T Instance
    {
        get
        {
            if (isDestoryed)
            {
                Debug.LogWarning("Instance is destroyed.");
                return null;
            }
            
            lock (lockObj)
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        instance = obj.AddComponent<T>();
                        print("Created new instance of " + typeof(T).Name + " as a singleton.");
                    }
                }
                return instance;
            }
        }
    }

    private void OnApplicationQuit()
    {
        isDestoryed = true;
    }
    
    private void OnDestroy()
    {
        isDestoryed = true;
    }
}