using Unity.Entities;
using UnityEngine;

//关联小节线移动的相关信息
public class ChapterLineAuthoring : MonoBehaviour
{
    public class Baker : Baker<ChapterLineAuthoring>
    {
        public override void Bake(ChapterLineAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ChapterMove());
        }
    }
}

public struct ChapterMove : IComponentData
{
    public double Bpm;
    public float AppearTime;
    public float MoveTime;
    public int Chapter;
    public double Scroll;
    public float JudgeTime;
    public int WaitingTime;
}
