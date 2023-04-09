using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tahometer : MonoBehaviour
{
    private float speedCopy;
    private float maxSpeedCopy;
    private float transmissionCopy;
    private float overheatVarCopy;
    private float hideValue;

    public RectMask2D Mask;
    public Text SpeedText;
    public Text TransmissionText;

    // Update is called once per frame
    void Update()
    {
        speedCopy = CarController.VelocityLimit;
        maxSpeedCopy = CarController.maxSpeedVariable;
        transmissionCopy = CarController.transmission;
        overheatVarCopy = CarController.overheatVar;

        SpeedText.text = (Mathf.Round(Mathf.Abs(speedCopy))).ToString();
        TransmissionText.text = transmissionCopy.ToString();

        RectMask2D rectMask = Mask.GetComponent<RectMask2D>();

        if (speedCopy == 0)
            hideValue = 158.82f;

        else if (speedCopy < maxSpeedCopy - 2)
        {
            hideValue = 39.705f + (119.115f - (speedCopy / (maxSpeedCopy - 2) * 119.115f));
        }

        else
        {
            hideValue = 39.705f - (39.705f * (Mathf.Round(overheatVarCopy * 7.941f) / 7.941f) / 3.9705f);
        }

        //hideValue =  158.82f - (speedCopy * 158.82f);

        rectMask.padding = new Vector4(0, 0, hideValue, 0);
    }
}
