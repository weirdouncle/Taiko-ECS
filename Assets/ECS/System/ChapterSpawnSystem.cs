using CommonClass;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

//生成小节线的系统
partial struct ChapterSpawnSystem : ISystem
{
    [BurstCompile]
    public readonly void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ChaptersSpawn>();
    }

    public void OnUpdate(ref SystemState state)
    {
        Entity spawn = SystemAPI.GetSingletonEntity<ChaptersSpawn>();
        ChaptersSpawn spawner = SystemAPI.GetComponent<ChaptersSpawn>(spawn);
        if (spawner.Spawning)
        {
            Entity note_pre = spawner.ChapterEntity;
            for (int i = 0; i < LoaderScript.Lines.Count; i++)
            {
                ChapterLine line = LoaderScript.Lines[i];
                Entity instance = state.EntityManager.Instantiate(note_pre);

                state.EntityManager.AddComponent<Parent>(instance);
                state.EntityManager.SetComponentData(instance, new Parent { Value = spawn });
                state.EntityManager.SetComponentData(instance, new LocalTransform
                {
                    Position = new float3(0, 0, 0),
                    Rotation = quaternion.identity,
                    Scale = 1
                });
                state.EntityManager.SetComponentData(instance, new ChapterMove
                {
                    Bpm = line.Bpm,
                    AppearTime = line.AppearTime,
                    MoveTime = line.MoveTime,
                    Chapter = line.Chapter,
                    Scroll = line.Scroll,
                    JudgeTime = line.JudgeTime,
                    WaitingTime = line.WaitingTime,
                });
            }
            SystemAPI.SetComponent(spawn, new ChaptersSpawn
            {
                ChapterEntity = spawner.ChapterEntity,
                NumberEntity = spawner.NumberEntity,
                Spawning = false,
                Number = true
            });
        }
        else if (spawner.Number)
        {
            /*
             * 
            //在练习模式下添加小节序号
            EntityCommandBuffer ecb = new(Allocator.Temp);
            foreach (var (parent, _, entity) in
                 SystemAPI.Query<RefRO<Parent>, RefRW<ChapterNumberParent>>()
                 .WithEntityAccess()) // 获取 Entity
            {
                ChapterMove move = SystemAPI.GetComponent<ChapterMove>(parent.ValueRO.Value);
                //添加小节数字
                Entity number_pre = spawner.NumberEntity;

                int temp = move.Chapter;
                int count = 0;

                // 计算位数
                if (temp == 0)
                {
                    count = 1; // 如果是 0，至少要有 1 位
                }
                else
                {
                    int num = temp;
                    while (num > 0)
                    {
                        num /= 10;
                        count++;
                    }
                }
                // 申请 NativeArray，确保正确分配内存
                NativeArray<int> Digits = new(count, Allocator.Temp);

                // 提取每一位（从低位到高位）
                for (int i = count - 1; i >= 0; i--)
                {
                    Digits[i] = temp % 10;
                    temp /= 10;
                }
                for (int i = 0; i < Digits.Length; i++)
                {
                    Entity instance = ecb.Instantiate(number_pre);
                    ecb.AddComponent<Parent>(instance);
                    ecb.SetComponent(instance, new Parent { Value = entity });
                    ecb.SetComponent(instance, new LocalTransform
                    {
                        Position = new float3((Digits.Length - 1) * 0.24f - i * 0.24f, 0, 0),
                        Rotation = quaternion.identity,
                        Scale = 1
                    });
                    ecb.SetComponent(instance, new ArrayFrameMaterial { Frame = Digits[Digits.Length - i - 1] });
                }

                Digits.Dispose();
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            */
            SystemAPI.SetComponent(spawn, new ChaptersSpawn
            {
                Spawning = false,
                Number = false,
                Ready = true
            });
        }
    }
}
