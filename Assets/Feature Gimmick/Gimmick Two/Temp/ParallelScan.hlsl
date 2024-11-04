#define THREAD_SIZE 256

#define NUM_BANKS 32
#define LOG_NUM_BANKS 5
#define ZERO_BANK_CONFLICTS

#ifdef ZERO_BANK_CONFLICTS
#define CONFLICT_FREE_OFFSET(n) ( ((n) >> LOG_NUM_BANKS) + ((n) >> (2 * LOG_NUM_BANKS)) )
#else
#define CONFLICT_FREE_OFFSET(n) ((n) >> LOG_NUM_BANKS)
#endif

RWStructuredBuffer<uint> _groupSumsBuffer : register(u0);

groupshared uint _temp[THREAD_SIZE * 2];

// https://developer.nvidia.com/gpugems/gpugems3/part-vi-gpu-computing/chapter-39-parallel-prefix-sum-scan-cuda
void LocalBlockScan(uint groupThreadID, uint groupID,
	RWStructuredBuffer<uint> input, RWStructuredBuffer<uint> output, uint numElements, RWStructuredBuffer<uint> test)
{
	uint idx = groupID * THREAD_SIZE + groupThreadID;
	
	int ai = groupThreadID;
	int bi = groupThreadID + THREAD_SIZE;
	
	int bankOffsetA = CONFLICT_FREE_OFFSET(ai);
	int bankOffsetB = CONFLICT_FREE_OFFSET(bi);
	
	_temp[ai + bankOffsetA] = groupThreadID < 8 ? input[groupThreadID] : 0u;
	_temp[bi + bankOffsetB] = groupThreadID + 8 < 8 * 2 ? input[groupThreadID + 8] : 0u;
	GroupMemoryBarrierWithGroupSync();
	if (groupThreadID < 8)
	{
		test[idx] = bankOffsetA;
	}
	// uint offset = 1u;
	//
	// for (uint d = 8 >> 1u; d > 0; d >>= 1u)
	// {
	// 	GroupMemoryBarrierWithGroupSync();
	// 	
	// 	if (groupThreadID < d)
	// 	{
	// 		uint ai = offset * ((groupThreadID << 1u) + 1u) - 1u;
	// 		uint bi = offset * ((groupThreadID << 1u) + 2u) - 1u;
	// 		ai += CONFLICT_FREE_OFFSET(ai);
	// 		bi += CONFLICT_FREE_OFFSET(bi);
	//
	// 		_temp[bi] += _temp[ai];
	// 	}
	// 	offset <<= 1u;
	// }
	// GroupMemoryBarrierWithGroupSync();
	//
	// if (groupThreadID == 0u)
	// {
	// 	_groupSumsBuffer[groupID] = _temp[8 - 1u + CONFLICT_FREE_OFFSET(8 - 1u)];
	// 	test[groupID * 256 + groupThreadID] = 8 - 1u + CONFLICT_FREE_OFFSET(8 - 1u);
	// 	_temp[8 - 1u + CONFLICT_FREE_OFFSET(8 - 1u)] = 0u;
	// }
	// GroupMemoryBarrierWithGroupSync();
	//
	// for (uint d = 1u; d < 8; d <<= 1u)
	// {
	// 	offset >>= 1u;
	// 	GroupMemoryBarrierWithGroupSync();
	//
	// 	if (groupThreadID < d)
	// 	{
	// 		uint ai = offset * ((groupThreadID << 1u) + 1u) - 1u;
	// 		uint bi = offset * ((groupThreadID << 1u) + 2u) - 1u;
	// 		ai += CONFLICT_FREE_OFFSET(ai);
	// 		bi += CONFLICT_FREE_OFFSET(bi);
	//
	// 		uint t = _temp[ai];
	// 		_temp[ai] = _temp[bi];
	// 		_temp[bi] += t;
	// 	}
	// }
	// GroupMemoryBarrierWithGroupSync();
	//
	// output[ai]	= groupThreadID < 8 ? _temp[groupThreadID] : 0u;
	// output[bi]	= groupThreadID + 8 < 16 ? _temp[groupThreadID + 8] : 0u;
	// GroupMemoryBarrierWithGroupSync();

}

void AddBlockSums(uint groupThreadID, uint groupID,
	RWStructuredBuffer<uint> output, uint numElements, RWStructuredBuffer<uint> test)
{
	// uint blockSum = _groupSumsBuffer[groupID];
	//
	// int idx = groupID * THREAD_SIZE + groupThreadID;
	//
	// if (idx < 8)
	// {
	// 	output[idx] += blockSum;
	// 	if (idx + 8 < 16)
	// 	{
	// 		output[idx + 8] += blockSum;
	// 	}
	// }
}
