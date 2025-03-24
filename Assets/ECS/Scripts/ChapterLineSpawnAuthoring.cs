using Unity.Entities;
using UnityEngine;

public class ChapterLineSpawnAuthoring : MonoBehaviour
{
    public GameObject Line_pre;
    public GameObject Number_pre;
    public class Baker : Baker<ChapterLineSpawnAuthoring>
    {
        public override void Bake(ChapterLineSpawnAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            Entity prefabEntity = GetEntity(authoring.Line_pre, TransformUsageFlags.Dynamic);
            Entity numberEntity = GetEntity(authoring.Number_pre, TransformUsageFlags.Dynamic);

            AddComponent(entity, new ChaptersSpawn
            {
                ChapterEntity = prefabEntity,
                NumberEntity = numberEntity,
                Number = false,
            });
        }
    }
}

public struct ChaptersSpawn : IComponentData
{
    public Entity ChapterEntity;
    public Entity NumberEntity;
    public bool Spawning;
    public bool Number;
    public bool Ready;
}