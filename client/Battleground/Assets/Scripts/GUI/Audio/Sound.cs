using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    [HideInInspector]
    public AudioSource source;

    public void SetAudioSource(AudioSource source) {
        this.source = source;
        this.source.clip = this.clip;
        this.source.volume = this.volume;
    }
}
