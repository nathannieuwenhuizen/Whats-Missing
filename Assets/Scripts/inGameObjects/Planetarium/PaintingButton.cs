using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingButton : InteractabelObject
{
    [SerializeField]
    private Painting painting;

    public override void Interact()
    {
        base.Interact();
        painting.Interact();
    }
    private void Reset() {
        Word = "appel";
    }

}
