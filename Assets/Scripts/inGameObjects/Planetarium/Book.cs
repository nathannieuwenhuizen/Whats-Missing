using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : PickableRoomObjectThatplaysSound
{
    public override string AudioFile()
    {
        return SFXFiles.book;
    }
    public override float AudioVolume()
    {
        return .8f;
    }
    protected override void Awake() {
        base.Awake();
        flamable = true;
        missingChangeEffect = MissingChangeEffect.dissolve;
    }

    private void Reset() {
        flamable = true;
        missingChangeEffect = MissingChangeEffect.dissolve;

        Word = "Book";
        AlternativeWords = new string[] { "books", "paper" };
    }
}
