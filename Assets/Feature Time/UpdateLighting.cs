using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateLighting : MonoBehaviour
{
    

    
    private Vector2[] targetRotation = {new(15, 115), new(10, 120), new(5, 125), new(-5, 130), new(-10, 135)}; // 1시간, 3시간, 5시간

    void Start()
    {
        
    }

    void Update()
    {   
        // 1시간 지나기 전
        if (TimeManager.playTimeToMin > 60)
        {

        }
    }
}
