/// <summary>
/// cell 안에 들어 있는 파티클을 이용해서 cell의 좌표를 계산
/// </summary>
/// <param name="position"></param>
/// <param name="particleRadius"></param>
/// <returns></returns>
inline int3 GetCellCoord(in float3 position, float particleRadius)
{
	float space = particleRadius * 2;					// cell의 한 변
	return floor(position / space);
}

/// <summary>
/// gridBox 안에 파티클 위치가 몇 번째 인덱스에 있는지 계산
/// </summary>
/// <param name="position"></param>
/// <param name="particleBoundBox"></param>
/// <returns></returns>
inline uint GetCellIndex(in float3 position, in float3 particleBoundBox)
{
	return (int) ((position.x * particleBoundBox.y + position.y) * particleBoundBox.z + position.z);
}



/// <summary>
/// infinite grid에 물을 표현하려면 hash Function을 사용해야 한다.
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