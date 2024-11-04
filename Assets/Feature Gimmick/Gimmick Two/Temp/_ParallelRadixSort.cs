using FMOD;
using System;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class _ParallelRadixSort : MonoBehaviour
{
    private const ushort THREAD_SIZE = 256;                  // Compute Shader�� �ִ� Thread Size�� ��ġ���Ѿ� ��.
    private int numBlockSumsBlocks;
    
    public ComputeShader radixSortShader;
    
    private ComputeBuffer unsortedBuffer;               // ������ �ȵ� �����͵�
    private ComputeBuffer localBitScanBuffer;         // Local Mask Prefix Sum�� ����� �����͵�
    private ComputeBuffer blockSumsBuffer;              // Local Block Sum�� ����� �����͵�
    private ComputeBuffer blockSumsScanBuffer;          // Local Block Sum�� Global Prefix Sum�� �����ؼ� ���� �����͵�
    private ComputeBuffer sortedBuffer;                 // ���� ���ĵ� �����͵�
    private ComputeBuffer isSortedBuffer;               // ������ �Ǿ����� Ȯ���ϴ� ����
    private ComputeBuffer groupSumsBuffer;              // Group Sum�� ����� �����͵�
    private ComputeBuffer testBuffer;

    private int isSorted = 0;
    private const int _size = 257;

    private int[] unsortedData = new int[_size];

    private int radixSortKernel;

    private void Awake()
    {
        int tmp = _size;
        for (int i = 0; i < _size; i++)
        {
            unsortedData[i] = tmp - i;
        }
        
        radixSortKernel                     = radixSortShader.FindKernel("RadixSort");
        numBlockSumsBlocks                  = Mathf.CeilToInt((float) unsortedData.Length / THREAD_SIZE);
    }

    private void OnEnable()
    {
        unsortedBuffer       = new ComputeBuffer(unsortedData.Length, sizeof(int));
        localBitScanBuffer   = new ComputeBuffer(unsortedData.Length, sizeof(int));
        blockSumsBuffer      = new ComputeBuffer(Mathf.CeilToInt((float)unsortedData.Length / THREAD_SIZE) * 4, sizeof(int));
        blockSumsScanBuffer  = new ComputeBuffer(Mathf.CeilToInt((float)unsortedData.Length / THREAD_SIZE) * 4, sizeof(int));
        sortedBuffer         = new ComputeBuffer(unsortedData.Length, sizeof(int));
        isSortedBuffer       = new ComputeBuffer(1, sizeof(int));
        groupSumsBuffer      = new ComputeBuffer(Mathf.CeilToInt((float)unsortedData.Length / THREAD_SIZE) * 4, sizeof(int));
        testBuffer           = new ComputeBuffer(1024, sizeof(int));
        
        unsortedBuffer.SetData(unsortedData);
    }

    private void Start()
    {
        for (int bitShift = 0; bitShift <= 0; bitShift += 2)
        {
            radixSortShader.SetInt("_bitShift", bitShift);
            radixSortShader.SetInt("_numBlockSumsBlocks", numBlockSumsBlocks);
            radixSortShader.SetBuffer(radixSortKernel, "_inputBuffer", unsortedBuffer);
            radixSortShader.SetBuffer(radixSortKernel, "_localBitScanBuffer", localBitScanBuffer);
            radixSortShader.SetBuffer(radixSortKernel, "_blockSumsBuffer", blockSumsBuffer);
            radixSortShader.SetBuffer(radixSortKernel, "_groupSumsBuffer", groupSumsBuffer);
            radixSortShader.SetBuffer(radixSortKernel, "_blockSumsScanBuffer", blockSumsScanBuffer);
            radixSortShader.SetBuffer(radixSortKernel, "_outputBuffer", sortedBuffer);
            radixSortShader.SetBuffer(radixSortKernel, "_testBuffer", testBuffer);
            radixSortShader.Dispatch(radixSortKernel, 4096, 1, 1);
        }
        var sortedBufferByRadixArray = new int[1024];
        testBuffer.GetData(sortedBufferByRadixArray);
            
        for (int i = 0; i < 1024; i++)
        {
            Debug.Log(sortedBufferByRadixArray[i] + " " + i);
        }
    }
    
    private void OnDestroy()
    {
        unsortedBuffer?.Release();
        unsortedBuffer = null;
        
        blockSumsScanBuffer?.Release();
        blockSumsScanBuffer = null;
        
        blockSumsBuffer?.Release();
        blockSumsBuffer = null;

        sortedBuffer?.Release();
        sortedBuffer = null;
        
        groupSumsBuffer?.Release();
        groupSumsBuffer = null;
        
        localBitScanBuffer?.Release();
        localBitScanBuffer = null;
        
        isSortedBuffer?.Release();
        isSortedBuffer = null;

        testBuffer?.Release();
        testBuffer = null;
    }
}