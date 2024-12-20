inline int3 CalculateGridPosition(in float3 position, float cellSize)
{
	return floor(position / cellSize);
}

inline uint GetCellFlatIndex(in float3 position, in float3 particleBoundBox)
{
	return (int) ((position.x * particleBoundBox.y + position.y) * particleBoundBox.z + position.z);
}

// Infinite grid에선 이 함수를 통해 cell index를 구할 수 있다.
inline uint HashFunction(in int3 position, in uint totalCellNumbers)
{
	uint hash = (73856093 * position.x) ^ (440817757 * position.y) ^ (83492791 * position.z);
	return hash % totalCellNumbers;
}