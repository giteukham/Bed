/// <summary>
/// cell �ȿ� ��� �ִ� ��ƼŬ�� �̿��ؼ� cell�� ��ǥ�� ���
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
/// gridBox �ȿ� ��ƼŬ ��ġ�� �� ��° �ε����� �ִ��� ���
/// </summary>
/// <param name="position"></param>
/// <param name="particleBoundBox"></param>
/// <returns></returns>
uint GetCellIndex(float3 position, float3 particleBoundBox)
{
	return (int) (position.x + position.y * particleBoundBox.x + position.z * particleBoundBox.x * particleBoundBox.y);
}