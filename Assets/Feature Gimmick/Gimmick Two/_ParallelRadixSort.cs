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
    private ComputeBuffer localMaskScanBuffer;     // Local Mask Prefix Sum�� ����� �����͵�
    private ComputeBuffer blockSumsScanBuffer;        // Local Block Sum�� Global Prefix Sum�� �����ؼ� ���� �����͵�
    private ComputeBuffer sortedBufferByRadix;          // Radix Sort�� ���� ���ĵ� �����͵�
    
    private int[] unsortedData =
    {
        170, 45, 75, 90, 802, 24, 2, 66, 25, 62, 431
    };
    private const int _size = 256;

    // private int[] unsortedData = new int[_size];

    private int radixSortKernel, localMaskScanKernel, globalBlockScanKernel;

    private void Awake()
    {
        // int tmp = _size;
        // for (int i = 0; i < _size; i++)
        // {
        //     unsortedData[i] = tmp - i;
        // }
        
        radixSortKernel = radixSortShader.FindKernel("RadixSort");
        localMaskScanKernel = radixSortShader.FindKernel("LocalMaskScan");
        globalBlockScanKernel = scanShader.FindKernel("GlobalBlockScan");
        blockSumGroupCount = Mathf.CeilToInt((float) unsortedData.Length / THREAD_SIZE);
    }

    private void OnEnable()
    {
        unsortedBuffer = new ComputeBuffer(unsortedData.Length, sizeof(int));
        blockSumsBuffer = new ComputeBuffer(256, sizeof(int));
        blockSumsScanBuffer = new ComputeBuffer(256, sizeof(int));
        localMaskScanBuffer = new ComputeBuffer(256, sizeof(int));
        sortedBufferByRadix = new ComputeBuffer(256, sizeof(int));
        
    }

    private void Start()
    {
        unsortedBuffer.SetData(unsortedData);

        radixSortShader.SetInt("_elementCount", unsortedData.Length);
        radixSortShader.SetInt("_blockSumGroupCount", blockSumGroupCount);

        radixSortShader.SetBuffer(localMaskScanKernel, "_unsortedBuffer", unsortedBuffer);
        radixSortShader.SetBuffer(localMaskScanKernel, "_localMaskScanBuffer", localMaskScanBuffer);
        radixSortShader.SetBuffer(localMaskScanKernel, "_blockSumsBuffer", blockSumsBuffer);
        radixSortShader.SetBuffer(localMaskScanKernel, "_sortedBufferByRadix", sortedBufferByRadix);
        
        radixSortShader.SetBuffer(radixSortKernel, "_unsortedBuffer", unsortedBuffer);
        radixSortShader.SetBuffer(radixSortKernel, "_localMaskScanBuffer", localMaskScanBuffer);
        radixSortShader.SetBuffer(radixSortKernel, "_sortedBufferByRadix", sortedBufferByRadix);
        
        for (int bitShift = 0; bitShift < 32; bitShift += 2)
        {
            radixSortShader.SetInt("_bitShift", bitShift);
            radixSortShader.SetBuffer(localMaskScanKernel, "_unsortedBuffer", unsortedBuffer);
            radixSortShader.Dispatch(localMaskScanKernel, 256, 1, 1);
            
            scanShader.SetInt("_bitShift", bitShift);
            scanShader.SetBuffer(globalBlockScanKernel, "_localBlockSumsBuffer", blockSumsBuffer);
            scanShader.SetBuffer(globalBlockScanKernel, "_blockSumsScanBuffer", blockSumsScanBuffer);
            scanShader.Dispatch(globalBlockScanKernel, 256, 1, 1);
            
            radixSortShader.SetBuffer(radixSortKernel, "_unsortedBuffer", unsortedBuffer);
            radixSortShader.SetBuffer(radixSortKernel, "_blockSumsScanBuffer", blockSumsScanBuffer);
            radixSortShader.SetBuffer(radixSortKernel, "_blockSumsBuffer", blockSumsBuffer);
            radixSortShader.Dispatch(radixSortKernel, 256, 1, 1);
        }

        var sortedBufferByRadixArray = new int[_size];
        sortedBufferByRadix.GetData(sortedBufferByRadixArray);
        
        for (int i = 0; i < sortedBufferByRadixArray.Length; i++)
        {
            Debug.Log(sortedBufferByRadixArray[i]);
        }
    }
    
    private void OnDestroy()
    {
        unsortedBuffer.Release();
        blockSumsScanBuffer.Release();
        blockSumsBuffer.Release();
        sortedBufferByRadix.Release();
        localMaskScanBuffer.Release();
    }
}