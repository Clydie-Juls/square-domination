using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] musics;
    AudioSource source;

    public static MusicManager _musicManager;

    [HideInInspector]
    public float soundValue = 1;

    Slider volumeSlider;
    Slider soundSlider;

    bool canStopSetValueVolumeSlider;
    bool canStopSetValueSoundSlider;

    void Awake()
    {
        source = GetComponent<AudioSource>();

        if(_musicManager != null && _musicManager != this)
            Destroy(gameObject);
        else
            _musicManager = this;
      

    }
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void SaveSliderValue(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    // Update is called once per frame
    void Update()
    {
        if (volumeSlider == null && SceneManager.GetActiveScene().buildIndex == 0)
        {
            volumeSlider = GameObject.FindGameObjectWithTag("VolumeSlider").GetComponent<Slider>();
            EventTrigger trigger = volumeSlider.gameObject.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.EndDrag;
            entry.callback.AddListener((data) => { SaveSliderValue("Volume",source.volume); });
            trigger.triggers.Add(entry);
            source.volume = PlayerPrefs.GetFloat("Volume", source.volume);
        }
       
        if (soundSlider == null && SceneManager.GetActiveScene().buildIndex == 0)
        {
            soundSlider = GameObject.FindGameObjectWithTag("SoundSlider").GetComponent<Slider>();
            EventTrigger trigger = soundSlider.gameObject.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.EndDrag;
            entry.callback.AddListener((data) => { SaveSliderValue("Sound", soundValue); });
            trigger.triggers.Add(entry);
            soundValue = PlayerPrefs.GetFloat("Sound", soundValue);
        }

        if (volumeSlider != null && !canStopSetValueVolumeSlider)
        {
            volumeSlider.value = source.volume;
            canStopSetValueVolumeSlider = true;
        }
        if (soundSlider != null && !canStopSetValueSoundSlider)
        {
            soundSlider.value = soundValue;
            canStopSetValueSoundSlider = true;
        }

        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            canStopSetValueVolumeSlider = false;
            canStopSetValueSoundSlider = false;
            volumeSlider = null;
            soundSlider = null;
        }

        DontDestroyOnLoad(gameObject);      
        source.clip = musics[SceneManager.GetActiveScene().buildIndex];
        if(!source.isPlaying)
        {
            source.Play();
        }

        if (volumeSlider != null)
            source.volume = volumeSlider.value;

        if (soundSlider != null)
            soundValue = soundSlider.value;       
    }
}
