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
    public Transform playerHandPanel;
    public Text currentPhaseText;

    private Deck deck;
    private GamePhase currentPhase;

    // Start is called before the first frame update
    void Start()
    {
        currentPhase = GamePhase.Dealing;
        BeginPhase(currentPhase);
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

        BeginPhase(currentPhase);
    }

    void BeginPhase(GamePhase phase)
    {
        currentPhaseText.text = $"Current phase: {phase}";
        switch (phase)
        {
            case GamePhase.Dealing:
                deck = new Deck();
                for (int i = 0; i < 5; i++)
                {
                    DealCard(deck, new Vector2(i * 75, 0));
                }
                break;

            case GamePhase.PlayerChoose:
                break;

            case GamePhase.PlayerExecute:
                break;

            case GamePhase.AiChoose:
                break;

            case GamePhase.AiExecute:
                break;

            case GamePhase.Decay:
                break;
        }
    }

    private void DealCard(Deck deck, Vector2 position)
    {
        var cardObj = Instantiate(cardPrefab);
        cardObj.transform.SetParent(playerHandPanel, false);

        var transform = cardObj.GetComponent<RectTransform>();
        transform.anchoredPosition = position;

        var card = cardObj.GetComponent<CardBehaviour>();
        var cardData = deck.GetNextCard();
        card.health = cardData.Health;
        card.effect = cardData.Effect;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
