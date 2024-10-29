using FMOD;
using System;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class _ParallelRadixSort : MonoBehaviour
{
    private const ushort THREAD_SIZE = 256;                  // Compute Shader에 있는 Thread Size와 일치시켜야 함.
    private int blockSumGroupCount;
    
    public ComputeShader radixSortShader;
    public ComputeShader scanShader;
    public ComputeShader checkSortShader;
    
    private ComputeBuffer unsortedBuffer;               // 정렬이 안된 데이터들
    private ComputeBuffer localPrefixSumBuffer;         // Local Mask Prefix Sum이 적용된 데이터들
    private ComputeBuffer blockSumsBuffer;              // Local Block Sum이 적용된 데이터들
    private ComputeBuffer blockSumsCount;               // block Sums Count
    private ComputeBuffer blockSumsScanBuffer;          // Local Block Sum을 Global Prefix Sum로 적용해서 넣은 데이터들
    private ComputeBuffer sortedBufferByRadix;          // Radix Sort를 통해 정렬된 데이터들
    private ComputeBuffer isSortedBuffer;               // 정렬이 되었는지 확인하는 버퍼
    
    private ComputeBuffer testBuffer;

    private int isSorted = 0;
    private const int _size = 1000000;

    private int[] unsortedData = new int[_size];

    private int radixSortKernel, localMaskScanKernel, globalBlockScanKernel, checkSortKernel, addBlockSumsKernel;

    private void Awake()
    {
        int tmp = _size;
        for (int i = 0; i < _size; i++)
        {
            unsortedData[i] = tmp - i;
        }
        
        radixSortKernel         = radixSortShader.FindKernel("RadixSort");
        localMaskScanKernel     = radixSortShader.FindKernel("LocalMaskScan");
        globalBlockScanKernel   = scanShader.FindKernel("GlobalBlockScan");
        addBlockSumsKernel      = scanShader.FindKernel("AddBlockSums");
        checkSortKernel         = checkSortShader.FindKernel("CheckSort");
        blockSumGroupCount      = Mathf.CeilToInt((float) unsortedData.Length / THREAD_SIZE);
    }   

    private void OnEnable()
    {
        unsortedBuffer       = new ComputeBuffer(unsortedData.Length, sizeof(int));
        localPrefixSumBuffer = new ComputeBuffer(unsortedData.Length, sizeof(int));
        blockSumsBuffer      = new ComputeBuffer(Mathf.CeilToInt((float)unsortedData.Length / THREAD_SIZE) * 4, sizeof(int));
        blockSumsScanBuffer  = new ComputeBuffer(Mathf.CeilToInt((float)unsortedData.Length / THREAD_SIZE) * 4, sizeof(int));
        blockSumsCount       = new ComputeBuffer(1, sizeof(int));
        sortedBufferByRadix  = new ComputeBuffer(unsortedData.Length, sizeof(int));
        isSortedBuffer       = new ComputeBuffer(1, sizeof(int));
        testBuffer           = new ComputeBuffer(unsortedData.Length, sizeof(int));
    }

    private void Start()
    {
        for (int bitShift = 0; bitShift <= 30; bitShift += 2)
        {
            unsortedBuffer.SetData(unsortedData);
            radixSortShader.SetInt("_bitShift", bitShift);
            scanShader.SetInt("_bitShift", bitShift);

            sortedBufferByRadix.SetData(unsortedData);
            checkSortShader.SetInt("_elementCount", unsortedData.Length);
            checkSortShader.SetBuffer(checkSortKernel, "_isSortedBuffer", isSortedBuffer);
            checkSortShader.SetBuffer(checkSortKernel, "_dataToCheckBuffer", sortedBufferByRadix);
            checkSortShader.Dispatch(checkSortKernel, Mathf.CeilToInt((float)_size / THREAD_SIZE), 1, 1);
            isSortedBuffer.GetData(new[] {isSorted});
            
            if (isSorted == 1)
            {
                Debug.Log("Already Sorted");
                break;
            }
            
            radixSortShader.SetInt("_blockSumGroupCount", blockSumGroupCount);
            radixSortShader.SetBuffer(localMaskScanKernel, "_unsortedBuffer", unsortedBuffer);
            radixSortShader.SetBuffer(localMaskScanKernel, "_localPrefixSumBuffer", localPrefixSumBuffer);
            radixSortShader.SetBuffer(localMaskScanKernel, "_blockSumsBuffer", blockSumsBuffer);
            radixSortShader.SetBuffer(localMaskScanKernel, "_blockSumsCount", blockSumsCount);
            radixSortShader.SetBuffer(localMaskScanKernel, "_sortedBufferByRadix", sortedBufferByRadix);
            radixSortShader.SetBuffer(localMaskScanKernel, "_testBuffer", testBuffer);
            radixSortShader.Dispatch(localMaskScanKernel, Mathf.CeilToInt((float)_size / THREAD_SIZE), 1, 1);
    
            scanShader.SetBuffer(globalBlockScanKernel, "_input", blockSumsBuffer);
            scanShader.SetBuffer(globalBlockScanKernel, "_output", blockSumsScanBuffer);
            scanShader.SetBuffer(globalBlockScanKernel, "_inputElementCount", blockSumsCount);
            scanShader.SetBuffer(globalBlockScanKernel, "_testBuffer", testBuffer);
            scanShader.Dispatch(globalBlockScanKernel, Mathf.CeilToInt((float)_size / THREAD_SIZE), 1, 1);
            
            scanShader.SetBuffer(addBlockSumsKernel, "_input", blockSumsBuffer);
            scanShader.SetBuffer(addBlockSumsKernel, "_output", blockSumsScanBuffer);
            scanShader.SetBuffer(addBlockSumsKernel, "_testBuffer", testBuffer);
            scanShader.SetBuffer(addBlockSumsKernel, "_inputElementCount", blockSumsCount);
            scanShader.Dispatch(addBlockSumsKernel, Mathf.CeilToInt((float)_size / THREAD_SIZE), 1, 1);
            
            radixSortShader.SetBuffer(radixSortKernel, "_unsortedBuffer", unsortedBuffer);
            radixSortShader.SetBuffer(radixSortKernel, "_localPrefixSumBuffer", localPrefixSumBuffer);
            radixSortShader.SetBuffer(radixSortKernel, "_blockSumsScanBuffer", blockSumsScanBuffer);
            radixSortShader.SetBuffer(radixSortKernel, "_sortedBufferByRadix", sortedBufferByRadix);
            radixSortShader.SetBuffer(radixSortKernel, "_testBuffer", testBuffer);
            radixSortShader.Dispatch(radixSortKernel, Mathf.CeilToInt((float)_size / THREAD_SIZE), 1, 1);
            
            sortedBufferByRadix.GetData(unsortedData);
        }
        var sortedBufferByRadixArray = new int[_size];
        testBuffer.GetData(sortedBufferByRadixArray);
        
        for (int i = 0; i < 10; i++)
        {
            Debug.Log(sortedBufferByRadixArray[i]);
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
        
        blockSumsCount?.Release();
        blockSumsCount = null;
        
        sortedBufferByRadix?.Release();
        sortedBufferByRadix = null;
        
        localPrefixSumBuffer?.Release();
        localPrefixSumBuffer = null;
        
        isSortedBuffer?.Release();
        isSortedBuffer = null;

        testBuffer?.Release();
        testBuffer = null;
    }
}