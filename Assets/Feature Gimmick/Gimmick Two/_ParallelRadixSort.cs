using FMOD;
using System;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class _ParallelRadixSort : MonoBehaviour
{
    private const int THREAD_SIZE = 256;                  // Compute Shader�� �ִ� Thread Size�� ��ġ���Ѿ� ��.
    private int blockSumGroupCount;
    
    public ComputeShader radixSortShader;
    public ComputeShader scanShader;
    public ComputeShader checkSortShader;
    
    private ComputeBuffer unsortedBuffer;               // ������ �ȵ� �����͵�
    private ComputeBuffer localPrefixSumBuffer;         // Local Mask Prefix Sum�� ����� �����͵�
    private ComputeBuffer blockSumsBuffer;              // Local Block Sum�� ����� �����͵�
    private ComputeBuffer blockSumsCount;               // block Sums Count
    private ComputeBuffer blockSumsScanBuffer;          // Local Block Sum�� Global Prefix Sum�� �����ؼ� ���� �����͵�
    private ComputeBuffer sortedBufferByRadix;          // Radix Sort�� ���� ���ĵ� �����͵�
    private ComputeBuffer checkedBuffer;                // ������ �� �Ǿ����� Ȯ���ϱ� ���� �����͵�
    
    private ComputeBuffer testBuffer;

    private bool isSorted = false;
    // private int[] unsortedData =
    // {
    //     170, 45, 75, 90, 802, 24, 2, 66, 25, 62, 431
    // };
    private const int _size = 333;

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
        checkedBuffer        = new ComputeBuffer(512, sizeof(int));
        testBuffer           = new ComputeBuffer(unsortedData.Length, sizeof(int));
    }

    private void Start()
    {
        //checkSortShader.Dispatch(checkSortKernel, Mathf.CeilToInt((float)_size / THREAD_SIZE), 1, 1);
        
        for (int bitShift = 0; bitShift <= 30; bitShift += 2)
        {
            unsortedBuffer.SetData(unsortedData);
            radixSortShader.SetInt("_bitShift", bitShift);
            scanShader.SetInt("_bitShift", bitShift);

            checkSortShader.SetInt("_elementCount", unsortedData.Length);
            checkSortShader.SetBool("_isSorted", isSorted);
            checkSortShader.SetBuffer(checkSortKernel, "_unsortedBuffer", unsortedBuffer);
            checkSortShader.SetBuffer(checkSortKernel, "_checkedBuffer", checkedBuffer);

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
        
        for (int i = 0; i < sortedBufferByRadixArray.Length; i++)
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
        
        checkedBuffer?.Release();
        checkedBuffer = null;
        
        testBuffer?.Release();
        testBuffer = null;
    }
}