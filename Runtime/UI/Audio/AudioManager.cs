using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Capstones.UnityEngineEx;

public class AudioManager
{
    private static string prePrefsKey = "___keyAudioVolume_";

    private static Dictionary<string, AudioPlayer> playerMap = new Dictionary<string, AudioPlayer>();

    /// <summary>
    /// 音量大小记录
    /// </summary>
    private static Dictionary<string, float> volumeMap = new Dictionary<string, float>();

    public static AudioPlayer GetPlayer(string category)
    {
        if (playerMap.ContainsKey(category))
        {
            var player = playerMap[category];
            if (!player)
            {
                playerMap.Remove(category);
                volumeMap.Remove(category);
                return null;
            }
            return player;
        }
        return null;
    }

    public static float GetAudioVolume(string category)
    {
        float valume = 1.0f;
        if (volumeMap.ContainsKey(category))
        {
            valume = volumeMap[category];
        }
        else
        {
            string key = prePrefsKey + category;
            if (PlayerPrefs.HasKey(key))
            {
                volumeMap[category] = PlayerPrefs.GetFloat(key);
                valume = volumeMap[category];
            }
        }

        return valume;
    }

    public static void SetAudioVolume(string category, float value)
    {
        string key = prePrefsKey + category;
        PlayerPrefs.SetFloat(key, value);
        if (!playerMap.ContainsKey(category))
        {
            return;
        }
        volumeMap[category] = value;
        AudioPlayer ap = playerMap[category];
        ap.ApplyVolume();
    }

    public static bool CreatePlayer(string category, bool ignoreClear = false)
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
        var go = new GameObject("AudioPlayer " + category, typeof(AudioSource), typeof(AudioPlayer));
        if (ignoreClear)
        {
            GameObject.DontDestroyOnLoad(go);
        }

        var audioPlayer = go.GetComponent<AudioPlayer>();
        audioPlayer.Category = category;
        playerMap.Add(category, audioPlayer);
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
        audioPlayer.ApplyVolume();

        return true;
    }

    public static void DestroyPlayer(string category, bool destroyTheAudio = false)
    {
        if (!playerMap.ContainsKey(category))
        {
            PlatDependant.LogWarning("Audio Player '"+ category + "' not exist!");
            return;
        }
        if (playerMap[category])
        {
            var player = playerMap[category];
            if (player)
            {
                player.Stop();
                if (destroyTheAudio)
                {
                    Object.Destroy(player);
                }
            }
        }

        playerMap.Remove(category);
        volumeMap.Remove(category);
    }

    public static void RemoveUnusedKeys()
    {
        var keys = playerMap.Keys.ToArray();

        foreach (var key in keys)
        {
            if (playerMap[key] == null)
            {
                playerMap.Remove(key);
                volumeMap.Remove(key);
            }
        }
    }

    public static void DestroyAllPlayers()
    {
        var keys = playerMap.Keys.ToArray();

        foreach (var key in keys)
        {
            Object.Destroy(playerMap[key]);
            playerMap.Remove(key);
            volumeMap.Remove(key);
        }
    }
}
