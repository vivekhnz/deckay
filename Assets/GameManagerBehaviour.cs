using UnityEngine;

public class GameManagerBehaviour : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform rootCanvas;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            var card = Instantiate(cardPrefab);
            card.transform.SetParent(rootCanvas, false);
            
            var transform = card.GetComponent<RectTransform>();
            transform.anchoredPosition = new Vector2(i * 75, 0);

            var cardData = card.GetComponent<CardBehaviour>();
            cardData.health = Random.Range(3, 7);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
