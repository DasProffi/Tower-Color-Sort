using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject towerRenderer; 
    public const int NumberOfTowers = 5; 
    private readonly GameObject[] _towersRenderer = new GameObject[NumberOfTowers];
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < NumberOfTowers; i++)
        {
            // Create a new box GameObject
            _towersRenderer[i] = Instantiate(towerRenderer, transform);

            RectTransform rectTransform = _towersRenderer[i].GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(2 - 3 * i, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
