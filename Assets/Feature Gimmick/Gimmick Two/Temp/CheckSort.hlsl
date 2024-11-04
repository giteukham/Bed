#pragma kernel CheckSort

#define THREAD_SIZE 256

StructuredBuffer<uint> _inputBuffer;
RWStructuredBuffer<uint> _outputBuffer;

uint _elementCount;
groupshared int _checkedData[THREAD_SIZE];
groupshared uint _tempInput[THREAD_SIZE];
groupshared uint _sdata[THREAD_SIZE];

void WarpReduce(uint groupThreadID)
{
	if (THREAD_SIZE >= 64) _sdata[groupThreadID] += _sdata[groupThreadID + 32];
	if (THREAD_SIZE >= 32) _sdata[groupThreadID] += _sdata[groupThreadID + 16];
	if (THREAD_SIZE >= 16) _sdata[groupThreadID] += _sdata[groupThreadID + 8];
	if (THREAD_SIZE >= 8) _sdata[groupThreadID] += _sdata[groupThreadID + 4];
	if (THREAD_SIZE >= 4) _sdata[groupThreadID] += _sdata[groupThreadID + 2];
	if (THREAD_SIZE >= 2) _sdata[groupThreadID] += _sdata[groupThreadID + 1];
}

// https://developer.download.nvidia.com/assets/cuda/files/reduction.pdf
void Reduction(uint groupThreadID, uint groupID)
{
	uint index = groupID * (_elementCount * 2) + groupThreadID;
	uint gridSize = _elementCount * 2;

	_sdata[groupThreadID] = 0;
	
	while (index < _elementCount)
	{
		_sdata[groupThreadID] += _tempInput[index] + _tempInput[index + _elementCount];
		index += gridSize;
	}
	GroupMemoryBarrierWithGroupSync();

	if (THREAD_SIZE >= 512) { if (groupThreadID < 256) _sdata[groupThreadID] += _sdata[groupThreadID + 256]; GroupMemoryBarrierWithGroupSync(); }
	if (THREAD_SIZE >= 256) { if (groupThreadID < 128) _sdata[groupThreadID] += _sdata[groupThreadID + 128]; GroupMemoryBarrierWithGroupSync(); }
	if (THREAD_SIZE >= 128) { if (groupThreadID < 64) _sdata[groupThreadID] += _sdata[groupThreadID + 64]; GroupMemoryBarrierWithGroupSync(); }

	if (groupThreadID < 32) WarpReduce(groupThreadID);
	if (groupThreadID == 0) _checkedData[groupID] = _sdata[0];
	GroupMemoryBarrierWithGroupSync();
}

// gridDim == _elementCount or tid.x
// blockIdx == gid.x
// threadIdx == gtid.x
[numthreads(THREAD_SIZE, 1, 1)]
void CheckSort(uint3 tid : SV_DispatchThreadID, uint3 gtid : SV_GroupThreadID, uint3 gid : SV_GroupID)
{
	uint element = tid.x < _elementCount ? _inputBuffer[gtid.x] : 0;
	uint next = tid.x < _elementCount - 1 ? _inputBuffer[gtid.x + 1] : 0;

	_tempInput[gtid.x] = element > next && tid.x < _elementCount - 1 ? 1 : 0;
	GroupMemoryBarrierWithGroupSync();
	
	Reduction(gtid.x, gid.x);

	if (gtid.x == 0)
	{
		_outputBuffer[0] = _checkedData[gid.x] == 0;
	}
}
