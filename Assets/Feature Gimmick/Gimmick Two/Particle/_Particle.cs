using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class _Particle : MonoBehaviour
{
    [SerializeField] private _Particle neighborParticle;
    private float _currentDensity;
    private float _currentPressure;
    private Vector3 _currentForce;
    private Vector3 _currentVelocity;
    
    private readonly float PI = 3.14159265358979323846f;
    [FormerlySerializedAs("particleSize")] public float particleRadius = 1f;
    public float restDensity = 1f;
    public float pressure = 0.5f;
    public float mass = 1f;
    public float gasConstant = 250f;
    public float damping = 1f;
    public float smoothingLength = 2f;
    
    [SerializeField] private TextMesh _textMesh;

    private void Update()
    {
        CalculateDensityAndPressure();
        CalculateForces();
        
        _textMesh.text = $"Density: {_currentDensity}\nPressure: {_currentPressure} \nForce: {_currentForce}";
    }

    private void CalculateDensityAndPressure()
    {
        float poly6 = CalculatePoly6Kernel(smoothingLength);
        float rho = 0.0f;
        Vector3 diff = transform.position - neighborParticle.transform.position;
        float r2 = Vector3.Dot(diff, diff);
        float r = Mathf.Sqrt(r2); 
        float h = smoothingLength;
        float h2 = h * h;
        Debug.Log(r + " " + h);
        if (r < h)
        {
            rho += poly6 * Mathf.Pow(h2 - r2, 3);
        }
        
        _currentDensity = rho * mass + 0.01f;
        _currentDensity = Mathf.Max(_currentDensity, restDensity);
        _currentPressure = gasConstant * (_currentDensity - restDensity);
    }

    private void CalculateForces()
    {
        //
        // float spiky = CalculateSpikyKernel(smoothingLength, particleRadius); 
        // float lapacian = CalculateLaplacianKernel(smoothingLength);
        // _currentForce = Vector3.zero;
        //
        // Vector3 diff = transform.position - neighborParticle.transform.position;
        // float r2 = Vector3.Dot(diff, diff);
        // float r = Mathf.Sqrt(r2);
        // float r3 = r2 * r;
        // float h = smoothingLength;
        // float h2 = h * h;
        // float h3 = h2 * h;
		      //
        // if (r > 0.0 && r < h)
        // {
        //     Vector3 dir = diff / r;
        //     float sp = -r * spiky * Mathf.Pow(h - r, 2);
        //     float la = ((-r3 / (2 * h3)) + (r2 / h2) + (h / (2 * r)) - 1);
        //     _currentForce -= mass * (_currentPressure + neighborParticle._currentPressure) / (2 * neighborParticle._currentDensity) * sp * dir;
        //     _currentForce += mass * (neighborParticle._currentVelocity - _currentVelocity) / neighborParticle._currentDensity * la * dir;
        // }
        //
        // _currentForce *= 0.018f;
    }
    
    private float CalculatePoly6Kernel(float h)
    {
        float h9 = Mathf.Pow(Mathf.Abs(h), 9);
        return 315 / (64 * PI * h9);
    }

    private float CalculateSpikyKernel(float h, float r)
    {
        float h6 = Mathf.Pow(h, 6);

        return 45 / (PI * h6 * r);
    }

    private float CalculateLaplacianKernel(float h)
    {
        float h3 = Mathf.Pow(h, 3);
	
        return 15 / (2 * PI * h3);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, smoothingLength);
    }
}
