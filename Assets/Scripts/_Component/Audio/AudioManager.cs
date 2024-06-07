using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMono<AudioManager>
{
    public AudioSource bgmSource; // ��������AudioSource
    public AudioSource sfxSource; // ��ЧAudioSource

    [System.Serializable]
    public class AudioClipItem
    {
        public string name;
        public AudioClip clip;
    }

    public AudioClipItem[] bgmClips;  // �������ּ�������
    public AudioClipItem[] sfxClips;  // ��Ч��������

    // �л���������
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
            Debug.LogWarning($"δ�ҵ�����Ϊ{name}�ı�������");
        }
    }

    // ������Ч
    public void PlaySFX(string name)
    {
        AudioClip clip = GetAudioClip(sfxClips, name);
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"δ�ҵ�����Ϊ{name}����Ч");
        }
    }

    // �������ƻ�ȡAudioClip
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

    // ���ñ�����������
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }

    // ������Ч����
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }
}
