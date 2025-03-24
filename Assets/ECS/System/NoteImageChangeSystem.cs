using Unity.Burst;
using Unity.Entities;

//变更音符图的系统，读取NoteTickScript的变更标签并进行对比，如有不同就进行更改
partial struct NoteImageChangeSystem : ISystem
{
    [BurstCompile]
    public readonly void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NotesSpawn>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        RefRW<NotesTickState> note = SystemAPI.GetSingletonRW<NotesTickState>();
        if (note.ValueRO.Animating && note.ValueRO.LastTick != note.ValueRO.Tick)
        {
            note.ValueRW.LastTick = note.ValueRO.Tick;
            NoteMatJob job = new()
            {
                Index1 = note.ValueRO.ComboType,
                Index2 = note.ValueRO.ComboTypeBig,
            };

            job.ScheduleParallel();
        }
    }
}

[BurstCompile]
public partial struct NoteMatJob : IJobEntity
{
    public int Index1;
    public int Index2;

    public void Execute(in NoteImageChange note, ref ArrayFrameMaterial mat)
    {
        if (note.Type == 3 || note.Type == 4 || note.Type == 6)
        {
            mat.Frame = Index2;
        }
        else
        {
            mat.Frame = Index1;
        }
    }
}
