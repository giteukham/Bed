using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListeningPosition : MonoBehaviour
{
    [SerializeField] private Transform sourcePosition, sourceRotation;

    private void Update()
    {
        transform.SetPositionAndRotation(sourcePosition.position, sourceRotation.rotation);
    }
}