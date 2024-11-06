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

        public void ExecutePairsSort(in Vector2Int[] pairs)
        {
            sorter = new OneSweep(
                oneSweep,
                numElements,
                ref keyBuffer,
                ref valueBuffer,
                ref globalHistBuffer,
                ref passHistBuffer,
                ref indexBuffer);

            int[] keys = new int[numElements];
            int[] values = new int[numElements];
            
            for (int i = 0; i < numElements; i++)
            {
                keys[i] = (int) pairs[i].x;
                values[i] = (int) pairs[i].y;
            }

            sortKeysBuffer.SetData(keys);
            sortValuesBuffer.SetData(values);

            sorter.Sort(
                numElements,
                sortKeysBuffer, sortValuesBuffer, 
                keyBuffer, valueBuffer, globalHistBuffer, passHistBuffer, indexBuffer,
                keys[0].GetType(),
                values[0].GetType(),
                true);

        }

        public T[] GetSortedKeys<T>()
        {
            T[] keys = new T[sortKeysBuffer.count];
            sortKeysBuffer.GetData(keys);
            return keys;
        }

        public Vector2Int[] GetSortedPairs()
        {
            int[] keys = new int[sortKeysBuffer.count];
            int[] values = new int[sortValuesBuffer.count];

            sortKeysBuffer.GetData(keys);
            sortValuesBuffer.GetData(values);

            Vector2Int[] pairs = new Vector2Int[sortKeysBuffer.count];
            
            for (int i = 0; i < sortKeysBuffer.count; i++)
            {
                pairs[i] = new Vector2Int(keys[i], values[i]);
            }

            return pairs;
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