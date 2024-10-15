#pragma kernel GlobalHistogram

#define THREAD_SIZE 256

StructuredBuffer<uint2> _inputBuffer;
RWStructuredBuffer<uint2> _outputBuffer;

groupshared uint2 _hist[THREAD_SIZE];

[numthreads(THREAD_SIZE, 1, 1)]
void GlobalHistogram(uint3 gtid : SV_GroupThreadID, uint gid : SV_GroupID)
{
	_hist[gtid.x] = _inputBuffer[gtid.x + gid.x * THREAD_SIZE];
	GroupMemoryBarrierWithGroupSync();
}

