using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip lastSecondsSound;
    [SerializeField] private AudioClip levelUpSound;
    [SerializeField] private AudioClip catSound;
    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioClip fireExplosionSound;

    [SerializeField] private AudioSource audioSource;

    void Awake()
    {
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

    private void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
