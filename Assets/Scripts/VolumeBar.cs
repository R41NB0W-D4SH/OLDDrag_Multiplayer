using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeBar : MonoBehaviour
{
    public Slider Music_Slider;
    public RectMask2D Music_Mask;
    
    public Slider SFX_Slider;
    public RectMask2D SFX_Mask;

    private float hideValue_music;
    private float hideValue_sfx;

    // Update is called once per frame
    void Update()
    {
        //Прогресс бар для музыки
        RectMask2D rectMask_m = Music_Mask.GetComponent<RectMask2D>();
        hideValue_music =  213.1f - (Music_Slider.value * 213.1f);
        rectMask_m.padding = new Vector4(0, 0, hideValue_music, 0);

        //Прогресс бар для звуков
        RectMask2D rectMask_s = SFX_Mask.GetComponent<RectMask2D>();
        hideValue_sfx =  213.1f - (SFX_Slider.value * 213.1f);
        rectMask_s.padding = new Vector4(0, 0, hideValue_sfx, 0);
    }
}
