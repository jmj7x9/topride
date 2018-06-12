using UnityEngine;

public class RideCheckpoints : MonoBehaviour
{

    [HideInInspector] public bool[] m_Checkpoints;
    [HideInInspector] public int m_LapsCompleted;

    // Use this for initialization
    void Start()
    {
        //m_Checkpoints initialized in RideManager
        m_LapsCompleted = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
