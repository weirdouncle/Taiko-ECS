using Unity.Entities;
using UnityEngine;

//关联音符下标文字的组件，不需要任何内容，只是为了方便索引
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