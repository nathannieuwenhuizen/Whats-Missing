using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeProperty : Property
{

    public override void onMissing()
    {
        base.onMissing();
        Time.timeScale = 0f;
    }
    public override void onAppearing()
    {
        base.onAppearing();
        Time.timeScale = 1;

    }
}
