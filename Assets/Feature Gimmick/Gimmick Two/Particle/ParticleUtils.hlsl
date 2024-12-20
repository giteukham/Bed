// r is the distance between two particles
// h is the radius of the particle
#define PI 3.14159265359

float Poly6Kernel(float h, float r)
{
	if (r >= h)
	{
		return 0;
	}
	float h9 = pow(h, 9);
	float v = h * h - r * r;
	return 315 / (64 * PI * h9) * pow(v, 3);
}

float SpikyKernel(float h, float r)
{
	if (r >= h)
	{
		return 0;
	}
	
	float h6 = pow(h, 6);

	return 15 / (PI * h6) * pow(h - r, 3);
}

float DerivativeSpikyKernel(float h, float r)
{
	if (r >= h)
	{
		return 0;
	}
	
	float h6 = pow(h, 6);

	return (-45 / (PI * h6 * r)) * pow(h - r, 2);
}

float ViscosityKernel(float h, float r)
{
	if (r >= h)
	{
		return 0;
	}
	
	float h3 = pow(h, 3);
	float h2 = pow(h, 2);
	float r2 = pow(r, 2);
	float r3 = pow(r, 3);
	float v = (-r3 / (2 * h3) + (r2 / h2) + h / (2 * r) - 1);
	
	return 15 / (2 * PI * h3) * v;
}