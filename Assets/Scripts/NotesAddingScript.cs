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
using CommonClass;

public class NotesAddingScript : MonoBehaviour
{
    [SerializeField] private MusicLoaderScript musicLoaderScript;
    [SerializeField] private GameObject[] Note_pres;
    [SerializeField] private GameObject Chapter_pre;
    [SerializeField] private Button[] Buttons;
    [SerializeField] private Transform[] Parents;

    private EntityManager entityManager;
    private bool ready = false;
    private bool dots_play;
    private bool legacy_play;
    private List<NoteMoveScript> notes = new List<NoteMoveScript>();
    private List<ChapterLineMoveScript> lines = new List<ChapterLineMoveScript>();

    public static float LastStart;
    public static bool Playing = false;

    protected readonly float time_before_start = 1f + (15000f / 120f * (4f / 4f)) * 16f / 1000;

    public void UseLagecy()
    {
        foreach (Button button in Buttons)
            button.gameObject.SetActive(false);

        legacy_play = true;
        SpawnLegacyNotes().Forget();
    }

    public void UseDOTS()
    {
        foreach (Button button in Buttons)
            button.gameObject.SetActive(false);

        dots_play= true;
        SpawnNotes().Forget();
        SpawnChapters().Forget();
    }

    public async UniTaskVoid SpawnChapters()
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
                chapter.Spawning = true;
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

    private async UniTaskVoid StartDotsPlay()
    {
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

        await UniTask.Delay(TimeSpan.FromSeconds(Mathf.Max(0, time_before_start - 0.01f)), true, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
        musicLoaderScript.PlayMusic(false);
    }

    private void Update()
    {
        if (dots_play && !ready && World.DefaultGameObjectInjectionWorld != null)
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
                        StartDotsPlay().Forget();
                    }
                }
            }
        }
    }

    private async UniTaskVoid StartLegacyPlay()
    {
        Playing = true;
        LastStart = Time.time;
        await UniTask.Delay(TimeSpan.FromSeconds(Mathf.Max(0, time_before_start - 0.01f)), true, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
        musicLoaderScript.PlayMusic(false);
    }

    private async UniTaskVoid SpawnLegacyNotes()
    {
        await UniTask.WaitForEndOfFrame(this, this.GetCancellationTokenOnDestroy());

        foreach (NoteChip chip in LoaderScript.Notes.Values)
        {
            GameObject game = null;
            switch (chip.Type)
            {
                case 1:
                    game = Instantiate(Note_pres[0], Parents[0]);
                    break;
                case 2:
                    game = Instantiate(Note_pres[1], Parents[0]);
                    break;
                case 3:
                    game = Instantiate(Note_pres[2], Parents[0]);
                    break;
                case 4:
                    game = Instantiate(Note_pres[3], Parents[0]);
                    break;
                case 5:
                    Debug.Log(chip.Type);
                    game = Instantiate(Note_pres[4], Parents[0]);
                    break;
                case 6:
                    Debug.Log(chip.Type);
                    game = Instantiate(Note_pres[5], Parents[0]);
                    break;
                case 7:
                    game = Instantiate(Note_pres[6], Parents[0]);
                    break;
            }
            NoteMoveScript script = game.GetComponent<NoteMoveScript>();
            script.Index = chip.Index;
            script.Type = chip.Type;
            script.Chapter = chip.Chapter;
            script.JudgeTime = chip.JudgeTime;
            script.MidTime = chip.MidTime;
            script.EndTime = chip.EndTime;
            script.BranchRelated = chip.BranchRelated;
            script.Branch = chip.Branch;
            script.Bpm = chip.Bpm;
            script.AppearTime = chip.AppearTime;
            script.MoveTime = chip.MoveTime;
            script.Scroll = chip.Scroll;
            script.WaitingTime = chip.WaitingTime;
            script.IsFixedSENote = chip.IsFixedSENote;
            script.Senote = chip.Senote;
            script.fBMSCROLLTime = chip.fBMSCROLLTime;
            script.Z_Value = chip.Z_Value;

            notes.Add(script);
        }

        foreach (NoteMoveScript note in notes)
            note.Prepare();

        foreach (ChapterLine line in LoaderScript.Lines)
        {
            GameObject chapter = Instantiate(Chapter_pre, Parents[1]);
            ChapterLineMoveScript script = chapter.GetComponent<ChapterLineMoveScript>();
            script.Bpm = line.Bpm;
            script.AppearTime = line.AppearTime;
            script.MoveTime = line.MoveTime;
            script.Chapter = line.Chapter;
            script.Scroll = line.Scroll;
            script.JudgeTime = line.JudgeTime;
            script.WaitingTime = line.WaitingTime;
            lines.Add(script);

            script.Prepare();
        }

        await UniTask.DelayFrame(5, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
        StartLegacyPlay().Forget();
    }

    public void OnParseEnd()
    {
        foreach (Button button in Buttons)
            button.gameObject.SetActive(true);
    }
}
