using UnityEngine;

public static class StoryBGMManager
{
    private static float storyBGMVolume = 1.0f;

    private static string category = "storyBGM";

    public static void Play(string file, float volume = -1f, bool loop = true)
    {
        if (volume == 0f)
        {
            volume = storyBGMVolume;
        }

        if (AudioManager.GetPlayer(category) == null)
        {
            AudioManager.CreatePlayer(category, false);
        }
        AudioManager.GetPlayer(category).PlayAudioInstantly("Game/Audio/BGM/Story/" + file, (float)volume, loop);
    }

    public static void Stop()
    {
        if (AudioManager.GetPlayer(category) != null)
        {
            AudioManager.GetPlayer(category).Stop();
        }
    }
}
