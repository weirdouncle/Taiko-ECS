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
using System;

public class NotesAddingScript : MonoBehaviour
{
    [SerializeField] private MusicLoaderScript musicLoaderScript;
    [SerializeField] private GameObject[] Note_pres;
    [SerializeField] private Button[] Buttons;
    private EntityManager entityManager;
    private bool ready = false;

    protected readonly float time_before_start = 1f + (15000f / 120f * (4f / 4f)) * 16f / 1000;

    public void UseLagecy()
    {
        foreach (Button button in Buttons)
            button.gameObject.SetActive(false);
    }

    public void UseDOTS()
    {
        foreach (Button button in Buttons)
            button.gameObject.SetActive(false);

        StartPlay().Forget();
    }

    public async UniTaskVoid SpawnCHapters()
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

    public async UniTaskVoid SpawnNotes()
    {
        Debug.Log($"音符数{LoaderScript.Notes.Count}");
        await UniTask.WaitUntil(() => World.DefaultGameObjectInjectionWorld != null);

        World world = World.DefaultGameObjectInjectionWorld;
        entityManager = world.EntityManager;

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
                note.Spawning = true;
                ecb.SetComponent(entities[0], note);
            }
        }
        // 应用 EntityCommandBuffer
        ecb.Playback(entityManager);
        ecb.Dispose();
    }

    private async UniTaskVoid StartPlay()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(Mathf.Max(0, time_before_start - 0.01f)), true, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
        musicLoaderScript.PlayMusic(false);

        // 创建 EntityCommandBuffer
        EntityCommandBuffer ecb = new(Allocator.Temp);

        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(NoteMoveControll));
        using (var datas = entityQuery.ToComponentDataArray<NoteMoveControll>(Allocator.TempJob))
        using (var entities = entityQuery.ToEntityArray(Allocator.TempJob))
        {
            if (datas.Length > 0 && entities.Length > 0) // 防止数组越界
            {
                NoteMoveControll note = datas[0];
                note.Playing = true;
                note.Ready = false;
                note.LastStart = (float)World.DefaultGameObjectInjectionWorld.Time.ElapsedTime;
                ecb.SetComponent(entities[0], note);
            }
        }
        // 应用 EntityCommandBuffer
        ecb.Playback(entityManager);
        ecb.Dispose();
    }

    private void Update()
    {
        if (!ready && World.DefaultGameObjectInjectionWorld != null)
        {
            EntityQuery entityQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(NoteMoveControll));
            using (var datas = entityQuery.ToComponentDataArray<NoteMoveControll>(Allocator.TempJob))
            using (var entities = entityQuery.ToEntityArray(Allocator.TempJob))
            {
                if (datas.Length > 0 && entities.Length > 0) // 防止数组越界
                {
                    NoteMoveControll note = datas[0];
                    if (note.Ready)
                    {
                        ready = true;
                        foreach (Button button in Buttons)
                            button.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
