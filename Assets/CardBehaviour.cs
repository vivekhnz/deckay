using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour
{
    public Text healthText;
    public CardData data = new CardData();
    public UnityEvent onClickAction = new UnityEvent();
    public Sprite backInfo;
    public Sprite cardWildcard;
    public Sprite[] cardInfo;
    public Sprite[] decayingFront;
    public Sprite[] decayingBack;
    public Image baseImage;
    public Image infoImage;
    private int decayIndex;
    private int maxHealth;

    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        decayIndex = decayingBack.Length - 1;
        maxHealth = data.Health;
        rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(0, 0, 1);
        rectTransform.DOScale(new Vector3(1, 1, 1), 0.7f);
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = data.Health.ToString();
        var cardFrontTop = data.IsWildcard ? cardWildcard : cardInfo[(int)data.Effect];
        if(data.Health == (int)(maxHealth / 2) && data.Health != 1)
        {
            decayIndex = 1;
        }
        else if(data.Health == 1)
        {
            decayIndex = 0;
        }
        baseImage.sprite = data.IsFaceDown ? decayingBack[decayIndex] : decayingFront[decayIndex];
        infoImage.sprite = data.IsFaceDown ? backInfo : cardFrontTop;
    }

    public void Animate(CardDestroyEffect destroyEffect, Action<CardBehaviour> onComplete)
    {
        if (destroyEffect == CardDestroyEffect.None)
        {
            onComplete(this);
            return;
        }

        /*
        rectTransform.DOBlendableLocalMoveBy(new Vector3(0, 20, 0), 0.3f);
        rectTransform.DOScale(new Vector3(1.1f, 1.1f, 0.3f), 0.3f);
        rectTransform.DOScale(new Vector3(0, 0, 1), 0.7f)
            .SetDelay(0.3f)
            .OnComplete(() => onComplete(this));
        */
        rectTransform.DOScale(new Vector3(0, 0, 1), 0.7f)
            .OnComplete(() => onComplete(this));
    }

    public void onCardClicked()
    {
        onClickAction.Invoke();
    }

}
