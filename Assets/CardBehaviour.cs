using UnityEngine;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour
{
    public Text healthText;
    public Text effectText;
    public CardData data;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Awake()
    {
        healthText.text = $"HP: {data.health}";
        effectText.text = $"Effect: {data.effect}";
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = $"HP: {data.health}";
        effectText.text = $"Effect: {data.effect}";
    }
}
