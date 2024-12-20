using UnityEngine;

namespace Bed.Gimmick
{
    public static class ComputeHelper
    {
        public static void SetKernels(ComputeShader compute, ComputeBuffer buffers, string name, params int[] kernelIndices)
        {
            foreach (var kernelIndex in kernelIndices)
            {
                compute.SetBuffer(kernelIndex, name, buffers);
            }
        }
    }
}