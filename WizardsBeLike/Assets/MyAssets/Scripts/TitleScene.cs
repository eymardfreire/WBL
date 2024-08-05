using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    //public AudioSource backgroundMusic; // Assign this in the inspector
    public string characterSelectionSceneName = "CharacterSelectionScene";

    void Start()
    {
        //backgroundMusic.Play();
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(characterSelectionSceneName);
        }
    }
}
