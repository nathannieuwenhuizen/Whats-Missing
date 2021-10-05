using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NothingProperty : Property
{
    private void Reset() {
        Word = "nothing";
        AlternativeWords = new string[]{ "none"};
    }
}
