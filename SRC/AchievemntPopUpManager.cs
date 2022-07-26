using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievemntPopUpManager : MonoBehaviour
{

    public List<AchievementPopUp> achievementPopUps;

    public void GiveAchievement(Achievement achievement)
    {
        Debug.Log("AchievementPopUp: " + achievement.title);
        float last_activated = Time.time; // Initialize to "now", actives must be older

        int count = 0;
        int older_pos = 0;
        bool found_one_free = false;
        foreach (AchievementPopUp popup in achievementPopUps)
        {
            if (!popup.active)
            {
                Debug.Log("Found inactive PopUp :" + count);
                popup.ShowAchievement(achievement);
                found_one_free = true;
                break;
            }
            if(popup.last_activated < last_activated)
            {
                last_activated = popup.last_activated;
                older_pos = count;
            }
            count++;
        }

        if (!found_one_free)
        {
            //Replace oldest one
            Debug.Log("AchievementPopUp Replace oldest one: "+ older_pos);
            achievementPopUps[older_pos].ShowAchievement(achievement);
        }

     }

}
