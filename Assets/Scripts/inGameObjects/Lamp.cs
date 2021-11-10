using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : InteractabelObject
{
    [SerializeField]
    private GameObject spotLight;

    [SerializeField]
    private Transform cord;
    
    private Vector3 cordStartPos;

    private bool inAnimation = false;
    private float animationOffset = .0025f;
    private float animationDuration = .5f;

    private bool lampIsOn = true;
    public bool LampIsOn {
        get { return lampIsOn;}
        set { 
            lampIsOn = value; 
            spotLight.SetActive(value);
            AudioHandler.Instance.PlaySound(SFXFiles.lamp_toggle, 1, value ? 1 : .8f);
        }
    }

    private void Start() {
        cordStartPos = cord.localPosition;
        cordStartPos.z += animationOffset;
        cord.localPosition = cordStartPos;
    }

    private IEnumerator CordAnimation() {
        float index = 0; 
        bool toggled = false;
        inAnimation = true;
        while(index < animationDuration) {
            index += Time.deltaTime;
            if (index / animationDuration > .5f && toggled == false) {
                LampIsOn = !LampIsOn;
                toggled = true;
            }
            Vector3 newPos = cordStartPos;
            newPos.z -= Mathf.Sin((index / animationDuration) * Mathf.PI) * animationOffset;
            cord.localPosition = newPos;
            yield return new WaitForEndOfFrame();
        }
        cord.localPosition = cordStartPos;
        inAnimation = false;

    }

    public override void Interact()
    {
        base.Interact();
        ToggleLight();
    }

    private void ToggleLight() {
        if (inAnimation) return;

        Focused = false;
        StartCoroutine(CordAnimation());

    }

    private void Reset() {
        Word = "lamp";
        AlternativeWords = new string[] {"lamps"};
    }
}
