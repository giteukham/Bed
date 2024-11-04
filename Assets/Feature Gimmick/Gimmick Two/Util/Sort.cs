using GPUSorting.Runtime;
using UnityEngine;

namespace Bed.Gimmick
{
    public class Sort : MonoSingleton<Sort>
    {
        public ComputeShader oneSweep;
        
        private ComputeBuffer keyBuffer, globalHistBuffer, passHistBuffer, indexBuffer;
        private ComputeBuffer sortBuffer;
        private OneSweep sorter;

        public void ExecuteSort<T>(T[] keys)
        {
            int numKeys = keys.Length;
            
            sortBuffer = new ComputeBuffer(numKeys, sizeof(uint));
            keyBuffer = new ComputeBuffer(numKeys, sizeof(uint));
            globalHistBuffer = new ComputeBuffer(numKeys, sizeof(uint));
            passHistBuffer = new ComputeBuffer(numKeys, sizeof(uint));
            indexBuffer = new ComputeBuffer(numKeys, sizeof(uint));
            sorter = new OneSweep(
                oneSweep,
                numKeys,
                ref keyBuffer,
                ref globalHistBuffer,
                ref passHistBuffer,
                ref indexBuffer);
            
            sortBuffer.SetData(keys);

            sorter.Sort(
                numKeys,
                sortBuffer, keyBuffer, globalHistBuffer, passHistBuffer, indexBuffer,
                typeof(T),
                true);
        }
        
        public T[] GetSortedKeys<T>()
        {
            T[] keys = new T[sortBuffer.count];
            sortBuffer.GetData(keys);
            return keys;
        }
    }
}