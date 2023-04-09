using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUpdate : MonoBehaviour
{

    public Text TooEarlyText;
    public Text InTimeText;
    public Text TooLateText;
    public Text OverheatText;

    public float timer = 0;
    public bool transmissionChangeCopy;

    private float MaxSpeedCopy;
    private float SpeedCopy;
    private bool OHtracker;

    // Update is called once per frame
    void LateUpdate()
    {
        transmissionChangeCopy = CarController.transmissionChange;
        MaxSpeedCopy = CarController.maxSpeedVariable;
        SpeedCopy = CarController.VelocityLimit;
        OHtracker = CarController.overheatBlock;

        if (SpeedCopy > MaxSpeedCopy - 2)
        {
            timer += Time.deltaTime;

            if (transmissionChangeCopy)
            {
                print("SignalRecieved");

                if (timer < 1.0f)
                {
                    TooEarlyText.gameObject.SetActive(true);
                    StartCoroutine("TooEarly");
                    return;
                }
                else if (timer < 2.0f)
                {
                    InTimeText.gameObject.SetActive(true);
                    StartCoroutine("Great");
                    return;
                }
                else
                {
                    TooLateText.gameObject.SetActive(true);
                    StartCoroutine("TooLate");
                    return;
                }
            }
        }
        else
            timer = 0;


        if (OHtracker)
            OverheatText.gameObject.SetActive(true);
        else
            OverheatText.gameObject.SetActive(false);
    }

    IEnumerator TooEarly()
    {

        float counter = Time.realtimeSinceStartup + 1f;
        while (Time.realtimeSinceStartup < counter)
            yield return 0;
        TooEarlyText.gameObject.SetActive(false);
    }

    IEnumerator Great()
    {
       
        float counter = Time.realtimeSinceStartup + 1f;
        while (Time.realtimeSinceStartup < counter)
            yield return 0;
        InTimeText.gameObject.SetActive(false);
    }

    IEnumerator TooLate()
    {
        
        float counter = Time.realtimeSinceStartup + 1f;
        while (Time.realtimeSinceStartup < counter)
            yield return 0;
        TooLateText.gameObject.SetActive(false);
    }
}