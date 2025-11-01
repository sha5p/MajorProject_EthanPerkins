using UnityEngine;
using UnityEngine.UI;

public class Stats_Show : MonoBehaviour
{
    public Slider healthBarSlider;

    
    public  void valueChange(float bob)
    {
        healthBarSlider.value = bob;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
