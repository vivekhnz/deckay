using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenLogic : MonoBehaviour
{
    // Start is called before the first frame update
    public void playGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void howToPlay()
    {
        SceneManager.LoadScene("HowToPlay");
    }

    public void Cerdits()
    {
        SceneManager.LoadScene("CreditsScene");
    }
}
