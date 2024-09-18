using Bed.Gimmick;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using ParticleSystem = Bed.Gimmick.ParticleSystem;

public class SpatialHash : MonoBehaviour
{
    private void Awake()
    {

    }

    private void Start()
    {

    }

    private void Update()
    {
        
    }

    [BurstCompile]
    public struct SpatialHashFunction : IDisposable
    {
        private static int numCells = 1000;
        SpatialHashFunction(int particleCount)
        {
            
        }
        
        public static int CalculateHashCoord(int xi, int yi, int zi)
        {
            var hash = (xi * 92837111) ^ (yi * 689287499) ^ (zi * 28392348);
            return Mathf.Abs(hash) % numCells; 
        }
        
        public void Dispose()
        {
            
        }
    }
}


