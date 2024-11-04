using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
public class _countingSort : MonoBehaviour
{
    public ComputeShader countingSortShader;
    public ComputeShader radixSortShader;

    private ComputeBuffer unsortedBuffer;               // 정렬이 안된 데이터들
    private ComputeBuffer sortedBufferByCounting;       // Counting Sort를 통해 정렬된 데이터들
    private ComputeBuffer sortedBufferByRadix;          // Radix Sort를 통해 정렬된 데이터들
    private ComputeBuffer sortedBuffer;                 // 최종 정렬된 데이터들
    private ComputeBuffer testBuffer;

    private int[] input = new int[1024];
    private float time;

    private int globalMaxFigureNumber = 0;

    private void Awake()
    {
        for (int i = 0; i < input.Length; i++)
        {
            input[i] = i;
        }
        
        globalMaxFigureNumber = GetFigureNumbers(input.Max());
    }

    private void OnEnable()
    {
        unsortedBuffer = new ComputeBuffer(input.Length, sizeof(int));
        sortedBufferByCounting = new ComputeBuffer(input.Length, sizeof(int));
        sortedBufferByRadix = new ComputeBuffer(input.Length, sizeof(int));
        sortedBuffer = new ComputeBuffer(input.Length, sizeof(int));
        testBuffer = new ComputeBuffer(input.Length, sizeof(int));
    }

    private void Start()
    {
        unsortedBuffer.SetData(input);
        
        countingSortShader.SetInt("_inputBufferLength", input.Length);
        countingSortShader.SetBuffer(0, "_unsortedBuffer", unsortedBuffer);
        countingSortShader.SetBuffer(0, "_sortedBufferByCounting", sortedBufferByCounting);
        countingSortShader.SetBuffer(0, "_testBuffer", testBuffer);
        
        for (int i = 0; i < globalMaxFigureNumber; i++)
        {
            countingSortShader.SetInt("_figureNumber", i);
            countingSortShader.Dispatch(countingSortShader.FindKernel("CountingSort"), 256, 1, 1);
        }
        
        var test = new int[input.Length];
        testBuffer.GetData(test);
        
        var sortedBufferByCountingArray = new int[input.Length];
        sortedBufferByCounting.GetData(sortedBufferByCountingArray);

        foreach (var i in input)
        {
            Debug.Log($"test: {sortedBufferByCountingArray[i]} input: {i}");
        }
    }

    private void OnDisable()
    {
        unsortedBuffer?.Release();
        sortedBuffer?.Release();
        testBuffer?.Release();
    }
    
    
    /// <summary>
    /// 숫자가 몇 자리수인지 구한다.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private int GetFigureNumbers(int value)
    {
        return (int)Math.Floor(Math.Log10(value)) + 1;
    }

}