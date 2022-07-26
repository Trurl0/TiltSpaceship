using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AchievementsMenu : MonoBehaviour
{
    public GameObject PagesFather;  //Father for dynamic pages, to keep Button on top!
    public GameObject Button;  //Father for dynamic pages, to keep Button on top!

    // public List<Achievement> achievements; Now Read from Save Manager
    public List<GameObject> pages;
    public GameObject page_prototype;

    public int max_rows = 3;
    public int max_columns = 4;
    public int ini_x = -100;
    public int ini_y = 230;
    public int delta_x = 100;
    public int delta_y = -140;

    SaveManager save_manager;

    public void Start()
    {
        save_manager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();

        UpdateAchievements();

    }

    public void UpdateAchievements()
    {
        //Delete previous pages
        foreach (Transform child in PagesFather.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        pages = new List<GameObject>();

        // Create first page
        GameObject current_page = Instantiate(page_prototype);
        current_page.transform.parent = PagesFather.transform;
        current_page.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        current_page.GetComponent<RectTransform>().localPosition = Vector3.zero;
        current_page.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        current_page.SetActive(false); //start inactive, TitleMenu will activate when needed
        pages.Add(current_page);

        int row_count = 0;
        int col_count = 0;


        foreach (Achievement achievement in save_manager.achievements)
        {

            // Create new page if needed
            if (col_count >= max_columns)
            {
                row_count = 0;
                col_count = 0;

                // New page needed
                current_page = Instantiate(page_prototype);
                current_page.transform.parent = PagesFather.transform;
                current_page.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                current_page.GetComponent<RectTransform>().localPosition = Vector3.zero;
                current_page.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                current_page.SetActive(false);
                pages.Add(current_page);
            }

            // Create my own instance to avoid stealing ownership from SaveManager (my children die with me...)
            Achievement achievement_instance = Instantiate(achievement);
            // Debug.Log(achievement_instance.name);

            // Add to the current page
            achievement_instance.transform.parent = current_page.transform;
            achievement_instance.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            achievement_instance.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            achievement_instance.GetComponent<RectTransform>().localPosition = new Vector3(ini_x + (delta_x * row_count), ini_y + (delta_y * col_count), 0f);

            row_count++;
            if (row_count >= max_rows)
            {
                row_count = 0;
                col_count++;
            }
        }
    }
}
