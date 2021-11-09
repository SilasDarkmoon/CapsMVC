using System.Collections;
using UnityEngine;
using Capstones.UnityEngineEx;

[RequireComponent(typeof(AudioSource))]
public class AudioMixerPlayer : AudioPlayer
{
    private string audioPath;
    private float runTime;
    private float endTime;

    public IEnumerator PlayAudio(string path, float volume, bool loop = false, float pitch = 1)
    {
        ClipVolume = volume;
        endTime = 0;
        var clip = ResManager.LoadRes(path, typeof(AudioClip)) as AudioClip;
        if (clip)
        {
            audioPath = path;
            audioSource.clip = clip;
            ApplyVolume();
            audioSource.time = 0;
            audioSource.pitch = pitch;

            AudioMixerConfig(pitch);
            audioSource.Play();
            audioSource.loop = loop;
            yield return new AudioPlayEndYieldInstruction(audioSource);
        }
        else
        {
            PlatDependant.LogError("Audio clip not found, path :" + path);
        }
    }

    public void PlayAudioInstantly(string path, float volume, bool loop = false, float pitch = 1)
    {
        PlayAudio(path, volume, loop, pitch).MoveNext();
    }

    public void AudioMixerConfig(float pitch = 1)
    {
        if (audioSource.outputAudioMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup.audioMixer.SetFloat("MasterPitch", 1f / pitch);
            audioSource.outputAudioMixerGroup.audioMixer.SetFloat("MasterVolume", pitch);
        }
    }

    private void AudioScheduledConfigure(float startTime, float playTime, bool loop = false, float pitch = 1)
    {
        var expand = (pitch < 1) ? (1 / pitch) : 1;
        ApplyVolume();
        audioSource.time = startTime;
        audioSource.pitch = pitch;

        AudioMixerConfig(pitch);
        audioSource.Play();
        audioSource.SetScheduledEndTime((AudioSettings.dspTime + playTime) * expand);
        audioSource.loop = loop;
    }

    public IEnumerator PlayAudioScheduled(string path, float volume, float startTime, float playTime, bool loop = false, float pitch = 1)
    {
        ClipVolume = volume;
        endTime = playTime;
        if (audioPath == path && audioSource.clip)
        {
            AudioScheduledConfigure(startTime, playTime, loop, pitch);
            yield return new AudioPlayEndYieldInstruction(audioSource);
        }
        else
        {
            var clip = ResManager.LoadRes(path, typeof(AudioClip)) as AudioClip;
            if (clip)
            {
                audioPath = path;
                audioSource.clip = clip;
                AudioScheduledConfigure(startTime, playTime, loop, pitch);
                yield return new AudioPlayEndYieldInstruction(audioSource);
            }
            else
            {
                PlatDependant.LogError("Audio clip not found, path :" + path);
            }
        }
    }

    public void PlayAudioScheduledInstantly(string path, float volume, float startTime, float playTime, bool loop = false, float pitch = 1)
    {
        PlayAudioScheduled(path, volume, startTime, playTime, loop, pitch).MoveNext();
    }

    public void IsPlayingAudioClip(bool isPlaying)
    {
        if (audioSource != null)
        {
            if (isPlaying)
            {
                if (endTime > 0)
                {
                    audioSource.time = runTime;
                    audioSource.Play();
                    audioSource.SetScheduledEndTime(AudioSettings.dspTime + endTime);
                }
                else
                {
                    audioSource.Play();
                }
            }
            else
            {
                runTime = audioSource.time;
                audioSource.Pause();
            }
        }
    }

    public new void ApplyVolume()
    {
        audioSource.volume = clipVolume * GlobalVolume * AudioMixerManager.GetAudioVolume(category);
    }

    private void OnDestroy()
    {
        AudioMixerManager.DestroyPlayer(this.Category);
        audioPath = null;
    }
}
