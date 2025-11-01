using TMPro;
using UnityEngine;

public class BattleText : MonoBehaviour
{
   public TextMeshProUGUI textMeshProUGUI;

    public void ChangeText(string newText)
    {
        // A method to change the text dynamically
        textMeshProUGUI.text = newText;
    }
}
