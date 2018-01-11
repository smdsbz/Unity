using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blood: MonoBehaviour {
    public Canvas BloodCanvas;
    public Slider lifeSlider;
    public Image BloodImage;
    // Use this for initialization
    void Start () {
		BloodCanvas.gameObject.SetActive(true);
		lifeSlider.value = 1f;
        BloodImage.color = new Color(0.5f, 0.5f, 0.5f, 1);
    }
	
	void Update () {

    }
    public void SetProgress(float progress)
    {
        lifeSlider.value = progress;
        if (progress > 0.99)
        {
            BloodImage.color = new Color(0.5f, 0.5f, 0.5f, 1);
        }
        else
        {
			BloodImage.color = new Color(0.5f + 0.3f * (1 - progress), 0.5f - 0.3f * (1 - progress), 0.5f - 0.3f * (1 - progress), 1);
        }
    }
}
