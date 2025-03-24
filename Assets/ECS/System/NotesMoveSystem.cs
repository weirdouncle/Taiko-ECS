using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct NotesMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NoteMoveControll>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        RefRW<NoteMoveControll> controller = SystemAPI.GetSingletonRW<NoteMoveControll>();
        //正常游戏状态
        if (controller.ValueRO.Playing)
        {
            // 运行 Job
            NoteMoveJob job = new NoteMoveJob
            {
                Time = (float)SystemAPI.Time.ElapsedTime,
                LastStart = controller.ValueRO.LastStart
            };
            job.ScheduleParallel();
            ChapterMoveJob chapter = new ChapterMoveJob
            {
                Time = (float)SystemAPI.Time.ElapsedTime,
                LastStart = controller.ValueRO.LastStart
            };
            chapter.ScheduleParallel();
        }
        else if (controller.ValueRO.Reset)      //重置游戏
        {
            EntityQuery query = SystemAPI.QueryBuilder().WithAll<Disabled>().WithAll<NoteMove>().Build();

            // 获取所有被禁用的实体
            using var entities = query.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                state.EntityManager.RemoveComponent<Disabled>(entity);
                DynamicBuffer<LinkedEntityGroup> linkedEntities = state.EntityManager.GetBuffer<LinkedEntityGroup>(entity);
                foreach (var child in linkedEntities)
                    state.EntityManager.RemoveComponent<Disabled>(child.Value);
            }

            NoteResetJob job = new();
            job.ScheduleParallel();


            EntityQuery query2 = SystemAPI.QueryBuilder().WithAll<Disabled>().WithAll<ChapterMove>().Build();

            // 获取所有被禁用的实体
            using var entities2 = query2.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities2)
            {
                state.EntityManager.RemoveComponent<Disabled>(entity);
                DynamicBuffer<LinkedEntityGroup> linkedEntities = state.EntityManager.GetBuffer<LinkedEntityGroup>(entity);
                foreach (var child in linkedEntities)
                    state.EntityManager.RemoveComponent<Disabled>(child.Value);
            }

            ChapterResetJob job2 = new();
            job2.ScheduleParallel();

            controller.ValueRW.CurrentTime = 0;
            controller.ValueRW.Reset = false;
            controller.ValueRW.Ready = true;
        }
    }
}

[BurstCompile]
public partial struct NoteMoveJob : IJobEntity
{
    public float Time;
    public float LastStart;

    public void Execute(ref LocalTransform LocalTransform, ref NoteMove note)
    {
        if (!note.Disable)
        {
            float time = note.JudgeTime - (Time - LastStart) * 1000;
            if (note.Type == 7)
            {
                if (time >= 0)
                    LocalTransform.Position = new float3((float)(time * note.Bpm * note.Scroll * (1 + 1.5f)) / 628.7f * 1.5f / 100, 0, note.Z_Value);
                else
                {
                    time = note.EndTime  - (Time - LastStart) * 1000;
                    if (time >= 0)
                        LocalTransform.Position = new float3(0, 0, note.Z_Value);
                    else
                        LocalTransform.Position = new float3((float)(time * note.Bpm * note.Scroll * (1 + 1.5f)) / 628.7f * 1.5f / 100, 0, note.Z_Value);
                }
            }
            else
            {
                LocalTransform.Position = new float3((float)(time * note.Bpm * note.Scroll * (1 + 1.5f)) / 628.7f * 1.5f / 100, 0, note.Z_Value);
            }
            if (note.Type <= 4)
            {
                if (time <= 0)      //自动打击
                {
                    note.NoteJudgeState = NoteMove.HitNoteResult.Perfect;
                    note.Disable = true;
                }
                else if (note.Type <= 4 && time < -125)     //miss逻辑
                    note.NoteJudgeState = NoteMove.HitNoteResult.Lost;
            }
            if (note.Type == 5 || note.Type == 6)
            {
                float time2 = note.EndTime - (Time - LastStart) * 1000;
                float x = (float)(time2 * note.Bpm * note.Scroll * (1 + 1.5f)) / 628.7f * 1.5f / 100;
                if ((note.Scroll >= 0 && x < -7) || (note.Scroll < 0 && x > 14))
                    note.Disable = true;
            }
            else if ((note.Scroll >= 0 && LocalTransform.Position.x < -7) || (note.Scroll < 0 && LocalTransform.Position.x > 14))
                note.Disable = true;
        }
    }
}

[BurstCompile]
public partial struct NoteResetJob : IJobEntity
{
    public void Execute(ref LocalTransform LocalTransform, ref NoteMove note)
    {
        note.Disable = false;
        note.NoteJudgeState = NoteMove.HitNoteResult.None;
        LocalTransform.Position = new float3((float)(note.JudgeTime * note.Bpm * note.Scroll * (1 + 1.5f)) / 628.7f * 1.5f / 100, 0, note.Z_Value);
    }
}

[BurstCompile]
public partial struct ChapterMoveJob : IJobEntity
{
    public float Time;
    public float LastStart;

    public void Execute(ref LocalTransform LocalTransform, ref ChapterMove note)
    {
        if (!note.Disable)
        {
            float time = note.JudgeTime - (Time - LastStart) * 1000;

            LocalTransform.Position = new float3((float)(time * note.Bpm * note.Scroll * (1 + 1.5f)) / 628.7f * 1.5f / 100, 0, 0);

            if ((note.Scroll >= 0 && LocalTransform.Position.x < -7) || (note.Scroll < 0 && LocalTransform.Position.x > 14))
                note.Disable = true;
        }
    }
}

[BurstCompile]
public partial struct ChapterResetJob : IJobEntity
{
    public void Execute(ref LocalTransform LocalTransform, ref ChapterMove note)
    {
        note.Disable = false;
        LocalTransform.Position = new float3((float)(note.JudgeTime * note.Bpm * note.Scroll * (1 + 1.5f)) / 628.7f * 1.5f / 100, 0, 0);
    }
}