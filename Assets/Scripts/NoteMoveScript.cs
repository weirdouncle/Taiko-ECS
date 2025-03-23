using CommonClass;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

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

    [SerializeField] private SpriteRenderer Main;
    [SerializeField] private SpriteRenderer[] SeNotes;
    [SerializeField] private Transform[] Bodys;
    [SerializeField] private Sprite[] Sprites;
    private int image_index = -1;

    private void Start()
    {
        //修改SeNote，气球等
        switch (Type)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5://小连打
            case 6://大连打
                float length = (float)((EndTime - JudgeTime) * Bpm * Scroll * (1 + 1.5f)) / 628.7f * 1.5f / 100;
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

        Vector2 randomPointInCircle = Random.insideUnitCircle * Time.deltaTime * 5;
        transform.localPosition += new Vector3(randomPointInCircle.x, randomPointInCircle.y);

        // 检测是否超出屏幕范围
        if (transform.localPosition.x < -6.2 || transform.localPosition.x > 13 ||
            transform.localPosition.y < -6.9 || transform.localPosition.y > 3.9)
        {
            // 如果超出范围，重置位置为 (0, 0)
            transform.localPosition = new Vector3(0, 0, transform.localPosition.z);
        }
    }
}
