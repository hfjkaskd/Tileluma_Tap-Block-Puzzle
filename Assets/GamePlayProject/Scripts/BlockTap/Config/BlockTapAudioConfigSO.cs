using UnityEngine;

namespace BlockTap.Config
{
    [CreateAssetMenu(menuName = "BlockTap/Audio Config", fileName = "BlockTapAudioConfig")]
    public class BlockTapAudioConfigSO : ScriptableObject
    {
        public AudioClip backgroundMusic;
        public AudioClip tapClip;
        public AudioClip eliminateClip;
        public AudioClip collectClip;
        public AudioClip propClip;
        public AudioClip winClip;
    }
}
