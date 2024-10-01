/// <summary>
/// cell 안에 들어 있는 파티클을 이용해서 cell의 좌표를 계산
/// </summary>
/// <param name="position"></param>
/// <param name="particleRadius"></param>
/// <returns></returns>
int3 GetCellCoord(float3 position, float particleRadius)
{
	float space = particleRadius * 2.;
	return (int3) (position / space);
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