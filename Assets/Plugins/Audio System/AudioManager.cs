using System;
using System.Collections;
using UnityEngine;
using Pelumi.ObjectPool;
using Plugins.Audio_System;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioSource soundEffectPlayer;
    [SerializeField] AudioSource combatSource;
    [SerializeField] AudioSource heroSource;
    [SerializeField] AudioSource itemsSource;
    [SerializeField] AudioSource uiSource;
    [SerializeField] AudioSource environmentSource;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float musicMaxVolume = 1f;

    [Header("Resources")]
    [SerializeField] private AudioBank musicBank;
    [SerializeField] private AudioBank soundEffectBank;
    [SerializeField] private Audio3DPlayer audio3DPlayerPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Init();
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        AdvanceButton.OnAnyButtonClicked += PlayButtonSoundEffect;
    }

    public void Init()
    {
        musicPlayer.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
        soundEffectPlayer.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Sound Effects")[0];
        combatSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Combat")[0];
        heroSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Hero")[0];
        itemsSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Items")[0];
        uiSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("UI")[0];
        environmentSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Environment")[0];
    }

    public void SetMusicVolume(float volume)
    {
       SetVolume("Music Volume", volume);
    }

    public void SetSoundEffectVolume(float volume)
    {
        SetVolume("Sound Effects Volume", volume);
    }

    public void SetMasterVolume(float volume)
    {
        SetVolume("Master Volume", volume);
    }

    public void SetMusicMute(bool mute)
    {
        SetMute("Music Volume", mute);
    }

    public void SetSoundEffectMute(bool mute)
    {
        SetMute("Sound Effects Volume", mute);
    }

    public void SetMute(string key,bool mute)
    {
        audioMixer.SetFloat(key, mute ? -80 : 0);
    }

    public void SetVolume(string key, float volume)
    {
        if (volume <= 0) volume = 0.0001f;
        audioMixer.SetFloat(key, Mathf.Log10(volume) * 20);
    }

    private void PlayButtonSoundEffect()
    {
        PlaySoundEffect("Button Click",SoundEffectCategory.UI);
    }

    private IEnumerator PlayMusicFade(AudioClip audioClip, bool loop = true, float fadeDuration = 1.0f)
    {
        // Fade out the music player
        float elapsedTime = 0f;
        float startVolume = musicPlayer.volume;
        float targetVolume = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            musicPlayer.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            yield return null;
        }

        musicPlayer.volume = 0f; // Ensure the volume is set to the target value
        musicPlayer.Stop();

        // Start the new audio clip
        musicPlayer.clip = audioClip;
        musicPlayer.loop = loop;
        musicPlayer.Play();

        // Fade in the music player
        elapsedTime = 0f;
        startVolume = 0f;
        targetVolume = musicMaxVolume;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            musicPlayer.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            yield return null;
        }

        musicPlayer.volume = musicMaxVolume; // Ensure the volume is set to the target value
    }

    private IEnumerator StopMusicFade()
    {
        float speed = 0.05f;

        while (musicPlayer.volume >= speed)
        {
            musicPlayer.volume -= speed;
            yield return new WaitForSecondsRealtime(0.1f);
        }

        musicPlayer.Stop();
    }

    private IEnumerator BlendTwoMusicRoutine(AudioClip intro, AudioClip loopMusic, bool loop  = true)
    {
        yield return PlayMusicFade(intro, false);
        yield return new WaitForSecondsRealtime(musicPlayer.clip.length - 0.5f);
        yield return PlayMusicFade(loopMusic, loop);
    }

    public static AudioClip GetMusicClip(string audioID)
    {
        if (!InstanceExists()) return null;
        return  Instance.musicBank.GetAsset(audioID);
    } 
    
    public static AudioClip GetSoundEffectClip(string audioID)
    {
        if (!InstanceExists()) return null;
        return Instance.soundEffectBank.GetAsset(audioID);
    }

    public AudioSource GetSfxAudioSource() => soundEffectPlayer;

    public static void PlaySoundEffect(string audioID,SoundEffectCategory category, bool randomPitch = false)
    {
        PlaySoundEffect(GetSoundEffectClip(audioID),category, randomPitch);
    }

    public static void PlaySoundEffect(AudioClip audioClip ,SoundEffectCategory category,bool randomPitch = false)
    {
        if (Instance == null) return;
        Instance.soundEffectPlayer.pitch = randomPitch ? Random.Range(0.8f, 1.2f) : 1;
        switch (category)
        {
            case SoundEffectCategory.Hero:
                Instance.heroSource.PlayOneShot(audioClip);
                break;
            case SoundEffectCategory.Combat:
                Instance.combatSource.PlayOneShot(audioClip);
                break;
            case SoundEffectCategory.Items:
                Instance.itemsSource.PlayOneShot(audioClip);
                break;
            case SoundEffectCategory.UI:
                Instance.uiSource.PlayOneShot(audioClip);
                break;
            case SoundEffectCategory.Environment:
                Instance.environmentSource.PlayOneShot(audioClip);
                break;
        }

        return;
    }

    public static void PlayMusic(string ID, bool loop = true)
    {
        if (!InstanceExists()) return;
        Instance.StopAllCoroutines();
        Instance.StartCoroutine(Instance.PlayMusicFade(GetMusicClip(ID), loop));
    }

    public static void PauseMusic()
    {
        if (!InstanceExists()) return;
        Instance.musicPlayer.Pause();
    }

    public static void ResumeMusic()
    {
        if (!InstanceExists()) return;
        Instance.musicPlayer.UnPause();
    }

    public static void StopMusic()
    {
        if (!InstanceExists()) return;
        Instance.StartCoroutine(Instance.StopMusicFade());
    }

    public static void BlendTwoMusic(string startAudioID, string nextAudioID, bool loop = true)
    {
        if (!InstanceExists()) return;
        Instance.StopAllCoroutines();
        Instance.StartCoroutine(Instance.BlendTwoMusicRoutine(GetMusicClip(startAudioID), GetMusicClip(nextAudioID), loop));
    }

    public static Audio3DPlayer Play3DSoundEffect(string audioID, Vector3 position, float _minDistance = 1, float _maxDistance = 500, float _dopplerLevel = 1, float _spread = 0, AudioRolloffMode _audioRolloffMode = AudioRolloffMode.Linear)
    {
        if (Instance == null) return null;
        Audio3DPlayer audio3DPlayer = ObjectPoolManager.SpawnObject(Instance.audio3DPlayerPrefab);
        audio3DPlayer.transform.position = position;
        audio3DPlayer.PlaySoundEffect(GetSoundEffectClip(audioID), _dopplerLevel, _spread, _audioRolloffMode, _minDistance, _maxDistance);
        return audio3DPlayer;
    }

    private static bool InstanceExists()
    {
        if (Instance == null) Debug.LogError("No Audio Manager in the scene");
        return Instance != null;
    }
}
