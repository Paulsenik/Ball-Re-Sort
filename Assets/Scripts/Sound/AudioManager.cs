using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance;

    public AudioMixer mixer;

    public Sound[] sounds;

    private Unity.Mathematics.Random rand;

    void Awake() {

        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        rand = new Unity.Mathematics.Random();
        rand.InitState();

        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.audioMixerGroup;
        }

    }

    private void Start() {
        foreach (Sound s in sounds)
            if (s.playOnStart)
                play(s);
    }

    public void play(Sound s) {
        play(s.name);
    }

    public void play(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null && s.source != null)
            s.source.Play();
        else {
            Debug.Log("AudioManager :: couldn't play audio " + s);
        }
    }

    public void playRandom(params string[] name) {
        if (name.Length != 0)
            play(name[rand.NextInt(0, name.Length - 1)]);
    }

    public void setVolume(string audioMixerGroup, float volume) {
        volume = (volume > 1f ? 1 : (volume <= 0f ? 0.0001f : volume));
        mixer.SetFloat(audioMixerGroup, Mathf.Log10(volume) * 20);
    }

    [Serializable]
    public class Sound {

        public String name = "Sound";

        public AudioClip clip;
        public AudioMixerGroup audioMixerGroup;

        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;

        public bool loop = false;
        public bool playOnStart = false;

        [HideInInspector]
        public AudioSource source;
    }

}

