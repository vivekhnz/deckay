using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DoThings : MonoBehaviour
{
    public void GoBack()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
