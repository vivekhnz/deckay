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
        healthText.text = data.Health.ToString();
        effectText.text = $"Effect: {data.Effect}";
    }

    public void onCardClicked()
    {
        if (isClickable == true)
        {
            var transform = this.GetComponent<RectTransform>();
            transform.anchoredPosition += Vector2.up * 20;
            isClickable = false;
            Invoke("Discard", 2f);
            onClickAction.Invoke();
        }
    }

    void Discard()
    {
        Destroy(gameObject);
    }
}
