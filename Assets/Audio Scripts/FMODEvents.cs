using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Cat SFX")]
    [field: SerializeField] public EventReference catMeow {get; private set;}

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference blanketMoving {get; private set;}

    public static FMODEvents instance {get; private set;}

    private void Awake() 
    {
        if (instance != null) Debug.LogError("FMOD Events가 이미 존재");
        instance = this;
    }
}
