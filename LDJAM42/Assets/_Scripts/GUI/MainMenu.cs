using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {


    bool musicState = true;
    public Image musicButtonImage;

    public GameObject musicController;

    public Sprite on;
    public Sprite off;

	// Use this for initialization
	void Start () {
		
	}


    public void LoadGame()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

    public void changeMusicState()
    {
        if(musicState)
        {
            musicState = false;
            musicButtonImage.sprite = off;
            musicController.GetComponent<AudioSource>().Stop();
        }
        else
        {
            musicState = true;
            musicButtonImage.sprite = on;
            musicController.GetComponent<AudioSource>().Play();
        }
    }
}
