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
        Sprint,
        Slide,
        UI
    }

    public static Dictionary<Sound,float> soundTimerDictionary;
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;

    public static void Initialize() {

        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.Sprint] = 0f;
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
            case Sound.Sprint:
                if (soundTimerDictionary.ContainsKey(sound)) {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = .05f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
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
