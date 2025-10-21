using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject canvas;
    public GameObject design;
    public GameObject Builds;
    public GameObject Battle;

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
    public void BattleInui()
    {
        Battle.gameObject.SetActive(true);
        canvas.gameObject.SetActive(false);
    }
    public void BattleOutui()
    {
        canvas.gameObject.SetActive(true);
        Battle.gameObject.SetActive(false);
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
