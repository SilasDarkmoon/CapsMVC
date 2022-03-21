using UnityEngine;

public static class UIVoiceManager
{
    private static float UISoundVolume = 1.0f;
    private static bool isPlayAwake = false;
    private static string category = "voice";
    private static string mixerPath = "Game/Audio/Mixer/VoiceMixer.mixer";

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

        if (AudioMixerManager.GetPlayer(category) == null)
        {
            AudioMixerManager.CreatePlayer(category, false, mixerPath);
        }
        AudioMixerManager.GetPlayer(category).PlayAudioInstantly("Game/Audio/UI/" + file, (float)volume, loop, pitch, isPlayAwake);
    }

    public static void PlayScheduled(string voice_category, string file, float volume = -1f, float startTime = 0f, float endTime = 0f, bool loop = false, float pitch = 1)
    {
        if (volume == 0f)
        {
            volume = UISoundVolume;
        }
        category = voice_category;

        if (AudioMixerManager.GetPlayer(category) == null)
        {
            AudioMixerManager.CreatePlayer(category, false, mixerPath);
        }
        var playTime = endTime - startTime;
        AudioMixerManager.GetPlayer(category).PlayAudioScheduledInstantly("Game/Audio/UI/" + file, (float)volume, startTime, playTime, loop, pitch, isPlayAwake);
    }

    public static void Stop()
    {
        AudioMixerPlayer mixerPlayer = AudioMixerManager.GetPlayer(category);
        if (mixerPlayer != null)
        {
            mixerPlayer.Stop();
        }
    }

    public static void IsPlayingAudio(bool isPlaying)
    {
        string[] categorys = new string[] { "voice", "voice_music", "voice_ui" };
        for (int i=0; i< categorys.Length; i++)
        {
            var category = categorys[i];
            AudioMixerPlayer mixerPlayer = AudioMixerManager.GetPlayer(category);
            if (mixerPlayer != null)
            {
                //if (mixerPlayer.audioSource != null && mixerPlayer.audioSource.time > 0)
                //{
                //    mixerPlayer.IsPlayingAudioClip(isPlaying);
                //}
            }
        }
    }

    public static void IsPlayAwake(bool isAwake)
    {
        isPlayAwake = isAwake;
    }

    public static void AudioMixerConfig(float pitch = 1)
    {
        string[] categorys = new string[] { "voice", "voice_music", "voice_ui" };
        for (int i = 0; i < categorys.Length; i++)
        {
            var category = categorys[i];
            AudioMixerPlayer mixerPlayer = AudioMixerManager.GetPlayer(category);

            if (mixerPlayer != null)
            {
                if (mixerPlayer.AudioSourceComponent.outputAudioMixerGroup == null)
                {
                    AudioMixerManager.BuildMixerPlayer(mixerPlayer, mixerPath);
                }

                mixerPlayer.AudioSourceComponent.pitch = pitch;
                mixerPlayer.AudioMixerConfig(pitch);
            }
        }
    }
}
