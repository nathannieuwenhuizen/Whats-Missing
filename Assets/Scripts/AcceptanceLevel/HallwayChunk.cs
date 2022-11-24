using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayChunk : MonoBehaviour
{
    [SerializeField]
    private HallwayObject[] hallwayObjects;
    [SerializeField]
    private GameObject roundCeiling;
    [SerializeField]
    private GameObject emptyCeiling;

    public void SetCeiling(bool _offset) {
        roundCeiling.SetActive(_offset);
        emptyCeiling.SetActive(!_offset);
    }
    public void ResetFurniture() {
        foreach(HallwayObject obj in hallwayObjects) {
            obj.RandomnizePosition(Random.value > 0.5f);
        }
    }
}

[System.Serializable]
public class HallwayObject {
    [SerializeField]
    GameObject gameObject;
    [SerializeField]
    bool attachedToWall = true;

    [HideInInspector]
    Vector3 startLocalPos;
    [HideInInspector]
    Vector3 startLocalRotation;

    public void Setup() {
        startLocalPos = gameObject.transform.localPosition;
        startLocalRotation = gameObject.transform.localEulerAngles;
    }

    public void RandomnizePosition(bool flipped) {
        if (startLocalPos == Vector3.zero) Setup();

        if (flipped) {
            gameObject.transform.localPosition = new Vector3(startLocalPos.x, -startLocalPos.y, startLocalPos.z);
            gameObject.transform.localRotation = Quaternion.Euler(startLocalRotation);
            gameObject.transform.Rotate(new Vector3(0,0,180));
        } else {
            gameObject.transform.localPosition = startLocalPos;
            gameObject.transform.localRotation = Quaternion.Euler(startLocalRotation);
        }
        if (!attachedToWall) {
            Vector3 temp =  gameObject.transform.localPosition;
            temp.y = Random.Range(temp.y * .8f,temp.y);
            gameObject.transform.localPosition = temp;
        }
    }
}
