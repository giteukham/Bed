inline int3 CalculateGridPosition(in float3 position, float cellSize)
{
	float space = cellSize;					// cell�� �� ��
	return floor(position / space);
}

inline uint GetCellFlatIndex(in float3 position, in float3 particleBoundBox)
{
	return (int) ((position.x * particleBoundBox.y + position.y) * particleBoundBox.z + position.z);
}

// Infinite grid���� �� �Լ��� ���� cell index�� ���� �� �ִ�.
inline uint HashFunction(in int3 position)
{
	uint hash = (73856093 * position.x) ^ (19349663 * position.y) ^ (83492791 * position.z);
	return hash;
}