using CommonClass;
using Cysharp.Threading.Tasks;
using FMOD;
using Unity.VisualScripting;
using UnityEngine;
using Channel = FMOD.Channel;
using Debug = UnityEngine.Debug;

public class MusicLoaderScript : MonoBehaviour
{
    [SerializeField] private TextAsset Music;
    private PcmData pcmData;
    private FMOD.System system;
    private Channel channel;

    private void Start()
    {
        FMODUnity.RuntimeManager.StudioSystem.getCoreSystem(out system);
        system.setDSPBufferSize(128, 2);
        system.setSoftwareFormat(44100, SPEAKERMODE.STEREO, 128);

        // 获取默认 ChannelGroup
        system.getMasterChannelGroup(out ChannelGroup masterChannelGroup);
        if (!masterChannelGroup.hasHandle())
        {
            Debug.LogError("获取 ChannelGroup 失败！");
            return;
        }

        LoadMusic(Music.bytes, Music.name).Forget();
    }
    
    private async UniTaskVoid LoadMusic(byte[] bytes, string title)
    {
        await UniTask.RunOnThreadPool(() =>
        {
            pcmData = WAV.DecodeOgg(bytes, title);
        }, true, this.GetCancellationTokenOnDestroy());

        PlayMusic(true);
    }

    public void PlayMusic(bool start_pause)
    {
        bool reset = true;
        if (channel.hasHandle() && !start_pause)
        {
            channel.isPlaying(out bool playing);
            if (playing)
                channel.setPaused(false);
                reset = false;
        }
        if (reset)
        {
            Sound sound = WAV.CreateFmodSound(system, pcmData);
            sound.setMode(MODE.LOOP_OFF);
            if (sound.hasHandle())
            {
                sound.getLength(out uint length, TIMEUNIT.MS);
                system.getMasterChannelGroup(out ChannelGroup masterChannelGroup);
                RESULT result = system.playSound(sound, masterChannelGroup, start_pause, out channel);

                channel.setVolume(1);
            }
        }
    }


}
