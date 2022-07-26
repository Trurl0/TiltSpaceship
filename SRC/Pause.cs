using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Pause : MonoBehaviour {

    // TODO separate start an end messages from this object

    public bool paused = false;
    public GameObject PauseMenu;
    public GameObject PauseButton;
    public GameObject StartMenu;
    public GameObject EndMenu;
    public GameObject AchievementsMenu; // to focus EventSystem in case of using a controller
    public GameObject AchievementsMenuButton; // to focus EventSystem in case of using a controller
    public GameObject resume_button;// to focus EventSystem in case of using a controller
    public GameObject start_button; // to focus EventSystem in case of using a controller
    public GameObject end_button;   // to focus EventSystem in case of using a controller
    public GameObject music_on;
    public GameObject music_off;
    public GameObject sound_on;
    public GameObject sound_off;
    public GameObject vibrate_on;
    public GameObject vibrate_off;
    public Playlist playlist;
    private PlayerShip player_ship;
    //public bool initial_pause;
    public bool muted;
    public bool no_music;
    public bool vibrate;
    float old_timescale = 1f; // In case of pause during timefreeze
    // public string initial_text = "Your ship is carrying a cargo of something.\n\n";
    int achievements_page = 0;
    public List<GameObject> SelectButtonsWheel; //To navigate the gui
    public int SelectButtonsIndex = 0; //To navigate the guy
    float select_menu_cooldown = .2f;
    float last_select_menu = 0f;

    // Use this for initialization
    void Start()
    {

        // Maintain screen active
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        player_ship = References.player.GetComponent<PlayerShip>();

        //STAR WARS-like text without pause
        paused = false;

        //StartMenu.GetComponentInChildren<Text>().text = initial_text;
        // StartMenu only activate at start
        //PauseMenu.SetActive(false);
        //PauseButton.SetActive(false);
        //StartMenu.SetActive(true);
        //EndMenu.SetActive(false);
        //StatsMenu.SetActive(false);
        //EventSystem.current.SetSelectedGameObject(start_button);

        //Time.timeScale = 0f;
        //paused = true;
        //initial_pause = true;

        // Read muted from PlayerPrefs
        if (PlayerPrefs.GetInt("muted", 0) != 0)
        {
            AudioListener.volume = 0f;
            muted = true;
            //SoundButton.GetComponentInChildren<Text>().text = "Sound off";
            sound_on.SetActive(false);
            sound_off.SetActive(true);
        }
        else
        {
            muted = false;
            AudioListener.volume = 1f;
            //SoundButton.GetComponentInChildren<Text>().text = "Sound on";
            sound_on.SetActive(true);
            sound_off.SetActive(false);
        }

        // Read no_music from PlayerPrefs
        if (PlayerPrefs.GetInt("no_music", 0) != 0)
        {
            no_music = true;
            playlist.StopPlaying();
            music_on.SetActive(false);
            music_off.SetActive(true);
        }
        else
        {
            no_music = false;
            playlist.StartPlaying();
            music_on.SetActive(true);
            music_off.SetActive(false);
        }

        // Read vibrate from PlayerPrefs
        if (PlayerPrefs.GetInt("vibrate", 1) != 0)
        {
            vibrate = true;
            player_ship.vibrate = true;
            vibrate_on.SetActive(true);
            vibrate_off.SetActive(false);
        }
        else
        {
            vibrate = false;
            player_ship.vibrate = false;
            vibrate_on.SetActive(false);
            vibrate_off.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetButtonDown("Pause"))
        {
            TooglePause();
        }

        // Override menu selection with arrows, to account for split sound/music menus
        if (PauseMenu.activeInHierarchy)
        {
            if (Input.GetAxis("Vertical") > 0.1f)
            {
                //Debug.Log("Axis Up;");
                if (Time.realtimeSinceStartup > last_select_menu + select_menu_cooldown)
                {
                    //Debug.Log("SelectButtonsIndex--");
                    last_select_menu = Time.realtimeSinceStartup;
                    SelectButtonsIndex--;
                    if (SelectButtonsIndex < 0) { SelectButtonsIndex = SelectButtonsWheel.Count - 1; }
                }
            }
            else if (Input.GetAxis("Vertical") < -0.1f)
            {
                //Debug.Log("Axis Down;");
                if (Time.realtimeSinceStartup > last_select_menu + select_menu_cooldown)
                {
                    //Debug.Log("SelectButtonsIndex++");
                    last_select_menu = Time.realtimeSinceStartup;
                    SelectButtonsIndex++;
                    if (SelectButtonsIndex > SelectButtonsWheel.Count - 1) { SelectButtonsIndex = 0; }
                }
            }

            EventSystem.current.SetSelectedGameObject(SelectButtonsWheel[SelectButtonsIndex]);

        }
    }

    public void PauseGame()
    {
        // HACK: only save GAME timescale (Timescale is 0 if we enter from other menus)
        if (Time.timeScale > 0.00001f)
        {
            old_timescale = Time.timeScale;
        }
        else {
            old_timescale = 1f;
        }
        Time.timeScale = 0;
        paused = true;
        PauseMenu.SetActive(true);
        PauseButton.SetActive(false);
        EndMenu.SetActive(false);
        StartMenu.SetActive(false);
        // AchievementsMenu.SetActive(false);  Always active, but all pages and button deactivated
        AchievementsMenu achievements_menu = AchievementsMenu.GetComponent<AchievementsMenu>();
        foreach (GameObject page in achievements_menu.pages)
        {
            page.SetActive(false);
        }
        achievements_menu.Button.SetActive(false);
        achievements_page = 0;

        EventSystem.current.SetSelectedGameObject(resume_button);
        SelectButtonsIndex = 0;

    }
    public void ResumeGame()
    {   
        Time.timeScale = old_timescale;
        paused = false;
        PauseMenu.SetActive(false);
        PauseButton.SetActive(true);
        StartMenu.SetActive(false);
        EndMenu.SetActive(false);
        // AchievementsMenu.SetActive(false);  Always active, but all pages and button deactivated
        AchievementsMenu achievements_menu = AchievementsMenu.GetComponent<AchievementsMenu>();
        foreach (GameObject page in achievements_menu.pages)
        {
            page.SetActive(false);
        }
        achievements_menu.Button.SetActive(false);
    }
    public void TooglePause()
    {
        if (paused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void DiscardAbilityPause()
    {
        // HACK: only save GAME timescale (Timescale is 0 if we enter from other menus)
        if (Time.timeScale > 0.00001f)
        {
            old_timescale = Time.timeScale;
        }
        else
        {
            old_timescale = 1f;
        }
        Time.timeScale = 0;
        paused = true;
        PauseMenu.SetActive(false);
        PauseButton.SetActive(false);
        EndMenu.SetActive(false);
        StartMenu.SetActive(false);
        // AchievementsMenu.SetActive(false);  Always active, but all pages and button deactivated
    }

    public void AchievementsMenuOn()
    {
        PauseMenu.SetActive(false);
        PauseButton.SetActive(false);
        EndMenu.SetActive(false);
        StartMenu.SetActive(false);
        // AchievementsMenu.SetActive(false);  Always active, but all pages and button deactivated


        AchievementsMenu achievements_menu = AchievementsMenu.GetComponent<AchievementsMenu>();
        achievements_menu.UpdateAchievements();

        // Back to tile if there are no more pages
        if (achievements_page >= achievements_menu.pages.Count)
        {
            achievements_page = 0;
            foreach (GameObject page in achievements_menu.pages)
            {
                page.SetActive(false);
            }
            achievements_menu.Button.SetActive(false);
            PauseGame();
        }
        else
        {
            // Activate one page
            foreach (GameObject page in achievements_menu.pages)
            {
                page.SetActive(false);
            }
            achievements_menu.pages[achievements_page].SetActive(true);
            achievements_menu.Button.SetActive(true);
            EventSystem.current.SetSelectedGameObject(AchievementsMenuButton);

            // Increase page for next call
            achievements_page++;
        }

    }

    public void ToogleSound()
    {
        muted = !muted;
        if (muted)
        {
            sound_on.SetActive(false);
            sound_off.SetActive(true);
            AudioListener.volume = 0f;
            PlayerPrefs.SetInt("muted", 1);
            PlayerPrefs.Save();

        }
        else
        {
            sound_on.SetActive(true);
            sound_off.SetActive(false);
            AudioListener.volume = 1f;
            PlayerPrefs.SetInt("muted", 0);
            PlayerPrefs.Save();
        }
    }
    public void ToogleMusic()
    {
        no_music = !no_music;

        if (no_music)
        {
            music_on.SetActive(false);
            music_off.SetActive(true);
            playlist.StopPlaying();
            PlayerPrefs.SetInt("no_music", 1);
            PlayerPrefs.Save();
        }
        else
        {
            music_on.SetActive(true);
            music_off.SetActive(false);
            playlist.StartPlaying();
            PlayerPrefs.SetInt("no_music", 0);
            PlayerPrefs.Save();
        }
    }
    public void ToogleVibrate()
    {
        vibrate = !vibrate;

        if (vibrate)
        {
            player_ship.vibrate = true;
            vibrate_on.SetActive(true);
            vibrate_off.SetActive(false);
            PlayerPrefs.SetInt("vibrate", 1);
            PlayerPrefs.Save();
        }
        else
        {
            player_ship.vibrate = false;
            vibrate_on.SetActive(false);
            vibrate_off.SetActive(true);
            PlayerPrefs.SetInt("vibrate", 0);
            PlayerPrefs.Save();
        }
    }
    public void Restart()
    {
        ResumeGame(); // Re-start unpaused!
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        //Application.Quit();

        // Unpause and go back to main menu
        Time.timeScale = 1f;
        paused = false;
        SceneManager.LoadScene("TitleScene");
    }
}
