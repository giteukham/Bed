using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
namespace Bed.Gimmick
{
    public class ParticleSystem : MonoBehaviour
    {
        public int particleCount = 1000;
        public float particleDiameter = 0.1f;
        public Vector3 particlesSpawnBox = new Vector3(1, 1, 1);
        public Vector3 boundingBox = new Vector3(1, 1, 1);

        [SerializeField] private Mesh particleMesh;
        [SerializeField] private Material particleMaterial;
        
        [SerializeField] private ComputeShader computeShader;

        private ComputeBuffer velocityBuffer;
        private GraphicsBuffer graphicsBuffer;
        private GraphicsBuffer positionBuffer;
        private static int commandCount = 1;
        
        private Vector3[] velocities;
        private RenderParams renderParams;
        private GraphicsBuffer.IndirectDrawIndexedArgs[] commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[commandCount];
        
        private static int positionPropertyID = Shader.PropertyToID("_Positions");

        private float3 myPosition;
        
        private void OnEnable()
        {
            velocityBuffer = new ComputeBuffer(particleCount, 3 * sizeof(float));
            positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 1, 3 * sizeof(float));
            graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, particleCount, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        }

        private void Start()
        {
            renderParams = new RenderParams()
            {
                material = particleMaterial,
                matProps = new MaterialPropertyBlock(),
                worldBounds = new Bounds(transform.position, boundingBox)
            };
            positionBuffer.SetData(new Vector3[] {new Vector3(10,10,10)});
            renderParams.matProps.SetBuffer(positionPropertyID, positionBuffer);
            
            commandData[0].indexCountPerInstance = particleMesh.GetIndexCount(0);
            commandData[0].startIndex = particleMesh.GetIndexStart(0);
            commandData[0].baseVertexIndex = particleMesh.GetBaseVertex(0);
            commandData[0].instanceCount = 1;
            graphicsBuffer.SetData(commandData);
            if (Vector3.Distance(particlesSpawnBox, boundingBox) < 0)
            {
                Debug.LogError("Spawn box 보다 bouding Box가 더 커야합니다.");
                return;
            }
            // velocities = new Vector3[particleCount];
            // velocityBuffer.SetData(velocities);
            // computeShader.SetBuffer(0, "_Velocity", velocityBuffer);
            // computeShader.SetFloat("deltaTime", Time.deltaTime);
        }

        private void Update()
        {
            //computeShader.Dispatch(0, particleCount / 8, 1, 1);
            Graphics.RenderMeshIndirect(renderParams, particleMesh, graphicsBuffer, commandCount);
        }

        private void SpawnParticles(GameObject particlePrefab)
        {
            for (int i = 0; i < particlesSpawnBox.x; i++)
            for (int j = 0; j < particlesSpawnBox.y; j++)
            for (int k = 0; k < particlesSpawnBox.z; k++)
            {
                var tmpObj = Instantiate(particlePrefab, new Vector3(i * particleDiameter, j * particleDiameter, k * particleDiameter), Quaternion.identity);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, boundingBox);
        }

        private void OnDisable()
        {
            velocityBuffer?.Release();
            graphicsBuffer?.Release();
            velocityBuffer = null;
            graphicsBuffer = null;
        }

    }


}
