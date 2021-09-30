using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ReSharper disable once CheckNamespace

public enum SoundType {
 DeathSound,
 ShotSound,
 EnemySkillSound,
 PlayerSkillSound,
}

[Serializable]
public class Sound
{
    public string name;
    public SoundType soundType;

    public AudioClip clip;

    [Range(0f, 1f)] public float volume = 1f;
    [Range(.1f, 3f)] public float pitch = 1f;

    public int sourcesNumber;

   // [HideInInspector]
    public List<AudioSource> sources;


}
