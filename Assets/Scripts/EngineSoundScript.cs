using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSoundScript : MonoBehaviour
{

    private float speedCopy;
    private float maxSpeedCopy;
    private float overheatVarCopy;
    private float pitchValue;

    public AudioSource audioSource;

    // Update is called once per frame
    void Update()
    {
        speedCopy = CarController.VelocityLimit;
        maxSpeedCopy = CarController.maxSpeedVariable;
        overheatVarCopy = CarController.overheatVar;

        if (speedCopy <= 2)
            audioSource.pitch = 0.17f;
        else if (speedCopy <= 10)
            audioSource.pitch = speedCopy / 10 * 0.75f;
        else audioSource.pitch = Mathf.Lerp(0, 1, (speedCopy / maxSpeedCopy)) * 0.9f + 0.3f + (overheatVarCopy / 5 * 0.3f);

        //else audioSource.pitch = Mathf.Lerp((speedCopy - 10.1f), (maxSpeedCopy - 10.1f), (speedCopy - 10.1f))/ (maxSpeedCopy - 10.1f) * 0.5f + 0.5f;
    }
}
