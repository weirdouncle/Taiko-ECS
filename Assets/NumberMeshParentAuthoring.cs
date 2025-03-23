using Unity.Entities;
using UnityEngine;

public class NumberMeshParentAuthoring : MonoBehaviour
{
    public class Baker : Baker<NumberMeshParentAuthoring>
    {
        public override void Bake(NumberMeshParentAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ChapterNumberParent());
        }
    }
}

public struct ChapterNumberParent : IComponentData
{
}