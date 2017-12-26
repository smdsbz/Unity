using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blood: MonoBehaviour {
    public Slider lifeSlider;
    public Image BloodImage;
    public float process = 0.7f;
    // Use this for initialization
    void Start () {
        lifeSlider.value = process;
        BloodImage.color = new Color(0.5f, 0.5f, 0.5f, 1);
    }
	
	void Update () {

    }
    public void SetProgress(float progress)
    {
        lifeSlider.value = progress;
        if (process > 0.99)
        {
            BloodImage.color = new Color(0.5f, 0.5f, 0.5f, 1);
        }
        else
        {
            BloodImage.color = new Color(0.5f + 0.3f * (1 - process), 0.5f - 0.3f * (1 - process), 0.5f - 0.3f * (1 - process), 1);
        }
    }
}
