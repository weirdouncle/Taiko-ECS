using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class NotesMoveAuthoring : MonoBehaviour
{
    public class Baker : Baker<NotesMoveAuthoring>
    {
        public override void Bake(NotesMoveAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new NoteMoveControll
            {
                Playing =false,
                Reset = false,
                Ready = false,
            });
        }
    }
}

//记录当前游玩状态
public struct NoteMoveControll : IComponentData
{
    public bool Playing;
    public bool Reset;
    public bool Ready;
    public float LastStart;
    public float CurrentTime;
}
