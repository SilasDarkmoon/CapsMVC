using UnityEngine;

public static class UIVoiceManager
{
    private static float UISoundVolume = 1.0f;

    private static string category = "voice";

    public static void Play(string file, float volume = -1f, bool loop = false)
    {
        if (volume == 0f)
        {
            volume = UISoundVolume;
        }

        if (AudioManager.GetPlayer(category) == null)
        {
            AudioManager.CreatePlayer(category, true);
        }
        AudioManager.GetPlayer(category)
            .PlayAudioInstantly("Game/Audio/UI/" + file, (float)volume, loop);
    }

    public static void PlayScheduled(string file, float volume = -1f, float startTime = 0f, float endTime = 0f, bool loop = false)
    {
        if (volume == 0f)
        {
            volume = UISoundVolume;
        }

        if (AudioManager.GetPlayer(category) == null)
        {
            AudioManager.CreatePlayer(category, true);
        }
        var playTime = endTime - startTime;
        AudioManager.GetPlayer(category)
            .PlayAudioScheduledInstantly("Game/Audio/UI/" + file, (float)volume, startTime, playTime, loop);
    }

    public static void Stop()
    {
        if (AudioManager.GetPlayer(category) != null)
        {
            AudioManager.GetPlayer(category).Stop();
        }
    }
}
