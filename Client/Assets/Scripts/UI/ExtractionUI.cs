using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExtractionUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI execTimeText = null;

    private void Awake() {
        
    }

    public void SetExecTime(float times) {
        execTimeText.text = times.ToString("0.00");
    }
}