using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playlist : MonoBehaviour {

    public List<AudioClip> soundtrack = new List<AudioClip>();
    public List<AudioClip> slow_tracks = new List<AudioClip>();
    public List<AudioClip> fast_tracks = new List<AudioClip>();
    public int tracklist_speed = 0;
    public List<AudioClip> track_list = new List<AudioClip>(); // random without replacement
    AudioSource audio_source;

    bool no_music = false;

    bool slow1_listened = false;
    bool slow2_listened = false;
    bool fast1_listened = false;
    bool fast2_listened = false;
    bool fast3_listened = false;

    // Use this for initialization
    void Start()
    {
        audio_source = GetComponent<AudioSource>();

        create_track_list(tracklist_speed);
    }

    // Update is called once per frame
    void Update()
    {
        if (!no_music && !audio_source.isPlaying)
        {
            audio_source.clip = pop_from_track_list();
            audio_source.Play();

            // For achievement
            if(AudioListener.volume > 0.01f)
            {
                if (audio_source.clip.name.Contains("slow1")) { slow1_listened = true; }
                else if (audio_source.clip.name.Contains("slow2")) { slow2_listened = true; }
                else if (audio_source.clip.name.Contains("fast1")) { fast1_listened = true; }
                else if (audio_source.clip.name.Contains("fast2")) { fast2_listened = true; }
                else if (audio_source.clip.name.Contains("fast3")) { fast3_listened = true; }
                if (slow1_listened && slow2_listened && fast1_listened && fast2_listened && fast3_listened)
                {
                    if (!References.save_manager.saved_data.achievement_the_real_folk_blues) { References.save_manager.GiveAchievement("The real folk blues"); }
                    References.save_manager.SaveData();
                }
            }
        }
    }

    public void StartPlaying()
    {
        no_music = false;
    }
    public void StopPlaying()
    {
        no_music = true;
        try
        {
            audio_source.Stop();
        }
        catch
        {
            //Can be called when audio_source is not ready, not a problem
        }
    }

    public void create_track_list(int speed=-1)
    {
        tracklist_speed = speed;
        if (speed == 0)
        {
            // slow_tracks
            List<AudioClip> temp_list = new List<AudioClip>(slow_tracks);
            while (temp_list.Count > 0)
            {
                int rand_index = Random.Range(0, temp_list.Count);
                track_list.Add(temp_list[rand_index]);
                temp_list.RemoveAt(rand_index);
            }
        }
        else if(speed == 1)
        {
            // fast_tracks
            List<AudioClip> temp_list = new List<AudioClip>(fast_tracks);
            while (temp_list.Count > 0)
            {
                int rand_index = Random.Range(0, temp_list.Count);
                track_list.Add(temp_list[rand_index]);
                temp_list.RemoveAt(rand_index);
            }
        }
        else
        {
            // All tracks
            List<AudioClip> temp_list = new List<AudioClip>(slow_tracks);
            temp_list.AddRange(fast_tracks);
            while (temp_list.Count > 0)
            {
                int rand_index = Random.Range(0, temp_list.Count);
                track_list.Add(temp_list[rand_index]);
                temp_list.RemoveAt(rand_index);
            }
        }
    }

    AudioClip  pop_from_track_list()
    {
        // fill if empty
        if (track_list.Count <= 0)
        {
            create_track_list(tracklist_speed); // Use last speed
        }

        // pop first
        AudioClip track = track_list[0];
        track_list.RemoveAt(0);

        return track;
    }

    public void SetTracklistSpeed(int speed, bool let_previous_playlist_finish=true)
    {
        tracklist_speed = speed;

        // Reset playlist immediatly?
        // Not needed if we only have 2 slow songs for example
        if (!let_previous_playlist_finish)
        {
            create_track_list();
        }
    }
}
