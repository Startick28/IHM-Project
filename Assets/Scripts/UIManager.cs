using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public enum MenuType 
{
    MainMenu,
    PauseMenu,
    LevelSelectionMenu,
    SettingsMenu,
    VolumeMenu,
    ControlsMenu
}

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private List<Button> levelButtonsList = new List<Button>();

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject levelSelectionMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject volumeMenu;
    [SerializeField] private GameObject controlsMenu;


    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Text sfxVolumeText;
    [SerializeField] private Text musicVolumeText;

    [SerializeField] private GameObject firstMainMenuButton;
    [SerializeField] private GameObject firstPauseMenuButton;
    [SerializeField] private GameObject firstLevelSelectionMenuButton;
    [SerializeField] private GameObject firstSettingsMenuButton;
    [SerializeField] private GameObject firstVolumeMenuButton;
    [SerializeField] private GameObject firstControlsMenuButton;

    private GameObject currentActiveMenu;
    private PlayerControllerTSafe player;
    private float sfxVolume;
    private float musicVolume;

    void Start()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
        currentActiveMenu = mainMenu;

        int counter = 0;
        foreach (Button button in levelButtonsList)
        {
            int tmp = counter;
            button.onClick.AddListener(() => SelectLevel(tmp));
            counter++;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && currentActiveMenu.gameObject.activeInHierarchy == false)
        {
            GameManager.instance.isGamePaused = true;
            currentActiveMenu = pauseMenu;
            currentActiveMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstPauseMenuButton);
        }    
    }

    public void GoToMainMenu()
    {
        SwitchMenu(MenuType.MainMenu);
    }
    public void GoToPauseMenu()
    {
        SwitchMenu(MenuType.PauseMenu);
    }
    public void GoToLevelSelectionMenu()
    {
        SwitchMenu(MenuType.LevelSelectionMenu);
    }
    public void GoToSettingsMenu()
    {
        SwitchMenu(MenuType.SettingsMenu);
    }
    public void GoToVolumeMenu()
    {
        SwitchMenu(MenuType.VolumeMenu);
    }
    public void GoToControlsMenu()
    {
        SwitchMenu(MenuType.ControlsMenu);
    }

    void SwitchMenu(MenuType menuToGo)
    {
        currentActiveMenu.SetActive(false);
        switch (menuToGo)
        {
            case MenuType.MainMenu:
                SceneManager.LoadScene(0);
                currentActiveMenu = mainMenu;
                mainMenu.SetActive(true);
                EventSystem.current.SetSelectedGameObject(firstMainMenuButton);
                break;

            case MenuType.PauseMenu:
                pauseMenu.SetActive(true);
                EventSystem.current.SetSelectedGameObject(firstPauseMenuButton);
                currentActiveMenu = pauseMenu;
                break;

            case MenuType.LevelSelectionMenu:
                levelSelectionMenu.SetActive(true);
                EventSystem.current.SetSelectedGameObject(firstLevelSelectionMenuButton);
                currentActiveMenu = levelSelectionMenu;
                break;

            case MenuType.SettingsMenu:
                settingsMenu.SetActive(true);
                EventSystem.current.SetSelectedGameObject(firstSettingsMenuButton);
                currentActiveMenu = settingsMenu;
                break;

            case MenuType.VolumeMenu:
                volumeMenu.SetActive(true);
                EventSystem.current.SetSelectedGameObject(firstVolumeMenuButton);
                currentActiveMenu = volumeMenu;
                break;

            case MenuType.ControlsMenu:
                controlsMenu.SetActive(true);
                EventSystem.current.SetSelectedGameObject(firstControlsMenuButton);
                currentActiveMenu = controlsMenu;
                break;

            default:
                break;
        }
    }
    
    public void Play()
    {
        currentActiveMenu.SetActive(false);
        SceneManager.LoadScene(1);
        if (GameManager.instance) GameManager.instance.isGamePaused = false;
    }

    public void Resume()
    {
        currentActiveMenu.SetActive(false);
        GameManager.instance.isGamePaused = false;
    }


    void SelectLevel(int levelNumber)
    {
        player.TPToLevel(GameManager.instance.GetLevelPosition(levelNumber));
        currentActiveMenu.SetActive(false);
        GameManager.instance.isGamePaused = false;
    }

    public void GoToPlayground()
    {
        player.TPToLevel(GameManager.instance.GetLevelPlaygroundPosition());
        currentActiveMenu.SetActive(false); 
        GameManager.instance.isGamePaused = false;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetPlayer(PlayerControllerTSafe p)
    {
        player = p;
    }

    public void SetMusicSliderValue()
    {
        SoundAssets.instance.changeVolume();
        musicVolumeText.text = musicVolumeSlider.value.ToString();
    }

    public void SetSfxSliderValue()
    {
        SoundAssets.instance.changeVolume();
        sfxVolumeText.text = sfxVolumeSlider.value.ToString();
    }

    public void PlayUISound()
    {
        SoundManager.PlaySound(SoundManager.Sound.UI);
    }
}
