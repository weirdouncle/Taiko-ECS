using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class NoteTickScript : MonoBehaviour
{
    public Animator Animator;
    public bool Player2;

    public static int ComboType;
    public static int ComboType2P;
    public static int ComboTypeBig;
    public static int ComboTypeBig2P;

    void Start()
    {
        ComboType = ComboTypeBig = ComboType2P = ComboTypeBig2P = 0;
    }

    public void SetSpeed(float speed)
    {
        Animator.speed = speed;
    }

    public void SendNoteTick(int type)
    {
        if (type > 2)
        {
            ComboType = type == 3 ? 0 : 2;
            ComboTypeBig = type == 3 ? 1 : 0;
            SetIComponentData().Forget();

            if (Player2)
            {
                ComboType2P = type == 3 ? 0 : 2;
                ComboTypeBig2P = type == 3 ? 1 : 0;
            }
        }
    }

    public async UniTaskVoid SetIComponentData()
    {
        await UniTask.WaitUntil(() => World.DefaultGameObjectInjectionWorld != null);

        World world = World.DefaultGameObjectInjectionWorld;
        EntityManager entityManager = world.EntityManager;

        // 创建 EntityCommandBuffer
        EntityCommandBuffer ecb = new(Allocator.Temp);

        // 查询 NotesTickState 组件的实体
        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(NotesTickState));
        using (NativeArray<NotesTickState> datas = entityQuery.ToComponentDataArray<NotesTickState>(Allocator.Temp))
        using (NativeArray<Entity> entities = entityQuery.ToEntityArray(Allocator.Temp))
        {
            if (datas.Length > 0 && entities.Length > 0) // 防止数组越界
            {
                NotesTickState note = datas[0];
                note.Animating = true;
                note.Tick = ComboType;
                note.ComboType = ComboType;
                note.ComboTypeBig = ComboTypeBig;
                ecb.SetComponent(entities[0], note);
            }
        }
        // 应用 EntityCommandBuffer
        ecb.Playback(entityManager);
        ecb.Dispose();
    }
}
