using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    private const string KEY_MASTER = "MasterVol";
    private const string KEY_MUSIC  = "MusicVol";
    private const string KEY_SFX    = "SFXVol";
    
    [SerializeField] private AudioMixer mixer;

    [SerializeField] private Slider sliderMaster;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSFX;
    
    /* Required scene layout:
     * UI Canvas
     * |- Settings container
     *   |- Sliders
     */

    private float _volumeVal;

    private void Start()
    {
        // Debug.Log("Start AudioSettingsManager LoadVolumeLevel");
        ProcessVolumeLoad(KEY_MASTER, sliderMaster);
        ProcessVolumeLoad(KEY_MUSIC, sliderMusic);
        ProcessVolumeLoad(KEY_SFX, sliderSFX);
        // Debug.Log("Ending AudioSettingsManager LoadVolumeLevel");
    }

    private void ProcessVolumeLoad(string keyName, Slider slider)
    {
        // Debug.Log(keyName + "'s PlayerPrefs loading returns: "+ PlayerPrefs.GetFloat(keyName, 0.5f));

        _volumeVal = PlayerPrefs.GetFloat(keyName, 0.5f);
        slider.value = _volumeVal;
        mixer.SetFloat(keyName, Mathf.Log10(_volumeVal) * 20);
    }

    public void SaveVolumeLevel(int sliderNum, float sliderVal)
    {
        switch (sliderNum)
        {
            case 1:
                // Debug.Log("Saving MasterVol");
                SaveSlider(sliderVal, KEY_MASTER);
                break;
                
            case 2:
                // Debug.Log("Saving MusicVol");
                SaveSlider(sliderVal, KEY_MUSIC);
                break;

            case 3:
                // Debug.Log("Saving SFXVol");
                SaveSlider(sliderVal, KEY_SFX);
                break;

            default:
                break;
        }
        // Debug.Log("AudioSettingsManager.cs, searching for slider num " + sliderNum);
    }

    private void SaveSlider(float sliderVal, string keyName)
    {
        // Debug.Log(keyName + ": Saving value " + sliderVal + " to PlayerPrefs");
        _volumeVal = sliderVal;
        mixer.SetFloat(keyName, Mathf.Log10(_volumeVal) * 20); 
        PlayerPrefs.SetFloat(keyName, _volumeVal);
        // Debug.Log(keyName + ": Saved" + PlayerPrefs.GetFloat(keyName, 0.5f));
    }
    
    public void SetMasterVolume(float vol)
    {
        SaveVolumeLevel(1, vol);
    }

    public void SetMusicVolume(float vol)
    {
        SaveVolumeLevel(2, vol);
    }

    public void SetSFXVolume(float vol)
    {
        SaveVolumeLevel(3, vol);
    }
}