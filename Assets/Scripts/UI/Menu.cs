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
    public static event MenuAction OnBehindThescenesOpen;
    public static event MenuAction OnSettingsClose;

    private AnimatedPopup popup;

    [SerializeField]
    private GameObject continueButton;

    [SerializeField]
    private Button testButton;

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

    [Header("Behind the scenes UI")]
    [SerializeField] private AnimatedPopup behindThescenesMessage;
    [SerializeField] private Button behindThescenesButton;
    [SerializeField] private Button behindThescenesConfirmButton;
    

    private void Awake() {
        Cursor.visible = true;
        Cursor.lockState =CursorLockMode.None;
        testButton.onClick.AddListener(TestGame);
    }

    private void Start() {
        
        SetupPlayButtons(true);
        settingsButton.onClick.AddListener(GoToSettings);
        AudioHandler.Instance.PlayMusic(MusicFiles.menu, .3f);

        popup = GetComponent<AnimatedPopup>();
        StartCoroutine(DelayMenuShow());
        // Debug.Log("behind the scenes " + PlayerData.BEHIND_THE_SCENES_UNLOCKED);
        // PlayerData.BEHIND_THE_SCENES_UNLOCKED = false;
        //behind the scenes
        if (CreditsRoller.FROM_CREDIT_SCREEN == true && PlayerData.BEHIND_THE_SCENES_UNLOCKED == false) {
            CreditsRoller.FROM_CREDIT_SCREEN = false;
            PlayerData.BEHIND_THE_SCENES_UNLOCKED = true;
            behindThescenesMessage.ShowAnimation(true);
             ControllerCheck.SelectUIGameObject(behindThescenesConfirmButton.gameObject, () => { 
                EventSystem.current.firstSelectedGameObject = behindThescenesConfirmButton.gameObject;
             });
            behindThescenesConfirmButton.onClick.AddListener(() => {
                ControllerCheck.SelectUIGameObject(behindThescenesButton.gameObject);
                behindThescenesMessage.ShowAnimation(false);
            });
            
        }

        behindThescenesButton.gameObject.SetActive(PlayerData.BEHIND_THE_SCENES_UNLOCKED);
        if (PlayerData.BEHIND_THE_SCENES_UNLOCKED) {
            behindThescenesButton.navigation = new Navigation(){ 
                mode = Navigation.Mode.Explicit,  
                selectOnUp = settingsButton,
                selectOnDown = quitButton.GetComponent<Button>()
            };

            Navigation settingsNav = settingsButton.navigation;
            settingsNav.selectOnDown = behindThescenesButton;
            settingsButton.navigation = settingsNav;

            Navigation quitNav = quitButton.GetComponent<Button>().navigation;
            quitNav.selectOnUp = behindThescenesButton;
            quitButton.GetComponent<Button>().navigation = quitNav;
        }
    }

    public void GoToSettings() {
        popup.ShowAnimation(false);
        OnSettingsOpen?.Invoke();
    }

    private IEnumerator DelayMenuShow() {
        yield return new WaitForSeconds(.3f);
        popup.ShowAnimation(true);
    }

    private void SetupPlayButtons(bool _withNavigation) {
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
                    if (_withNavigation)
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
            if (_withNavigation)
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

    public void TestGame() {
        SaveData.current.roomIndex = 0;
        SaveData.current.areaIndex = 2;
        SerializationManager.Save(SaveData.FILE_NAME, SaveData.current);

        sceneLoader.LoadNewSceneAnimated(Scenes.GetSceneNameBasedOnAreaIndex(2));
    }

    public void GoToSlideOverview() {
        popup.ShowAnimation(false);
        OnBehindThescenesOpen?.Invoke();
    }

    public void OpenNewGameWarning() {
        newGameWarning.gameObject.SetActive(true);
        newGameWarning.ShowAnimation(true);
        ControllerCheck.SelectUIGameObject(newGameWarningNoButton);
    }
    public void BackToMenu() {
        SetupPlayButtons(false);
        newGameWarning.ShowAnimation(false);
        popup.ShowAnimation(true);
        OnSettingsClose?.Invoke();
    }

    public void NewGameSelected() {
        SaveData newSave = new SaveData();
        SerializationManager.Save(SaveData.FILE_NAME, newSave);
        PlayerData.TIME_IN_MS = 0;
        Debug.Log("new save");
    }

    private void OnEnable() {
        SettingPanel.OnSettingsClose += BackToMenu;
        SlideMainMenu.OnBehindTheScenesClose += BackToMenu;
    }
    private void OnDisable() {
        SettingPanel.OnSettingsClose -= BackToMenu;
        SlideMainMenu.OnBehindTheScenesClose -= BackToMenu;
    }
}
