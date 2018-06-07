using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    public Color m_PlayerColor;            
    public Transform m_SpawnPoint;         
    [HideInInspector] public int m_PlayerNumber;             
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public GameObject m_Instance;          
    [HideInInspector] public int m_Wins;
    //should be m_check..
    //[HideInInspector] bool[] checkpointsArray;


    private RideMovement m_Movement;       
    //private TankShooting m_Shooting;
    private GameObject m_CanvasGameObject;
    //my addition


    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<RideMovement>();
        //m_Shooting = m_Instance.GetComponent<TankShooting>();
        //m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        //m_Shooting.m_PlayerNumber = m_PlayerNumber;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }

        //my addition, may need to make the checkpoint array size a var in TankManager's constructor
        //sent by GameManger
        //checkpointsArray = new bool[2];
        resetCheckpoints();
}


    public void DisableControl()
    {
        m_Movement.enabled = false;
        //m_Shooting.enabled = false;

        //m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        m_Movement.enabled = true;
        //m_Shooting.enabled = true;

        //m_CanvasGameObject.SetActive(true);
    }


    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }

    //my addition
    //public void passedCheckpoint(int checkpointNumber)
    //{
    //   m_Movement.m_Checkpoints[checkpointNumber] = true;
    //}
    public bool passedAllCheckpoints()
    {
        for (int i = 0; i < m_Movement.m_Checkpoints.Length; i++)
        {
            if (m_Movement.m_Checkpoints[i].Equals(false)) return false;
        }
        return true;
    }
    public void resetCheckpoints()
    {
        for (int i = 0; i < m_Movement.m_Checkpoints.Length; i++)
        {
            m_Movement.m_Checkpoints[i] = false;
        }
    }
}
