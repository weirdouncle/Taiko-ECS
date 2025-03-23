using Unity.Entities;
using UnityEngine;

//���������±����ֵ����������Ҫ�κ����ݣ�ֻ��Ϊ�˷�������
public class SeNoteAuthoring : MonoBehaviour
{
    public class Baker : Baker<SeNoteAuthoring>
    {
        public override void Bake(SeNoteAuthoring authoring)
        {
            Entity se_note_entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(se_note_entity, new SeNoteImage());
        }
    }
}

public struct SeNoteImage : IComponentData
{
}