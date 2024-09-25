using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Bed.Gimmick
{
    public class ParticleSystem : MonoBehaviour
    {
        [StructLayout(LayoutKind.Sequential, Size = 12)]
        struct Particle
        {
            public float3 position;
        }

        [Header("��ƼŬ ���� �ɼ�")]
        public float particleRadius = 0.1f; // ��ƼŬ�� ������
        public Vector3 particleSpawnPoint;  // ��ƼŬ�� �����Ǵ� ��ġ
        public Vector3Int particleSpawnBox; // �ڽ��� ���� �� �ȿ� ��ƼŬ�� �����ǵ��� ��
        public Vector3 particleBoundBox;    // ��ƼŬ�� ���δ� �ڽ�

        [Header("��ƼŬ ������ �ʿ��� ��")]
        [SerializeField] private Mesh particleMesh;
        [SerializeField] private Material particleMaterial;
        [SerializeField] private ComputeShader computeShader;
        
        private Particle[] particles;
        private int particleCount => particleSpawnBox.x * particleSpawnBox.y * particleSpawnBox.z;
        
        private ComputeBuffer particleBuffer;
        private GraphicsBuffer graphicsBuffer;
        private const int commandCount = 1;

        private RenderParams renderParams;
        private GraphicsBuffer.IndirectDrawIndexedArgs[] commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[commandCount];

        private void Awake()
        {
            if (Vector3.Distance(particleSpawnBox, particleBoundBox) < 0)
            {
                Debug.LogError("Spawn box ���� bouding Box�� �� Ŀ���մϴ�.");
                return;
            }
        }

        private void OnEnable()
        {
            SpawnParticles();
            particleBuffer = new ComputeBuffer(particleCount, Marshal.SizeOf(typeof(Particle)));
            particleBuffer.SetData(particles);
            
            commandData[0].indexCountPerInstance = particleMesh.GetIndexCount(0);
            commandData[0].startIndex = particleMesh.GetIndexStart(0);
            commandData[0].baseVertexIndex = particleMesh.GetBaseVertex(0);
            commandData[0].instanceCount = (uint) particleCount;
            graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, particleCount, GraphicsBuffer.IndirectDrawIndexedArgs.size);
            graphicsBuffer.SetData(commandData);
        }

        private void Start()
        {
            renderParams = new RenderParams()
            {
                material = particleMaterial,
                matProps = new MaterialPropertyBlock(),
                worldBounds = new Bounds(transform.position, particleBoundBox)
            };
            computeShader.SetBuffer(0, "_Particles", particleBuffer);

        }

        private void Update()
        {
            //computeShader.Dispatch(0, particleCount / 8, 1, 1);
            particleMaterial.SetBuffer("_Particles", particleBuffer);
            particleMaterial.SetFloat("_ParticleRadius", particleRadius);
            Graphics.RenderMeshIndirect(renderParams, particleMesh, graphicsBuffer, commandCount);
        }

        private void SpawnParticles()
        {
            float x, y, z;
            List<Particle> particleList = new List<Particle>();
            
            // �ڽ��� �߽��� �ٽ� �ڽ��� ��ǥ�� ���� �ڽ��� �������� �������ν� Center�� �߽����� �¿�� �ڽ��� �׸��� ��
            particleSpawnPoint = new Vector3()
            {
                x = particleSpawnPoint.x - (particleSpawnBox.x * particleRadius),
                y = particleSpawnPoint.y - (particleSpawnBox.y * particleRadius),
                z = particleSpawnPoint.z - (particleSpawnBox.z * particleRadius)
            };
            particleSpawnPoint += Random.onUnitSphere * particleRadius * 0.2f;
            
            for (int i = 0; i < particleSpawnBox.x; i++)
            for (int j = 0; j < particleSpawnBox.y; j++)
            for (int k = 0; k < particleSpawnBox.z; k++)
            {
                x = particleSpawnPoint.x + (i * particleRadius * 2f);
                y = particleSpawnPoint.y + (j * particleRadius * 2f);
                z = particleSpawnPoint.z + (k * particleRadius * 2f);
                particleList.Add(new Particle()
                {
                    position = new Vector3(x, y, z)
                });
            }
            particles = particleList.ToArray();
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, particleBoundBox);
        }

        private void OnDisable()
        {
            graphicsBuffer?.Release();
            particleBuffer?.Release();
            graphicsBuffer = null;
            particleBuffer = null;
        }

    }


}
