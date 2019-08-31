﻿using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour
{
    [SerializeField]
    private RectTransform healthBarRect = null;
    [SerializeField]
    private Text healthText = null;

    private void Start() {
        if(healthBarRect == null) {
            Debug.LogError("STATUS INDICATOR: No health bar object referenced");
        }
        if (healthText == null) {
            Debug.LogError("STATUS INDICATOR: No text object referenced");
        }
    }

    public void SetHealth(int _cur, int _max) {
        float _value = (float)_cur / _max;

        healthBarRect.localScale = new Vector3(_value, healthBarRect.localScale.y, healthBarRect.localScale.z);
        healthText.text = _cur + "/" + _max + "HP";
    }


}