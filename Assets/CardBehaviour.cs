using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour
{
    public Text healthText;
    public CardAction effect;
    public CardData data = new CardData();
    public UnityEvent onClickAction = new UnityEvent();
    public Sprite cardFront;
    public Sprite cardBack;
    public Sprite cardWildcard;
    public Sprite[] cardInfo;
    public Sprite[] decaying;
    public Sprite[] icons;
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    private void Awake()
    {
        healthText.text = $"HP: {data.Health}";
        effect = data.Effect;
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = data.Health.ToString();
        effect = data.Effect;
        cardFront = data.IsWildcard ? cardWildcard : cardInfo[(int)effect];
        image.sprite = data.IsFaceDown ? cardBack : cardFront;
    }

    public void onCardClicked()
    {
        onClickAction.Invoke();
    }
}
