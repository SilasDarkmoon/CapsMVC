using System.Collections;
using UnityEngine;
using Capstones.UnityEngineEx;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    private float clipVolume = 1f;
    private float? globalVolume = 1f;
    private string category;
    private string audioPath;
    private AudioSource audioSource = null;

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
            audioPath = path;
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

    public IEnumerator PlayAudioScheduled(string path, float volume, float startTime, float playTime, bool loop = false)
    {
        if (audioPath == path && audioSource.clip)
        {
            ApplyVolume();
            audioSource.time = startTime;
            audioSource.Play();
            audioSource.SetScheduledEndTime(AudioSettings.dspTime + playTime);
            audioSource.loop = loop;
            yield return new AudioPlayEndYieldInstruction(audioSource);
        }
        else
        {
            ClipVolume = volume;
            var clip = ResManager.LoadRes(path, typeof(AudioClip)) as AudioClip;
            if (clip)
            {
                audioPath = path;
                audioSource.clip = clip;
                ApplyVolume();
                audioSource.time = startTime;
                audioSource.Play();
                audioSource.SetScheduledEndTime(AudioSettings.dspTime + playTime);
                audioSource.loop = loop;
                yield return new AudioPlayEndYieldInstruction(audioSource);
            }
            else
            {
                PlatDependant.LogError("Audio clip not found, path :" + path);
            }
        }
    }

    public void PlayAudioScheduledInstantly(string path, float volume, float startTime, float playTime, bool loop = false)
    {
        PlayAudioScheduled(path, volume, startTime, playTime, loop).MoveNext();
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
        audioPath = null;
    }
}
