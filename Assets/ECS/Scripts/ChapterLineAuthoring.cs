using Unity.Entities;
using UnityEngine;

//����С�����ƶ��������Ϣ
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
