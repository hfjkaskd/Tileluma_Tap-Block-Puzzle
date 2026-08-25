using BlockTap.Config;
using UnityEngine;

namespace BlockTap.Gameplay
{
    public class AudioCueController
    {
        private readonly AudioSource audioSource;
        private readonly BlockTapAudioConfigSO config;

        public AudioCueController(Transform parent, BlockTapAudioConfigSO config)
        {
            this.config = config;
            var go = new GameObject("BlockTapAudio");
            go.transform.SetParent(parent, false);
            audioSource = go.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = true;
        }

        public void PlayBgm()
        {
            // if (config.backgroundMusic == null)
            // {
            //     return;
            // }

            // audioSource.clip = config.backgroundMusic;
            // if (!audioSource.isPlaying)
            // {
            //     audioSource.Play();
            // }
        }

        public void PlayTap() => PlayOneShot(config.tapClip);
        public void PlayEliminate() => PlayOneShot(config.eliminateClip);
        public void PlayCollect() => PlayOneShot(config.collectClip);
        public void PlayProp() => PlayOneShot(config.propClip);
        public void PlayWin() => PlayOneShot(config.winClip);

        private void PlayOneShot(AudioClip clip)
        {
            if (clip != null)
            {
                // audioSource.PlayOneShot(clip);

                SoundManager.Instance.PlaySFX(clip);

            }
        }
    }
}
