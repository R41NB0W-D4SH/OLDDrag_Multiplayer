using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CarController : NetworkBehaviour
{
    public static float maxSpeedVariable = 20.0f;
    public float accelerationVariable = 30.0f;
    public float steeringVariable = 3.0f;
    public float backwardSpeed = 0.3f;
    public float driftVariable = 0;
    public float lowSpeedSteeringVar = 8.0f;
    public float frictionVariable;
    public float transmissionChangeVariable;

    public Text TooEarlyText;
    public Text InTimeText;
    public Text TooLateText;
    public Text OverheatText;

    public float timer = 0;

    public static bool overheatBlock = false;
    public static bool transmissionChange = false;
    public static float VelocityLimit = 0;
    public static float transmission = 1;
    public static float overheatVar = 0;

    private float accelerationInput = 0;
    private float steeringInput = 0;
    private float rotationAngle = 0;

    Rigidbody2D carRigidbody2D;

    void Awake()
    {
        carRigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Transmission();
    }

    void FixedUpdate()
    {
        ApplyEngineForce();
        DeleteSidewayVelocity();
        ApplySteering();
        Overheat();
    }

    void ApplyEngineForce()
    {
        VelocityLimit = Mathf.Round(Vector2.Dot(transform.up, carRigidbody2D.velocity) * 100f) / 100f;

        if (VelocityLimit > maxSpeedVariable && accelerationInput > 0)
            return;

        if (VelocityLimit < -maxSpeedVariable * backwardSpeed && accelerationInput < 0)
            return;

        if (carRigidbody2D.velocity.sqrMagnitude > maxSpeedVariable * maxSpeedVariable && accelerationInput > 0)
            return;

        if (overheatBlock)
            accelerationInput = 0;

        if (accelerationInput == 0)
            carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 3.0f, Time.fixedDeltaTime * frictionVariable);
        else carRigidbody2D.drag = 0;

        Vector2 engineForceVector = transform.up * accelerationInput * accelerationVariable;
        carRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);

    }


    void Transmission()
    {
        if (VelocityLimit > maxSpeedVariable - 2)
        {
            timer += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space) && transmission < 6)
            {
                maxSpeedVariable += transmissionChangeVariable;
                transmission += 1;

                print("SignalRecieved");

                if (timer < 1.0f)
                {
                    TooEarlyText.gameObject.SetActive(true);
                    StartCoroutine("TooEarly");
                    VelocityLimit -= 4;
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
                    VelocityLimit -= 4;
                    return;
                }
            }

        }

        else
            timer = 0;

        if (VelocityLimit < maxSpeedVariable - (transmissionChangeVariable + 4) && transmission > 1)
        {
            maxSpeedVariable -= transmissionChangeVariable;
            transmission -= 1;
        }
    }

    void ApplySteering()
    {
        float minSpeedBeforeAllowTurningVariable = (carRigidbody2D.velocity.magnitude / lowSpeedSteeringVar);
        minSpeedBeforeAllowTurningVariable = Mathf.Clamp01(minSpeedBeforeAllowTurningVariable);
        rotationAngle -= steeringInput * steeringVariable * minSpeedBeforeAllowTurningVariable;
        carRigidbody2D.MoveRotation(rotationAngle);
    }

    void DeleteSidewayVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody2D.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);

        carRigidbody2D.velocity = forwardVelocity + rightVelocity * driftVariable;
    }

    public void SetInputVector(Vector2 inputVector)
    {

        accelerationInput = inputVector.x;

        steeringInput = inputVector.y;
    }

    void Overheat()
    {
        if (accelerationInput > 0 && VelocityLimit > maxSpeedVariable - 2)
        {
            overheatVar += 0.02f;
        }
        else
        {
            if (overheatVar >= 0.04f) overheatVar -= 0.04f;
        }

        if (overheatVar >= 5f)
        {
            overheatVar = 0f;
            overheatBlock = true;
            OverheatText.gameObject.SetActive(true);
        }

        if (VelocityLimit < 0.1f)
        {
            overheatBlock = false;
            OverheatText.gameObject.SetActive(false);
        }
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
