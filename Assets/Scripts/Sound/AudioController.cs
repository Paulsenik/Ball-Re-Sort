using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {
    public void play(string s) {
        AudioManager.instance.play(s);
    }
    public void setVolume(string audioMixerGroup, float volume) {
        AudioManager.instance.setVolume(audioMixerGroup, volume);
    }
}