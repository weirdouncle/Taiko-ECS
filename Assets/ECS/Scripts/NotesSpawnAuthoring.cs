using Unity.Entities;
using UnityEngine;

public class NotesSpawnAuthoring : MonoBehaviour
{
    public GameObject Note_pre;
    public GameObject NoteDon_pre;
    public GameObject NoteKa_pre;
    public GameObject NoteBigDon_pre;
    public GameObject NoteBigKa_pre;
    public GameObject Balloon_pre;
    public GameObject Rapid_pre;
    public GameObject BigRapid_pre;
    public GameObject Kusudama_pre;

    public class Baker : Baker<NotesSpawnAuthoring>
    {
        public override void Bake(NotesSpawnAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            Entity prefabEntity = GetEntity(authoring.Note_pre, TransformUsageFlags.Dynamic);
            Entity don = GetEntity(authoring.NoteDon_pre, TransformUsageFlags.Dynamic);
            Entity ka = GetEntity(authoring.NoteKa_pre, TransformUsageFlags.Dynamic);
            Entity big_don = GetEntity(authoring.NoteBigDon_pre, TransformUsageFlags.Dynamic);
            Entity big_ka = GetEntity(authoring.NoteBigKa_pre, TransformUsageFlags.Dynamic);
            Entity balloon = GetEntity(authoring.Balloon_pre, TransformUsageFlags.Dynamic);
            Entity rapid = GetEntity(authoring.Rapid_pre, TransformUsageFlags.Dynamic);
            Entity big_rapid = GetEntity(authoring.BigRapid_pre, TransformUsageFlags.Dynamic);
            Entity kusudama = GetEntity(authoring.Kusudama_pre, TransformUsageFlags.Dynamic);

            AddComponent(entity, new NotesSpawn
            {
                NoteEntity = prefabEntity,
                DonImage = don,
                KaImage = ka,
                BigDonImage = big_don,
                BigKaImage = big_ka,
                BalloonImage = balloon,
                RapidImage = rapid,
                BigRapidImage = big_rapid,
                KusudamaImage = kusudama,
                SetSeNote = false,
                Rapid = false,
            });
            AddComponent(entity, new NotesTickState { LastTick = -1 });
        }
    }
}

public struct NotesSpawn : IComponentData
{
    public Entity NoteEntity;
    public Entity DonImage;
    public Entity KaImage;
    public Entity BigDonImage;
    public Entity BigKaImage;
    public Entity RapidImage;
    public Entity BigRapidImage;
    public Entity BalloonImage;
    public Entity KusudamaImage;
    public int SpawnCount;
    public bool SetSeNote;
    public bool Rapid;
}

public struct NotesTickState : IComponentData
{
    public bool Animating;
    public int Tick;
    public int ComboType;
    public int ComboTypeBig;
    public int LastTick;
}