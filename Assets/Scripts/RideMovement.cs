// code has a basis in code from the "Tanks!" Unity tutorial:
// https://unity3d.com/learn/tutorials/projects/tanks-tutorial/tank-creation-control?playlist=20081

using UnityEngine;

public class RideMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;         
    public float m_Speed = 6f;            
    public float m_TurnSpeed = 180f;
    public float m_BonusDecay = 0.95f;
    public float m_BoostBonus = 4;
    public float m_BoostPadBonus = 8;
    //public AudioSource m_MovementAudio;    
    //public AudioClip m_EngineIdling;       
    //public AudioClip m_EngineDriving;      
    //public float m_PitchRange = 0.2f;
    public float m_BrakeSpeed = .9f;
    public float m_MaxBrakePitch = .2f;
    public float m_MaxTurnPitch = 35f;
    public float m_TurnPitchSpeed = 1.5f;
    public int m_BrakeBoostThreshold = 12;
    // BoostPanelManager needs to know
    [HideInInspector] public bool m_OnBoostPanel = false;
    [HideInInspector] public int m_BeenBraking;

    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         
    private float m_MovementInputValue;    
    private float m_TurnInputValue;

    private string m_BrakeName;
    private float m_BrakeInputValue;
    private float m_BrakePull;

    private float m_Bonus;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 2f;
        m_TurnInputValue = 0f;
        m_BrakePull = 1f;
        m_BeenBraking = 0;
        m_Bonus = 1;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;
        m_BrakeName = "brake" + m_PlayerNumber;
    }
    

    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        m_BrakeInputValue = Input.GetAxis(m_BrakeName);

        //EngineAudio();
    }

    /*
    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the topride is moving and what audio is currently playing.
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }

    }
    */

    private void FixedUpdate()
    {
        BrakeAndBoost();
        Move();
        Turn();
    }

    private void BrakeAndBoost()
    {
        if (m_Bonus > 1)
            m_Bonus = (m_Bonus - 1) * m_BonusDecay + 1;
        //m_Bonus *= m_BonusDecay;

        //if on a boost panel when you release the brake i.e., when the brake's input
        // is 0, and the brake's pull is still is active (<1) then increase boost
        if (m_OnBoostPanel && m_BrakePull < 1 && m_BrakeInputValue < 1) m_Bonus = 8;

        //if braking
        if (m_BrakeInputValue > .1f) //&& m_BrakePull > 0f)
        {
            m_BrakePull *= m_BrakeSpeed;
            m_BeenBraking += 1;

            //if (m_Light.intensity < m_BrakeBoostThreshold) m_Light.intensity += 1;

                //Get some initial speed if significantly braked (speed of ~0.0).
                if (m_BrakePull <= .1f)
            {
                m_BrakePull = .2f;
            }
        }

        //if recently released brake
        else if (m_BrakeInputValue <= .1f && m_BrakePull < 1f)
        {
            //if braked enough, add a speed bonus
            if (m_BeenBraking > m_BrakeBoostThreshold && m_Bonus < m_BoostPadBonus)
            {
                m_Bonus = m_BoostBonus; //make bonus a public variable
                //Make visual indicator
            }
            m_BrakePull = 1;
            m_BeenBraking = 0;
            //m_Light.intensity = 0;

        }
    }

    private void Move()
    {
        // Adjust the position of the tank based on the player's movementInputValue, application of brakes, and speed bonuses

        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime * m_BrakePull * m_Bonus;

        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }


    private void Turn()
    {
        // Adjust the rotation of the topride based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        //Bank(); //unwritten, aim is to dip the z-axis of the topride when turning, and "un-dip" the ride when done turning

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);

    }

}
