using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    
    [SerializeField]
    private Television[] televisions;

    [SerializeField]
    private IChangable[] objects;
    

    public void ApplyChange(Television television) {


    }

    public bool checkIfChangeExist(Television television) {
        return false;
    }
    
}
