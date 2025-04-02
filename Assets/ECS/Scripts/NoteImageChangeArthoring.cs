using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

//控制音符图在连段数增加后变化
public class NoteImageChangeArthoring : MonoBehaviour
{
    public float2 OffsetX;
    public int Type;
    public class Baker : Baker<NoteImageChangeArthoring>
    {
        public override void Bake(NoteImageChangeArthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new NoteImageChange
            {
                Type = authoring.Type,
                Offset = authoring.OffsetX,
            });
            AddComponent(entity, new ArrayFrameMaterial
            {
                Frame = 0,
            });
            AddComponent(entity, new RenderQueueMaterial
            {
                Queue = 3000,
            });
        }
    }
}
//记录关联的音符类型
public struct NoteImageChange : IComponentData
{
    public int Type;
    public float2 Offset;
}

//关联材质的_Frame属性，修改他shader会索引TextureArray中的贴图序号
[MaterialProperty("_Frame")]
public struct ArrayFrameMaterial : IComponentData
{
    public float Frame;
}

[MaterialProperty("RenderQueue")]
public struct RenderQueueMaterial : IComponentData
{
    public float Queue;
}