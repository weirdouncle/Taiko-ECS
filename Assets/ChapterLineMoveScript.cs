using Unity.Mathematics;
using UnityEngine;

public class ChapterLineMoveScript : MonoBehaviour
{
    public double Bpm;
    public float AppearTime;
    public float MoveTime;
    public int Chapter;
    public double Scroll;
    public float JudgeTime;
    public int WaitingTime;

    void Update()
    {
        if (NotesAddingScript.Playing)
        {
            float time = JudgeTime - (Time.time - NotesAddingScript.LastStart) * 1000;
            transform.localPosition = new float3((float)(time * Bpm * Scroll * (1 + 1.5f)) / 628.7f * 1.5f / 100, 0, 0);

            if ((Scroll >= 0 && transform.localPosition.x < -7) || (Scroll < 0 && transform.localPosition.x > 14))
                gameObject.SetActive(false);
        }
    }

    public void Prepare()
    {
        transform.localPosition = new Vector3((float)(JudgeTime * Bpm * Scroll * (1 + 1.5f)) / 628.7f * 1.5f / 100, 0, 0);
        gameObject.SetActive(true);
    }
}
