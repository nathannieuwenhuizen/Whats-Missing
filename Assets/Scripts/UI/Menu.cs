using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{

    [SerializeField]
    private GameObject continueButton;

    [SerializeField]
    private GameObject newGameButton;

    private void Awake() {
        SetupPlayButtons();
    }

    private void SetupPlayButtons() {
        object data = SerializationManager.Load(SerializationManager.filePath + "/" + SaveData.FILE_NAME +".save");
        if (data != null && (data as SaveData).roomIndex != 0) {
            continueButton.SetActive(true);
            newGameButton.SetActive(false);
        } else {
            //set new game buttons
            continueButton.SetActive(false);
            newGameButton.SetActive(true);
        }
    }

    public void NewGameSelected() {
        SaveData newSave = new SaveData();
        SerializationManager.Save(SaveData.FILE_NAME, newSave);
        Debug.Log("new save");
    }
}
