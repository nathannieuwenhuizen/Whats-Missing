using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Dropdown))]
public class BaseDropDownMenu : MonoBehaviour
{
    private TMP_Dropdown dropDown;
    private bool canPlayAudio = false;
    // Start is called before the first frame update
    void Start()
    {
        dropDown = GetComponent<TMP_Dropdown>();
        dropDown.onValueChanged.AddListener(OnValueChange);
    }
    public IEnumerator DelayAudioEnabling() {
        yield return new WaitForSeconds(.5f);
        canPlayAudio = true;
    }


    private void OnValueChange(int val) {
        if (canPlayAudio) AudioHandler.Instance.PlayUISound(SFXFiles.ui_button_click);
    }

}
