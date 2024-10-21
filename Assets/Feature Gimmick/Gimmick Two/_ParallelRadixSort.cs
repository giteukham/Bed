using System;
using UnityEngine;
public class _ParallelRadixSort : MonoBehaviour
{
    private const int THREAD_SIZE = 256;                  // Compute Shader�� �ִ� Thread Size�� ��ġ���Ѿ� ��.
    private int blockSumGroupCount;
    
    public ComputeShader radixSortShader;
    public ComputeShader scanShader;
    
    private ComputeBuffer unsortedBuffer;               // ������ �ȵ� �����͵�
    private ComputeBuffer blockSumsBuffer;              // Local Block Sum�� ����� �����͵�
    private ComputeBuffer localMaskPrefixSumBuffer;     // Local Mask Prefix Sum�� ����� �����͵�
    private ComputeBuffer prefixBlockSumsBuffer;        // Local Block Sum�� Global Prefix Sum�� �����ؼ� ���� �����͵�
    private ComputeBuffer sortedGlobalPositionBuffer;   // ���ĵ� �����͵��� ��ġ�� ��Ÿ���� �����͵�
    private ComputeBuffer sortedBufferByRadix;          // Radix Sort�� ���� ���ĵ� �����͵�
    
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