using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    Audio_Manager audio_manager;
    public GameObject canvas;
    public GameObject design;
    public GameObject Builds;
    public GameObject Battle;

    public void Start()
    {
        audio_manager = GameObject.FindGameObjectWithTag("Audio").GetComponent<Audio_Manager>();
    }
    public void FadeOutUI()
    {
        audio_manager.PlaySFX(audio_manager.ClickSound);
        design.gameObject.SetActive(false);
        canvas.gameObject.SetActive(true);
    }

    public void FadeInUI()
    {
        audio_manager.PlaySFX(audio_manager.ClickSound);
        design.gameObject.SetActive(true);
        canvas.gameObject.SetActive(false);
    }
    public void BattleInui()
    {
        audio_manager.PlaySFX(audio_manager.ClickSound);
        Battle.gameObject.SetActive(true);
        canvas.gameObject.SetActive(false);
    }
    public void BattleOutui()
    {
        audio_manager.PlaySFX(audio_manager.ClickSound);
        canvas.gameObject.SetActive(true);
        Battle.gameObject.SetActive(false);
    }
    public void BuildsFadeIN()
    {
        audio_manager.PlaySFX(audio_manager.ClickSound);
        Builds.gameObject.SetActive(true);
        canvas.gameObject.SetActive(false);
    }
    public void BuildsFadeOut()
    {
        
        audio_manager.PlaySFX(audio_manager.ClickSound);
        Builds.gameObject.SetActive(false);
        canvas.gameObject.SetActive(true);
    }
}
