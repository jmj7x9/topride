using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;              
    public GameObject m_TankPrefab;         
    public TankManager[] m_Tanks;
    //mine
    public int m_NumberOfLaps = 2;
    public int m_NumberOfCheckpoints;


    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;
    //mine
    //private float m_countdownClock;


    private void Start()
    {
        //dont need this if checkpoints are triggers
        //for (int i = 0; i < m_Checkpoints.Length; i++)
        //{
        //    m_Checkpoints[i].GetComponent<Renderer>().enabled = false;
        //}

        m_StartWait = new WaitForSeconds(m_StartDelay);
        //m_countdownClock = 3.00f; //hardcoded a three second countdown
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();

        StartCoroutine(GameLoop());
    }


    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            //my addition
            m_Tanks[i].setNumberOfCheckpoints(m_NumberOfCheckpoints);
            //
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }


    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();
        m_CameraControl.SetStartPositionAndSize();
        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;
        yield return m_StartWait;
    }

    private IEnumerator RoundPlaying()
    {

        int countdownClock = 3;
        while (countdownClock >= 0)
        {
            if (countdownClock == 0)
            {
                m_MessageText.text = "GO";
                EnableTankControl();
                countdownClock = -1;
            }
            else
            {
                m_MessageText.text = countdownClock.ToString();
                countdownClock -= 1;
            }
            yield return new WaitForSeconds(0.5f);
        }

        //EnableTankControl();
        m_MessageText.text = string.Empty;
        while (!finishedAllLaps())
        //while (!crossedAllCheckpoints())
        //while (!OneTankLeft())
        {
            yield return null;
        }

        
    }
    /*
    private IEnumerator countdown()
    {
        float countdownClock = 3.00f;
        while (countdownClock > -.2f)
        {
            if (countdownClock <= 0)
            {
                m_MessageText.text = "GO";
            }
            else
            {
                m_MessageText.text = countdownClock.ToString("N2");
                countdownClock -= .01f;
            }
        }
        yield return null;
    }
    */
    private IEnumerator RoundEnding()
    {
        DisableTankControl();
        m_RoundWinner = null;
        m_RoundWinner = GetRoundWinner();
        if (m_RoundWinner != null)
        {
            m_RoundWinner.m_Wins++;
        }
        m_GameWinner = GetGameWinner();
        string message = EndMessage();
        m_MessageText.text = message;

        yield return m_EndWait;
    }

    /*
    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }
    */
    //my addition
    /*
    private bool crossedAllCheckpoints()
    {
        bool allcheckpoints = false;
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            var tankCollider = m_Tanks[i].m_Instance.GetComponent<Collider>();
            Bounds bounds = tankCollider.bounds;
            for (int j = 0; j < m_Checkpoints.Length; j++)
            {
                var planeBounds = m_Checkpoints[j].GetComponent<Collider>();
                if (bounds.Intersects(planeBounds.bounds))
                {
                    int howManyCheckpointsPassed = m_Tanks[i].passedCheckpoint();
                    if (howManyCheckpointsPassed >= m_Checkpoints.Length)
                    {
                        allcheckpoints = true;
                    }
                }
            }
        }
        return allcheckpoints;
        
    }
    */
    private bool finishedAllLaps()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            bool passedAllCheckpoints = m_Tanks[i].passedAllCheckpoints();
            if (m_Tanks[i].getLapsCompleted() == m_NumberOfLaps) return true;
            //else if passed all checkpoints and not equal number laps, then increase number of laps
            else if (passedAllCheckpoints)
            {
                m_Tanks[i].lapCompleted();
                m_Tanks[i].resetCheckpoints();
            }
        }
        return false;
    }

    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}
