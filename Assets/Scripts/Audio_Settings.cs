using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Audio_Settings: MonoBehaviour
{
    [Header("Mixers")]
    [SerializeField] public AudioMixer Mixer;


    [Header("Sliders")]
    [SerializeField] private Slider Masterslider;
    [SerializeField] private Slider SFX_slider;
    [SerializeField] private Slider Musicslider;





    [Header("Game_Checks")]

    Audio_Manager audio_manager;



    
    public void OnMusicClicked()
    {
        audio_manager = GameObject.FindGameObjectWithTag("Audio").GetComponent<Audio_Manager>();
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log(currentScene + "This is the current scene");
        LoadVolume();

    }
    private void Start()
    {
        audio_manager = GameObject.FindGameObjectWithTag("Audio").GetComponent<Audio_Manager>();


        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMasterVolume();
            SetMusicVolume();
            SetSFXVolume();
        }

    }

    private void LoadVolume()
    {
        Masterslider.value = PlayerPrefs.GetFloat("MasterVolume");
        SFX_slider.value = PlayerPrefs.GetFloat("SFXVolume");
        Musicslider.value = PlayerPrefs.GetFloat("MusicVolume");
      //SetMasterVolume();
      //SetMusicVolume();
      //SetSFXVolume();
    }
    public void SetMasterVolume()
    {
        float volume = Masterslider.value;
        Mixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
    public void SetSFXVolume()
    {
        float volume = SFX_slider.value;
        Mixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
    public void SetMusicVolume()
    {
        float volume = Musicslider.value;
        Mixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }


}
