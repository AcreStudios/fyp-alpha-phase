﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class NewMenuScript : MonoBehaviour {

    [System.Serializable]
    public class StartScreen {
        public GameObject StartCanvas;
        public Button PressToStart;
    }
    public StartScreen startScreen;

    [System.Serializable]
    public class MainMenu {
        public GameObject MainCanvas;
        public GameObject GORedNewGame;
        public GameObject GORedSettings;
        public GameObject GORedCredits;
        public GameObject GORedQuit;
        public GameObject NewGame;
        public GameObject Settings;
        public GameObject Credits;
        public GameObject Quit;
    }
    public MainMenu mainMenu;

    [System.Serializable]
    public class SettingsMenu {
        public GameObject SettingsCanvas;
        public GameObject GORedGame;
        public GameObject GORedAudio;
        public GameObject GORedVideo;
        public GameObject GORedBack;
        public GameObject Game;
        public GameObject Audio;
        public GameObject Video;
        public GameObject Back;
    }
    public SettingsMenu settingsMenu;

    [System.Serializable]
    public class CreditsMenu {
        public GameObject CreditsCanvas;
        public GameObject GORedCreditsBack;
    }
    public CreditsMenu creditsMenu;

    [System.Serializable]
    public class QuitMenu {
        public GameObject QuitCanvas;
        public GameObject GORedYes;
        public GameObject GORedNo;
        public GameObject Yes;
        public GameObject No;
    }
    public QuitMenu quitMenu;
    
    [System.Serializable]
    public class LoadingScreen {
        public GameObject LoadingCanvas;
        public GameObject PressToContinue;
        public GameObject PressToContinueButton;
        public GameObject LoadingText;
        public GameObject DoneText;
    }
    public LoadingScreen loadingScreen;

    void Start () {
        #region Start Screen
        startScreen.StartCanvas.SetActive(true);

        startScreen.PressToStart.enabled = true;
        #endregion
        #region Main Menu
        mainMenu.MainCanvas.SetActive(false);

        mainMenu.GORedNewGame.SetActive(true);
        mainMenu.GORedSettings.SetActive(false);
        mainMenu.GORedCredits.SetActive(false);
        mainMenu.GORedQuit.SetActive(false);

        mainMenu.NewGame.SetActive(false);
        mainMenu.Settings.SetActive(true);
        mainMenu.Credits.SetActive(true);
        mainMenu.Quit.SetActive(true);
        #endregion
        #region Settings Menu
        settingsMenu.SettingsCanvas.SetActive(false);

        settingsMenu.GORedGame.SetActive(true);
        settingsMenu.GORedAudio.SetActive(false);
        settingsMenu.GORedVideo.SetActive(false);
        settingsMenu.GORedBack.SetActive(false);

        settingsMenu.Game.SetActive(false);
        settingsMenu.Audio.SetActive(true);
        settingsMenu.Video.SetActive(true);
        settingsMenu.Back.SetActive(true);
        #endregion
        #region Credits Menu
        creditsMenu.CreditsCanvas.SetActive(false);

        creditsMenu.GORedCreditsBack.SetActive(true);
        #endregion
        #region Quit Menu
        quitMenu.QuitCanvas.SetActive(false);

        quitMenu.GORedYes.SetActive(false);
        quitMenu.GORedNo.SetActive(true);

        quitMenu.Yes.SetActive(true);
        quitMenu.No.SetActive(false);
        #endregion
        #region Loading Screen
        //Loading Screen//*****************
        loadingScreen.LoadingCanvas.SetActive(false);

        loadingScreen.PressToContinue.SetActive(false);
        loadingScreen.PressToContinueButton.SetActive(false);

        loadingScreen.LoadingText.SetActive(false);
        loadingScreen.DoneText.SetActive(false);
        #endregion
    }

    // Handles all MouseOver Events
    #region Main Menu Mouseover Handler
    public void MONewGame() {
        mainMenu.NewGame.SetActive(false);
        mainMenu.Settings.SetActive(true);
        mainMenu.Credits.SetActive(true);
        mainMenu.Quit.SetActive(true);

        mainMenu.GORedNewGame.SetActive(true);
        mainMenu.GORedSettings.SetActive(false);
        mainMenu.GORedCredits.SetActive(false);
        mainMenu.GORedQuit.SetActive(false);
    }
    public void MOSettings() {
        mainMenu.NewGame.SetActive(true);
        mainMenu.Settings.SetActive(false);
        mainMenu.Credits.SetActive(true);
        mainMenu.Quit.SetActive(true);

        mainMenu.GORedNewGame.SetActive(false);
        mainMenu.GORedSettings.SetActive(true);
        mainMenu.GORedCredits.SetActive(false);
        mainMenu.GORedQuit.SetActive(false);
    }
    public void MOCredits() {
        mainMenu.NewGame.SetActive(true);
        mainMenu.Settings.SetActive(true);
        mainMenu.Credits.SetActive(false);
        mainMenu.Quit.SetActive(true);

        mainMenu.GORedNewGame.SetActive(false);
        mainMenu.GORedSettings.SetActive(false);
        mainMenu.GORedCredits.SetActive(true);
        mainMenu.GORedQuit.SetActive(false);
    }
    public void MOQuit() {
        mainMenu.NewGame.SetActive(true);
        mainMenu.Settings.SetActive(true);
        mainMenu.Credits.SetActive(true);
        mainMenu.Quit.SetActive(false);

        mainMenu.GORedNewGame.SetActive(false);
        mainMenu.GORedSettings.SetActive(false);
        mainMenu.GORedCredits.SetActive(false);
        mainMenu.GORedQuit.SetActive(true);
    }
    #endregion
    #region Settings Mouseover Handler
    public void MOGame() {
        settingsMenu.GORedGame.SetActive(true);
        settingsMenu.GORedAudio.SetActive(false);
        settingsMenu.GORedVideo.SetActive(false);
        settingsMenu.GORedBack.SetActive(false);

        settingsMenu.Game.SetActive(false);
        settingsMenu.Audio.SetActive(true);
        settingsMenu.Video.SetActive(true);
        settingsMenu.Back.SetActive(true);
    }
    public void MOAudio() {
        settingsMenu.GORedGame.SetActive(false);
        settingsMenu.GORedAudio.SetActive(true);
        settingsMenu.GORedVideo.SetActive(false);
        settingsMenu.GORedBack.SetActive(false);

        settingsMenu.Game.SetActive(true);
        settingsMenu.Audio.SetActive(false);
        settingsMenu.Video.SetActive(true);
        settingsMenu.Back.SetActive(true);
    }
    public void MOVideo() {
        settingsMenu.GORedGame.SetActive(false);
        settingsMenu.GORedAudio.SetActive(false);
        settingsMenu.GORedVideo.SetActive(true);
        settingsMenu.GORedBack.SetActive(false);

        settingsMenu.Game.SetActive(true);
        settingsMenu.Audio.SetActive(true);
        settingsMenu.Video.SetActive(false);
        settingsMenu.Back.SetActive(true);
    }
    public void MOBack() {
        settingsMenu.GORedGame.SetActive(false);
        settingsMenu.GORedAudio.SetActive(false);
        settingsMenu.GORedVideo.SetActive(false);
        settingsMenu.GORedBack.SetActive(true);

        settingsMenu.Game.SetActive(true);
        settingsMenu.Audio.SetActive(true);
        settingsMenu.Video.SetActive(true);
        settingsMenu.Back.SetActive(false);
    }
    #endregion
    #region Quit Mouseover Handler
    public void MOYes() {
        quitMenu.GORedYes.SetActive(true);
        quitMenu.GORedNo.SetActive(false);

        quitMenu.Yes.SetActive(false);
        quitMenu.No.SetActive(true);
    }
    public void MONo() {
        quitMenu.GORedYes.SetActive(false);
        quitMenu.GORedNo.SetActive(true);

        quitMenu.Yes.SetActive(true);
        quitMenu.No.SetActive(false);
    }
    #endregion

    // Handles all Button Press Events. Call functions from editor
    #region Button Press Events
    public void SelectSettings() {
        settingsMenu.SettingsCanvas.SetActive(true);
        mainMenu.MainCanvas.SetActive(false);
    }
    public void SelectBack() {
        settingsMenu.SettingsCanvas.SetActive(false);
        creditsMenu.CreditsCanvas.SetActive(false);
        mainMenu.MainCanvas.SetActive(true);
    }
    public void SelectNewGame() {
        loadingScreen.LoadingCanvas.SetActive(true);
        mainMenu.MainCanvas.SetActive(false);
        loadingScreen.LoadingText.SetActive(true);

        StartCoroutine(loadingtime());
    }
    public void SelectCredits() {
        creditsMenu.CreditsCanvas.SetActive(true);
        mainMenu.MainCanvas.SetActive(false);
    }
    public void SelectQuit() {
        quitMenu.QuitCanvas.SetActive(true);
        mainMenu.MainCanvas.SetActive(false);
    }
    public void SelectNewQuit() {
        mainMenu.MainCanvas.SetActive(false);
        startScreen.StartCanvas.SetActive(true);
    }
    public void SelectStart() {
        mainMenu.MainCanvas.SetActive(true);
        startScreen.StartCanvas.SetActive(false);
    }
    public void SelectNo() {
        quitMenu.QuitCanvas.SetActive(false);
        mainMenu.MainCanvas.SetActive(true);
    }
    public void SelectYes() {
        Application.Quit();
    }
    public void loadApplication() {
        SceneManager.LoadScene("NEW Interior GI and Reflection Test");
        Debug.Log("Interior Loaded");
    }
    public void load2Application() {
        SceneManager.LoadScene("NEW Exterior GI and Reflection Test");
        Debug.Log("Exterior Loaded");
    }
    #endregion

    IEnumerator loadingtime() {
        Debug.Log("Scene Loading");
        yield return new WaitForSeconds(3);
        loadingScreen.PressToContinue.SetActive(true);
        loadingScreen.PressToContinueButton.SetActive(true);
        loadingScreen.LoadingText.SetActive(false);
        loadingScreen.DoneText.SetActive(true);
    }
}
