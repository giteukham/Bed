using System;
using UnityEngine;
using Random = UnityEngine.Random;
public class _countingSort : MonoBehaviour
{
    public ComputeShader computeShader;

    private ComputeBuffer inputBuffer;
    private ComputeBuffer outputBuffer;
    private ComputeBuffer testBuffer;

    private int[] input = { 28, 39, 10, 2, 48, 49};

    private void OnEnable()
    {
        inputBuffer = new ComputeBuffer(512, sizeof(int));
        outputBuffer = new ComputeBuffer(512, sizeof(int));
        testBuffer = new ComputeBuffer(512, sizeof(int));

        // int num = input.Length;
        // for (int i = 0; i < input.Length; i++)
        // {
        //     input[i] = num;
        //     num--;
        // }
        
        inputBuffer.SetData(input);
        
        computeShader.SetInt("_inputBufferLength", input.Length);
        computeShader.SetBuffer(0, "_inputBuffer", inputBuffer);
        computeShader.SetBuffer(0, "_sortedBuffer", outputBuffer);
        computeShader.SetBuffer(0, "_testBuffer", testBuffer);
        
        computeShader.Dispatch(computeShader.FindKernel("CountingSort"), 2, 1, 1);
        
        int[] output = new int[512];
        outputBuffer.GetData(output);
        
        int[] test = new int[512];
        testBuffer.GetData(test);
        
        for (int i = 0; i < 512; i++)
        {
            Debug.Log(test[i] + " " + i);
        }
    }
    
    private void OnDisable()
    {
        inputBuffer.Release();
        outputBuffer.Release();
        testBuffer.Release();
    }
}