using Unity.Entities;
using UnityEngine;
public struct Test : IComponentData
{
    public int Value;
}

public class EntityTest : MonoBehaviour
{
    public int Value;
    
    public class Baker : Baker<EntityTest>
    {
        public override void Bake(EntityTest authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new Test
            {
                Value = authoring.Value
            });
        }
    }
}