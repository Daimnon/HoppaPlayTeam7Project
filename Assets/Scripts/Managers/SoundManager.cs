using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance => _instance;

    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip lastSecondsSound;
    [SerializeField] private AudioClip levelUpSound;
    [SerializeField] private AudioClip catSound;
    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioClip fireExplosionSound;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip buttonPlaySound;

    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        _instance = this;

        if (!audioSource)
            audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        EventManager.OnLose += PlayGameOverSound;
        EventManager.OnGrowth += PlayLevelUpSound;
    }

    private void OnDestroy()
    {
        EventManager.OnLose -= PlayGameOverSound;
        EventManager.OnGrowth -= PlayLevelUpSound;
    }

    public void PlayGameOverSound()
    {
        PlaySound(gameOverSound);
    }

    public void PlayLastSecondsSound()
    {
        PlaySound(lastSecondsSound);
    }

    public void PlayLevelUpSound()
    {
        PlaySound(levelUpSound);
    }

    public void PlayCatSound()
    {
        PlaySound(catSound);
    }

    public void PlayCoinSound()
    {
        PlaySound(coinSound);
    }
    
    public void PlayFireExplosionSound()
    {
        PlaySound(fireExplosionSound);
    }

    public void PlayPickupSound()
    {
        PlaySound(pickupSound);
    }

    public void PlayButtonPlay()
    {
        PlaySound(buttonPlaySound);
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
