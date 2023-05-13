using UnityEngine;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour
{
    public Text healthText;
    public Text effectText;
    public int health = 5;
    public string effect = "Nuetral";

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Awake()
    {
        healthText.text = $"HP: {health}";
        effectText.text = $"Effect: {effect}";
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = $"HP: {health}";
        effectText.text = $"Effect: {effect}";
    }
}
