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
    private GameObject startButton;

    [SerializeField]
    private Button settingsButton;

    [SerializeField]
    private GameObject quitButton;

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
            newGameButton.SetActive(true);
            startButton.SetActive(false);
            ControllerCheck.SelectUIGameObject(continueButton, () => {
                EventSystem.current.firstSelectedGameObject = continueButton;
                settingsButton.navigation = new Navigation(){ mode = Navigation.Mode.Explicit, 
                selectOnUp = newGameButton.GetComponent<Button>(),
                selectOnDown = quitButton.GetComponent<Button>()
                };
            });
        } else {
            //set new game buttons
            continueButton.SetActive(false);
            newGameButton.SetActive(false);
            startButton.SetActive(true);
            ControllerCheck.SelectUIGameObject(startButton, () => {
                EventSystem.current.firstSelectedGameObject = startButton;
                settingsButton.navigation = new Navigation(){ mode = Navigation.Mode.Explicit,  
                selectOnUp = startButton.GetComponent<Button>(),
                selectOnDown = quitButton.GetComponent<Button>()
                };
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
