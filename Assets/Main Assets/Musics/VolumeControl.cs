using UnityEngine;
using UnityEngine.Audio;

public class VolumeControl : MonoBehaviour
{
    public AudioMixer masterMixer;

    public void SetBGMVolume(float volume)
    {
        // Convert linear 0-1 range to logarithmic -80 to 0 dB
        masterMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
    }
}