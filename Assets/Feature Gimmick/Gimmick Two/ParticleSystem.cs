using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
namespace Bed.Gimmick
{
    [System.Serializable]
    struct Particle
    {
        public float presssure;
        public float density;
        public float velocity;
        public Vector3 position;
    };
    
    public class ParticleSystem : MonoBehaviour
    {
        public int particleCount = 1000;
        public float particleDiameter = 0.1f;
        private NativeArray<Particle> particles;

        [SerializeField] private Mesh particleMesh;
        [SerializeField] private Material particleMaterial;
        static Mesh ParticleMesh;
        static Material ParticleMaterial;
        
        private void Awake()
        {
            ParticleMesh = particleMesh;
            particles = new NativeArray<Particle>(particleCount, Allocator.Persistent);
        }

        private void Start()
        {

        }


        [BurstCompile]
        public struct ParticleSpawner : IJobParallelForTransform
        {
            public float particleDiameter;
            public int particleCount;
            
            ParticleSpawner(float particleDiameter, int particleCount)
            {
                this.particleDiameter = particleDiameter;
                this.particleCount = particleCount;    
            }
            // GameObject particlePrefab = new GameObject("Particle");
            // particlePrefab.AddComponent<MeshFilter>().sharedMesh = ParticleMesh;
            // particlePrefab.AddComponent<MeshRenderer>().sharedMaterial = ParticleMaterial;
            public void Execute(int index, TransformAccess transform)
            {
                for (int i = 0; i < 3; i++)
                {
                    // var particle = Instantiate(particlePrefab, new Vector3(i * particleDiameter, 0, 0), Quaternion.identity);
                    // particle.transform.localScale = new Vector3(particleDiameter, particleDiameter, particleDiameter);
                }
            }
        }
    }


}
