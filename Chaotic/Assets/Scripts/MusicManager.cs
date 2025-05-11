using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip initialMusic;
    [SerializeField] private AudioClip successMusic;

    void Awake()
    {
        // Singleton để giữ nhạc xuyên suốt game
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayInitialMusic();
    }

    public void PlayInitialMusic()
    {
        audioSource.clip = initialMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlaySuccessMusic()
    {
        audioSource.clip = successMusic;
        audioSource.loop = true;
        audioSource.Play();
    }
}
