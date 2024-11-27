using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptimalSpatialHashing : MonoBehaviour {
    #region Settings and References
    [Header("Particles")]
    public GameObject particlePrefab;
    public int particleCount = 500;
    public Bounds particleBounds = new(Vector3.zero, new Vector3(20, 20, 20));
    public float maxRadius = 1.5f;
    public float cellSize = 2f;
    
    [Header("Query Settings")]
    public Transform querySphere;
    public float queryRadius = 5f;
    
    [Header("UI Controls")]
    public Slider particleSlider;
    public Slider cellSizeSlider;
    public Slider queryRadiusSlider;
    public TextMeshProUGUI resultsText;
    public Toggle showGridToggle;
    
    bool showGrid;
    
    GameObject querySphereVisual;
    public Material querySphereMaterial;
    #endregion
    
    GameObject[] particleInstances;
    Renderer[] particleRenderers;
    
    NativeArray<Particle> particlesNative;
    NativeArray<HashAndIndex> hashAndIndices;
    NativeList<int> resultIndices;

    struct Particle {
        public float3 Position;
        public float3 Velocity;
        public float Radius;
    }

    struct HashAndIndex : IComparable<HashAndIndex> {
        public int Hash;
        public int Index;
        
        public int CompareTo(HashAndIndex other) {
            return Hash.CompareTo(other.Hash);
        }
    }

    public static int Hash(int3 gridPos) {
        unchecked {
            return gridPos.x * 73856093 ^ gridPos.y * 19349663 ^ gridPos.z * 83492791;
        }
    }

    static int3 GridPosition(float3 position, float cellSize) {
        return new int3(math.floor(position / cellSize));
    }
    
    void Start() {
        particleSlider.value = particleCount;
        particleSlider.onValueChanged.AddListener(_ => InitializeParticles());
        showGridToggle.onValueChanged.AddListener(value => showGrid = value);
        
        cellSizeSlider.value = cellSize;
        cellSizeSlider.onValueChanged.AddListener(value => cellSize = value);
        
        queryRadiusSlider.value = queryRadius;
        queryRadiusSlider.onValueChanged.AddListener(value => queryRadius = value);
        
        InitializeParticles();
        CreateQuerySphereVisual();
        
        if (!particlesNative.IsCreated) return;
        
        UpdateQuerySphereVisual();

        var updateJob = new UpdateParticlesJob {
            particles = particlesNative,
            boundsMin = particleBounds.min,
            boundsMax = particleBounds.max,
            deltaTime = Time.deltaTime
        };
        
        JobHandle updateJobHandle = updateJob.Schedule(particlesNative.Length, 64);
        
        var hashJob = new HashParticlesJob {
            particles = particlesNative,
            cellSize = cellSize,
            hashAndIndices = hashAndIndices
        };
        
        JobHandle hashJobHandle = hashJob.Schedule(particlesNative.Length, 64, updateJobHandle);
        
        var sortJob = new SortHashCodesJob {
            hashAndIndices = hashAndIndices
        };
        
        JobHandle sortJobHandle = sortJob.Schedule(hashJobHandle);
        
        var queryJob = new QueryJob {
            particles = particlesNative,
            hashAndIndices = hashAndIndices,
            queryPosition = querySphere.position,
            queryRadius = queryRadius,
            cellSize = cellSize,
            resultIndices = new NativeList<int>(Allocator.TempJob)
        };
        
        JobHandle queryJobHandle = queryJob.Schedule(sortJobHandle);
        
        queryJobHandle.Complete();
        
        if (resultIndices.IsCreated) resultIndices.Dispose();
        resultIndices = queryJob.resultIndices;

        foreach (var pr in particleRenderers) {
            pr.material.color = Color.white;
        }

        foreach (var index in resultIndices) {
            particleRenderers[index].material.color = Color.red;
        }

        for (int i = 0; i < particlesNative.Length; i++) {
            particleInstances[i].transform.position = particlesNative[i].Position;
        }
        
        resultsText.text = $"Particles within query radius: {resultIndices.Length}";
    }

    void CreateQuerySphereVisual() {
        querySphereVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        querySphereVisual.transform.SetParent(querySphere);
        querySphereVisual.GetComponent<Renderer>().material = querySphereMaterial;
        querySphereVisual.GetComponent<Collider>().enabled = false;
        UpdateQuerySphereVisual();
    }

    void UpdateQuerySphereVisual() {
        if (querySphereVisual != null) {
            querySphereVisual.transform.localScale = Vector3.one * (queryRadius * 2f);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;

        if (querySphere != null) {
            Gizmos.DrawWireSphere(querySphere.position, queryRadius);
        }

        if (showGrid) {
            DrawSpatialHashGrid();
        }
    }

    void DrawSpatialHashGrid() {
        int gridXCount = Mathf.CeilToInt(particleBounds.size.x / cellSize);
        int gridYCount = Mathf.CeilToInt(particleBounds.size.y / cellSize);
        int gridZCount = Mathf.CeilToInt(particleBounds.size.z / cellSize);

        for (int x = 0; x < gridXCount; x++) {
            for (int y = 0; y < gridYCount; y++) {
                for (int z = 0; z < gridZCount; z++) {
                    Vector3 cellCenter = particleBounds.min + new Vector3(x, y, z) * cellSize + Vector3.one * (cellSize / 2f);
                    Gizmos.DrawWireCube(cellCenter, Vector3.one * cellSize);
                }
            }
        }
    }

    void InitializeParticles() {
        if (particlesNative.IsCreated) particlesNative.Dispose();
        if (hashAndIndices.IsCreated) hashAndIndices.Dispose();
        
        particleCount = Mathf.RoundToInt(particleSlider.value);

        if (particleInstances != null) {
            foreach (var instance in particleInstances) {
                Destroy(instance);
            }
        }
        
        particleInstances = new GameObject[particleCount];
        particleRenderers = new Renderer[particleCount];
        
        particlesNative = new NativeArray<Particle>(particleCount, Allocator.Persistent);
        hashAndIndices = new NativeArray<HashAndIndex>(particleCount, Allocator.Persistent);

        for (int i = 0; i < particleCount; i++) {
            Vector3 position = new(
                UnityEngine.Random.Range(particleBounds.min.x, particleBounds.max.x),
                UnityEngine.Random.Range(particleBounds.min.y, particleBounds.max.y),
                UnityEngine.Random.Range(particleBounds.min.z, particleBounds.max.z)
            );
            float radius = UnityEngine.Random.Range(0.5f, maxRadius);
            Vector3 velocity = new(
                UnityEngine.Random.Range(-1f, 1f),
                UnityEngine.Random.Range(-1f, 1f),
                UnityEngine.Random.Range(-1f, 1f)
            );
            
            particlesNative[i] = new Particle {
                Position = position,
                Radius = radius,
                Velocity = velocity
            };
            
            var instance = Instantiate(particlePrefab, position, Quaternion.identity);
            instance.transform.localScale = Vector3.one * radius * 2f;
            instance.transform.SetParent(transform);
            particleInstances[i] = instance;
            particleRenderers[i] = instance.GetComponent<Renderer>();
        }
    }

    void Update() {
        
    }

    void OnDestroy() {
        if (particlesNative.IsCreated) particlesNative.Dispose();
        if (hashAndIndices.IsCreated) hashAndIndices.Dispose();
        if (resultIndices.IsCreated) resultIndices.Dispose();
    }

    [BurstCompile]
    struct UpdateParticlesJob : IJobParallelFor {
        public NativeArray<Particle> particles;
        public float3 boundsMin;
        public float3 boundsMax;
        public float deltaTime;
        
        public void Execute(int index) {
            Particle particle = particles[index];
            
            particle.Position += particle.Velocity * deltaTime;
            
            // Bounce off the bounds
            if (particle.Position.x - particle.Radius < boundsMin.x && particle.Velocity.x < 0 ||
                particle.Position.x + particle.Radius > boundsMax.x && particle.Velocity.x > 0) {
                particle.Velocity.x = -particle.Velocity.x;
            }
            
            if (particle.Position.y - particle.Radius < boundsMin.y && particle.Velocity.y < 0 ||
                particle.Position.y + particle.Radius > boundsMax.y && particle.Velocity.y > 0) {
                particle.Velocity.y = -particle.Velocity.y;
            }

            if (particle.Position.z - particle.Radius < boundsMin.z && particle.Velocity.z < 0 ||
                particle.Position.z + particle.Radius > boundsMax.z && particle.Velocity.z > 0) {
                particle.Velocity.z = -particle.Velocity.z;
            }
            
            particles[index] = particle;
        }
    }

    [BurstCompile]
    struct HashParticlesJob : IJobParallelFor {
        [ReadOnly] public NativeArray<Particle> particles;
        public NativeArray<HashAndIndex> hashAndIndices;
        
        public float cellSize;
        public void Execute(int index) {
            Particle particle = particles[index];
            int hash = Hash(GridPosition(particle.Position, cellSize));
            Debug.Log($"GridPosition : {GridPosition(particle.Position, cellSize)} position : {particle.Position} hash : {hash}");
            hashAndIndices[index] = new HashAndIndex { Hash = hash, Index = index };
        }
    }

    [BurstCompile]
    struct SortHashCodesJob : IJob {
        public NativeArray<HashAndIndex> hashAndIndices;

        public void Execute() {
            hashAndIndices.Sort();
        }
    }

    [BurstCompile]
    struct QueryJob : IJob {
        [ReadOnly] public NativeArray<Particle> particles;
        [ReadOnly] public NativeArray<HashAndIndex> hashAndIndices;
        public float3 queryPosition;
        public float queryRadius;
        public float cellSize;
        public NativeList<int> resultIndices;
        public int prevHash;

        public void Execute() {
            float radiusSquared = queryRadius * queryRadius;
            int3 minGridPos = GridPosition(queryPosition - queryRadius, cellSize);
            int3 maxGridPos = GridPosition(queryPosition + queryRadius, cellSize);
            
            Debug.Log($"minGridPos: {queryPosition - queryRadius}, maxGridPos: {queryPosition + queryRadius}");
            for (int x = minGridPos.x; x <= maxGridPos.x; x++) {
                for (int y = minGridPos.y; y <= maxGridPos.y; y++) {
                    for (int z = minGridPos.z; z <= maxGridPos.z; z++) {
                        int3 gridPos = new(x, y, z);
                        int hash = Hash(gridPos);
                        
                        int startIndex = BinarySearchFirst(hashAndIndices, hash);
                        
                        if (startIndex < 0) continue;

                        for (int i = startIndex; i < hashAndIndices.Length && hashAndIndices[i].Hash == hash; i++) {
                            int particleIndex = hashAndIndices[i].Index;
                            Particle particle = particles[particleIndex];
                            float3 toParticle = particle.Position - queryPosition;

                            if (math.lengthsq(toParticle) <= radiusSquared) {
                                resultIndices.Add(particleIndex);
                            }
                        }
                    }
                }
            }
        }

        int BinarySearchFirst(NativeArray<HashAndIndex> array, int hash) {
            int left = 0;
            int right = array.Length - 1;
            int result = -1;

            while (left <= right) {
                int mid = (left + right) / 2;
                int midHash = array[mid].Hash;

                if (midHash == hash) {
                    result = mid;
                    right = mid - 1;
                } else if (midHash < hash) {
                    left = mid + 1;
                } else {
                    right = mid - 1;
                }
            }
            return result;
        }
    }
}