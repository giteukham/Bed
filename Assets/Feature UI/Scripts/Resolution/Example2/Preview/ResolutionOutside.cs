
using UnityEngine;

public class ResolutionOutside : MonoBehaviour
{
    private RectTransform rect;
    
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
}
