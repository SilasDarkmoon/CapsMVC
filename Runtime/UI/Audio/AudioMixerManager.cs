using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using Capstones.UnityEngineEx;

public class AudioMixerManager : AudioManager<AudioMixerPlayer>
{
    public static bool CreatePlayer(string category, bool ignoreClear = false, string mixerPath = "")
    {
        if (playerMap.ContainsKey(category))
        {
            var player = playerMap[category];
            if (!player)
            {
                playerMap.Remove(category);
                volumeMap.Remove(category);
            }
            else
            {
                PlatDependant.LogWarning("Audio Player '" + category + "' already created! Returning original player!");
                return false;
            }
        }

        // Create new Audio Player
        var go = new GameObject("AudioMixerPlayer " + category, typeof(AudioSource), typeof(AudioMixerPlayer));
        if (ignoreClear)
        {
            GameObject.DontDestroyOnLoad(go);
        }

        var audioMixerPlayer = go.GetComponent<AudioMixerPlayer>();
        audioMixerPlayer.Category = category;

        if (mixerPath != "")
        {
            AudioMixer mixer = ResManager.LoadRes(mixerPath) as AudioMixer;
            AudioMixerGroup[] groups = mixer.FindMatchingGroups("Master");
            audioMixerPlayer.AudioSourceComponent.outputAudioMixerGroup = groups[0];
        }
        playerMap.Add(category, audioMixerPlayer);
        string key = prePrefsKey + category;
        float volume = 1.0f;
        if (PlayerPrefs.HasKey(key))
        {
            volume = PlayerPrefs.GetFloat(key);
        }
        else
        {
            PlayerPrefs.SetFloat(key, volume);
        }
        volumeMap[category] = volume;
        audioMixerPlayer.ApplyVolume();

        return true;
    }
}
