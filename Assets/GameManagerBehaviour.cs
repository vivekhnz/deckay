using UnityEngine;

public class GameManagerBehaviour : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform rootCanvas;

    // Start is called before the first frame update
    void Start()
    {
        var deck = new Deck();
        for (int i = 0; i < 5; i++)
        {
            DealCard(deck, new Vector2(i * 75, 0));
        }
    }

    private void DealCard(Deck deck, Vector2 position)
    {
        var cardObj = Instantiate(cardPrefab);
        cardObj.transform.SetParent(rootCanvas, false);

        var transform = cardObj.GetComponent<RectTransform>();
        transform.anchoredPosition = position;

        var card = cardObj.GetComponent<CardBehaviour>();
        var cardData = deck.GetNextCard();
        card.health = cardData.Health;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
