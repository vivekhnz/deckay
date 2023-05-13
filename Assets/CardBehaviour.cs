using UnityEngine;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour
{
    public Text healthText;
    public Text effectText;
    public CardData data = new CardData();

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Awake()
    {
        healthText.text = $"HP: {data.Health}";
        effectText.text = $"Effect: {data.Effect}";
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = $"HP: {data.Health}";
        effectText.text = $"Effect: {data.Effect}";
    }
}
