using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    [CreateAssetMenu(menuName = "Audio Events/Simple")]
    public class SimpleAudioEvent : AudioEvent
    {
        public AudioClip[] Clips;
        public RangedFloat Volume;

        [MinMaxRange(0, 2)] public RangedFloat Pitch;

        public override void Play(AudioSource source)
        {
            if (Clips.Length == 0) return;

            source.clip = Clips[Random.Range(0, Clips.Length)];
            source.volume = Random.Range(Volume.minValue, Volume.maxValue);
            source.pitch = Random.Range(Pitch.minValue, Pitch.maxValue);

            source.Play();
        }
    }
}
