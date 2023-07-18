using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Pool;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioBank _musicBank;
    [SerializeField] AudioBank _soundEffectBank;
    [SerializeField] AudioSource _musicPlayer;
    [SerializeField] AudioSource _soundEffectPlayer;
    [SerializeField] Audio3DPlayer audio3DPlayerPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            ChangeMusicWithFade("Menu", true);
        }
    }

    private void ChangeMusic(AudioClip audioClip, bool loop)
    {
        _musicPlayer.clip = audioClip;
        _musicPlayer.loop = loop;
        _musicPlayer.Play();
    }

    private IEnumerator PlayMusicFade(AudioClip audioClip, bool loop = true)
    {
        _musicPlayer.clip = audioClip;
        _musicPlayer.volume = 0;
        _musicPlayer.loop = loop;
        _musicPlayer.Play();
        while (_musicPlayer.volume < 1)
        {
            _musicPlayer.volume += 0.05f;
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    private IEnumerator StopMusicFade()
    {
        float speed = 0.05f;

        while (_musicPlayer.volume >= speed)
        {
            _musicPlayer.volume -= speed;
            yield return new WaitForSecondsRealtime(0.1f);
        }

        _musicPlayer.Stop();
    }

    private IEnumerator ChangeMusicWithFadeRoutine(AudioClip audioClip, bool loop)
    {
        while (_musicPlayer.volume > 0)
        {
            _musicPlayer.volume -= 0.05f;
            yield return new WaitForSecondsRealtime(0.1f);
        }

        _musicPlayer.clip = audioClip;
        _musicPlayer.loop = loop;
        _musicPlayer.Play();

        while (_musicPlayer.volume < 1)
        {
            _musicPlayer.volume += 0.05f;
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    private IEnumerator BlendTwoMusicRoutine(AudioClip intro, AudioClip loopMusic, bool loop  = true)
    {
        ChangeMusic(intro, false);
        yield return new WaitForSecondsRealtime(_musicPlayer.clip.length - 0.5f);
        ChangeMusic(loopMusic, loop);
    }
    public static AudioClip GetMusicClip(string audioID)
    {
        if (!InstanceExists()) return null;
        return  Instance._musicBank.GetAsset(audioID);
    } 
    
    public static AudioClip GetSoundEffectClip(string audioID)
    {
        if (!InstanceExists()) return null;
        return Instance._soundEffectBank.GetAsset(audioID);
    }

    public AudioSource GetSfxAudioSource() => _soundEffectPlayer;

    public static void PlaySoundEffect(string audioID, bool randomPitch = false)
    {
        if (Instance == null) return;
        Instance._soundEffectPlayer.pitch = randomPitch ? Random.Range(0.8f, 1.2f) : 1;
        Instance._soundEffectPlayer.PlayOneShot(GetSoundEffectClip(audioID));
    }

    public static void PlayMusic(string ID, bool loop = true)
    {
        if (!InstanceExists()) return;
        Instance.StartCoroutine(Instance.PlayMusicFade(GetMusicClip(ID), loop));
    }

    public static void PauseMusic()
    {
        if (!InstanceExists()) return;
        Instance._musicPlayer.Pause();
    }

    public static void ResumeMusic()
    {
        if (!InstanceExists()) return;
        Instance._musicPlayer.UnPause();
    }

    public static void StopMusic()
    {
        if (!InstanceExists()) return;
        Instance.StartCoroutine(Instance.StopMusicFade());
    }

    public static void ChangeMusicWithFade(string audioID, bool loop = true)
    {
        if (!InstanceExists()) return;
        Instance.StartCoroutine(Instance.ChangeMusicWithFadeRoutine(GetMusicClip(audioID), loop));
    }

    public static void BlendTwoMusic(string startAudioID, string nextAudioID, bool loop = true)
    {
        if (!InstanceExists()) return;
        Instance.StartCoroutine(Instance.BlendTwoMusicRoutine(GetMusicClip(startAudioID), GetMusicClip(nextAudioID), loop));
    }

    //public static Audio3DPlayer Play3DSoundEffect(string audioID, Vector3 position, float _dopplerLevel = 1, float _spread = 0, AudioRolloffMode _audioRolloffMode = AudioRolloffMode.Linear, float _minDistance = 1, float _maxDistance = 500)
    //{
    //    if (Instance == null) return null;
    //    Audio3DPlayer audio3DPlayer = ObjectPoolManager.SpawnObject(Instance.audio3DPlayerPrefab);
    //    audio3DPlayer.transform.position = position;
    //    audio3DPlayer.PlaySoundEffect(GetSoundEffectClip(audioID),_dopplerLevel,_spread,_audioRolloffMode,_minDistance,_maxDistance);
    //    return audio3DPlayer;
    //}

    //public static Audio3DPlayer Play3DSoundEffect(string audioID, Transform position, float _dopplerLevel = 1, float _spread = 0, AudioRolloffMode _audioRolloffMode = AudioRolloffMode.Linear, float _minDistance = 1, float _maxDistance = 500)
    //{
    //    if (Instance == null) return null;
    //    Audio3DPlayer audio3DPlayer = ObjectPoolManager.SpawnObject(Instance.audio3DPlayerPrefab);
    //    audio3DPlayer.transform.position = position.position;
    //    audio3DPlayer.PlaySoundEffect(GetSoundEffectClip(audioID), _dopplerLevel, _spread, _audioRolloffMode, _minDistance, _maxDistance);
    //    audio3DPlayer.transform.SetParent(position);
    //    return audio3DPlayer;
    //}

    private static bool InstanceExists()
    {
        if (Instance == null) Debug.LogError("No Audio Manager in the scene");
        return Instance != null;
    }
}
