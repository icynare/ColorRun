using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager:Singleton<AudioManager>
{
    //public AudioManager()
    //{

    //}

    ////AudioSource _background = GameObject.Find("GameEntry/AudioBackground").GetComponent<AudioSource>();
    ////AudioSource _ui = GameObject.Find("GameEntry/AudioUI").GetComponent<AudioSource>();
    //public override void Initialize()
    //{
    //    addEventListener<float>(EventName.SetBackgroundVolume, SetBackgroundVolume);
    //    addEventListener<float>(EventName.SetUIVolume, SetUIVolume);

    //    //_background.volume = PlayerLocalSetting.Instance.MusicVoluem;
    //    //_ui.volume = PlayerLocalSetting.Instance.SoundVolume;
    //}
    
    //public float GetBackgroundVolume() {
    //    return _background.volume;
    //}
    //public float GetUIVolume() {
    //    return _ui.volume;
    //}
    //public bool IsMute
    //{
    //    get
    //    {
    //        return _background.mute;
    //    }
    //}


    //public void SetMute(bool isMute)
    //{
    //    _background.mute = isMute;
    //    _ui.mute = isMute;
    //}

    //public void Init(AudioSource background, AudioSource ui)
    //{
    //    _background = background;
    //    _ui = ui;
    //}

    //public void SetBackgroundVolume(float value)
    //{
    //    if (value > 0) SetMute(false);
    //    _background.volume = value;
    //}

    //public void SetUIVolume(float value)
    //{
    //    if (value > 0) SetMute(false);
    //    _ui.volume = value;
    //}

    //public void PlayBackground(string clip, bool repeat = true)
    //{
    //    _background.clip = ResourceManager.Instance.LoadClip(clip);
    //    _background.loop = repeat;
    //    _background.Play();
    //}

    //public void PlayUIAudio(string clip)
    //{
    //    _ui.clip = ResourceManager.Instance.LoadClip(clip);
    //    _ui.loop = false;
    //    _ui.Play();
    //}

    //public override void UnInitialize()
    //{
    //    removeEventListener<float>(EventName.SetBackgroundVolume, SetBackgroundVolume);
    //    removeEventListener<float>(EventName.SetUIVolume, SetUIVolume);
    //}
}
