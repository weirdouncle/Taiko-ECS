using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

//��������ͼ�����������Ӻ�仯
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
//��¼��������������
public struct NoteImageChange : IComponentData
{
    public int Type;
    public float2 Offset;
}

//�������ʵ�_Frame���ԣ��޸���shader������TextureArray�е���ͼ���
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