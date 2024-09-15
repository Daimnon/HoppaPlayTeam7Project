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
        if (!gameOverSound) return;
        PlaySound(gameOverSound);
    }

    public void PlayLastSecondsSound()
    {
        if (!lastSecondsSound) return;
        PlaySound(lastSecondsSound);
    }

    public void PlayLevelUpSound()
    {
        if (!levelUpSound) return;
        PlaySound(levelUpSound);
    }

    public void PlayCatSound()
    {
        if (!catSound) return;
        PlaySound(catSound);
    }

    public void PlayCoinSound()
    {
        if (!coinSound) return;
        PlaySound(coinSound);
    }
    
    public void PlayFireExplosionSound()
    {
        if (!fireExplosionSound) return;
        PlaySound(fireExplosionSound);
    }

    public void PlayPickupSound()
    {
        if (!pickupSound) return;
        PlaySound(pickupSound);
    }

    public void PlayButtonPlay()
    {
        if (!buttonPlaySound) return;
        PlaySound(buttonPlaySound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (!clip) return;
        audioSource.PlayOneShot(clip);
    }
}
