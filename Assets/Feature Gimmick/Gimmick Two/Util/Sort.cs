using GPUSorting.Runtime;
using UnityEngine;

namespace Bed.Gimmick
{
    public class Sort : MonoSingleton<Sort>
    {
        public ComputeShader oneSweep;
        
        private ComputeBuffer keyBuffer, valueBuffer, globalHistBuffer, passHistBuffer, indexBuffer;
        private ComputeBuffer sortKeysBuffer, sortValuesBuffer;
        private OneSweep sorter;

        public void ExecuteKeysSort<T>(T[] keys)
        {
            int numKeys = keys.Length;
            
            sortKeysBuffer = new ComputeBuffer(numKeys, sizeof(uint));
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
            
            sortKeysBuffer.SetData(keys);

            sorter.Sort(
                numKeys,
                sortKeysBuffer, keyBuffer, globalHistBuffer, passHistBuffer, indexBuffer,
                typeof(T),
                true);
        }

        public void ExecutePairsSort(Vector2Int[] pairs)
        {
            int numPairs = pairs.Length;

            sortKeysBuffer = new ComputeBuffer(numPairs, sizeof(uint));
            sortValuesBuffer = new ComputeBuffer(numPairs, sizeof(uint));
            keyBuffer = new ComputeBuffer(numPairs, sizeof(uint));
            valueBuffer = new ComputeBuffer(numPairs, sizeof(uint));
            globalHistBuffer = new ComputeBuffer(numPairs, sizeof(uint));
            passHistBuffer = new ComputeBuffer(numPairs, sizeof(uint));
            indexBuffer = new ComputeBuffer(numPairs, sizeof(uint));
            sorter = new OneSweep(
                oneSweep,
                numPairs,
                ref keyBuffer,
                ref valueBuffer,
                ref globalHistBuffer,
                ref passHistBuffer,
                ref indexBuffer);

            int[] keys = new int[numPairs];
            int[] values = new int[numPairs];


            for (int i = 0; i < numPairs; i++)
            {
                keys[i] = (int) pairs[i].x;
                values[i] = (int) pairs[i].y;
            }

            sortKeysBuffer.SetData(keys);
            sortValuesBuffer.SetData(values);

            sorter.Sort(
                numPairs,
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

            for (int i = 0; i < 1000; i++)
            {
                Debug.Log(keys[i]);
            }

            sortKeysBuffer.GetData(keys);
            sortValuesBuffer.GetData(values);

            Vector2Int[] pairs = new Vector2Int[sortKeysBuffer.count];


            for (int i = 0; i < sortKeysBuffer.count; i++)
            {
                pairs[i] = new Vector2Int(keys[i], values[i]);
            }

            return pairs;
        }

        private void OnDestroy()
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