using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ScoreExhibiter : MonoBehaviour
{
    GameObject ParentPanel;
    GameManager gameManager;
    [SerializeField] public TMP_Text Text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = ParentPanel.GetComponent<GameManager>();
        Text.text = "${gameManager.score}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
