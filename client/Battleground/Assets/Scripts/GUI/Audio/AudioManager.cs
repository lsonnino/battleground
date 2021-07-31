using System;
using UnityEngine;

/*
 * Based on the Brackeys tutorial: https://www.youtube.com/watch?v=6OT43pvUyfY
 */
public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    [Range(0f, 1f)]
    public float masterVolume = 1f;

    public static AudioManager instance;

    void Awake() {
        // Avoid having multiple instances of AudioManager
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in this.sounds) {
            s.SetAudioSource(gameObject.AddComponent<AudioSource>());
            s.source.volume *= this.masterVolume;
        }
    }

    public void SetVolume(float volume) {
        this.masterVolume = volume;

        foreach (Sound s in this.sounds) {
            s.source.volume = s.volume * this.masterVolume;
        }
    }
    public void Play(string name) {
        Sound s = Array.Find(this.sounds, sound => sound.name == name);
        if (s == null) {
            Debug.Log("[AudioManager] Sound " + name + " not found");
            return;
        }

        s.source.Play();
    }
}
