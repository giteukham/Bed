// r is the distance between two particles
// h is the radius of the particle
#define PI 3.14159265359
inline float CalculatePoly6Kernel(float3 r, float h)
{
    float h9 = pow(h, 9);
	return 315.0f / (64.0f * PI * h9) * pow(h * h - dot(r, r), 3);
}

inline float CalculateSpikyKernel(float3 r, float h)
{
	float h6 = pow(h, 6);

	return -45.0f / (PI * h6) * pow(h - r, 2);
}

inline float CalculateLaplacianKernel(float3 r, float h)
{
	float h2 = pow(h, 2);
	float h3 = pow(h, 3);
	float r2 = dot(r, r);
	float r3 = r2 * sqrt(r2);
	
	return -(r3 / (2 * h3)) + (r2 / h2) + (h / (2 * r)) - 1;
}