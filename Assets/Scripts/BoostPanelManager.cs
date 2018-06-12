
using UnityEngine;

public class BoostPanelManager : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        RideMovement rm = other.transform.root.GetComponent("RideMovement") as RideMovement;
        rm.m_OnBoostPanel = true;
    }

    private void OnTriggerExit(Collider other)
    {
        RideMovement rm = other.transform.root.GetComponent("RideMovement") as RideMovement;
        rm.m_OnBoostPanel = false;
    }
}
