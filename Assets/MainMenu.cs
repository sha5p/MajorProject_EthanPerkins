using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject canvas;
    public GameObject design;
    public GameObject Builds;

    public void FadeOutUI()
    {
        design.gameObject.SetActive(false);
        canvas.gameObject.SetActive(true);
    }

    public void FadeInUI()
    {
        design.gameObject.SetActive(true);
        canvas.gameObject.SetActive(false);
    }
    public void LoadMAP()
    {
        SceneManager.LoadScene("BattleGround");
    }
    public void BuildsFadeIN()
    {
        Builds.gameObject.SetActive(true);
        canvas.gameObject.SetActive(false);
    }
    public void BuildsFadeOut()
    {
        Builds.gameObject.SetActive(false);
        canvas.gameObject.SetActive(true);
    }
}
