using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class TitleMenu : MonoBehaviour {

    public GameObject Cover;
    public GameObject Cover_letters;
    public GameObject Title;
    public GameObject Controls;
    public GameObject Stats;
    public GameObject Achievements;
    public GameObject AchievementsTopButton;
    public GameObject ButtonFishing;
    public GameObject About;
    public GameObject StartButton;
    public GameObject music_on;
    public GameObject music_off;
    public GameObject sound_on;
    public GameObject sound_off;
    public GameObject vibrate_on;
    public GameObject vibrate_off;
    public GameObject ControlsButton;
    public GameObject StatsButton;
    public GameObject AchievementsButton;
    public GameObject AboutButton;
    public GameObject Difficulty;
    public GameObject StartNormalButton;
    public GameObject StartHardButton;
    public GameObject StartMoonshotButton;
    public GameObject StartSingularityButton;
    public GameObject StartEndlessButton;
    public GameObject Minelayer;
    public GameObject ScorpionQueen;
    public GameObject BeetleQueen;
    public GameObject EelQueen;
    public GameObject GeckoQueen;
    public GameObject Frigate;
    public GameObject Frigate1;
    public GameObject Frigate2;
    public GameObject Frigate3;
    public SaveManager save_manager;
    public Playlist playlist;
    public bool muted;
    public bool no_music;
    public bool vibrate;
    int stats_page = 0;
    int achievements_page = 0;
    public List<GameObject> SelectButtonsWheel; //To navigate the gui
    public int SelectButtonsIndex = 0; //To navigate the guy
    float select_menu_cooldown = .3f;
    float last_select_menu = 0f;

    public void Start()
    {   //Find SaveManager by force, since it's kept between scenes due to android file access issues!
        save_manager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();

        InitTitleMenu();

        // Don't show defeated bosses yet
        // Minelayer.SetActive(false);
        // ScorpionQueen.SetActive(false);
        // BeetleQueen.SetActive(false);
        // EelQueen.SetActive(false);
        // GeckoQueen.SetActive(false);
        // Frigate.SetActive(false);
        // Frigate1.SetActive(false);
        // Frigate2.SetActive(false);
        // Frigate3.SetActive(false);

        //Start with cover
        Title.SetActive(false);
    }

    void InitTitleMenu()
    {
        // TitleOn(); Cover on at start

        // Read muted from PlayerPrefs
        if (PlayerPrefs.GetInt("muted", 0) != 0)
        {
            AudioListener.volume = 0f;
            muted = true;
            sound_on.SetActive(false);
            sound_off.SetActive(true);
        }
        else
        {
            muted = false;
            AudioListener.volume = 1f;
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
            vibrate_on.SetActive(true);
            vibrate_off.SetActive(false);
        }
        else
        {
            vibrate = false;
            vibrate_on.SetActive(false);
            vibrate_off.SetActive(true);
        }

        // Show defeated bosses
        Minelayer.SetActive(false);
        ScorpionQueen.SetActive(false);
        BeetleQueen.SetActive(false);
        EelQueen.SetActive(false);
        GeckoQueen.SetActive(false);
        Frigate.SetActive(false);
        Frigate1.SetActive(false);
        Frigate2.SetActive(false);
        Frigate3.SetActive(false);
        if (save_manager.saved_data.minelayer_kills > 0){ Minelayer.SetActive(true); }
        if (save_manager.saved_data.moonwurm_kills > 0){ ScorpionQueen.SetActive(true); }
        if (save_manager.saved_data.moonwurm_beetle_kills > 0){ BeetleQueen.SetActive(true); }
        if (save_manager.saved_data.moonwurm_eel_kills > 0){ EelQueen.SetActive(true); }
        if (save_manager.saved_data.moonwurm_gecko_kills > 0){ GeckoQueen.SetActive(true); }
        if (save_manager.saved_data.frigate_kills > 0){ Frigate.SetActive(true); }
        if (save_manager.saved_data.frigate1_kills > 0){ Frigate1.SetActive(true); }
        if (save_manager.saved_data.frigate2_kills > 0){ Frigate2.SetActive(true); }
        if (save_manager.saved_data.frigate3_kills > 0){ Frigate3.SetActive(true); }
    }

    public void Update()
    {
        // Override menu selection with arrows, to account for split sound/music menus
        if(Title.activeInHierarchy)
        {
            if (Input.GetAxis("Vertical") > 0.1f)
            {
                if(Time.realtimeSinceStartup > last_select_menu + select_menu_cooldown)
                {
                    last_select_menu = Time.realtimeSinceStartup;
                    SelectButtonsIndex--;
                    if (SelectButtonsIndex < 0) { SelectButtonsIndex = SelectButtonsWheel.Count - 1; }
                }
            }
            else if (Input.GetAxis("Vertical") < -0.1f)
            {
                if (Time.realtimeSinceStartup > last_select_menu + select_menu_cooldown)
                {
                    last_select_menu = Time.realtimeSinceStartup;
                    SelectButtonsIndex++;
                    if (SelectButtonsIndex > SelectButtonsWheel.Count-1) { SelectButtonsIndex = 0; }
                }
            }

            EventSystem.current.SetSelectedGameObject(SelectButtonsWheel[SelectButtonsIndex]);

        }
    }

    public void TitleOn()
    {
        Cover.SetActive(false);
        Cover_letters.SetActive(false);
        Title.SetActive(true);
        Controls.SetActive(false);
        Stats.SetActive(false);
        About.SetActive(false);
        Difficulty.SetActive(false);
        ButtonFishing.SetActive(false);
        // Achievements.SetActive(false);  Always active, but all pages and button deactivated
        SelectButtonsIndex = 0;
        achievements_page = 0;
        EventSystem.current.SetSelectedGameObject(SelectButtonsWheel[SelectButtonsIndex]);

        // Initialise some stuff for this menu
        InitTitleMenu();

    }
    public void ControlsOn()
    {
        Cover.SetActive(false);
        Cover_letters.SetActive(false);
        Title.SetActive(false);
        Controls.SetActive(true);
        Stats.SetActive(false);
        About.SetActive(false);
        Difficulty.SetActive(false);
        // Achievements.SetActive(false);  Always active, but all pages and button deactivated
        AchievementsMenu achievements_menu = Achievements.GetComponent<AchievementsMenu>();
        foreach (GameObject page in achievements_menu.pages)
        {
            page.SetActive(false);
        }
        achievements_menu.Button.SetActive(false);
        EventSystem.current.SetSelectedGameObject(AboutButton);
        EventSystem.current.SetSelectedGameObject(ControlsButton);
    }
    public void StatsOn()
    {
        Cover.SetActive(false);
        Cover_letters.SetActive(false);
        Title.SetActive(false);
        Controls.SetActive(false);
        Stats.SetActive(true);
        About.SetActive(false);
        Difficulty.SetActive(false);
        // Achievements.SetActive(false);  Always active, but all pages and button deactivated
        AchievementsMenu achievements_menu = Achievements.GetComponent<AchievementsMenu>();
        foreach (GameObject page in achievements_menu.pages)
        {
            page.SetActive(false);
        }
        achievements_menu.Button.SetActive(false);
        EventSystem.current.SetSelectedGameObject(AboutButton);
        EventSystem.current.SetSelectedGameObject(StatsButton);

        // Open the first page, then the second, back to title when done
        // Tell SaveManager to update info
        if (!save_manager.UpdateStatsMenu(stats_page++))
        {
            // Back to tile if there are no more pages
            stats_page = 0;
            TitleOn();
        }

    }
    public void AchievementsOn()
    {
        Cover.SetActive(false);
        Cover_letters.SetActive(false);
        Title.SetActive(false);
        Controls.SetActive(false);
        Stats.SetActive(false);
        About.SetActive(false);
        Difficulty.SetActive(false);
        // Achievements.SetActive(true);  Always active, but all pages and button deactivated
        EventSystem.current.SetSelectedGameObject(AchievementsButton);

        AchievementsMenu achievements_menu = Achievements.GetComponent<AchievementsMenu>();

        // Back to tile if there are no more pages
        if (achievements_page >= achievements_menu.pages.Count)
        {
            achievements_page = 0;
            foreach (GameObject page in achievements_menu.pages)
            {
                page.SetActive(false);
            }
            achievements_menu.Button.SetActive(false);
            TitleOn();
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
            EventSystem.current.SetSelectedGameObject(AchievementsTopButton);

            //Hack: activate fishing button on top of active fishing achievement
            ButtonFishing.SetActive(false);
            foreach (Achievement achievement in achievements_menu.pages[achievements_page].GetComponentsInChildren<Achievement>())
            {
                if(achievement.name.Contains("Master Baiter"))
                {
                    ButtonFishing.SetActive(true);
                    ButtonFishing.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                    ButtonFishing.GetComponent<RectTransform>().localPosition = achievement.gameObject.GetComponent<RectTransform>().localPosition + new Vector3(0f, -25f, 0f);
                }
            }


            // Increase page for next call
            achievements_page++;
        }

    }
    public void AboutOn()
    {
        Cover.SetActive(false);
        Cover_letters.SetActive(false);
        Title.SetActive(false);
        Controls.SetActive(false);
        Stats.SetActive(false);
        About.SetActive(true);
        Difficulty.SetActive(false);
        // Achievements.SetActive(false);  Always active, but all pages and button deactivated
        AchievementsMenu achievements_menu = Achievements.GetComponent<AchievementsMenu>();
        foreach (GameObject page in achievements_menu.pages)
        {
            page.SetActive(false);
        }
        achievements_menu.Button.SetActive(false);
        EventSystem.current.SetSelectedGameObject(AboutButton);
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
            vibrate_on.SetActive(true);
            vibrate_off.SetActive(false);
            //playlist.StopPlaying();
            PlayerPrefs.SetInt("vibrate", 1);
            PlayerPrefs.Save();
        }
        else
        {
            vibrate_on.SetActive(false);
            vibrate_off.SetActive(true);
            //playlist.StartPlaying();
            PlayerPrefs.SetInt("vibrate", 0);
            PlayerPrefs.Save();
        }
    }

    public void DifficultyOn()
    {
        if(save_manager.saved_data.max_difficulty_unlocked < 1)
        {
            // Start directly if only 1 difficulty
            StartGame(0);
        }
        else
        {
            Cover.SetActive(false);
            Title.SetActive(false);
            Controls.SetActive(false);
            Stats.SetActive(false);
            About.SetActive(false);
            // Achievements.SetActive(false);  Always active, but all pages and button deactivated
            Difficulty.SetActive(true);
            StartNormalButton.SetActive(true);
            StartHardButton.SetActive(false);
            StartMoonshotButton.SetActive(false);
            StartSingularityButton.SetActive(false);
            StartEndlessButton.SetActive(false);
            if (save_manager.saved_data.max_difficulty_unlocked > 0)
            {
                StartHardButton.SetActive(true);
            }
            if (save_manager.saved_data.max_difficulty_unlocked > 1)
            {
                StartMoonshotButton.SetActive(true);
            }
            if (save_manager.saved_data.max_difficulty_unlocked > 2)
            {
                StartSingularityButton.SetActive(true);
                StartEndlessButton.SetActive(true);
            }
            /*if (save_manager.saved_data.max_difficulty_unlocked > 3)
            {
                StartEndlessButton.SetActive(true);
            }*/
            EventSystem.current.SetSelectedGameObject(StartNormalButton);
        }
    }
    
    public void StartGame(int difficulty=0)
    {
        // Set dificulty for main scene
        save_manager.SetDifficulty(difficulty);
        SceneManager.LoadScene("MainScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
