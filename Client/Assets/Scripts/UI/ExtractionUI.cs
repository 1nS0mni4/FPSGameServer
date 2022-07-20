using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExtractionUI : MonoBehaviour
{
    private TextMeshProUGUI execTimeText = null;

    private void Awake() {
        execTimeText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetExecTime(float times) {
        execTimeText.text = times.ToString("0.00");
    }
}