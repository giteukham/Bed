using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bed.Gimmick
{
    public class ParticleSystem : MonoBehaviour
    {
        [StructLayout(LayoutKind.Sequential, Size = 28)]
        public struct Particle                  // TODO: ���̴� �� ����ü�� �����ϰ� ���������
        {
            public int id;
            public Vector3 position;
            public Vector4 color;
        }

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
        [SerializeField] private ComputeShader computeShader;
        
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
            velocities = new Vector3[particleCount];
            cellLists = new Vector2Int[cellCount];
        }

        private void OnEnable()
        {
            SpawnParticles();
            InitKernel();
        }

        private void Start()
        {
            renderParams = new RenderParams()
            {
                material = particleMaterial,
                matProps = new MaterialPropertyBlock(),
                worldBounds = new Bounds(Vector3.zero, Vector3.one * 1000f),
            };
            InitBuffers();
            InitComputeShader();
        }

        private void Update()
        {
            particleMaterial.SetBuffer("_particles", particleBuffer);
            particleMaterial.SetFloat("_particleRadius", particleRadius);

            computeShader.Dispatch(updateHashKernel, 128 * (128 / cellCount + 1), 1, 1);
            computeShader.Dispatch(integrateKernel, 128 * (128 / particleCount + 1), 1, 1);
            Graphics.RenderMeshIndirect(renderParams, particleMesh, graphicsBuffer, commandCount);
            
            cellListBuffer.GetData(cellLists);
            
        }

        private void InitKernel()
        {
            updateHashKernel = computeShader.FindKernel("UpdateHash");
            integrateKernel = computeShader.FindKernel("Integrate");
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
            particleSpawnPoint += Random.onUnitSphere * particleRadius * 0.2f;          // �� ǥ�鿡 �� ���� �����ؼ� �� ���� �߽����� 0.2f ��ŭ ������ ���� ����
            
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
            computeShader.SetBuffer(integrateKernel, "_particles", particleBuffer);
            computeShader.SetBuffer(integrateKernel, "_velocities", velocityBuffer);
            
            computeShader.SetBuffer(updateHashKernel, "_particles", particleBuffer);
            computeShader.SetBuffer(updateHashKernel, "_cellLists", cellListBuffer);
            
            computeShader.SetFloats("_boundingBox", particleBoundBox.x, particleBoundBox.y, particleBoundBox.z);
            computeShader.SetFloat("_particleRadius", particleRadius);
            computeShader.SetFloat("_damping", damping);
            computeShader.SetFloat("_time", Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            for (float i = 0; i < particleBoundBox.x; i++)
            for (float j = 0; j < particleBoundBox.y; j++)
            for (float k = 0; k < particleBoundBox.z; k++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(new Vector3(i - 0.5f, j - 0.5f, k - 0.5f), Vector3.one * particleRadius);
            }
        }

        private void OnDisable()
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
