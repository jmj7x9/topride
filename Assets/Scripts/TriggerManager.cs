﻿using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Transform frontTransform = transform.Find("frontLocation");
        Transform backTransform = transform.Find("backLocation");

        //quickly just puts these transforms relative to plane
        frontTransform.localPosition = new Vector3(0, -10, 0);
        backTransform.localPosition = new Vector3(0, 10, 0);

        Vector3 frontPosition = frontTransform.transform.position;
        Vector3 backPosition = backTransform.transform.position;
        Vector3 othersPosition = other.transform.position;
        if (Vector3.Distance(othersPosition, frontPosition) < Vector3.Distance(othersPosition, backPosition))
        {
            int checkpointNumber = int.Parse(this.name.Substring(1));
            RideCheckpoints rc = other.transform.root.GetComponent("RideCheckpoints") as RideCheckpoints;
            for (int i = 0; i < checkpointNumber; i++)
            {
                if (rc.m_Checkpoints[i] == false) return;
            }
            rc.m_Checkpoints[checkpointNumber] = true;
        }
    }
}
