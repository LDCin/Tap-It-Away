using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("SFX")]
    [SerializeField] private AudioClip defaultSfx;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip cubeMoveSound;
    [SerializeField] private AudioClip cubeBlockSound;
    [SerializeField] private AudioClip levelSuccessSound;
    [SerializeField] private AudioClip levelFailSound;
    [SerializeField] private AudioClip coinFlySound;

    public override void Awake()
    {
        base.Awake();
        if (Instance != this)
        {
            return;
        }
    }

    private void Start()
    {
        ApplySettingsFromUserDataAsync().Forget();
    }

    private void OnEnable()
    {
        DataManager.OnSettingsChanged += ApplySettingsFromUserData;
        InputManager.OnTapCube += HandleTapCube;
        CubeMover.OnCubeBlock += PlayCubeBlockSound;
        LevelManager.OnLevelCompleted += PlayLevelSuccessSound;
        LevelManager.OnLevelFailed += PlayLevelFailSound;
    }

    private void OnDisable()
    {
        DataManager.OnSettingsChanged -= ApplySettingsFromUserData;
        InputManager.OnTapCube -= HandleTapCube;
        CubeMover.OnCubeBlock -= PlayCubeBlockSound;
        LevelManager.OnLevelCompleted -= PlayLevelSuccessSound;
        LevelManager.OnLevelFailed -= PlayLevelFailSound;
    }

    private void ApplySettingsFromUserData()
    {
        DataManager dataManager = DataManager.Instance;
        if (dataManager == null)
        {
            Debug.LogWarning("AudioManager: DataManager not found, defaulting audio enabled.");
            SetBgmEnabled(true);
            SetSfxEnabled(true);
            return;
        }

        SetBgmEnabled(dataManager.BgmEnabled);
        SetSfxEnabled(dataManager.SfxEnabled);
    }

    private async UniTaskVoid ApplySettingsFromUserDataAsync()
    {
        await UniTask.WaitUntil(() => DataManager.Instance != null);

        ApplySettingsFromUserData();
    }

    private void SetBgmEnabled(bool enabled)
    {
        if (bgmSource == null)
        {
            Debug.Log("Not Found: BGM");
            return;
        }

        bgmSource.loop = true;
        bgmSource.mute = !enabled;

        if (enabled && !bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    private void SetSfxEnabled(bool enabled)
    {
        if (sfxSource == null)
        {
            Debug.Log("Not Found: SFX");
            return;
        }

        sfxSource.mute = !enabled;
    }

    public void PlayBGM()
    {
        if (bgmSource == null)
        {
            Debug.Log("Not Found: BGM");
            return;
        }

        DataManager.Instance?.SetBgmEnabled(true);
    }

    public void StopBGM()
    {
        if (bgmSource == null)
        {
            Debug.Log("Not Found: BGM");
            return;
        }

        DataManager.Instance?.SetBgmEnabled(false);
    }

    public void ChangeBGMState()
    {
        DataManager dataManager = DataManager.Instance;
        if (dataManager == null)
        {
            Debug.LogWarning("AudioManager: DataManager not found.");
            return;
        }

        dataManager.ToggleBgmEnabled();
    }

    public void PlaySFX()
    {
        if (sfxSource == null)
        {
            Debug.Log("Not Found: SFX");
            return;
        }

        DataManager.Instance?.SetSfxEnabled(true);
    }

    public void StopSFX()
    {
        if (sfxSource == null)
        {
            Debug.Log("Not Found: SFX");
            return;
        }

        DataManager.Instance?.SetSfxEnabled(false);
    }

    public void StopCurrentSFX()
    {
        if (sfxSource == null)
        {
            Debug.Log("Not Found: SFX");
            return;
        }

        sfxSource.Stop();
    }

    public void ChangeSFXState()
    {
        DataManager dataManager = DataManager.Instance;
        if (dataManager == null)
        {
            Debug.LogWarning("AudioManager: DataManager not found.");
            return;
        }

        dataManager.ToggleSfxEnabled();
    }

    public void PlayDefaultSfx()
    {
        PlayOneShot(defaultSfx, "Default SFX");
    }

    public void RegisterButtonClickSounds(Transform root)
    {
        if (root == null)
        {
            return;
        }

        Button[] buttons = root.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            button.onClick.RemoveListener(PlayButtonClickSound);
            button.onClick.AddListener(PlayButtonClickSound);
        }
    }

    public void PlayButtonClickSound()
    {
        PlayOneShot(buttonClickSound != null ? buttonClickSound : defaultSfx, "Button Click Sound");
    }

    public void PlayCubeMoveSound()
    {
        PlayOneShot(cubeMoveSound != null ? cubeMoveSound : defaultSfx, "Cube Move Sound");
    }

    public void PlayCubeBlockSound()
    {
        PlayOneShot(cubeBlockSound != null ? cubeBlockSound : defaultSfx, "Cube Block Sound");
    }

    public void PlayLevelSuccessSound()
    {
        PlayOneShot(levelSuccessSound != null ? levelSuccessSound : defaultSfx, "Level Success Sound");
    }

    public void PlayLevelFailSound()
    {
        PlayOneShot(levelFailSound != null ? levelFailSound : defaultSfx, "Level Fail Sound");
    }

    public void PlayCoinFlySound()
    {
        PlayOneShot(coinFlySound != null ? coinFlySound : defaultSfx, "Coin Fly Sound");
    }

    private void HandleTapCube(CubeMover cubeMover)
    {
        PlayCubeMoveSound();
    }

    private void PlayOneShot(AudioClip clip, string clipName)
    {
        if (sfxSource == null || clip == null)
        {
            Debug.Log("Not Found: " + clipName);
            return;
        }

        sfxSource.PlayOneShot(clip);
    }
}
