#define __DEBUG__

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bed.Gimmick
{
   
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Particle                  // TODO: ���̴� �� ����ü�� �����ϰ� ���������
    {
        public Vector3 position;
        public Vector4 color;
    }

    public struct CellInfo
    {
        public int particleIndex;
        public int hash;
    }
    
    public class ParticleSystem : MonoBehaviour
    {
        private const int THREAD_SIZE = 256;
        
        [Header("��ƼŬ ���� �ɼ�")]
        public float particleRadius = 0.1f;     // ��ƼŬ�� ������
        public Vector3 particleSpawnPoint;      // ��ƼŬ�� �����Ǵ� ��ġ
        public Vector3Int particleNumOfSpawn;   // ��ƼŬ�� �����Ǵ� ���� 3�������� ����
        public Bounds boundBox = new Bounds(Vector3.zero, new Vector3(1, 1, 1));                // ��ƼŬ�� ���δ� �ڽ�
        
        [Tooltip("Cell�� ũ��. ���� ��ƼŬ ������ 2��� ����")]
        public float cellSize = 0.2f;           // �ڽ��� ũ�� ���� ��ƼŬ�� ������ 2��� ����
        
        [Tooltip("����. �⺻ ���� 1f")]
        public float mass = 1f;                 // ��ƼŬ�� ����
        
        [Tooltip("������. �⺻ ���� 0.018f")]
        public float viscosity = 0.018f;        // ��ƼŬ�� ����
        
        [Tooltip("���� �е�. �⺻ ���� 1f")]
        public float restDensity = 1f;          // ��ƼŬ�� ���� �е�
        
        [Tooltip("���� ���. �⺻ ���� 250f")]
        public float gasConstant = 250f;        // ��ƼŬ�� ���� ���
        
        [Tooltip("���� ��. �⺻ ���� 1f")]
        public float damping = 1f;              // ��ƼŬ�� ����
        
        [Tooltip("��ƼŬ�� ���� ������ ��ġ�� ����. CellSize���� �۰ų� ���ƾ� ��")]
        public float smoothingLength = 3f;     // ��ƼŬ�� ���� ������ ��ġ�� ����

        [Header("��ƼŬ ������ �ʿ��� ��")]
        [SerializeField] private Mesh particleMesh;
        [SerializeField] private Material particleMaterial;
        [SerializeField] private ComputeShader particleCompute;
        [SerializeField] private ParticleSpawner spawner;

        [Header("��Ÿ �ɼ�")] 
        [SerializeField] private bool isOnBoundBox;

        [Header("����")] 
        [SerializeField] private ComputeShader oneSweep;
        private Sort sorter;
        
        private Particle[] particles;
        private Vector3[] viscosities;
        private Vector3[] velocities;
        private float[] densities;
        private float[] pressures;
        private Vector3[] forces;
 
        private CellInfo[] cellIndices;
        private uint[] cellStarts, cellEnds;
        private uint[] neighbors;
        
        private int ParticleCount => particleNumOfSpawn.x * particleNumOfSpawn.y * particleNumOfSpawn.z;
        private int TotalCellNumbers => Mathf.CeilToInt((boundBox.size.x / cellSize) * (boundBox.size.y / cellSize) * (boundBox.size.z / cellSize));
        
        private ComputeBuffer particleBuffer;
        private ComputeBuffer viscosityBuffer;
        private ComputeBuffer velocitiesBuffer;
        private ComputeBuffer densitiesBuffer;
        private ComputeBuffer pressuresBuffer;
        private ComputeBuffer forcesBuffer;
        
        private ComputeBuffer cellIndicesBuffer;
        private ComputeBuffer cellStartsBuffer;
        private ComputeBuffer cellEndsBuffer;
        private ComputeBuffer neighborsBuffer;
        
        private GraphicsBuffer graphicsBuffer;
        private const int commandCount = 1;

        private RenderParams renderParams;
        
        private int integrateKernel;
        private int insertParticleKernel;
        private int updateCellStartEndsKernel;
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
            
            cellIndices = new CellInfo[ParticleCount];
            cellStarts = new uint[200000];
            cellEnds = new uint[200000];
            neighbors = new uint[TotalCellNumbers * 27];
            
            testArray = new Vector3[256];
        }
        
        private void InitKernel()
        {
            insertParticleKernel = particleCompute.FindKernel("InsertParticlesInCell");
            updateCellStartEndsKernel = particleCompute.FindKernel("UpdateCellStartEnds");
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
            
            viscosityBuffer = new ComputeBuffer(ParticleCount * ParticleCount, Marshal.SizeOf(typeof(Vector3)));                      // Vector3�� ũ��� 4 * 3
            viscosityBuffer.SetData(viscosities);
            
            velocitiesBuffer = new ComputeBuffer(ParticleCount * ParticleCount, Marshal.SizeOf(typeof(Vector3)));
            velocitiesBuffer.SetData(velocities);
            
            densitiesBuffer = new ComputeBuffer(ParticleCount * ParticleCount, sizeof(float));                      // float�� ũ��� 4
            densitiesBuffer.SetData(densities);
            
            pressuresBuffer = new ComputeBuffer(ParticleCount * ParticleCount, sizeof(float));
            pressuresBuffer.SetData(pressures);
            
            forcesBuffer = new ComputeBuffer(ParticleCount * ParticleCount, Marshal.SizeOf(typeof(Vector3)));                     // Vector3�� ũ��� 4 * 3
            forcesBuffer.SetData(forces);
            
            cellIndicesBuffer = new ComputeBuffer(ParticleCount, Marshal.SizeOf(typeof(CellInfo)));
            cellIndicesBuffer.SetData(cellIndices);
            
            cellStartsBuffer = new ComputeBuffer(TotalCellNumbers * ParticleCount, sizeof(int));
            cellStartsBuffer.SetData(cellStarts);
            
            cellEndsBuffer = new ComputeBuffer(TotalCellNumbers * ParticleCount, sizeof(int));
            cellEndsBuffer.SetData(cellEnds);
            
            neighborsBuffer = new ComputeBuffer(TotalCellNumbers * 27, sizeof(int));
            neighborsBuffer.SetData(neighbors);
            
            testBuffer = new ComputeBuffer(256, 4 * 3);
            testBuffer.SetData(testArray);
            
            graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
            graphicsBuffer.SetData(commandData);
        }
        
        private void InitComputeShader()
        {
            particleCompute.SetFloats("_boundingBox", boundBox.size.x / cellSize, boundBox.size.y / cellSize, boundBox.size.z / cellSize);
            particleCompute.SetFloats("_boundMin", boundBox.min.x, boundBox.min.y, boundBox.min.z);
            particleCompute.SetFloats("_boundMax", boundBox.max.x, boundBox.max.y, boundBox.max.z);
            particleCompute.SetFloat("_particleRadius", particleRadius);
            particleCompute.SetFloat("_damping", damping);
            particleCompute.SetFloat("_cellSize", cellSize);
            particleCompute.SetInt("_totalCellNumbers", TotalCellNumbers);
            particleCompute.SetInt("_particleCount", ParticleCount);
            
            particleCompute.SetFloat("_mass", mass);
            particleCompute.SetFloat("_restDensity", restDensity);
            particleCompute.SetFloat("_viscosity", viscosity);
            particleCompute.SetFloat("_gasConstant", gasConstant);
            particleCompute.SetFloat("_smoothingLength", smoothingLength);
            
            ComputeHelper.SetKernels(particleCompute, particleBuffer,       "_particles",   insertParticleKernel, updateCellStartEndsKernel, neighborSearchKernel, densityAndPressuresKernel, forcesKernel, integrateKernel);
            ComputeHelper.SetKernels(particleCompute, cellIndicesBuffer,    "_cellIndices", insertParticleKernel, updateCellStartEndsKernel, neighborSearchKernel);
            ComputeHelper.SetKernels(particleCompute, cellStartsBuffer,     "_cellStarts",  updateCellStartEndsKernel, neighborSearchKernel);
            ComputeHelper.SetKernels(particleCompute, cellEndsBuffer,       "_cellEnds",    updateCellStartEndsKernel, neighborSearchKernel);
            ComputeHelper.SetKernels(particleCompute, neighborsBuffer,      "_neighbors",   neighborSearchKernel, densityAndPressuresKernel, forcesKernel);
            ComputeHelper.SetKernels(particleCompute, densitiesBuffer,      "_densities",   densityAndPressuresKernel, forcesKernel, integrateKernel);
            ComputeHelper.SetKernels(particleCompute, pressuresBuffer,      "_pressures",   densityAndPressuresKernel, forcesKernel, integrateKernel);
            ComputeHelper.SetKernels(particleCompute, velocitiesBuffer,     "_velocities",  forcesKernel, integrateKernel);
            ComputeHelper.SetKernels(particleCompute, forcesBuffer,         "_forces",      forcesKernel, integrateKernel);
            ComputeHelper.SetKernels(particleCompute, testBuffer,           "_test",        insertParticleKernel, updateCellStartEndsKernel, neighborSearchKernel, densityAndPressuresKernel, forcesKernel, integrateKernel);
        }
        
        private void Awake()
        {
            if (particleNumOfSpawn == Vector3Int.zero || boundBox.size == Vector3.zero)
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
            InitComputeShader();
            
            particleMaterial.SetBuffer("_particles", particleBuffer);
            particleMaterial.SetFloat("_particleRadius", particleRadius);
            
            particleCompute.Dispatch(insertParticleKernel, Mathf.CeilToInt(ParticleCount), 1, 1);
            SortCellIndices();
            particleCompute.Dispatch(updateCellStartEndsKernel, Mathf.CeilToInt(ParticleCount), 1, 1);
            particleCompute.Dispatch(neighborSearchKernel, Mathf.CeilToInt(ParticleCount), 1, 1);
            particleCompute.Dispatch(densityAndPressuresKernel, Mathf.CeilToInt(ParticleCount), 1, 1);
            particleCompute.Dispatch(forcesKernel, Mathf.CeilToInt(ParticleCount), 1, 1);
            particleCompute.Dispatch(integrateKernel, Mathf.CeilToInt(ParticleCount), 1, 1);
            testBuffer.GetData(testArray);

            for (int i = 0; i < testArray.Length; i++)
            {
                Debug.Log(testArray[i]);
            }
        }
        
        private void Update()
        {

            Graphics.RenderMeshIndirect(renderParams, particleMesh, graphicsBuffer, commandCount);

            //Debug.Log((double)testArray[0].x + " " + (double)testArray[0].y + " " + (double)testArray[0].z);
        }
        
        private void SortCellIndices()
        {
            cellIndicesBuffer.GetData(cellIndices);
            
            int[] particleIndices = new int[cellIndices.Length];
            int[] hashes = new int[cellIndices.Length];
            
            for (int i = 0; i < cellIndices.Length; i++)
            {
                particleIndices[i] = cellIndices[i].particleIndex;
                hashes[i] = cellIndices[i].hash;
            }
            sorter.ExecutePairsSort(ref hashes, ref particleIndices);
            
            for (int i = 0; i < cellIndices.Length; i++)
            {
                cellIndices[i].particleIndex = particleIndices[i];
                cellIndices[i].hash = hashes[i];
            }
            
            cellIndicesBuffer.SetData(cellIndices);
        }
        
    #if __DEBUG__
        private void OnDrawGizmos()
        {
            if (isOnBoundBox) DrawBoxPerCell();
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
            neighborsBuffer?.Release();
            cellIndicesBuffer?.Release();
            cellStartsBuffer?.Release();
            cellEndsBuffer?.Release();
            testBuffer?.Release();
            
            graphicsBuffer = null;
            particleBuffer = null;
            viscosityBuffer = null;
            velocitiesBuffer = null;
            densitiesBuffer = null;
            pressuresBuffer = null;
            forcesBuffer = null;
            neighborsBuffer = null;
            cellIndicesBuffer = null;
            cellStartsBuffer = null;
            cellEndsBuffer = null;
            testBuffer = null;
        }

    }


}
