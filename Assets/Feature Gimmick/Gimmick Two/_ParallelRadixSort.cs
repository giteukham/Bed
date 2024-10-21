using System;
using UnityEngine;
public class _ParallelRadixSort : MonoBehaviour
{
    private const int THREAD_SIZE = 256;                  // Compute Shader에 있는 Thread Size와 일치시켜야 함.
    private int blockSumGroupCount;
    
    public ComputeShader radixSortShader;
    public ComputeShader scanShader;
    
    private ComputeBuffer unsortedBuffer;               // 정렬이 안된 데이터들
    private ComputeBuffer blockSumsBuffer;              // Local Block Sum이 적용된 데이터들
    private ComputeBuffer localMaskPrefixSumBuffer;     // Local Mask Prefix Sum이 적용된 데이터들
    private ComputeBuffer prefixBlockSumsBuffer;        // Local Block Sum을 Global Prefix Sum로 적용해서 넣은 데이터들
    private ComputeBuffer sortedGlobalPositionBuffer;   // 정렬된 데이터들의 위치를 나타내는 데이터들
    private ComputeBuffer sortedBufferByRadix;          // Radix Sort를 통해 정렬된 데이터들
    
    private int[] unsortedData =
    {
        170, 45, 75, 90, 802, 24, 2, 66, 25, 62, 431
    };

    private int radixSortKernel, localMaskScanKernel, globalBlockScanKernel;

    private void Awake()
    {
        radixSortKernel = radixSortShader.FindKernel("RadixSort");
        localMaskScanKernel = radixSortShader.FindKernel("LocalMaskScan");
        globalBlockScanKernel = scanShader.FindKernel("GlobalBlockScan");
        blockSumGroupCount = Mathf.CeilToInt((float) unsortedData.Length / THREAD_SIZE);
    }

    private void OnEnable()
    {
        unsortedBuffer = new ComputeBuffer(unsortedData.Length, sizeof(int));
        blockSumsBuffer = new ComputeBuffer(256, sizeof(int));
        prefixBlockSumsBuffer = new ComputeBuffer(256, sizeof(int));
        localMaskPrefixSumBuffer = new ComputeBuffer(unsortedData.Length, sizeof(int));
        sortedGlobalPositionBuffer = new ComputeBuffer(unsortedData.Length, sizeof(int));
        sortedBufferByRadix = new ComputeBuffer(256, sizeof(int));
        
    }

    private void Start()
    {
        unsortedBuffer.SetData(unsortedData);

        radixSortShader.SetInt("_elementCount", unsortedData.Length);
        radixSortShader.SetInt("_blockSumGroupCount", blockSumGroupCount);
        
        radixSortShader.SetBuffer(localMaskScanKernel, "_unsortedBuffer", unsortedBuffer);
        radixSortShader.SetBuffer(localMaskScanKernel, "_localMaskPrefixSumBuffer", localMaskPrefixSumBuffer);
        radixSortShader.SetBuffer(localMaskScanKernel, "_blockSumsBuffer", blockSumsBuffer);
        
        radixSortShader.SetBuffer(radixSortKernel, "_unsortedBuffer", unsortedBuffer);
        radixSortShader.SetBuffer(radixSortKernel, "_localMaskPrefixSumBuffer", localMaskPrefixSumBuffer);
        
        for (int i = 0; i < 32; i += 2)
        {
            radixSortShader.SetInt("_bitShift", i);
            radixSortShader.Dispatch(localMaskScanKernel, 256, 1, 1);
            
            scanShader.SetInt("_bitShift", i);
            scanShader.SetBuffer(globalBlockScanKernel, "_localBlockSumsBuffer", blockSumsBuffer);
            scanShader.SetBuffer(globalBlockScanKernel, "_prefixBlockSumsBuffer", prefixBlockSumsBuffer);
            scanShader.Dispatch(globalBlockScanKernel, 256, 1, 1);
            
            radixSortShader.SetBuffer(radixSortKernel, "_prefixBlockSumsBuffer", prefixBlockSumsBuffer);
            radixSortShader.SetBuffer(radixSortKernel, "_sortedBufferByRadix", sortedBufferByRadix);
            radixSortShader.Dispatch(radixSortKernel, 256, 1, 1);
        }

        var sortedBufferByRadixArray = new int[256];
        sortedBufferByRadix.GetData(sortedBufferByRadixArray);
        
        for (int i = 0; i < sortedBufferByRadixArray.Length; i++)
        {
            Debug.Log(sortedBufferByRadixArray[i]);
        }
    }
    
    private void OnDestroy()
    {
        unsortedBuffer.Release();
        prefixBlockSumsBuffer.Release();
        blockSumsBuffer.Release();
        sortedBufferByRadix.Release();
        localMaskPrefixSumBuffer.Release();
        sortedGlobalPositionBuffer.Release();
    }
}