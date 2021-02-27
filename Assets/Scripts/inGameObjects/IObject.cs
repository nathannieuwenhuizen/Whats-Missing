using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMissable {
    void onMissing();
    void onAppearing();
}


public interface IChangable : IMissable
{
    string Word {get; set; }
}
