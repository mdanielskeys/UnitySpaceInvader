using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Audio Events/Looped")]
public class LoopedAudioEvent : AudioEvent
{
    public AudioClip[] clips;
    public float volume;

    public override void Play(AudioSource source)
    {
        if (clips.Length == 0) return;

        source.clip = clips[Random.Range(0, clips.Length)];
        source.loop = true;
        source.volume = volume;

        source.Play();
    }
}
