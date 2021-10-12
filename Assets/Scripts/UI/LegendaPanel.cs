using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LegendaPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;


    public void UpdateText(string value) {
        text.text = value.ToLower();
    }

    private void OnEnable() {
        Legenda.OnFocus += UpdateText;
    }
    private void OnDisable() {
        Legenda.OnFocus -= UpdateText;
    }
}
