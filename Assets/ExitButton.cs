using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ExitButton : MonoBehaviour
{
    void Start()
    {
#if UNITY_WEBGL
        // hide the exit button when running in WebGL
        gameObject.SetActive(false);
#else
        var button = GetComponent<Button>();
        button.onClick.AddListener(QuitGame);
#endif
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
