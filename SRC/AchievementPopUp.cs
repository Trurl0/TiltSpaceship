using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AchievementPopUp : MonoBehaviour
{

    public Image image;
    public Text title;
    public Text description;
    public GameObject main_button;
    public float display_time = 3f;
    public bool active = false;
    public float last_activated = 0f;

    void Update()
    {
        if(Time.time > last_activated + display_time)
        {
            Hide();
        }
    }

    public void ShowAchievement(Achievement achievement)
    {

        Debug.Log("AchievementPopUp ShowAchievement: " + achievement.title);
        title.GetComponent<Text>().text = achievement.title;
        description.GetComponent<Text>().text = achievement.description;
        image.sprite = achievement.image;

        main_button.SetActive(true);
        active = true;
        last_activated = Time.time;
    }

    public void Hide()
    {
        main_button.SetActive(false);
        active = false;
    }
}
