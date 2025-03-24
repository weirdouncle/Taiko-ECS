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

//����С���ߵ�ϵͳ
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
            //����ϰģʽ�����С�����
            EntityCommandBuffer ecb = new(Allocator.Temp);
            foreach (var (parent, _, entity) in
                 SystemAPI.Query<RefRO<Parent>, RefRW<ChapterNumberParent>>()
                 .WithEntityAccess()) // ��ȡ Entity
            {
                ChapterMove move = SystemAPI.GetComponent<ChapterMove>(parent.ValueRO.Value);
                //���С������
                Entity number_pre = spawner.NumberEntity;

                int temp = move.Chapter;
                int count = 0;

                // ����λ��
                if (temp == 0)
                {
                    count = 1; // ����� 0������Ҫ�� 1 λ
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
                // ���� NativeArray��ȷ����ȷ�����ڴ�
                NativeArray<int> Digits = new(count, Allocator.Temp);

                // ��ȡÿһλ���ӵ�λ����λ��
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
