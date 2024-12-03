using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;
    public float speed = 1f;
    public bool cooldown = true;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI timer;
    public float maxTime = 70f;
    public float timeRemaining = 70f;
    public GameEnding scripta;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
        timer.text = "0:00";
}

    IEnumerator SpeedDown()
    {
        yield return new WaitForSeconds(2f);
        speed = 1f;
        speedText.text = "Building Stamina...";
        Debug.Log("Cooldown!");
        yield return new WaitForSeconds(5f);
        cooldown = true;
        Debug.Log("Ready!");
        speedText.text = "Ready!";
    }


    void FixedUpdate()
    {

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime; // Decrease timeRemaining by deltaTime
            timer.text = $"Time Left: {timeRemaining:F2} seconds";
        }
        else
        {
            timeRemaining = 0; // Ensure it doesn't go negative
            timer.text = "Time's up!";
            scripta.CaughtPlayer();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            if (cooldown) 
            {
                speed = 2f;
                //gameManager.UpdatePowerupText("Picked up Speed!");
                //thruster.gameObject.SetActive(true);
                Debug.Log("Speed Up!");
                speedText.text = "Running!";
                StartCoroutine(SpeedDown());
                cooldown = false;
            }
        }



        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("IsWalking", isWalking);

        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop();
        }

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);
    }

    void OnAnimatorMove()
    {
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude * speed);
        m_Rigidbody.MoveRotation(m_Rotation);
    }
}
