using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music_load : MonoBehaviour {

    public AudioClip[] music;
    bool music_on;
    bool effect_on;
    public AudioSource music_audio,click_audio;
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        music_on = true;
        effect_on = true;
        music_audio = this.GetComponent<AudioSource>();
        music_audio.Play();

    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetMouseButton(0)&&effect_on)
        {
            click_audio.clip = music[2];
            click_audio.PlayOneShot(music[2]);
            click_audio.loop = false;
        }
        if (Input.GetMouseButton(1) && effect_on)
        {
            click_audio.clip = music[3];
            click_audio.PlayOneShot(music[3]);
            click_audio.loop = false;
        }
    }
    public void music_change()
    {
        music_on = !music_on;
        if(music_on)
        {
            music_audio.Play();
        }
        else
        {
            music_audio.Pause();
        }
    }
    public void effect_change()
    {
        effect_on = !effect_on;
        click_audio.enabled = effect_on;
    }
}
