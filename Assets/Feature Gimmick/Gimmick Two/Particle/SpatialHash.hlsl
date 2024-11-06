/// <summary>
/// cell �ȿ� ��� �ִ� ��ƼŬ�� �̿��ؼ� cell�� ��ǥ�� ���
/// </summary>
/// <param name="position"></param>
/// <param name="particleRadius"></param>
/// <returns></returns>
int3 GetCellCoord(in float3 position, float particleRadius)
{
	float space = particleRadius;					// cell�� �� ��
	return (int3) floor(position / space);
}

/// <summary>
/// gridBox �ȿ� ��ƼŬ ��ġ�� �� ��° �ε����� �ִ��� ���
/// </summary>
/// <param name="position"></param>
/// <param name="particleBoundBox"></param>
/// <returns></returns>
uint GetCellIndex(in float3 position, in float3 particleBoundBox)
{
	return (int) (position.x + position.y * particleBoundBox.x + position.z * particleBoundBox.x * particleBoundBox.y);
}



/// <summary>
/// infinite grid�� ���� ǥ���Ϸ��� hash Function�� ����ؾ� �Ѵ�.
/// </summary>
/// <param name="position"></param>
/// <param name="hashTableSize"></param>
/// <returns></returns>
uint HashFunction(in int3 position, int hashTableSize)
{
	uint hash = (position.x * 92837111) ^ (position.y * 689287499) ^ (position.z * 283923481);
	return hash %= hashTableSize;
}