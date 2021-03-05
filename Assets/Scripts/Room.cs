using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    
    [SerializeField]
    private Television[] allTelevisions;

    [SerializeField]
    private List<IChangable> allObjects;
    

    public List<IChangable> GetAllObjectsInRoom() {
        List<IChangable> result = new List<IChangable>();
        for(int i = 0; i < transform.childCount; i++) {
            if (transform.GetChild(i).GetComponent<IChangable>() != null) {
                result.Add(transform.GetChild(i).GetComponent<IChangable>());
            }
        }
        return result;
    }

    // Apply the change to the object 
    public void ApplyChange(Television television) {
        foreach (IChangable obj in allObjects)
        {
            if (obj.Word == television.Word) {
                obj.setChange(television.changeType);
            }
        }
    }

    public bool checkIfChangeExist(Television television) {
        return false;
    }
    
}
