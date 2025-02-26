
using UnityEngine;

public class ResolutionInside : MonoBehaviour
{
    private RectTransform rect;
    
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
}
