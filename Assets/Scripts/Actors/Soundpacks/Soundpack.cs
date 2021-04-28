using Cavern;
using UnityEngine;

namespace Actors.Soundpacks {
    public class Soundpack : MonoBehaviour {
        public AudioClip[] attacks;
        public AudioClip[] hits;

        public void Play(AudioSource3D source, AudioClip[] soundpack) {
            if (soundpack.Length != 0)
                source.PlayOneShot(soundpack[Random.Range(0, soundpack.Length)]);
        }
    }
}