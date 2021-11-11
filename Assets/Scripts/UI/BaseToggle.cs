using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///<summary>
/// A toggle that plays a sound on click,hover and unhover.
///</summary>
[RequireComponent(typeof(Toggle))]
public class BaseToggle : MonoBehaviour
{
    private Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggle);
    }

    private void OnToggle(bool val) {
        AudioHandler.Instance.PlayUISound(val ? SFXFiles.toggle_on : SFXFiles.toggle_off,  1f, val ?  1f : .8f);
    }
}
