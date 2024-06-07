using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMono<AudioManager>
{
    public AudioSource bgmSource; // 背景音乐AudioSource
    public AudioSource sfxSource; // 音效AudioSource

    [System.Serializable]
    public class AudioClipItem
    {
        public string name;
        public AudioClip clip;
    }

    public AudioClipItem[] bgmClips;  // 背景音乐剪辑数组
    public AudioClipItem[] sfxClips;  // 音效剪辑数组

    // 切换背景音乐
    public void SwitchBGM(string name)
    {
        AudioClip clip = GetAudioClip(bgmClips, name);
        if (clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"未找到名称为{name}的背景音乐");
        }
    }

    // 播放音效
    public void PlaySFX(string name)
    {
        AudioClip clip = GetAudioClip(sfxClips, name);
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"未找到名称为{name}的音效");
        }
    }

    // 根据名称获取AudioClip
    private AudioClip GetAudioClip(AudioClipItem[] clips, string name)
    {
        foreach (var item in clips)
        {
            if (item.name == name)
            {
                return item.clip;
            }
        }
        return null;
    }

    // 设置背景音乐音量
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }

    // 设置音效音量
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }
}
