using UnityEngine;
using UnityEngine.UI;

public enum GamePhase
{
    Dealing = 0,
    PlayerChoose,
    PlayerExecute,
    AiChoose,
    AiExecute,
    Decay
}

public class GameManagerBehaviour : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform rootCanvas;
    public Text currentPhaseText;

    private GamePhase currentPhase;

    // Start is called before the first frame update
    void Start()
    {
        currentPhase = GamePhase.Dealing;
        currentPhaseText.text = $"Current phase: {currentPhase}";

        var deck = new Deck();
        for (int i = 0; i < 5; i++)
        {
            DealCard(deck, new Vector2(i * 75, 0));
        }
    }

    public void MoveToNextPhase()
    {
        if (currentPhase == GamePhase.Decay)
        {
            currentPhase = GamePhase.PlayerChoose;
        }
        else
        {
            currentPhase++;
        }

        currentPhaseText.text = $"Current phase: {currentPhase}";
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
