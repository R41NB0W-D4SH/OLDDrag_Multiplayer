using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeScript : MonoBehaviour {

    public Slider SlideVolume;

    public void ChangeVolumeMusicLevel() {
        AudioListener.volume = SlideVolume.value;
    }
}
