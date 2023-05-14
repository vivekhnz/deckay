using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour
{
    public Text healthText;
    public CardAction effect;
    public CardData data = new CardData();
    public UnityEvent onClickAction = new UnityEvent();
    public Sprite backInfo;
    public Sprite cardWildcard;
    public Sprite[] cardInfo;
    public Sprite[] decayingFront;
    public Sprite[] decayingBack;
    public Image baseImage;
    public Image infoImage;

    // Start is called before the first frame update
    void Start()
    {
        //frontImage = GetComponentInChildren<Image>();
        //backImage = transform.GetChild(0).GetComponent<Image>();
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
        var cardFrontTop = data.IsWildcard ? cardWildcard : cardInfo[(int)effect];
        int decayIndex = System.Math.Min(data.Health,decayingBack.Length-1);
        baseImage.sprite = data.IsFaceDown ? decayingBack[decayIndex] : decayingFront[decayIndex] ;
        infoImage.sprite = data. IsFaceDown ? backInfo : cardFrontTop;
    }

    public void onCardClicked()
    {
        onClickAction.Invoke();
    }

}
