using System;
using Unity.Mathematics;
using UnityEngine;
public class _Sum : MonoBehaviour
{
    public ComputeShader computeShader;

    private ComputeBuffer numbersBuffer;
    private ComputeBuffer resultsBuffer;
    private int[] numbers = new int[256 * 256];
    private int[] results = new int[256];

    private void Awake()
    {
        for (int i = 0; i < 256 * 256; i++)
        {
            numbers[i] = i;
        }
    }
    private void OnEnable()
    {
        numbersBuffer = new ComputeBuffer(256 * 256, 4);
        numbersBuffer.SetData(numbers);
        
        resultsBuffer = new ComputeBuffer(256, 4);
        resultsBuffer.SetData(results);
        
        computeShader.SetBuffer(0, "_result", resultsBuffer);
        computeShader.SetBuffer(0, "_numbers", numbersBuffer);
    }
    private void Start()
    {  
        computeShader.Dispatch(0, 256, 1, 1);
        
        resultsBuffer.GetData(results);
        
        foreach (var result in results)
        {
            Debug.Log(result);
        }
    }
}