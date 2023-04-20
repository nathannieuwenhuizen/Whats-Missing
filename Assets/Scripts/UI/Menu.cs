using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{    
    public delegate void MenuAction();
    public static event MenuAction OnSettingsOpen;
    public static event MenuAction OnSettingsClose;

    private AnimatedPopup popup;

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
    private AnimatedPopup newGameWarning;
    [SerializeField]
    private GameObject newGameWarningNoButton;

    private SaveData saveData;
    [SerializeField]
    private SceneLoader sceneLoader;
    

    private void Awake() {
        Cursor.visible = true;
        Cursor.lockState =CursorLockMode.None;
    }

    private void Start() {
        SetupPlayButtons();
        settingsButton.onClick.AddListener(GoToSettings);
        AudioHandler.Instance.PlayMusic(MusicFiles.menu, .3f);

        popup = GetComponent<AnimatedPopup>();
        StartCoroutine(DelayMenuShow());
    }

    public void GoToSettings() {
        popup.ShowAnimation(false);
        OnSettingsOpen?.Invoke();
    }

    private IEnumerator DelayMenuShow() {
        yield return new WaitForSeconds(.3f);
        popup.ShowAnimation(true);
    }

    private void SetupPlayButtons() {
        object data = SerializationManager.Load(SerializationManager.filePath + "/" + SaveData.FILE_NAME +".save");
        Debug.Log((data as SaveData));

        if ((data as SaveData) != null) {

            if (data != null && (data as SaveData) != null && ((data as SaveData).roomIndex != 0) || (data as SaveData).areaIndex != 0) {
                saveData = (data as SaveData);
                continueButton.SetActive(true);
                newGameButton.SetActive(true);
                startButton.SetActive(false);
                ControllerCheck.SelectUIGameObject(continueButton, () => {
                    EventSystem.current.firstSelectedGameObject = continueButton;
                    settingsButton.navigation = new Navigation(){ 
                        mode = Navigation.Mode.Explicit, 
                        selectOnUp = newGameButton.GetComponent<Button>(),
                        selectOnDown = quitButton.GetComponent<Button>()
                    };
                });
                return;
            }
        }
        
        //set new game buttons
        continueButton.SetActive(false);
        newGameButton.SetActive(false);
        startButton.SetActive(true);
        ControllerCheck.SelectUIGameObject(startButton, () => {
            EventSystem.current.firstSelectedGameObject = startButton;
            settingsButton.navigation = new Navigation(){ 
                mode = Navigation.Mode.Explicit,  
                selectOnUp = startButton.GetComponent<Button>(),
                selectOnDown = quitButton.GetComponent<Button>()
            };
        });

    }

    public void ContinueGame() {
        sceneLoader.LoadNewSceneAnimated(Scenes.GetSceneNameBasedOnAreaIndex(saveData.areaIndex));
    }

    public void OpenNewGameWarning() {
        newGameWarning.gameObject.SetActive(true);
        newGameWarning.ShowAnimation(true);
        ControllerCheck.SelectUIGameObject(newGameWarningNoButton);
    }
    public void BackToMenu() {
        SetupPlayButtons();
        popup.ShowAnimation(true);
        OnSettingsClose?.Invoke();
    }


    public void NewGameSelected() {
        SaveData newSave = new SaveData();
        SerializationManager.Save(SaveData.FILE_NAME, newSave);
        Debug.Log("new save");
    }

    private void OnEnable() {
        SettingPanel.OnSettingsClose += BackToMenu;
    }
    private void OnDisable() {
        SettingPanel.OnSettingsClose -= BackToMenu;
    }
}
