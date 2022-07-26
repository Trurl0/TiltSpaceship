using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievement : MonoBehaviour
{
    public Sprite image;
    public string title;
    public string description;
    public bool unlocked; // Updated by SaveManager at initialisation or unlocking

    public Image image_field;
    public Text title_field;
    public Text description_field;


    private void Start()
    {
        image_field.sprite = image;
        title_field.text = title;
        description_field.text = description;

        if (!unlocked)
        {
            Color c = image_field.color;
            c.a = 0.3f;
            image_field.color = c;

            c = title_field.color;
            c.a = 0.3f;
            title_field.color = c;

            c = title_field.color;
            c.a = 0.3f;
            description_field.color = c;
        }

    }

    public void Unlock()
    {
        unlocked = true;

        Color c = image_field.color;
        c.a =1f;
        image_field.color = c;

        c = title_field.color;
        c.a = 1f;
        title_field.color = c;

        c = title_field.color;
        c.a = 1f;
        description_field.color = c;
    }
}
