using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMaster : MonoBehaviour
{
    public List<Sound> initializedSounds;

    public static SoundMaster instance;

    public AudioMixerGroup _audioMixerGroup;

    public bool soundsOff;

    void Awake()
    {
        GenerateAudioSources(initializedSounds);
        instance = this;
    }

    public void Play(string soundName, float pitch = 1f, float volume = 1f)
    {
        if (soundsOff) return;

        Sound playSound = null;

        bool foundEm = false;

        foreach (Sound sound in initializedSounds)
        {
            if (sound.name.Equals(soundName))
            {
                playSound = sound;
                foundEm = true;
            }
        }

        if (!foundEm) return;


        ChooseFreeThenPlay(pitch, volume, playSound);
    }

    public void Play(int number, float pitch = 1f, float volume = 1f)
    {
        if (soundsOff) return;
        if (number > initializedSounds.Count || number < 0) return;
        Sound playSound = null;
        playSound = initializedSounds[number];


        if (playSound == null) return;

        ChooseFreeThenPlay(pitch, volume, playSound);
    }

    private static void ChooseFreeThenPlay(float pitch, float volume, Sound playSound)
    {
        bool sourcesIsFull = true;
        foreach (AudioSource playSoundSource in playSound.sources.Where(playSoundSource => !playSoundSource.isPlaying))
        {
            playSoundSource.pitch = pitch;
            playSoundSource.volume = volume;
            playSoundSource.Play();
            sourcesIsFull = false;
        }

        if (!sourcesIsFull) return;

        float maxTime = Mathf.NegativeInfinity;
        AudioSource maxTimeSource = null;
        foreach (AudioSource playSoundSource in playSound.sources.Where(playSoundSource =>
            maxTime < playSoundSource.time))
        {
            maxTime = playSoundSource.time;
            maxTimeSource = playSoundSource;
        }


        if (maxTimeSource == null) return;
        maxTimeSource.pitch = pitch;
        maxTimeSource.volume = volume;
        maxTimeSource.Play();
    }

    public void GenerateAudioSources(List<Sound> sounds)
    {
        foreach (Sound sound in sounds)
        {
            sound.sources = new List<AudioSource>();
            for (int i = 0; i < sound.sourcesNumber; i++)
            {
                sound.sources.Add(new AudioSource());
                sound.sources[i] = gameObject.AddComponent<AudioSource>();
                sound.sources[i].clip = sound.clip;

                sound.sources[i].outputAudioMixerGroup = _audioMixerGroup;
                if (soundsOff) sound.sources[i].volume = 0;
                else sound.sources[i].volume = sound.volume;
                sound.sources[i].pitch = sound.pitch;
            }
        }
    }
}