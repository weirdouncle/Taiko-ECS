using CommonClass;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.EventSystems.EventTrigger;

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
        if (note.Spawning && LoaderScript.Notes.Count > 0)
        {
            int index = 3000;
            foreach (NoteChip chip in LoaderScript.Notes.Values)
            {
                NoteMove move = new NoteMove
                {
                    Index = chip.Index,
                    Type = chip.Type,
                    Chapter = chip.Chapter,
                    JudgeTime = chip.JudgeTime,
                    MidTime = chip.MidTime,
                    EndTime = chip.EndTime,
                    BranchRelated = chip.BranchRelated,
                    Branch = chip.Branch,
                    Bpm = chip.Bpm,
                    AppearTime = chip.AppearTime,
                    MoveTime = chip.MoveTime,
                    Scroll = chip.Scroll,
                    WaitingTime = chip.WaitingTime,
                    IsFixedSENote = chip.IsFixedSENote,
                    Senote = chip.Senote,
                    fBMSCROLLTime = chip.fBMSCROLLTime,
                    Z_Value = chip.Z_Value,
                };

                //添加音符图片
                Entity note_pre = Entity.Null; // 获取 `NotesSpawn` 组件中的预制体 Entity
                switch (chip.Type)
                {
                    case 1:
                        note_pre = note.DonImage;
                        break;
                    case 2:
                        note_pre = (note.KaImage);
                        break;
                    case 3:
                        note_pre = (note.BigDonImage);
                        break;
                    case 4:
                        note_pre = (note.BigKaImage);
                        break;
                    case 5:
                        Debug.Log(chip.Type);
                        note_pre = (note.RapidImage);
                        break;
                    case 6:
                        note_pre = (note.BigRapidImage);
                        break;
                    case 7:
                        note_pre = (note.BalloonImage);
                        break;
                }
                Entity instance = state.EntityManager.Instantiate(note_pre);

                state.EntityManager.AddComponent<Parent>(instance);
                state.EntityManager.SetComponentData(instance, new Parent { Value = spawn });
                state.EntityManager.SetComponentData(instance, new LocalTransform
                {
                    Position = new float3(0, 0, 0),
                    Rotation = quaternion.identity,
                    Scale = 1
                });
                state.EntityManager.SetComponentData(instance, move);

                DynamicBuffer<LinkedEntityGroup> linkedEntities = state.EntityManager.GetBuffer<LinkedEntityGroup>(instance);
                foreach (var child in linkedEntities)
                {
                    if (SystemAPI.HasComponent<RenderQueueMaterial>(child.Value)) SystemAPI.SetComponent(child.Value, new RenderQueueMaterial { Queue = index });
                }
                index++;
            }
            SystemAPI.SetComponent(spawn, new NotesSpawn
            {
                DonImage = note.DonImage,
                KaImage = note.KaImage,
                BigDonImage = note.BigDonImage,
                BigKaImage = note.BigKaImage,
                Spawning = false,
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

            SystemAPI.SetComponent(spawn, new NotesSpawn
            {
                DonImage = note.DonImage,
                KaImage = note.KaImage,
                BigDonImage = note.BigDonImage,
                BigKaImage = note.BigKaImage,
                Spawning = false,
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
                            RenderBounds bounds = new RenderBounds
                            {
                                Value = new AABB
                                {
                                    Center = new float3(0,0,0),
                                    Extents = new float3(1, 0.5f, 0.00000000000000003061617f)
                                }
                            };
                            ecb.SetComponent(child, bounds);        //修改黄条的RenderBounds，不然若其离开屏幕太远会被遮挡

                            //更新body和senote图片
                            DynamicBuffer<Child> Se1 = SystemAPI.GetBuffer<Child>(child);
                            Entity Se1Entity = Se1[0].Value;
                            MaterialMeshInfo mat = state.EntityManager.GetComponentData<MaterialMeshInfo>(Se1Entity);
                            mat.MaterialID = PicsControllScript.BatchMaterials[9];
                            ecb.SetComponent(Se1Entity, mat);
                            ecb.SetComponent(Se1Entity, bounds);
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
                DonImage = note.DonImage,
                KaImage = note.KaImage,
                BigDonImage = note.BigDonImage,
                BigKaImage = note.BigKaImage,
                Spawning = false,
                SetSeNote = false,
                Rapid = false,
            });

            Entity controller = SystemAPI.GetSingletonEntity<NoteMoveControll>();
            SystemAPI.SetComponent(controller, new NoteMoveControll { Reset = true, Ready =false, Playing = false });
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
                info.MaterialID = Ids[move.Senote <= 3 ? move.Senote : 0];
                break;
            case 2:
                int i = math.max(move.Senote + 1, 4);
                info.MaterialID = Ids[i <= 6 ? i : 4];
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
                info.MaterialID = Ids[10];
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