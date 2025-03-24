using CommonClass;
using Unity.Mathematics;
using UnityEngine;

public class NoteMoveScript : MonoBehaviour
{
    public int Index;
    public int Type;
    public int Chapter;
    public float JudgeTime;
    public float MidTime;
    public float EndTime;
    public bool BranchRelated;
    public CourseBranch Branch;
    public float Bpm;
    public float AppearTime;
    public float MoveTime;
    public float Scroll;
    public int WaitingTime;
    public bool IsFixedSENote;
    public int Senote;
    public float fBMSCROLLTime;
    public float Z_Value;
    public NoteMove.HitNoteResult NoteJudgeState = NoteMove.HitNoteResult.None;

    [SerializeField] private SpriteRenderer Main;
    [SerializeField] private SpriteRenderer[] SeNotes;
    [SerializeField] private Transform[] Bodys;
    [SerializeField] private Sprite[] Sprites;
    private int image_index = -1;

    public void Prepare()
    {
        NoteJudgeState = NoteMove.HitNoteResult.None;
        transform.localPosition = new Vector3((float)(JudgeTime * Bpm * Scroll * (1 + 1.5f)) / 628.7f * 1.5f / 100, 0, Z_Value);
        gameObject.SetActive(true);
        //修改SeNote，气球等
        switch (Type)
        {
            case 1:
                SeNotes[0].sprite = PicsControllScript.SeSprite[Senote <= 3 ? Senote : 0];
                break;
            case 2:
                int index = Mathf.Max(Senote + 1, 4);
                SeNotes[0].sprite = PicsControllScript.SeSprite[index <= 6 ? index : 4];
                break;
            case 3:
                break;
            case 4:
                break;
            case 5://小连打
            case 6://大连打
                float length = (float)((EndTime - JudgeTime) * Bpm * Scroll * (1 + 1.5f)) / 628.7f * 1.5f / 100;
                Debug.Log(length);
                Bodys[0].localScale = new Vector3(length / 8 * 100, 1, 1);
                Bodys[1].localPosition = new Vector3(length, Bodys[1].localPosition.y, 0);
                break;
            case 7://气球
                break;
        }
    }

    void Update()
    {
        //更换音符图片
        if (image_index != NoteTickScript.ComboType)
        {
            image_index = NoteTickScript.ComboType;
            switch (Type)
            {
                case 1:
                case 2:
                case 5://小连打
                case 7://气球
                    Main.sprite = Sprites[NoteTickScript.ComboType];
                    break;
                case 3:
                case 4:
                case 6://大连打
                    Main.sprite = Sprites[NoteTickScript.ComboTypeBig];
                    break;
            }
        }
        if (NotesAddingScript.Playing)
        {
            float time = JudgeTime - (Time.time - NotesAddingScript.LastStart) * 1000;
            if (Type == 7)
            {
                if (time >= 0)
                    transform.localPosition = new Vector3((float)(time * Bpm * Scroll * (1 + 1.5f)) / 628.7f * 1.5f / 100, 0, Z_Value);
                else
                {
                    time = EndTime - (Time.time - NotesAddingScript.LastStart) * 1000;
                    if (time >= 0)
                        transform.localPosition = new Vector3(0, 0, Z_Value);
                    else
                        transform.localPosition = new Vector3((float)(time * Bpm * Scroll * (1 + 1.5f)) / 628.7f * 1.5f / 100, 0, Z_Value);
                }
            }
            else
            {
                transform.localPosition = new float3((float)(time * Bpm * Scroll * (1 + 1.5f)) / 628.7f * 1.5f / 100, 0, Z_Value);
            }
            if (Type <= 4)
            {
                if (time <= 0)      //自动打击
                {
                    NoteJudgeState = NoteMove.HitNoteResult.Perfect;
                    gameObject.SetActive(false);
                    if (Type == 1 || Type == 3)
                        SoundPool.Instance.PlaySound(true);
                    else
                        SoundPool.Instance.PlaySound(false);
                }
                else if (Type <= 4 && time < -125)     //miss逻辑
                {
                    NoteJudgeState = NoteMove.HitNoteResult.Lost;
                    gameObject.SetActive(false);
                }
            }
            if (Type == 5 || Type == 6)
            {
                if ((Scroll >= 0 && Bodys[1].transform.localPosition.x < -7) || (Scroll < 0 && Bodys[1].transform.localPosition.x > 14))
                    gameObject.SetActive(false);
            }
            else if ((Scroll >= 0 && transform.localPosition.x < -7) || (Scroll < 0 && transform.localPosition.x > 14))
                gameObject.SetActive(false);
        }
    }
}
