using UnityEngine;

public class DonEmotionScript : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer Face;

    public void SetEmotion(int index)
    {
        Face.material.SetInt("_Frame", index);
    }

    public void SetDefaultEmotion()
    {
        SetEmotion(4);
    }
}
