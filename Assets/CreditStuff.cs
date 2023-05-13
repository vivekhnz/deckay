using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditStuff : MonoBehaviour
{
    public void GoBack()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
