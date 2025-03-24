using CommonClass;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

//控制音符移动等基本属性
public class NoteMoveAuthoring : MonoBehaviour
{
    public GameObject SeNote;
    public int Index;
    public int Type;
    public int Chapter;
    public float JudgeTime;
    public float MidTime;
    public float EndTime;
    public bool BranchRelated;
    public CourseBranch Branch;
    public float Bpm;
    public float AppearTime;
    public float MoveTime;
    public float Scroll;
    public int WaitingTime;
    public bool IsFixedSENote;
    public int Senote;
    public float fBMSCROLLTime;

    public class Baker : Baker<NoteMoveAuthoring>
    {
        public override void Bake(NoteMoveAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new NoteMove
            {
                Index = authoring.Index,
                Type = authoring.Type,
                Chapter = authoring.Chapter,
                AppearTime = authoring.AppearTime,
                EndTime = authoring.EndTime,
                BranchRelated = authoring.BranchRelated,
                Branch = authoring.Branch,
                Bpm = authoring.Bpm,
                MoveTime = authoring.MoveTime,
                Scroll = authoring.Scroll,
                WaitingTime = authoring.WaitingTime,
                IsFixedSENote = authoring.IsFixedSENote,
                Senote = authoring.Senote,
                fBMSCROLLTime = authoring.fBMSCROLLTime,
                JudgeTime = authoring.JudgeTime,
                MidTime = authoring.MidTime,
            });
        }
    }
}

public struct NoteMove : IComponentData
{
    public int Index;
    public int Type;
    public int Chapter;
    public float JudgeTime;
    public float MidTime;
    public float EndTime;
    public bool BranchRelated;
    public CourseBranch Branch;
    public float Bpm;
    public float AppearTime;
    public float MoveTime;
    public float Scroll;
    public int WaitingTime;
    public bool IsFixedSENote;
    public int Senote;
    public float fBMSCROLLTime;
    public float Z_Value;
    public HitNoteResult NoteJudgeState;
    public bool Disable;
    public enum HitNoteResult
    {
        Bad,
        Good,
        Perfect,
        None,
        Lost,
    }
}
