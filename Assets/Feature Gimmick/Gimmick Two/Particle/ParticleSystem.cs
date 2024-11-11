using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Bed.Gimmick
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Particle                  // TODO: ���̴� �� ����ü�� �����ϰ� ���������
    {
        public int id;
        public Vector3 position;
        public Vector4 color;
    }
    
    public class ParticleSystem : MonoBehaviour
    {
        [Header("��ƼŬ ���� �ɼ�")]
        public float particleRadius = 0.1f;     // ��ƼŬ�� ������
        public Vector3 particleSpawnPoint;      // ��ƼŬ�� �����Ǵ� ��ġ
        public Vector3Int particleNumOfSpawn;   // ��ƼŬ�� �����Ǵ� ���� 3�������� ����
        public Vector3 particleBoundBox;        // ��ƼŬ�� ���δ� �ڽ�
        public float mass = 1f;                 // ��ƼŬ�� ����
        public float viscosity = 0.018f;        // ��ƼŬ�� ����
        public float pressure = 1f;             // ��ƼŬ�� �з�
        public float restDensity = 1f;          // ��ƼŬ�� ���� �е�
        public float gasConstant = 250f;        // ��ƼŬ�� ���� ���
        public float damping = 1f;              // ��ƼŬ�� ����

        [Header("��ƼŬ ������ �ʿ��� ��")]
        [SerializeField] private Mesh particleMesh;
        [SerializeField] private Material particleMaterial;
        [SerializeField] private ComputeShader particleCompute;
        [SerializeField] private ParticleSpawner spawner;

        [Header("����")] 
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
        private int CellCount => Mathf.FloorToInt(particleBoundBox.x * particleBoundBox.y * particleBoundBox.z);
        
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
            
            cellLists = new Vector2Int[CellCount];
            cellStarts = new uint[CellCount];
            neighbors = new uint[CellCount * 8];
            
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
            
            viscosityBuffer = new ComputeBuffer(ParticleCount, 4 * 3);                      // Vector3�� ũ��� 4 * 3
            viscosityBuffer.SetData(viscosities);
            
            velocitiesBuffer = new ComputeBuffer(ParticleCount, 4 * 3);                      // Vector3�� ũ��� 4 * 3
            velocitiesBuffer.SetData(velocities);
            
            densitiesBuffer = new ComputeBuffer(ParticleCount, 4);                      // float�� ũ��� 4
            densitiesBuffer.SetData(densities);
            
            pressuresBuffer = new ComputeBuffer(ParticleCount, 4);                      // float�� ũ��� 4
            pressuresBuffer.SetData(pressures);
            
            forcesBuffer = new ComputeBuffer(ParticleCount, 4 * 3);         // Vector3�� ũ��� 4 * 3
            forcesBuffer.SetData(forces);
            
            cellListBuffer = new ComputeBuffer(CellCount, 8);                           // Vector2Int�� ũ��� 4 * 2
            cellListBuffer.SetData(cellLists);
            
            cellStartBuffer = new ComputeBuffer(CellCount, 4);                          // int�� ũ��� 4
            cellStartBuffer.SetData(cellStarts);
            
            neighborsBuffer = new ComputeBuffer(CellCount * 8, 4);                 // �� ĭ �� 8���� Cell �̿��� ���� �� �ְ� �ű⿡ ��ü Cell�� ������ �����ָ� �� �̿��� ������ ����
            neighborsBuffer.SetData(neighbors);
            
            testBuffer = new ComputeBuffer(256, 4 * 3);
            testBuffer.SetData(testArray);
            
            graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
            graphicsBuffer.SetData(commandData);
        }
        
        private void InitComputeShader()
        {
            particleCompute.SetFloats("_boundingBox", particleBoundBox.x, particleBoundBox.y, particleBoundBox.z);
            particleCompute.SetFloat("_particleRadius", particleRadius);
            particleCompute.SetFloat("_damping", damping);
            particleCompute.SetInt("_particleCount", ParticleCount);
            
            particleCompute.SetFloat("_mass", mass);
            particleCompute.SetFloat("_restDensity", restDensity);
            particleCompute.SetFloat("_viscosity", viscosity);
            particleCompute.SetFloat("_gasConstant", gasConstant);
            
            particleCompute.SetBuffer(insertParticleKernel, "_particles", particleBuffer);
            particleCompute.SetBuffer(insertParticleKernel, "_cellLists", cellListBuffer);
            particleCompute.SetBuffer(insertParticleKernel, "_test", testBuffer);
            
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
            if (particleNumOfSpawn == Vector3Int.zero || particleBoundBox == Vector3.zero)
            {
                throw new Exception("particleNumOfSpawn�� particleBoundBox�� �������ּ���");
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
        }
        
        private void Update()
        {
            InitComputeShader();

            particleMaterial.SetBuffer("_particles", particleBuffer);
            particleMaterial.SetFloat("_particleRadius", particleRadius);
            
            particleCompute.Dispatch(insertParticleKernel, 128 * (128 / CellCount + 1), 1, 1);
            cellListBuffer.GetData(cellLists);

            sorter.ExecutePairsSort(cellLists);
            cellLists = sorter.GetSortedPairs();
            
            cellListBuffer.SetData(cellLists);
            particleCompute.Dispatch(insertCellStartKernel, 128 * (128 / CellCount + 1), 1, 1);
            cellStartBuffer.GetData(cellStarts);
            
            particleCompute.Dispatch(neighborSearchKernel, 128 * (128 / ParticleCount + 1), 1, 1);
            
            particleCompute.Dispatch(densityAndPressuresKernel, 128 * (128 / CellCount + 1), 1, 1);
            
            particleCompute.Dispatch(forcesKernel, 128 * (128 / ParticleCount + 1), 1, 1);
            
            particleCompute.Dispatch(integrateKernel, 128 * (128 / ParticleCount + 1), 1, 1);
            
            Graphics.RenderMeshIndirect(renderParams, particleMesh, graphicsBuffer, commandCount);
            
            testBuffer.GetData(testArray);
            Debug.Log(testArray[0]);
            // for (int i = 0; i < testArray.Length; i++)
            // {
            //     Debug.Log(testArray[i]);
            // }
        }

        private void OnDrawGizmos()
        {
            float x, y, z;
            
            for (float i = 0; i < particleBoundBox.x; i++)
            for (float j = 0; j < particleBoundBox.y; j++)
            for (float k = 0; k < particleBoundBox.z; k++)
            {
                x = (i * particleRadius * 2f) + particleSpawnPoint.x;
                y = (j * particleRadius * 2f) + particleSpawnPoint.y;
                z = (k * particleRadius * 2f) + particleSpawnPoint.z;
                
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
