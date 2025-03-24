using System.Diagnostics;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

partial struct NoteStateSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new(Allocator.Temp);

        foreach (var (note, entity) in SystemAPI.Query<RefRO<NoteMove>>().WithEntityAccess())
        {
            if (note.ValueRO.Type <= 4 && note.ValueRO.NoteJudgeState == NoteMove.HitNoteResult.Perfect)
            {
                if (note.ValueRO.Type == 1 || note.ValueRO.Type == 3)
                    SoundPool.Instance.PlaySound(true);
                else
                    SoundPool.Instance.PlaySound(false);

                //ecb.AddComponent(entity, new Disabled());
                ecb.SetComponentEnabled<NoteMove>(entity, false);
                DynamicBuffer<LinkedEntityGroup> linkedEntities = state.EntityManager.GetBuffer<LinkedEntityGroup>(entity);
                if (SystemAPI.HasComponent<MaterialMeshInfo>(entity)) ecb.SetComponentEnabled<MaterialMeshInfo>(entity, false);
                foreach (var child in linkedEntities)
                {
                    if (SystemAPI.HasComponent<MaterialMeshInfo>(child.Value)) ecb.SetComponentEnabled<MaterialMeshInfo>(child.Value, false);
                }
            }
            else if (note.ValueRO.Disable)
            {
                ecb.SetComponentEnabled<NoteMove>(entity, false);
                DynamicBuffer<LinkedEntityGroup> linkedEntities = state.EntityManager.GetBuffer<LinkedEntityGroup>(entity);
                if (SystemAPI.HasComponent<MaterialMeshInfo>(entity)) ecb.SetComponentEnabled<MaterialMeshInfo>(entity, false);
                foreach (var child in linkedEntities)
                {
                    if (SystemAPI.HasComponent<MaterialMeshInfo>(child.Value)) ecb.SetComponentEnabled<MaterialMeshInfo>(child.Value, false);
                }
            }
        }

        foreach (var (note, entity) in SystemAPI.Query<RefRO<ChapterMove>>().WithEntityAccess())
        {
            if (note.ValueRO.Disable)
            {
                ecb.SetComponentEnabled<ChapterMove>(entity, false);
                DynamicBuffer<LinkedEntityGroup> linkedEntities = state.EntityManager.GetBuffer<LinkedEntityGroup>(entity);
                if (SystemAPI.HasComponent<MaterialMeshInfo>(entity)) ecb.SetComponentEnabled<MaterialMeshInfo>(entity, false);
                foreach (var child in linkedEntities)
                {
                    if (SystemAPI.HasComponent<MaterialMeshInfo>(child.Value)) ecb.SetComponentEnabled<MaterialMeshInfo>(child.Value, false);
                }
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
