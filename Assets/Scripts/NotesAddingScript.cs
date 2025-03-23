using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Cysharp.Threading.Tasks;
using System.Linq;
using Unity.Rendering;
using UnityEngine.UI;

public class NotesAddingScript : MonoBehaviour
{
    public int Count = 200;

    [SerializeField] private GameObject[] Note_pres;
    [SerializeField] private Button[] Buttons;


    public void UseLagecy()
    {
        foreach (Button button in Buttons)
            button.gameObject.SetActive(false);

        for (int i = 0; i < Count; i++)
        {
            int index = UnityEngine.Random.Range(0, 6);
            Instantiate(Note_pres[index], transform);
        }
    }

    public void UseDOTS()
    {
        foreach (Button button in Buttons)
            button.gameObject.SetActive(false);

        SpawnNotes().Forget();
        //SpawnCHapters().Forget();
    }

    private async UniTaskVoid SpawnCHapters()
    {
        await UniTask.WaitUntil(() => World.DefaultGameObjectInjectionWorld != null);

        World world = World.DefaultGameObjectInjectionWorld;
        EntityManager entityManager = world.EntityManager;

        // 创建 EntityCommandBuffer
        EntityCommandBuffer ecb = new(Allocator.Temp);
        // 查询 ChaptersSpawn 组件的实体
        EntityQuery query = entityManager.CreateEntityQuery(typeof(ChaptersSpawn));
        using (var datas = query.ToComponentDataArray<ChaptersSpawn>(Allocator.TempJob))
        using (var entities = query.ToEntityArray(Allocator.TempJob)) // 修正：使用 query，而不是 entityQuery
        {
            if (datas.Length > 0 && entities.Length > 0) // 防止数组越界
            {
                ChaptersSpawn chapter = datas[0];
                chapter.SpawnCount = 100;
                ecb.SetComponent(entities[0], chapter);
            }
        }

        // 应用 EntityCommandBuffer
        ecb.Playback(entityManager);
        ecb.Dispose();
    }

    private async UniTaskVoid SpawnNotes()
    {
        await UniTask.WaitUntil(() => World.DefaultGameObjectInjectionWorld != null);

        World world = World.DefaultGameObjectInjectionWorld;
        EntityManager entityManager = world.EntityManager;

        // 创建 EntityCommandBuffer
        EntityCommandBuffer ecb = new(Allocator.Temp);

        // 查询 NotesSpawn 组件的实体
        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(NotesSpawn));
        using (var datas = entityQuery.ToComponentDataArray<NotesSpawn>(Allocator.TempJob))
        using (var entities = entityQuery.ToEntityArray(Allocator.TempJob))
        {
            if (datas.Length > 0 && entities.Length > 0) // 防止数组越界
            {
                NotesSpawn note = datas[0];
                note.SpawnCount = Count; // 确保 Count 已定义
                ecb.SetComponent(entities[0], note);
            }
        }
        // 应用 EntityCommandBuffer
        ecb.Playback(entityManager);
        ecb.Dispose();
    }
}
