using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraController : MonoBehaviour
{
    //Variables
    public AudioSource SFX;
    public AudioSource Music;
    public AudioClip Freeze;
    public AudioClip UnFreeze;
    public AudioClip NotDio;
    public AudioClip IntroMusic;
    void Start()
    {
        SFX.volume = PlayerPrefs.GetFloat("SFXVolume")*0.8f;
        Music.volume = PlayerPrefs.GetFloat("MusicVolume");
        GameObject.Find("VolcanoNoise").GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SFXVolume"); //sets volume to the volume inputted in settings
        Music.PlayOneShot(IntroMusic); // Plays intro music at the beginning
        if (PlayerPrefs.GetInt("PostProccessing") == 0) //If PostProccessing set to off then turn it off
        {
            PostProcessVolume Postvolume = GetComponent<PostProcessVolume>();
            Postvolume.enabled = false;
            PostProcessLayer PostLayer = GetComponent<PostProcessLayer>();
            PostLayer.enabled = false;
        }
        else if (PlayerPrefs.GetInt("PostProccessing") == 1) //If PostProccessing set to low then turn all post proccessing to 0.25 intensity
        {
            PostProcessVolume Postvolume = GetComponent<PostProcessVolume>();
            AmbientOcclusion ambient;
            Postvolume.profile.TryGetSettings(out ambient);
            ambient.enabled.value = true;
            ambient.intensity.value = 0.25f;
            Bloom bloom;
            Postvolume.profile.TryGetSettings(out bloom);
            bloom.enabled.value = true;
            bloom.intensity.value = 0.25f;
        }
        else if (PlayerPrefs.GetInt("PostProccessing") == 2) //If PostProccessing set to high then turn all post proccessing to 0.75 intensity
        {
            PostProcessVolume Postvolume = GetComponent<PostProcessVolume>();
            AmbientOcclusion ambient;
            Postvolume.profile.TryGetSettings(out ambient);
            ambient.enabled.value = true;
            ambient.intensity.value = 0.75f;
            Bloom bloom;
            Postvolume.profile.TryGetSettings(out bloom);
            bloom.enabled.value = true;
            bloom.intensity.value = 0.75f;
        }
    }

    public void stop() // Runs coroutine to stop time (accessed from qbert script)
    {
        StartCoroutine(StopTime());
    }
    public IEnumerator StopTime()
    {
        SFX.PlayOneShot(NotDio); //Play freeze sound
        SpawnerScript.timestop = true; // stops spawner script from spawning enemies
        for (int i = 0; i < 100; i++) // Slow down movement to make it look less choppy
        {
            SnakeScript.movementScale = 6.0f;
            EnemyScript.movementScale = 6.0f;
            TimeStopScript.movementScale = 6.0f;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(8f); //Wait 8 seconds for timestop to end
        SFX.PlayOneShot(NotDio); //Play freeze sound again

        for (int i = 0; i < 100; i++) // speed up time for all objects
        {
            
            SnakeScript.movementScale = 0.01f;
            EnemyScript.movementScale = 0.01f;
            TimeStopScript.movementScale = 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        SpawnerScript.timestop = false; // enemies can continue spawning
    }
}
