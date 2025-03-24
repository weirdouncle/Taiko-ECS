using System.Diagnostics;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
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

                ecb.AddComponent(entity, new Disabled());
                DynamicBuffer<LinkedEntityGroup> linkedEntities = state.EntityManager.GetBuffer<LinkedEntityGroup>(entity);
                foreach (var child in linkedEntities)
                    ecb.AddComponent(child.Value, new Disabled());
            }
            else if (note.ValueRO.Disable)
            {
                ecb.AddComponent(entity, new Disabled());
                DynamicBuffer<LinkedEntityGroup> linkedEntities = state.EntityManager.GetBuffer<LinkedEntityGroup>(entity);
                foreach (var child in linkedEntities)
                    ecb.AddComponent(child.Value, new Disabled());
            }
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
