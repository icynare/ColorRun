using UnityEngine;
using System.Collections;

public class PlayerLocalSetting : Singleton<PlayerLocalSetting> 
{


    public float MusicVoluem; //背景音乐声音大小
    public bool IsMusicSilent; //是否背景音乐静音
    public float SoundVolume; //音效声音大小
    public bool IsSoundSilent; //是否音效静音

	
    public int GameEffect; //游戏特效

	public override void Initialize()
	{
		MusicVoluem = PlayerPrefs.GetFloat("ls.musicVolume", 1f);
		IsMusicSilent = PlayerPrefs.GetInt("ls.isMusicSilent", 0) == 1 ? true : false;
		SoundVolume = PlayerPrefs.GetFloat("ls.soundVolume", 1f);
		IsSoundSilent = PlayerPrefs.GetInt("ls.isSoundSilent", 0) == 1 ? true : false;
		GameEffect = PlayerPrefs.GetInt("ls.gameEffect", 2);
	}

	public void SaveSetting()
	{
		//保存设置
		PlayerPrefs.SetFloat("ls.musicVolume", PlayerLocalSetting.Instance.MusicVoluem);
		PlayerPrefs.SetInt("ls.isMusicSilent", PlayerLocalSetting.Instance.IsMusicSilent ? 1 : 0);
		PlayerPrefs.SetFloat("ls.soundVolume", PlayerLocalSetting.Instance.SoundVolume);
		PlayerPrefs.SetInt("ls.isSoundSilent", PlayerLocalSetting.Instance.IsSoundSilent ? 1 : 0);
		PlayerPrefs.SetInt("ls.gameEffect", PlayerLocalSetting.Instance.GameEffect);

		PlayerPrefs.Save();
	}
}
