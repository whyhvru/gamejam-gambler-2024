using System;
using System.Collections;
using UnityEngine;

namespace Module.Presentation.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioPlayer : MonoBehaviour
    {
        private Coroutine _scheduled;

        [field: SerializeField] public AudioSource MusicSource { get; private set; }

        private void Awake()
        {
            MusicSource = GetComponent<AudioSource>();
            MusicSource.loop = true;
            MusicSource.playOnAwake = false;
        }

        public void PlayLoop(AudioClip clip)
        {
            if (clip == null) return;
            if (MusicSource.clip == clip && MusicSource.isPlaying) return;

            CancelScheduled();

            MusicSource.Stop();
            MusicSource.clip = clip;
            MusicSource.loop = true;
            MusicSource.Play();
        }

        public void PlayOnce(AudioClip clip)
        {
            if (clip == null) return;
            MusicSource.PlayOneShot(clip);
        }

        public void PlayNonLoopThen(Action onFinished, float delaySeconds)
        {
            CancelScheduled();
            _scheduled = StartCoroutine(DelayCoroutine(onFinished, delaySeconds));
        }

        public void CancelScheduled()
        {
            if (_scheduled == null) return;
            StopCoroutine(_scheduled);
            _scheduled = null;
        }

        private static IEnumerator DelayCoroutine(Action action, float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            action?.Invoke();
        }
    }
}