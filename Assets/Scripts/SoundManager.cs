using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
     public enum Sound {
        Jump,
        DoubleJump,
        Dash,
        Death,
        Spawn,
        Propel,
        Land,
        UI,
        Test
    }

    public static Dictionary<Sound,float> soundTimerDictionary;
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;

    public void Start() {

        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.Propel] = 1f;
    }


    public static void PlaySound(Sound sound, Vector3 position, float volume = 1f) {
        if (CanPlaySound(sound)) {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = position;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(sound);
            audioSource.maxDistance = 100f;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.dopplerLevel = 0f;
            audioSource.volume = volume * SoundAssets.instance.sfxVolumeModifier;
            audioSource.Play();
            Object.Destroy(soundGameObject, audioSource.clip.length);
        }
        
    }

    public static void PlaySound(Sound sound, float volume = 1f) {
        if (CanPlaySound(sound)) {
            if (oneShotGameObject == null) {
                oneShotGameObject = new GameObject("One Shot Sound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
            }
            oneShotAudioSource.PlayOneShot(GetAudioClip(sound),volume * SoundAssets.instance.sfxVolumeModifier);
        }
        
    }

    private static bool CanPlaySound(Sound sound) {
        switch (sound){
            default:
                return true;
            case Sound.Propel:
                if (soundTimerDictionary.ContainsKey(sound)) {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float propelSoundTimerMax = .5f;
                    if (lastTimePlayed + propelSoundTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    } 
                    else return false;
                }
                else return true;
        }
    }

    public static AudioClip GetAudioClip(Sound sound) {
        foreach (SoundAssets.SoundAudioClip soundAudioClip in SoundAssets.instance.soundAudioClipsArray) {
            if (soundAudioClip.sound == sound) {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("Sound "+ sound  + " not found");
        return null;
    }
}
