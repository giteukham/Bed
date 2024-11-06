using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bed.Gimmick
{
    public class ParticleSpawner : MonoBehaviour
    {
        /// <summary>
        /// 박스의 중심을 다시 박스의 좌표를 빼서 박스의 시작점을 구함으로써 Center를 중심으로 좌우로 박스를 그리게 함
        /// </summary>
        public void SpawnParticles(Vector3Int particleNumOfSpawn, Vector3 particleSpawnPoint, float particleRadius, out Particle[] particles)
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
            //particleSpawnPoint += Random.onUnitSphere * particleRadius * 0.2f;          // 구 표면에 한 점을 선택해서 그 점을 중심으로 0.2f 만큼 떨어진 점을 선택
            
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
    }
}