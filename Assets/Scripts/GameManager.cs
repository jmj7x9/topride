// code has a basis in code from the "Tanks!" Unity tutorial:
// https://unity3d.com/learn/tutorials/projects/tanks-tutorial/game-managers?playlist=20081

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
    public GameObject m_RidePrefab;
    public RideManager[] m_Rides;
    public int m_NumberOfLaps = 2;
    public int m_NumberOfCheckpoints;


    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private RideManager m_RoundWinner;
    private RideManager m_GameWinner;


    private void Start()
    {

        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllRides();
        SetCameraTargets();

        StartCoroutine(GameLoop());
    }


    private void SpawnAllRides()
    {
        for (int i = 0; i < m_Rides.Length; i++)
        {
            //my addition
            m_Rides[i].setNumberOfCheckpoints(m_NumberOfCheckpoints);
            //
            m_Rides[i].m_Instance =
                Instantiate(m_RidePrefab, m_Rides[i].m_SpawnPoint.position, m_Rides[i].m_SpawnPoint.rotation) as GameObject;
            m_Rides[i].m_PlayerNumber = i + 1;
            m_Rides[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Rides.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Rides[i].m_Instance.transform;
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
        ResetAllRides();
        DisableRideControl();
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
                EnableRideControl();
                countdownClock = -1;
            }
            else
            {
                m_MessageText.text = countdownClock.ToString();
                countdownClock -= 1;
            }
            yield return new WaitForSeconds(0.5f);
        }
        float time = 0.00f;

        //m_MessageText.text = string.Empty;
        m_MessageText.text = time.ToString("F2") + "\n\n\n\n\n";
        while (!finishedAllLaps())
        {
            time += Time.deltaTime;
            m_MessageText.text = time.ToString("F2") + "\n\n\n\n\n";
            yield return null;
        }

        
    }

    private IEnumerator RoundEnding()
    {
        DisableRideControl();
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

    private bool finishedAllLaps()
    {
        for (int i = 0; i < m_Rides.Length; i++)
        {
            bool passedAllCheckpoints = m_Rides[i].passedAllCheckpoints();
            if (m_Rides[i].getLapsCompleted() == m_NumberOfLaps) return true;
            //else if passed all checkpoints and not equal number laps, then increase number of laps
            else if (passedAllCheckpoints)
            {
                m_Rides[i].lapCompleted();
                m_Rides[i].resetCheckpoints();
            }
        }
        return false;
    }

    private RideManager GetRoundWinner()
    {
        for (int i = 0; i < m_Rides.Length; i++)
        {
            if (m_Rides[i].m_Instance.activeSelf)
                return m_Rides[i];
        }

        return null;
    }

    private RideManager GetGameWinner()
    {
        for (int i = 0; i < m_Rides.Length; i++)
        {
            if (m_Rides[i].m_Wins == m_NumRoundsToWin)
                return m_Rides[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Rides.Length; i++)
        {
            message += m_Rides[i].m_ColoredPlayerText + ": " + m_Rides[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }


    private void ResetAllRides()
    {
        for (int i = 0; i < m_Rides.Length; i++)
        {
            m_Rides[i].Reset();
        }
    }


    private void EnableRideControl()
    {
        for (int i = 0; i < m_Rides.Length; i++)
        {
            m_Rides[i].EnableControl();
        }
    }


    private void DisableRideControl()
    {
        for (int i = 0; i < m_Rides.Length; i++)
        {
            m_Rides[i].DisableControl();
        }
    }
}
