using System.Collections;
using UnityEngine;
using Capstones.UnityEngineEx;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    protected float clipVolume = 1f;
    protected float? globalVolume = 1f;
    protected string category;
    protected AudioSource audioSource = null;

    public bool isPlaying
    {
        get { return audioSource.isPlaying; }
    }

    public float ClipVolume
    {
        get { return clipVolume; }
        set
        {
            clipVolume = value;
            ApplyVolume();
        }
    }

    public string Category
    {
        get { return category; }
        set
        {
            category = value;
        }
    }

    public AudioSource AudioSourceComponent
    {
        get { return audioSource; }
        set
        {
            audioSource = value;
        }
    }

    public float GlobalVolume
    {
        get
        {
            if (!globalVolume.HasValue)
            {
                globalVolume = 1f;
            }

            return globalVolume.Value;
        }
        set
        {
            globalVolume = value;
            ApplyVolume();
        }
    }

    public float ClipLength
    {
        get
        {
            if (audioSource.clip != null)
                return audioSource.clip.length;
            return 0;
        }
    }

    public float RemainingClipLength
    {
        get
        {
            if (audioSource.loop)
                return -1;
            if (audioSource.clip != null)
                return audioSource.clip.length - audioSource.time;
            return 0;
        }
    }

    public IEnumerator PlayAudio(string path, float volume, bool loop = false)
    {
        ClipVolume = volume;
        var clip = ResManager.LoadRes(path, typeof(AudioClip)) as AudioClip;
        if (clip)
        {
            audioSource.clip = clip;
            ApplyVolume();
            audioSource.Play();
            audioSource.loop = loop;
            yield return new AudioPlayEndYieldInstruction(audioSource);
        }
        else
        {
            PlatDependant.LogError("Audio clip not found, path :" + path);
        }
    }

    public void PlayAudioInstantly(string path, float volume, bool loop = false)
    {
        PlayAudio(path, volume, loop).MoveNext();
    }

    public void Stop()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    public void ApplyVolume()
    {
        audioSource.volume = clipVolume * GlobalVolume * AudioManager.GetAudioVolume(category);
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnDestroy()
    {
        AudioManager.DestroyPlayer(this.Category);
    }
}
