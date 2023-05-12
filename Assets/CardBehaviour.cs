using UnityEngine;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour
{
    public Text text;
    public int health = 5;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Awake()
    {
        text.text = $"HP: {health}";
    }

    // Update is called once per frame
    void Update()
    {
        text.text = $"HP: {health}";
    }
}
