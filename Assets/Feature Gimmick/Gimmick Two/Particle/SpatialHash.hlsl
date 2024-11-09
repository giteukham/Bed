/// <summary>
/// cell �ȿ� ��� �ִ� ��ƼŬ�� �̿��ؼ� cell�� ��ǥ�� ���
/// </summary>
/// <param name="position"></param>
/// <param name="particleRadius"></param>
/// <returns></returns>
inline int3 GetCellCoord(in float3 position, float particleRadius)
{
	float space = particleRadius * 2;					// cell�� �� ��
	return floor(position / space);
}

/// <summary>
/// gridBox �ȿ� ��ƼŬ ��ġ�� �� ��° �ε����� �ִ��� ���
/// </summary>
/// <param name="position"></param>
/// <param name="particleBoundBox"></param>
/// <returns></returns>
inline uint GetCellIndex(in float3 position, in float3 particleBoundBox)
{
	return (int) ((position.x * particleBoundBox.y + position.y) * particleBoundBox.z + position.z);
}



/// <summary>
/// infinite grid�� ���� ǥ���Ϸ��� hash Function�� ����ؾ� �Ѵ�.
/// </summary>
/// <param name="position"></param>
/// <param name="hashTableSize"></param>
/// <returns></returns>
inline int HashFunction(in int3 position, int hashTableSize)
{
	const uint p1 = 73856093;
	const uint p2 = 19349663;
	const uint p3 = 83492791;
	int hash = p1 * position.x ^ p2 * position.y ^ p3 * position.z;
	hash %= hashTableSize;
	return hash;
}