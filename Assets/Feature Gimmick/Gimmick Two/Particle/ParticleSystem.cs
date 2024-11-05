using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Bed.Gimmick
{
    [StructLayout(LayoutKind.Sequential, Size = 28)]
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
        public float viscosity = 1f;            // ��ƼŬ�� ����
        public float pressure = 1f;             // ��ƼŬ�� �з�
        public float restDensity = 1f;          // ��ƼŬ�� ���� �е�
        public float gasConstant = 1f;          // ��ƼŬ�� ���� ���
        public float damping = 1f;              // ��ƼŬ�� ����

        [Header("��ƼŬ ������ �ʿ��� ��")]
        [SerializeField] private Mesh particleMesh;
        [SerializeField] private Material particleMaterial;
        [SerializeField] private ComputeShader particleCompute;
        
        private Particle[] particles;
        private Vector3[] velocities;
        private Vector2Int[] cellLists;
        private int particleCount => particleNumOfSpawn.x * particleNumOfSpawn.y * particleNumOfSpawn.z;
        private int cellCount => Mathf.FloorToInt((particleBoundBox.x / particleRadius) * (particleBoundBox.y / particleRadius) * (particleBoundBox.z / particleRadius));
        
        private ComputeBuffer particleBuffer;
        private ComputeBuffer velocityBuffer;
        private ComputeBuffer cellListBuffer;
        private GraphicsBuffer graphicsBuffer;
        private const int commandCount = 1;

        private RenderParams renderParams;
        
        private int integrateKernel;
        private int updateHashKernel;

        private void Awake()
        {
            if (particleNumOfSpawn == Vector3Int.zero || particleBoundBox == Vector3.zero)
            {
                throw new Exception("particleNumOfSpawn�� particleBoundBox�� �������ּ���");
            }
            
            velocities = new Vector3[particleCount];
            cellLists = new Vector2Int[cellCount];
        }

        private void OnEnable()
        {
            SpawnParticles();
        }

        private void Start()
        {
            renderParams = new RenderParams()
            {
                material = particleMaterial,
                matProps = new MaterialPropertyBlock(),
                worldBounds = new Bounds(Vector3.zero, Vector3.one * 1000f),
            };
            InitKernel();
            InitBuffers();
            InitComputeShader();

            particleMaterial.SetBuffer("_particles", particleBuffer);
            particleMaterial.SetFloat("_particleRadius", particleRadius);

            particleCompute.Dispatch(updateHashKernel, 128 * (128 / cellCount + 1), 1, 1);
            particleCompute.Dispatch(integrateKernel, 128 * (128 / particleCount + 1), 1, 1);
            cellListBuffer.GetData(cellLists);

            Sort.Instance.ExecutePairsSort(cellLists);

            cellLists = Sort.Instance.GetSortedPairs();


        }

        private void Update()
        {
            Graphics.RenderMeshIndirect(renderParams, particleMesh, graphicsBuffer, commandCount);


            
        }

        private void InitKernel()
        {
            updateHashKernel = particleCompute.FindKernel("UpdateHash");
            integrateKernel = particleCompute.FindKernel("Integrate");
        }
        
        /// <summary>
        /// �ڽ��� �߽��� �ٽ� �ڽ��� ��ǥ�� ���� �ڽ��� �������� �������ν� Center�� �߽����� �¿�� �ڽ��� �׸��� ��
        /// </summary>
        private void SpawnParticles()
        {
            float x, y, z;
            int index = 0;
            List<Particle> particleList = new List<Particle>();
            
            particleSpawnPoint = new Vector3()
            {
                x = particleSpawnPoint.x - (particleNumOfSpawn.x * particleRadius),
                y = particleSpawnPoint.y - (particleNumOfSpawn.y * particleRadius),
                z = particleSpawnPoint.z - (particleNumOfSpawn.z * particleRadius)
            };
            //particleSpawnPoint += Random.onUnitSphere * particleRadius * 0.2f;          // �� ǥ�鿡 �� ���� �����ؼ� �� ���� �߽����� 0.2f ��ŭ ������ ���� ����
            
            for (int i = 0; i < particleNumOfSpawn.x; i++)
            for (int j = 0; j < particleNumOfSpawn.y; j++)
            for (int k = 0; k < particleNumOfSpawn.z; k++)
            {
                x = particleSpawnPoint.x + (i * particleRadius * 2f);
                y = particleSpawnPoint.y + (j * particleRadius * 2f);
                z = particleSpawnPoint.z + (k * particleRadius * 2f);
                particleList.Add(new Particle()
                {
                    id = index,
                    position = new Vector3(x, y, z)
                });
                index++;
            }
            particles = particleList.ToArray();
        }
        
        private void InitBuffers()
        {
            GraphicsBuffer.IndirectDrawIndexedArgs[] commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[commandCount];
            commandData[0].indexCountPerInstance = particleMesh.GetIndexCount(0);
            commandData[0].startIndex = particleMesh.GetIndexStart(0);
            commandData[0].baseVertexIndex = particleMesh.GetBaseVertex(0);
            commandData[0].instanceCount = (uint) particleCount;
            
            particleBuffer = new ComputeBuffer(particleCount, Marshal.SizeOf(typeof(Particle)));
            particleBuffer.SetData(particles);
            
            velocityBuffer = new ComputeBuffer(particleCount, 12);      // Vector3�� ũ��� 4 * 3
            velocityBuffer.SetData(velocities);
            
            cellListBuffer = new ComputeBuffer(cellCount, 8);           // Vector2Int�� ũ��� 4 * 2
            cellListBuffer.SetData(cellLists);
            
            graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
            graphicsBuffer.SetData(commandData);
        }
        
        private void InitComputeShader()
        {
            particleCompute.SetBuffer(integrateKernel, "_particles", particleBuffer);
            particleCompute.SetBuffer(integrateKernel, "_velocities", velocityBuffer);
            
            particleCompute.SetBuffer(updateHashKernel, "_particles", particleBuffer);
            particleCompute.SetBuffer(updateHashKernel, "_cellLists", cellListBuffer);
            
            particleCompute.SetFloats("_boundingBox", particleBoundBox.x, particleBoundBox.y, particleBoundBox.z);
            particleCompute.SetFloat("_particleRadius", particleRadius);
            particleCompute.SetFloat("_damping", damping);
            particleCompute.SetFloat("_time", Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            float halfX = particleNumOfSpawn.x / 2f;
            float halfY = particleNumOfSpawn.y / 2f;
            float halfZ = particleNumOfSpawn.z / 2f;
            
            for (float i = 0; i < particleNumOfSpawn.x; i++)
            for (float j = 0; j < particleNumOfSpawn.y; j++)
            for (float k = 0; k < particleNumOfSpawn.z; k++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(
                    new Vector3(i - halfX, j - halfY, k - halfZ), 
                    new Vector3(particleNumOfSpawn.x, particleNumOfSpawn.y, particleNumOfSpawn.z));
            }
        }

        private void OnDestroy()
        {
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
