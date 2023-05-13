using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour
{
    public Text healthText;
    public Text effectText;
    public CardData data = new CardData();
    public bool isClickable = false;
    public UnityEvent onClickAction = new UnityEvent();
    public Sprite cardFront;
    public Sprite cardBack;
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    private void Awake()
    {
        healthText.text = $"HP: {data.Health}";
        effectText.text = $"Effect: {data.Effect}";
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = data.Health.ToString();
        effectText.text = $"Effect: {data.Effect}";
        image.sprite = data.Effect == CardAction.Draw ? cardBack : cardFront;
    }

    public void onCardClicked()
    {
        if (isClickable == true)
        {
            onClickAction.Invoke();
        }
    }
}
