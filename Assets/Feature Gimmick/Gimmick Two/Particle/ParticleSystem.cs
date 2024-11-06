using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Bed.Gimmick
{
    [StructLayout(LayoutKind.Sequential, Size = 28)]
    public struct Particle                  // TODO: 쉐이더 내 구조체와 동일하게 맞춰줘야함
    {
        public int id;
        public Vector3 position;
        public Vector4 color;
    }
    
    public class ParticleSystem : MonoBehaviour
    {
        [Header("파티클 생성 옵션")]
        public float particleRadius = 0.1f;     // 파티클의 반지름
        public Vector3 particleSpawnPoint;      // 파티클이 생성되는 위치
        public Vector3Int particleNumOfSpawn;   // 파티클이 생성되는 개수 3차원으로 생성
        public Vector3 particleBoundBox;        // 파티클을 가두는 박스
        public float mass = 1f;                 // 파티클의 질량
        public float viscosity = 1f;            // 파티클의 점성
        public float pressure = 1f;             // 파티클의 압력
        public float restDensity = 1f;          // 파티클의 정지 밀도
        public float gasConstant = 1f;          // 파티클의 가스 상수
        public float damping = 1f;              // 파티클의 감쇠

        [Header("파티클 생성에 필요한 것")]
        [SerializeField] private Mesh particleMesh;
        [SerializeField] private Material particleMaterial;
        [SerializeField] private ComputeShader particleCompute;
        [SerializeField] private ParticleSpawner spawner;

        [Header("정렬")] 
        [SerializeField] private ComputeShader oneSweep;
        private Sort sorter;
        
        private Particle[] particles;
        private Vector3[] velocities;
        private Vector2Int[] cellLists;
        private uint[] cellStarts;
        private int ParticleCount => particleNumOfSpawn.x * particleNumOfSpawn.y * particleNumOfSpawn.z;
        private int CellCount => Mathf.FloorToInt((particleBoundBox.x / particleRadius) * (particleBoundBox.y / particleRadius) * (particleBoundBox.z / particleRadius));
        
        private ComputeBuffer particleBuffer;
        private ComputeBuffer velocityBuffer;
        private ComputeBuffer cellListBuffer;
        private ComputeBuffer cellStartBuffer;
        private GraphicsBuffer graphicsBuffer;
        private const int commandCount = 1;

        private RenderParams renderParams;
        
        private int integrateKernel;
        private int insertParticleKernel;
        private int insertCellStartKernel;

        private void Awake()
        {
            if (particleNumOfSpawn == Vector3Int.zero || particleBoundBox == Vector3.zero)
            {
                throw new Exception("particleNumOfSpawn과 particleBoundBox를 설정해주세요");
            }
            
            velocities = new Vector3[ParticleCount];
            cellLists = new Vector2Int[CellCount];
            cellStarts = new uint[CellCount];
            sorter = new Sort(oneSweep, CellCount);
            
            renderParams = new RenderParams()
            {
                material = particleMaterial,
                matProps = new MaterialPropertyBlock(),
                worldBounds = new Bounds(Vector3.zero, Vector3.one * 1000f),
            };
        }

        private void Start()
        {
            spawner.SpawnParticles(particleNumOfSpawn, particleSpawnPoint, particleRadius, out particles);
            
            InitKernel();
            InitBuffers();
            InitComputeShader();

            particleMaterial.SetBuffer("_particles", particleBuffer);
            particleMaterial.SetFloat("_particleRadius", particleRadius);

            particleCompute.Dispatch(insertParticleKernel, 128 * (128 / CellCount + 1), 1, 1);
            particleCompute.Dispatch(integrateKernel, 128 * (128 / ParticleCount + 1), 1, 1);
            cellListBuffer.GetData(cellLists);

            sorter.ExecutePairsSort(cellLists);
            cellLists = sorter.GetSortedPairs();
            
            cellListBuffer.SetData(cellLists);
            particleCompute.Dispatch(insertCellStartKernel, 128 * (128 / CellCount + 1), 1, 1);
            cellStartBuffer.GetData(cellStarts);

            // for (int i = 0; i < cellLists.Length; i++)
            // {
            //     Debug.Log(cellLists[i]);
            // }
        }

        private void Update()
        {
            Graphics.RenderMeshIndirect(renderParams, particleMesh, graphicsBuffer, commandCount);
        }
        
        private void InitKernel()
        {
            insertParticleKernel = particleCompute.FindKernel("InsertParticlesInCell");
            insertCellStartKernel = particleCompute.FindKernel("InsertCellStartByCellId");
            integrateKernel = particleCompute.FindKernel("Integrate");
        }
        
        private void InitBuffers()
        {
            GraphicsBuffer.IndirectDrawIndexedArgs[] commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[commandCount];
            commandData[0].indexCountPerInstance = particleMesh.GetIndexCount(0);
            commandData[0].startIndex = particleMesh.GetIndexStart(0);
            commandData[0].baseVertexIndex = particleMesh.GetBaseVertex(0);
            commandData[0].instanceCount = (uint) ParticleCount;
            
            particleBuffer = new ComputeBuffer(ParticleCount, Marshal.SizeOf(typeof(Particle)));
            particleBuffer.SetData(particles);
            
            velocityBuffer = new ComputeBuffer(ParticleCount, 12);      // Vector3의 크기는 4 * 3
            velocityBuffer.SetData(velocities);
            
            cellListBuffer = new ComputeBuffer(CellCount, 8);           // Vector2Int의 크기는 4 * 2
            cellListBuffer.SetData(cellLists);
            
            cellStartBuffer = new ComputeBuffer(CellCount, 4);          // int의 크기는 4
            cellStartBuffer.SetData(cellStarts);
            
            graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
            graphicsBuffer.SetData(commandData);
        }
        
        private void InitComputeShader()
        {
            particleCompute.SetBuffer(integrateKernel, "_particles", particleBuffer);
            particleCompute.SetBuffer(integrateKernel, "_velocities", velocityBuffer);
            
            particleCompute.SetBuffer(insertParticleKernel, "_particles", particleBuffer);
            particleCompute.SetBuffer(insertParticleKernel, "_cellLists", cellListBuffer);
            
            particleCompute.SetBuffer(insertCellStartKernel, "_cellLists", cellListBuffer);
            particleCompute.SetBuffer(insertCellStartKernel, "_cellStartIndices", cellStartBuffer);
            
            particleCompute.SetFloats("_boundingBox", particleBoundBox.x, particleBoundBox.y, particleBoundBox.z);
            particleCompute.SetFloat("_particleRadius", particleRadius);
            particleCompute.SetFloat("_damping", damping);
            particleCompute.SetFloat("_time", Time.deltaTime);
            particleCompute.SetInt("_particleCount", ParticleCount);
        }

        private void OnDrawGizmos()
        {
            float x, y, z;
            
            for (float i = 0; i < particleBoundBox.x; i++)
            for (float j = 0; j < particleBoundBox.y; j++)
            for (float k = 0; k < particleBoundBox.z; k++)
            {
                x = ((i * particleRadius * 2f) - particleNumOfSpawn.x) + particleSpawnPoint.x;
                y = ((j * particleRadius * 2f) - particleNumOfSpawn.y) + particleSpawnPoint.y;
                z = ((k * particleRadius * 2f) - particleNumOfSpawn.z) + particleSpawnPoint.z;
                
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(
                    new Vector3(x, y, z),
                    new Vector3(particleRadius * 2f, particleRadius * 2f, particleRadius * 2f));
            }
            
        }

        private void OnDestroy()
        {
            sorter.Dispose();
            graphicsBuffer?.Release();
            particleBuffer?.Release();
            velocityBuffer?.Release();
            cellListBuffer?.Release();
            graphicsBuffer = null;
            particleBuffer = null;
            velocityBuffer = null;
            cellListBuffer = null;
        }

    }


}
