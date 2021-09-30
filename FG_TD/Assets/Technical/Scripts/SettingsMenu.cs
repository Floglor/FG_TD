using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
   public AudioMixer mainMixer;
   public AudioMixer effectsMixer;
   public AudioMixer musicMixer;

   public void SetMainMixer(float volume)
   {
      mainMixer.SetFloat("volume", volume);
   }

   public void SetEffectsMixer(float volume)
   {
      mainMixer.SetFloat("EffectsVolume", volume);
   }
   
   public void SetMusicMixer(float volume)
   {
      mainMixer.SetFloat("MusicVolume", volume);
   }
}
