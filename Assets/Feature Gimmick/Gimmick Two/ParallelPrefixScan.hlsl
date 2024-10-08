#define THREAD_SIZE 256

[numthreads(THREAD_SIZE, 1, 1)]
void PrefixScan (uint3 tid : SV_DisPatchThreadID)
{
	
}