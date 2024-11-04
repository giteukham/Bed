/// <summary>
/// cell 안에 들어 있는 파티클을 이용해서 cell의 좌표를 계산
/// </summary>
/// <param name="position"></param>
/// <param name="particleRadius"></param>
/// <returns></returns>
int3 GetCellCoord(float3 position, float particleRadius)
{
	float space = particleRadius;					// cell의 한 변
	return (int3) floor(position / space);
}

/// <summary>
/// gridBox 안에 파티클 위치가 몇 번째 인덱스에 있는지 계산
/// </summary>
/// <param name="position"></param>
/// <param name="particleBoundBox"></param>
/// <returns></returns>
uint GetCellIndex(float3 position, float3 particleBoundBox)
{
	return (int) (position.x + position.y * particleBoundBox.x + position.z * particleBoundBox.x * particleBoundBox.y);
}



/// <summary>
/// infinite grid에 물을 표현하려면 hash Function을 사용해야 한다.
/// </summary>
/// <param name="position"></param>
/// <param name="hashTableSize"></param>
/// <returns></returns>
uint HashFunction(int3 position, int hashTableSize)
{
	uint hash = (position.x * 92837111) ^ (position.y * 689287499) ^ (position.z * 283923481);
	return hash %= hashTableSize;
}