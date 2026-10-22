using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject howtoButton;
    [SerializeField] GameObject howtoPanel;
    [SerializeField] GameObject howtoText;
    [SerializeField] GameObject startText;
    [SerializeField] GameObject howtoClose;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        howtoPanel.SetActive(false);
        howtoText.SetActive(false);
        howtoClose.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void HowtoGame()
    {
        howtoPanel.SetActive(true);
        howtoText.SetActive(true);
        howtoClose.SetActive(true);
        howtoButton.SetActive(false);
        startButton.SetActive(false);
        startText.SetActive(false);
    }

    public void HowtoClose()
    {
        howtoPanel.SetActive(false);
        howtoText.SetActive(false);
        howtoClose.SetActive(false);
        howtoButton.SetActive(true);
        startButton.SetActive(true);
        startText.SetActive(true);
    }
}
