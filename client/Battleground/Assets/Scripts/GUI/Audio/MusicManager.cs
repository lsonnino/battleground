using System;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public Sound[] sounds;
    [HideInInspector]
    public string playing;

    [Range(0f, 1f)]
    public float masterVolume = 1f;

    private AudioSource source;

    public static MusicManager instance;

    void Awake() {
        // Avoid having multiple instances of MusicManager
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        this.source = gameObject.GetComponent<AudioSource>();
        this.source.volume = this.masterVolume;
        this.source.loop = true;
    }

    public void SetVolume(float volume) {
        this.masterVolume = volume;
        this.source.volume = this.masterVolume;
    }
    public void Play(string name) {
        Sound s = Array.Find(this.sounds, sound => sound.name == name);
        if (s == null) {
            Debug.Log("[MusicManager] Music " + name + " not found");
            return;
        }

        if (this.source.clip != null) {
            this.source.Stop();
        }
        this.source.clip = s.clip;
        this.source.volume = this.masterVolume * s.volume;
        this.source.Play();

        this.playing = name;
    }
}
