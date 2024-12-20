using GPUSorting.Runtime;
using UnityEngine;

namespace Bed.Gimmick
{
    public class Sort
    {
        private ComputeShader oneSweep;
        
        private ComputeBuffer keyBuffer, valueBuffer, globalHistBuffer, passHistBuffer, indexBuffer;
        private ComputeBuffer sortKeysBuffer, sortValuesBuffer;
        private OneSweep sorter;
        private int numElements;
        
        public Sort(ComputeShader oneSweep, int numElements)
        {
            this.oneSweep = oneSweep;
            this.numElements = numElements;
            
            sortKeysBuffer = new ComputeBuffer(numElements, sizeof(uint));
            sortValuesBuffer = new ComputeBuffer(numElements, sizeof(uint));
            keyBuffer = new ComputeBuffer(numElements, sizeof(uint));
            valueBuffer = new ComputeBuffer(numElements, sizeof(uint));
            globalHistBuffer = new ComputeBuffer(numElements, sizeof(uint));
            passHistBuffer = new ComputeBuffer(numElements, sizeof(uint));
            indexBuffer = new ComputeBuffer(numElements, sizeof(uint));
        }

        public void ExecuteKeysSort<T>(in T[] keys)
        {
            sorter = new OneSweep(
                oneSweep,
                numElements,
                ref keyBuffer,
                ref globalHistBuffer,
                ref passHistBuffer,
                ref indexBuffer);
            
            sortKeysBuffer.SetData(keys);

            sorter.Sort(
                numElements,
                sortKeysBuffer, keyBuffer, globalHistBuffer, passHistBuffer, indexBuffer,
                typeof(T),
                true);
        }

        public void ExecutePairsSort<K, V>(ref K[] keys, ref V[] values)
        {
            sorter = new OneSweep(
                oneSweep,
                numElements,
                ref keyBuffer,
                ref valueBuffer,
                ref globalHistBuffer,
                ref passHistBuffer,
                ref indexBuffer);
            
            sortKeysBuffer.SetData(keys);
            sortValuesBuffer.SetData(values);

            sorter.Sort(
                numElements,
                sortKeysBuffer, sortValuesBuffer, 
                keyBuffer, valueBuffer, globalHistBuffer, passHistBuffer, indexBuffer,
                typeof(K),
                typeof(V),
                true);
            
            sortKeysBuffer.GetData(keys);
            sortValuesBuffer.GetData(values);
        }

        public T[] GetSortedKeys<T>()
        {
            T[] keys = new T[sortKeysBuffer.count];
            sortKeysBuffer.GetData(keys);
            return keys;
        }
        
        public void Dispose()
        {
            keyBuffer?.Dispose();
            valueBuffer?.Dispose();
            globalHistBuffer?.Dispose();
            passHistBuffer?.Dispose();
            indexBuffer?.Dispose();
            sortKeysBuffer?.Dispose();
            sortValuesBuffer?.Dispose();
        }
    }
}