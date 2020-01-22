using UnityEngine;

public static class UISoundManager
{
    private static float UISoundVolume = 1.0f;
    private static bool IsSet = false;

    //public static void Set()
    //{
    //    IsSet = true;
    //}

    public static void Play(string file, float volume = -1f, bool loop = false)
    {
        //if (!IsSet)
        //{
        //    Set();
        //}
        if (volume == 0f)
        {
            volume = UISoundVolume;
        }

        if (AudioManager.GetPlayer("ui") == null)
        {
            AudioManager.CreatePlayer("ui", true);
        }
        AudioManager.GetPlayer("ui")
            .PlayAudioInstantly("Game/Audio/UI/" + file, (float)volume, loop);
    }

    public static void Stop()
    {
        if (AudioManager.GetPlayer("ui") != null)
        {
            AudioManager.GetPlayer("ui").Stop();
        }
    }
}
