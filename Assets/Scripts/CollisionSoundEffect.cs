using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class CollisionSoundEffect : MonoBehaviour
    {
        public AudioClip audioClip;
        [Range(0.5f, 1f)]
        public float volumeModifier = 1.0f;

        public AudioSource audioSource;

        // Use this for initialization
        void Awake()
        {
            gameObject.AddComponent<AudioSource>();
            audioSource = GetComponent<AudioSource>();
        }

        void Start()
        {
            audioSource.clip = audioClip;
            audioSource.volume = volumeModifier;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PlayAndPause()
        {
           audioSource.Play();
        }
    }
}