using Unity.Entities;
using UnityEngine;

public class NumberMeshAuthoring : MonoBehaviour
{
    public class Baker : Baker<NumberMeshAuthoring>
    {
        public override void Bake(NumberMeshAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ArrayFrameMaterial());
        }
    }
}