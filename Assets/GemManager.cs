using UnityEngine;

public class GemManager : MonoBehaviour {

    public float m_ColorChangeRateZeroToOne = .05f;

    private Color m_OriginalGemColor;
    private Color m_NewGemColor;
    private int m_lastFramesBrakingValue;
    private RideMovement m_rm;
    private int m_BrakeThreshold;

    // Use this for initialization
    void Start () {
        m_OriginalGemColor = GetComponent<MeshRenderer>().material.color;
        m_NewGemColor = m_OriginalGemColor;
        m_lastFramesBrakingValue = 0;
        m_rm = GetComponentInParent<RideMovement>();
        m_BrakeThreshold = m_rm.m_BrakeBoostThreshold;
    }
	
	// Update is called once per frame
	void Update () {

        int m_BrakingValue = GetComponentInParent<RideMovement>().m_BeenBraking;

        if (m_BrakingValue == 0)
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", m_OriginalGemColor);
            m_NewGemColor = m_OriginalGemColor;
        }
        else if (m_BrakingValue >= m_BrakeThreshold) GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
        
        else if (m_BrakingValue > m_lastFramesBrakingValue)
        {
            m_NewGemColor.r += m_ColorChangeRateZeroToOne;
            m_NewGemColor.g += m_ColorChangeRateZeroToOne;
            m_NewGemColor.b += m_ColorChangeRateZeroToOne;
            GetComponent<MeshRenderer>().material.SetColor("_Color", m_NewGemColor);
        }
        
        m_lastFramesBrakingValue = m_BrakingValue;
    }
}
