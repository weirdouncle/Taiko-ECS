using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using static CriWare.CriAtomExBeatSync;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Random = Unity.Mathematics.Random;

partial struct NoteSpawnSystem : ISystem
{
    [BurstCompile]
    public readonly void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NotesSpawn>();
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity spawn = SystemAPI.GetSingletonEntity<NotesSpawn>();
        NotesSpawn note = SystemAPI.GetComponent<NotesSpawn>(spawn);
        if (note.SpawnCount > 0)
        {
            Entity note_pre = note.NoteEntity; // 获取 `NotesSpawn` 组件中的预制体 Entity
            for (int i = 0; i < note.SpawnCount; i++)
            {
                Entity instance = state.EntityManager.Instantiate(note_pre);

                state.EntityManager.AddComponent<Parent>(instance);
                state.EntityManager.SetComponentData(instance, new Parent { Value = spawn });
                state.EntityManager.SetComponentData(instance, new LocalTransform
                {
                    Position = new float3(0, 0, i * -0.000001f),
                    Rotation = quaternion.identity,
                    Scale = 1
                });

                //添加音符图片
                Random rand = new(math.hash(new int2(1234, i))); // 确保唯一种子
                int type = rand.NextInt(1, 8);
                //type += 5;
                //int type = 5;
                Entity image = Entity.Null;
                NoteMove move = new NoteMove { Type = type };
                switch (type)
                {
                    case 1:
                        image = state.EntityManager.Instantiate(note.DonImage);
                        break;
                    case 2:
                        image = state.EntityManager.Instantiate(note.KaImage);
                        break;
                    case 3:
                        image = state.EntityManager.Instantiate(note.BigDonImage);
                        break;
                    case 4:
                        image = state.EntityManager.Instantiate(note.BigKaImage);
                        break;
                    case 5:
                        {
                            image = state.EntityManager.Instantiate(note.RapidImage);
                            move.JudgeTime = 3000;
                            move.EndTime = 3500;
                            move.Bpm = 60;
                            move.Scroll = 2;
                        }
                        break;
                    case 6:
                        {
                            image = state.EntityManager.Instantiate(note.BigRapidImage);
                            move.JudgeTime = 3000;
                            move.EndTime = 3200;
                            move.Bpm = 60;
                            move.Scroll = 2;
                        }
                        break;
                    case 7:
                        image = state.EntityManager.Instantiate(note.BalloonImage);
                        break;
                }
                state.EntityManager.SetComponentData(instance, move);
                state.EntityManager.AddComponent<Parent>(image);
                state.EntityManager.SetComponentData(image, new Parent { Value = instance });
                state.EntityManager.SetComponentData(image, new LocalTransform
                {
                    Position = new float3(0, 0, 0),
                    Rotation = quaternion.identity,
                    Scale = 1
                });
            }
            SystemAPI.SetComponent(spawn, new NotesSpawn
            {
                NoteEntity = note.NoteEntity,
                DonImage = note.DonImage,
                KaImage = note.KaImage,
                BigDonImage = note.BigDonImage,
                BigKaImage = note.BigKaImage,
                SpawnCount = 0,
                SetSeNote = true,        //因为刚刚生成entity，可能子组件的SeNoteImage还未生成，因此生成senote的步骤放到下一帧
                Rapid = false,
            });
        }
        else if (note.SetSeNote)
        {
            EntityQuery entityQuery = SystemAPI.QueryBuilder().WithAll<Parent, SeNoteImage, MaterialMeshInfo> ().Build();
            int count = entityQuery.CalculateEntityCount();


            NativeArray<NoteMove> Array = new(count, Allocator.TempJob);
            NativeArray<BatchMaterialID> Ids = new(PicsControllScript.BatchMaterials.Count, Allocator.TempJob);
            Ids.CopyFrom(PicsControllScript.BatchMaterials.ToArray());

            NativeArray<Parent> entities = entityQuery.ToComponentDataArray<Parent>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                NoteMove move = SystemAPI.GetComponent<NoteMove>(entities[i].Value);
                Array[i] = move;
            }
            entities.Dispose();
            // 运行 Job
            SetSeNoteJob job = new()
            {
                NotesArray = Array,
                Ids = Ids
            };
            // 确保 Job 完成后再释放数组
            state.Dependency = job.ScheduleParallel(entityQuery, state.Dependency);
            state.Dependency = Array.Dispose(state.Dependency);
            state.Dependency = Ids.Dispose(state.Dependency);

            //foreach (var (parent, image, entity) in SystemAPI.Query<RefRO<Parent>, RefRO<SeNoteImage>>().WithEntityAccess())
            //{
            //    NoteImageChange move = SystemAPI.GetComponent<NoteImageChange>(parent.ValueRO.Value);
            //    SpriteRenderer sprite = state.EntityManager.GetComponentObject<SpriteRenderer>(entity);
            //    switch (move.Type)
            //    {
            //        case 1:
            //            sprite.sprite = PicsControllScript.SeSprite[0];
            //            break;
            //        case 2:
            //            sprite.sprite = PicsControllScript.SeSprite[4];
            //            break;
            //        case 3:
            //            sprite.sprite = PicsControllScript.SeSprite[3];
            //            break;
            //        case 4:
            //            sprite.sprite = PicsControllScript.SeSprite[6];
            //            break;
            //        case 5:
            //            sprite.sprite = PicsControllScript.SeSprite[7];
            //            break;
            //        case 6:
            //            sprite.sprite = PicsControllScript.SeSprite[8];
            //            break;
            //        case 7:
            //            sprite.sprite = PicsControllScript.SeSprite[9];
            //            break;
            //    }
            //}

            SystemAPI.SetComponent(spawn, new NotesSpawn
            {
                NoteEntity = note.NoteEntity,
                DonImage = note.DonImage,
                KaImage = note.KaImage,
                BigDonImage = note.BigDonImage,
                BigKaImage = note.BigKaImage,
                SpawnCount = 0,
                SetSeNote = false,
                Rapid = true
            });
        }
        else if (note.Rapid)
        {
            EntityCommandBuffer ecb = new(Allocator.Temp);
            //将音符图标根据NoteImageChange之中OffsetX对其
            foreach (var (parent, transform, image, entity) in SystemAPI.Query<RefRO<Parent>, RefRW<LocalTransform>, RefRO<NoteImageChange>>().WithEntityAccess())
            {
                //transform.ValueRW.Position += new float3(image.ValueRO.Offset.xy, 0);
                if (image.ValueRO.Type == 5 || image.ValueRO.Type == 6)
                {
                    NoteMove move = SystemAPI.GetComponent<NoteMove>(parent.ValueRO.Value);

                    float length = (float)((move.EndTime - move.JudgeTime) * move.Bpm * move.Scroll * (1 + 1.5f)) / 628.7f * 1.5f / 100;

                    DynamicBuffer<Child> children = SystemAPI.GetBuffer<Child>(entity);
                    for (int i = 0; i < children.Length; i++)
                    {
                        Entity child = children[i].Value;
                        if (SystemAPI.HasComponent<RapidBodyScale>(child))
                        {
                            //调整body缩放
                            RapidBodyScale body = SystemAPI.GetComponent<RapidBodyScale>(child);
                            PostTransformMatrix postMatrix = state.EntityManager.GetComponentData<PostTransformMatrix>(child);
                            AffineTransform affine = new AffineTransform(postMatrix.Value);
                            // 使用 math.decompose 解析
                            math.decompose(affine, out float3 translation, out quaternion rotation, out float3 scale);

                            float3 new_scale = new float3(length / 8 * body.Scale.x * 100, scale.y, 1);
                            //Debug.Log($"身体局部缩放从: X={scale.x}, Y={scale.y}, Z={scale.z} 到 X={ new_scale.x }, Length:{length}");

                            ecb.AddComponent(child, new PostTransformMatrix
                            {
                                Value = float4x4.Scale(new_scale)        //image.ValueRO.Scale.x记录的是其原始scale值
                            });

                            //更新body和senote图片
                            DynamicBuffer<Child> Se1 = SystemAPI.GetBuffer<Child>(child);
                            Entity Se1Entity = Se1[0].Value;
                            MaterialMeshInfo mat = state.EntityManager.GetComponentData<MaterialMeshInfo>(Se1Entity);
                            mat.MaterialID = PicsControllScript.BatchMaterials[9];
                            ecb.SetComponent(Se1Entity, mat);
                        }
                        else
                        {
                            LocalTransform localTransform = state.EntityManager.GetComponentData<LocalTransform>(child);
                            localTransform.Position = new float3(length / 1.94f, localTransform.Position.y, localTransform.Position.z);     //要除以父entity的scale.x
                            ecb.SetComponent(child, localTransform);
                            //更新tail的senote图片
                            DynamicBuffer<Child> Se2 = SystemAPI.GetBuffer<Child>(child);
                            Entity Se2Entity = Se2[0].Value;
                            MaterialMeshInfo mat = state.EntityManager.GetComponentData<MaterialMeshInfo>(Se2Entity);
                            mat.MaterialID = PicsControllScript.BatchMaterials[10];
                            ecb.SetComponent(Se2Entity, mat);
                        }
                    }
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            SystemAPI.SetComponent(spawn, new NotesSpawn
            {
                NoteEntity = note.NoteEntity,
                DonImage = note.DonImage,
                KaImage = note.KaImage,
                BigDonImage = note.BigDonImage,
                BigKaImage = note.BigKaImage,
                SpawnCount = 0,
                SetSeNote = false,
                Rapid = false,
            });
        }
    }
}

[BurstCompile]
public partial struct SetSeNoteJob : IJobEntity
{
    [ReadOnly] public NativeArray<NoteMove> NotesArray;
    [ReadOnly] public NativeArray<BatchMaterialID> Ids;

    //添加下方音符文字，PicsControllScript脚本中已将材质注册到系统中，直接替换即可
    public void Execute(in Parent Parent, in SeNoteImage image, ref MaterialMeshInfo info, [EntityIndexInQuery] int index)
    {
        NoteMove move = NotesArray[index];
        switch (move.Type)
        {
            case 1:
                info.MaterialID = Ids[0];
                break;
            case 2:
                info.MaterialID = Ids[4];
                break;
            case 3:
                info.MaterialID = Ids[3];
                break;
            case 4:
                info.MaterialID = Ids[6];
                break;
            case 5:
                info.MaterialID = Ids[7];
                break;
            case 6:
                info.MaterialID = Ids[8];
                break;
            case 7:
                info.MaterialID = Ids[11];
                break;
        }
    }
}

[BurstCompile]
public partial struct NoteImageAdjustJob : IJobEntity
{
    public void Execute(ref LocalTransform LocalTransform, in NoteImageChange note)
    {
        LocalTransform.Position += new float3(note.Offset.xy, 0);
    }
}