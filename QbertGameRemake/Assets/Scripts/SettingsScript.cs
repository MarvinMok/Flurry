using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    public Slider sfxSlider;
    public Slider MusicSlider;
    public AudioSource MenuMusicStuff;
    public GameObject SettingsPanel;
    public AudioSource ButtonSound;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetFloat("SFXVolume", 0.5f);
        PlayerPrefs.SetFloat("MusicVolume", 0.5f);
        PlayerPrefs.SetInt("PostProccessing", 0);
        
    }

    private void Update()
    {
        MenuMusicStuff.volume = MusicSlider.value*0.4f;


    }
    public void ReturnToMain()
    {
        ButtonSound.Play();
        SettingsPanel.gameObject.SetActive(false);
    }
    public void GoodGrphics(bool tog)
    {
        ButtonSound.Play();
        PlayerPrefs.SetInt("Graphics", (tog ? 1 : 0));
        Debug.Log(PlayerPrefs.GetInt("Graphics"));
    }
    public void SFX()
    {
        ButtonSound.Play();
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        Debug.Log(PlayerPrefs.GetFloat("SFXVolume"));
    }
    public void Music()
    {
        ButtonSound.Play();
        PlayerPrefs.SetFloat("MusicVolume", MusicSlider.value);
        Debug.Log(PlayerPrefs.GetFloat("MusicVolume"));
    }
    public void Graphics(int index)
    {
        ButtonSound.Play();
        QualitySettings.SetQualityLevel(index);
    }
    public void PostProccessing(int index)
    {
        ButtonSound.Play();
        PlayerPrefs.SetInt("PostProccessing", index);
        Debug.Log(PlayerPrefs.GetInt("PostProccessing"));
    }
}
