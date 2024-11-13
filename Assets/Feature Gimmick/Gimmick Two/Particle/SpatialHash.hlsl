inline int3 GetCellDimIndex(in float3 position, float particleRadius)
{
	float space = particleRadius;					// cell의 한 변
	return floor(position / space);
}

inline uint GetCellFlatIndex(in float3 position, in float3 particleBoundBox)
{
	return (int) ((position.x * particleBoundBox.y + position.y) * particleBoundBox.z + position.z);
}

// Infinite grid에선 이 함수를 통해 cell index를 구할 수 있다.
inline int HashFunction(in int3 position, int hashTableSize)
{
	int hash = (73856093 * position.x) ^ (19349663 * position.y) ^ (83492791 * position.z);
	return abs(hash) % hashTableSize;
}