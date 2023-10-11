using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string Name;
    public AudioClip Clip;
}


public enum ClipType {PowerButtonClick}

public class GlobalSoundPlayer : MonoBehaviour
{
    [SerializeField] private List<Sound> sounds;

    private static Dictionary<string, AudioClip> clips = new ();

    private static List<AudioSource> sources;

    private static GlobalSoundPlayer inst;

    private void Awake()
    {
        inst = this;
        sounds.ForEach(sound => clips.Add(sound.Name, sound.Clip));
        sources = GetComponentsInChildren<AudioSource>().ToList();
    }

    public static void Play(string clipName)
    {
        var source = sources.Find(source => !source.isPlaying);
        if (source == null) source = sources[0];
        source.clip = clips[clipName];
        source.Play();
    }
}
