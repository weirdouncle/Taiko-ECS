using CommonClass;
using System;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

partial struct NotesMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NotesSpawn>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, NoteMove>().Build();
        int count = entityQuery.CalculateEntityCount();

        NativeArray <float2> randomArray = new NativeArray<float2>(count, Allocator.TempJob);
        for (int i = 0; i < count; i++)
        {
            Random rand = new Random(math.hash(new int3(1234, i, (int)SystemAPI.Time.ElapsedTime * 10000))); // 确保唯一种子
            randomArray[i] = rand.NextFloat2(-1, 1) * SystemAPI.Time.DeltaTime;
        }

        // 运行 Job
        NoteMoveJob job = new NoteMoveJob
        {
            randomArray = randomArray
        };

        // 确保 Job 完成后再释放数组
        job.ScheduleParallel();
        state.Dependency = randomArray.Dispose(state.Dependency);

        //foreach ((RefRW<LocalTransform> LocalTransform, RefRO<NoteMove> NoteMove) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<NoteMove>>())
        //{
        //    float2 randomOffset = random.NextFloat2() * 2.0f - 1.0f;
        //    // 将随机偏移量与 DeltaTime 相乘
        //    float2 scaledOffset = randomOffset * SystemAPI.Time.DeltaTime * 0.0000000001f;
        //    LocalTransform.ValueRW.Position += new float3(randomOffset.xy, 0);

        //    float3 position = LocalTransform.ValueRW.Position;

        //    // 检测是否超出屏幕范围
        //    if (position.x < -9.6 || position.x > 9.6 ||
        //        position.y < -5.4 || position.y > 5.4)
        //    {
        //        // 如果超出范围，重置位置为 (0, 0)
        //        LocalTransform.ValueRW.Position = new float3(0, 0, position.z);
        //    }
        //}
    }
}

[BurstCompile]
public partial struct NoteMoveJob : IJobEntity
{
    [NativeDisableParallelForRestriction]
    public NativeArray<float2> randomArray; // Store a unique Random per thread

    public void Execute(ref LocalTransform LocalTransform, in NoteMove note, [EntityIndexInQuery] int index)
    {
        // 将随机偏移量与 DeltaTime 相乘
        LocalTransform.Position += new float3(randomArray[index].xy, 0);

        float3 position = LocalTransform.Position;

        // 检测是否超出屏幕范围
        if (position.x < -6.2 || position.x > 13 ||
            position.y < -6.9 || position.y > 3.9)
        {
            // 如果超出范围，重置位置为 (0, 0)
            LocalTransform.Position = new float3(0, 0, position.z);
        }
    }
}
