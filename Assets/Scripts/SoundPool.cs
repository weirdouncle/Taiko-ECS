using CriWare;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundPool : MonoBehaviour
{
    public CriAtomSource Don;
    public CriAtomSource Ka;


    private string don_acb_cuename;
    private string ka_acb_cuename;
    private string scene;

    void Start()
    {
        DelayInit().Forget();
    }

    private async UniTaskVoid DelayInit()
    {
        await UniTask.WaitForEndOfFrame(this, this.GetCancellationTokenOnDestroy());
        string don_note = string.Format("don_{0}", 0);
        string ka_note = string.Format("ka_{0}", 0);

        CriAtomCueSheet sheet = CriAtom.AddCueSheet(don_note, Resources.Load<TextAsset>(string.Format("Notes/{0}", don_note)).bytes, string.Empty);
        sheet.acb.GetCueInfoByIndex(0, out CriAtomEx.CueInfo don_info);

        Don.cueSheet = don_acb_cuename = sheet.name;
        Don.cueName = don_info.name;

        CriAtomCueSheet ka_sheet = CriAtom.AddCueSheet(ka_note, Resources.Load<TextAsset>(string.Format("Notes/{0}", ka_note)).bytes, string.Empty);
        ka_sheet.acb.GetCueInfoByIndex(0, out CriAtomEx.CueInfo ka_info);

        Ka.cueSheet = ka_acb_cuename = ka_sheet.name;
        Ka.cueName = ka_info.name;
        scene = SceneManager.GetActiveScene().name;

        InitSound();
    }

    void OnDestroy()
    {
        CriAtom.RemoveCueSheet(don_acb_cuename);
        CriAtom.RemoveCueSheet(ka_acb_cuename);
    }

    public void PlaySound(bool don)
    {
        if (don)
            Don.PlayDirectly();
        else
            Ka.PlayDirectly();
    }

    private void InitSound()
    {
        Don.Init();
        Ka.Init();
    }
}
