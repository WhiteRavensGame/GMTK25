using UnityEditor.Overlays;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySFX(AudioClip audio)
    {
        sfxAudioSource.PlayOneShot(audio);
    }
    public void PlaySFX(AudioClip[] audios)
    {
        AudioClip audio = audios[UnityEngine.Random.Range(0, audios.Length)];
        sfxAudioSource.PlayOneShot(audio);
    }
}
