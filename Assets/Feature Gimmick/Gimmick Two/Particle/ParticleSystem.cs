#define __DEBUG__

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bed.Gimmick
{
   
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Particle                  // TODO: 쉐이더 내 구조체와 동일하게 맞춰줘야함
    {
        public Vector3 position;
        public Vector4 color;
    }
    
    public class ParticleSystem : MonoBehaviour
    {
        private const int THREAD_SIZE = 256;
        
        [Header("파티클 생성 옵션")]
        public float particleRadius = 0.1f;     // 파티클의 반지름
        public Vector3 particleSpawnPoint;      // 파티클이 생성되는 위치
        public Vector3Int particleNumOfSpawn;   // 파티클이 생성되는 개수 3차원으로 생성
        public Bounds boundBox = new Bounds(Vector3.zero, new Vector3(1, 1, 1));                // 파티클을 가두는 박스
        public float cellSize = 0.2f;           // 박스의 크기 보통 파티클의 반지름의 2배로 설정
        
        public float mass = 1f;                 // 파티클의 질량
        public float viscosity = 0.018f;        // 파티클의 점성
        public float pressure = 1f;             // 파티클의 압력
        public float restDensity = 1f;          // 파티클의 정지 밀도
        public float gasConstant = 250f;        // 파티클의 가스 상수
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
        private Vector3[] viscosities;
        private Vector3[] velocities;
        private float[] densities;
        private float[] pressures;
        private Vector3[] forces;
 
        private Vector2Int[] cellLists;
        private uint[] cellStarts;
        private uint[] neighbors;
        
        private int ParticleCount => particleNumOfSpawn.x * particleNumOfSpawn.y * particleNumOfSpawn.z;
        private int CellNumbers => Mathf.CeilToInt(boundBox.size.x * boundBox.size.y * boundBox.size.z);
        
        private ComputeBuffer particleBuffer;
        private ComputeBuffer viscosityBuffer;
        private ComputeBuffer velocitiesBuffer;
        private ComputeBuffer densitiesBuffer;
        private ComputeBuffer pressuresBuffer;
        private ComputeBuffer forcesBuffer;
        
        private ComputeBuffer cellListBuffer;
        private ComputeBuffer cellStartBuffer;
        private ComputeBuffer neighborsBuffer;
        
        private GraphicsBuffer graphicsBuffer;
        private const int commandCount = 1;

        private RenderParams renderParams;
        
        private int integrateKernel;
        private int insertParticleKernel;
        private int insertCellStartKernel;
        private int neighborSearchKernel;
        private int densityAndPressuresKernel;
        private int forcesKernel;

        private ComputeBuffer testBuffer;
        private Vector3[] testArray;
        
        private void InitArrays()
        {
            particles = new Particle[ParticleCount];
            viscosities = new Vector3[ParticleCount];
            velocities = new Vector3[ParticleCount];
            densities = new float[ParticleCount];
            pressures = new float[ParticleCount];
            forces = new Vector3[ParticleCount];
            
            cellLists = new Vector2Int[CellNumbers * ParticleCount];
            cellStarts = new uint[200000];
            neighbors = new uint[CellNumbers * ParticleCount * 8];
            
            testArray = new Vector3[256];
        }
        
        private void InitKernel()
        {
            insertParticleKernel = particleCompute.FindKernel("InsertParticlesInCell");
            insertCellStartKernel = particleCompute.FindKernel("InsertCellStartByCellId");
            neighborSearchKernel = particleCompute.FindKernel("NeighborSearch");
            densityAndPressuresKernel = particleCompute.FindKernel("CalculateDensityAndPressures");
            forcesKernel = particleCompute.FindKernel("CalculateForces");
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
            
            viscosityBuffer = new ComputeBuffer(ParticleCount, 4 * 3);                      // Vector3의 크기는 4 * 3
            viscosityBuffer.SetData(viscosities);
            
            velocitiesBuffer = new ComputeBuffer(ParticleCount, 4 * 3);
            velocitiesBuffer.SetData(velocities);
            
            densitiesBuffer = new ComputeBuffer(ParticleCount, 4);                      // float의 크기는 4
            densitiesBuffer.SetData(densities);
            
            pressuresBuffer = new ComputeBuffer(ParticleCount, 4);
            pressuresBuffer.SetData(pressures);
            
            forcesBuffer = new ComputeBuffer(ParticleCount, 4 * 3);                     // Vector3의 크기는 4 * 3
            forcesBuffer.SetData(forces);
            
            cellListBuffer = new ComputeBuffer(CellNumbers * ParticleCount, 8);      // Vector2Int의 크기는 4 * 2
            cellListBuffer.SetData(cellLists);
            
            cellStartBuffer = new ComputeBuffer(200000, 4);
            cellStartBuffer.SetData(cellStarts);
            
            neighborsBuffer = new ComputeBuffer(CellNumbers * ParticleCount * 8, 4);                 // 한 칸 당 8개의 Cell 이웃을 가질 수 있고 거기에 전체 Cell의 개수를 곱해주면 총 이웃의 개수가 나옴
            neighborsBuffer.SetData(neighbors);
            
            testBuffer = new ComputeBuffer(256, 4 * 3);
            testBuffer.SetData(testArray);
            
            graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
            graphicsBuffer.SetData(commandData);
        }
        
        private void InitComputeShader()
        {
            particleCompute.SetFloats("_boundingBox", boundBox.size.x, boundBox.size.y, boundBox.size.z);
            particleCompute.SetFloat("_particleRadius", particleRadius);
            particleCompute.SetFloat("_damping", damping);
            particleCompute.SetFloat("_cellSize", cellSize);
            particleCompute.SetInt("_cellNumbers", CellNumbers);
            particleCompute.SetInt("_particleCount", ParticleCount);
            
            particleCompute.SetFloat("_mass", mass);
            particleCompute.SetFloat("_restDensity", restDensity);
            particleCompute.SetFloat("_viscosity", viscosity);
            particleCompute.SetFloat("_gasConstant", gasConstant);
            
            particleCompute.SetBuffer(insertParticleKernel, "_particles", particleBuffer);
            particleCompute.SetBuffer(insertParticleKernel, "_cellLists", cellListBuffer);
            particleCompute.SetBuffer(insertParticleKernel, "_test", testBuffer);
            
            particleCompute.SetBuffer(insertCellStartKernel, "_particles", particleBuffer);
            particleCompute.SetBuffer(insertCellStartKernel, "_cellLists", cellListBuffer);
            particleCompute.SetBuffer(insertCellStartKernel, "_cellStartIndices", cellStartBuffer);
            particleCompute.SetBuffer(insertCellStartKernel, "_test", testBuffer);
            
            particleCompute.SetBuffer(neighborSearchKernel, "_particles", particleBuffer);
            particleCompute.SetBuffer(neighborSearchKernel, "_cellStartIndices", cellStartBuffer);
            particleCompute.SetBuffer(neighborSearchKernel, "_cellLists", cellListBuffer);
            particleCompute.SetBuffer(neighborSearchKernel, "_neighbors", neighborsBuffer);
            particleCompute.SetBuffer(neighborSearchKernel, "_test", testBuffer);
            
            particleCompute.SetBuffer(densityAndPressuresKernel, "_particles", particleBuffer);
            particleCompute.SetBuffer(densityAndPressuresKernel, "_neighbors", neighborsBuffer);
            particleCompute.SetBuffer(densityAndPressuresKernel, "_densities", densitiesBuffer);
            particleCompute.SetBuffer(densityAndPressuresKernel, "_pressures", pressuresBuffer);
            particleCompute.SetBuffer(densityAndPressuresKernel, "_test", testBuffer);
            
            particleCompute.SetBuffer(forcesKernel, "_particles", particleBuffer);
            particleCompute.SetBuffer(forcesKernel, "_neighbors", neighborsBuffer);
            particleCompute.SetBuffer(forcesKernel, "_densities", densitiesBuffer);
            particleCompute.SetBuffer(forcesKernel, "_pressures", pressuresBuffer);
            particleCompute.SetBuffer(forcesKernel, "_viscosities", viscosityBuffer);
            particleCompute.SetBuffer(forcesKernel, "_velocities", velocitiesBuffer);
            particleCompute.SetBuffer(forcesKernel, "_forces", forcesBuffer);
            particleCompute.SetBuffer(forcesKernel, "_test", testBuffer);
            
            particleCompute.SetBuffer(integrateKernel, "_particles", particleBuffer);
            particleCompute.SetBuffer(integrateKernel, "_densities", densitiesBuffer);
            particleCompute.SetBuffer(integrateKernel, "_forces", forcesBuffer);
            particleCompute.SetBuffer(integrateKernel, "_velocities", velocitiesBuffer);
            particleCompute.SetBuffer(integrateKernel, "_test", testBuffer);
        }
        
        private void Awake()
        {
            if (particleNumOfSpawn == Vector3Int.zero || boundBox.size == Vector3.zero)
            {
                throw new Exception("particleNumOfSpawn과 particleBoundBox를 설정해주세요");
            }
            
            InitArrays();
            sorter = new Sort(oneSweep, ParticleCount);
            
            
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
            
            particleCompute.Dispatch(insertParticleKernel, Mathf.CeilToInt(ParticleCount * CellNumbers / THREAD_SIZE), 1, 1);
            cellListBuffer.GetData(cellLists);

            sorter.ExecutePairsSort(cellLists);
            cellLists = sorter.GetSortedPairs();
            
            cellListBuffer.SetData(cellLists);
            particleCompute.Dispatch(insertCellStartKernel, Mathf.CeilToInt(ParticleCount * CellNumbers / THREAD_SIZE), 1, 1);
            cellStartBuffer.GetData(cellStarts);
            
            particleCompute.Dispatch(neighborSearchKernel, Mathf.CeilToInt(ParticleCount * CellNumbers / THREAD_SIZE), 1, 1);
            
            particleCompute.Dispatch(densityAndPressuresKernel, Mathf.CeilToInt(ParticleCount * CellNumbers / THREAD_SIZE), 1, 1);
            
            particleCompute.Dispatch(forcesKernel, Mathf.CeilToInt(ParticleCount * CellNumbers / THREAD_SIZE), 1, 1);
            
            particleCompute.Dispatch(integrateKernel, Mathf.CeilToInt(ParticleCount * CellNumbers / THREAD_SIZE), 1, 1);


            
            testBuffer.GetData(testArray);
            //Debug.Log(testArray[0]);
            for (int i = 0; i < testArray.Length; i++)
            {
                Debug.Log(testArray[i]);
            }
        }
        
        private void Update()
        {

            Graphics.RenderMeshIndirect(renderParams, particleMesh, graphicsBuffer, commandCount);
        }
        
    #if __DEBUG__
        private void OnDrawGizmos()
        {
            DrawBoxPerCell();
        }

        private void DrawBoxPerCell()
        {
            Gizmos.color = Color.blue;
            Vector3 cellCoord = Vector3.zero;
            for (float i = 0; i < Mathf.CeilToInt(boundBox.size.x / cellSize); i++)
            for (float j = 0; j < Mathf.CeilToInt(boundBox.size.y / cellSize); j++)
            for (float k = 0; k < Mathf.CeilToInt(boundBox.size.z / cellSize); k++)
            {
                cellCoord.x = i * cellSize + boundBox.min.x;
                cellCoord.y = j * cellSize + boundBox.min.y;
                cellCoord.z = k * cellSize + boundBox.min.z;
                Gizmos.DrawWireCube(cellCoord + new Vector3(cellSize / 2, cellSize / 2, cellSize / 2), new Vector3(cellSize, cellSize, cellSize));
            }
        }
    #endif

        private void OnDestroy()
        {
            sorter.Dispose();
            graphicsBuffer?.Release();
            particleBuffer?.Release();
            viscosityBuffer?.Release();
            velocitiesBuffer?.Release();
            densitiesBuffer?.Release();
            pressuresBuffer?.Release();
            forcesBuffer?.Release();
            cellListBuffer?.Release();
            cellStartBuffer?.Release();
            testBuffer?.Release();
            
            graphicsBuffer = null;
            particleBuffer = null;
            viscosityBuffer = null;
            velocitiesBuffer = null;
            densitiesBuffer = null;
            pressuresBuffer = null;
            forcesBuffer = null;
            cellListBuffer = null;
            cellStartBuffer = null;
            testBuffer = null;
        }

    }


}
