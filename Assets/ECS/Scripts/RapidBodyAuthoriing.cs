using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class RapidBodyAuthoriing : MonoBehaviour
{
    public float2 Scale;
    public class Baker : Baker<RapidBodyAuthoriing>
    {
        public override void Bake(RapidBodyAuthoriing authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RapidBodyScale
            {
                Scale = authoring.Scale,
            });
        }
    }
}

//��¼��������������
public struct RapidBodyScale : IComponentData
{
    public float2 Scale;
}