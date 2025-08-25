using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject canvas;
    public GameObject design;

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

}
