using UnityEngine;

public static class UIVoiceManager
{
    private static float UISoundVolume = 1.0f;

    private static string category = "voice";

    public static void Play(string voice_category, string file, float volume = -1f, bool loop = false, float pitch = 1)
    {
        if (volume == 0f)
        {
            volume = UISoundVolume;
        }

        category = voice_category;
        if (voice_category == "")
        {
            category = "voice";
        }

        if (AudioManager.GetPlayer(category) == null)
        {
            AudioManager.CreatePlayer(category, true);
        }
        AudioManager.GetPlayer(category)
            .PlayAudioInstantly("Game/Audio/UI/" + file, (float)volume, loop, pitch);
    }

    public static void PlayScheduled(string voice_category, string file, float volume = -1f, float startTime = 0f, float endTime = 0f, bool loop = false, float pitch = 1)
    {
        if (volume == 0f)
        {
            volume = UISoundVolume;
        }
        category = voice_category;

        if (AudioManager.GetPlayer(category) == null)
        {
            AudioManager.CreatePlayer(category, true);
        }
        var playTime = endTime - startTime;
        AudioManager.GetPlayer(category)
            .PlayAudioScheduledInstantly("Game/Audio/UI/" + file, (float)volume, startTime, playTime, loop, pitch);
    }

    public static void Stop()
    {
        if (AudioManager.GetPlayer(category) != null)
        {
            AudioManager.GetPlayer(category).Stop();
        }
    }

    public static void IsPlayingAudio(bool isPlaying)
    {
        string[] categorys = new string[] { "voice", "voice_music", "voice_ui" };
        for (int i=0; i< categorys.Length; i++)
        {
            var category = categorys[i];
            if (AudioManager.GetPlayer(category) != null)
            {
                AudioManager.GetPlayer(category).IsPlayingAudioClip(isPlaying);
            }
        }
    }
}
