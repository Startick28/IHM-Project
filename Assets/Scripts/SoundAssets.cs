using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundAssets : MonoBehaviour
{
    public static SoundAssets instance;

    public SoundAudioClip[] soundAudioClipsArray;

    [System.Serializable]
    public class SoundAudioClip {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }


    public AudioClip mainMusic;
    private AudioSource musicSource;
    private AudioSource musicSource2;
    private AudioSource sfxSource;

    private bool firstSourcePlaying;


    public float musicVolumeModifier = 1f;
    public float sfxVolumeModifier = 1f;

    public Slider musicSlide;
    public Slider sfxSlide;


    public void Awake()
    {
        if (instance)
        {
            Debug.Log("Il y a déjà une instance de SoundManager " + name);
            Destroy(this);
        }
        DontDestroyOnLoad(this.gameObject);
        instance = this;
        musicSource = this.gameObject.AddComponent<AudioSource>();
        musicSource2 = this.gameObject.AddComponent<AudioSource>();
        sfxSource = this.gameObject.AddComponent<AudioSource>();
            
        musicSource.loop = true;
        musicSource2.loop = true;
        sfxSource.loop = true;
        musicSource.volume = 0.35f;
        musicSource2.volume = 0.35f;
        musicVolumeModifier = 0.35f;
        sfxVolumeModifier = 0.5f;
        sfxSource.volume = 0.8f;
        
        
        PlayMusicWithFade(mainMusic,3f);
    }

    public void Update()
    {
        
    }

    public void changeVolume(float musicValue, float sfxValue)
    {
        musicVolumeModifier = musicValue/10f;
        sfxVolumeModifier = sfxValue/10f;
        
        musicSource.volume = 0.35f * musicVolumeModifier;
        musicSource2.volume = 0.35f * musicVolumeModifier;
        sfxSource.volume = 0.8f * sfxVolumeModifier;
    }

    public void PlayMusic(AudioClip musicClip)
    {
        AudioSource activesource = (firstSourcePlaying) ? musicSource : musicSource2;
        
        activesource.clip = musicClip;
        activesource.volume =  0.7f * musicVolumeModifier;
    }

    public void PlayMusicWithFade(AudioClip newClip, float transitionTime = 1.0f)
    {
        AudioSource activeSource = musicSource;
        if (!activeSource.isPlaying)
        {
            StartCoroutine(UpdateMusicWithFade(activeSource,newClip,transitionTime));
        }
        
    }

    public IEnumerator StopMusicWithFade(float transitionTime = 1.0f) {

        AudioSource activeSource = (firstSourcePlaying) ? musicSource : musicSource2;
        float t = 0f;

        for (t= 0f; t<= transitionTime; t+=Time.deltaTime)
        {
            activeSource.volume = (1-(t/transitionTime)) *  0.7f * musicVolumeModifier;
            yield return null;
        }

        activeSource.Stop();
    }

    public IEnumerator StopSFXWithFade(float transitionTime = 1.0f) {

        AudioSource activeSource = sfxSource;
        float t = 0f;

        for (t= 0f; t<= transitionTime; t+=Time.deltaTime)
        {
            activeSource.volume = (1-(t/transitionTime)) *  0.7f * musicVolumeModifier;
            yield return null;
        }

        activeSource.Stop();
    }

    public void PlayMusicWithCrossFade(AudioClip musicClip, float transitionTime = 1.0f)

    {
        AudioSource activeSource = (firstSourcePlaying) ? musicSource : musicSource2;
        AudioSource newSource = (firstSourcePlaying) ? musicSource2 : musicSource;
        
        firstSourcePlaying = !firstSourcePlaying;

        newSource.clip = musicClip;
        newSource.Play();

        StartCoroutine(UpdateMusicWithCrossFade(activeSource,newSource,transitionTime));
    }
    private IEnumerator UpdateMusicWithFade(AudioSource activeSource, AudioClip newClip, float transitionTime)
    {
        if (!activeSource.isPlaying)
            activeSource.Play();

        float t = 0.0f;

        for (t= 0; t< transitionTime; t+=Time.deltaTime)
        {
            activeSource.volume = (1-(t/transitionTime)) *  0.7f * musicVolumeModifier;
            yield return null;
        }
        activeSource.Stop();
        activeSource.clip = newClip;
        activeSource.Play();

        for (t= 0; t< transitionTime; t+=Time.deltaTime)
        {
            activeSource.volume = (t/transitionTime) *  0.7f * musicVolumeModifier;
            yield return null;
        }
    }
    private IEnumerator UpdateMusicWithCrossFade(AudioSource original, AudioSource newSource, float transitionTime)
    {
        float t = 0f;

        for (t= 0f; t<= transitionTime; t+=Time.deltaTime)
        {
            original.volume = (1-(t/transitionTime)) *  0.7f * musicVolumeModifier;
            newSource.volume = (t / transitionTime) *  0.7f * musicVolumeModifier;
            yield return null;
        }

        original.Stop();
    }
   
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        musicSource2.volume = volume;
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolumeModifier = volume;
    }

    public void ChangeLoop(bool value)
    {
        musicSource.loop = value;
        musicSource2.loop = value; 
    }
}
