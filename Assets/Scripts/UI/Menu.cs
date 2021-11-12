using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    [SerializeField]
    private GameObject continueButton;

    [SerializeField]
    private GameObject newGameButton;

    [SerializeField]
    private Button settingsButton;

    [SerializeField]
    private GameObject newGameWarning;
    [SerializeField]
    private GameObject newGameWarningNoButton;

    private void Awake() {
    }

    private void Start() {
        SetupPlayButtons();
        AudioHandler.Instance.PlayMusic(MusicFiles.menu, .3f);
    }

    private void SetupPlayButtons() {
        object data = SerializationManager.Load(SerializationManager.filePath + "/" + SaveData.FILE_NAME +".save");
        if (data != null && (data as SaveData).roomIndex != 0) {
            continueButton.SetActive(true);
            ControllerCheck.SelectUIGameObject(continueButton.transform.GetChild(0).gameObject, () => {
                EventSystem.current.firstSelectedGameObject =(continueButton.transform.GetChild(0).gameObject);
                settingsButton.navigation = new Navigation(){ mode = Navigation.Mode.Explicit, selectOnUp = continueButton.transform.GetChild(0).GetComponent<Button>()};
            });
            newGameButton.SetActive(false);
        } else {
            //set new game buttons
            continueButton.SetActive(false);
            newGameButton.SetActive(true);
            ControllerCheck.SelectUIGameObject(newGameButton, () => {
                EventSystem.current.firstSelectedGameObject =(newGameButton);
                settingsButton.navigation = new Navigation(){ mode = Navigation.Mode.Explicit,  selectOnUp = newGameButton.GetComponent<Button>()};
            });


        }
    }

    public void OpenNewGameWarning() {
        newGameWarning.SetActive(true);
        ControllerCheck.SelectUIGameObject(newGameWarningNoButton);
    }
    public void BackToMenu() {
        SetupPlayButtons();
    }


    public void NewGameSelected() {
        SaveData newSave = new SaveData();
        SerializationManager.Save(SaveData.FILE_NAME, newSave);
        Debug.Log("new save");
    }
}
