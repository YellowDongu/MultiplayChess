using System.Collections;
using TMPro;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================
    void Start()
    {
        movement *= moveSpeed;
        PanelUnFold();
        resetButton.SetActive(false);
        matchmakingPanel.SetActive(true);
        endGamePanel.SetActive(false);
    }


    // ==============================================================================
    // Methods
    // ==============================================================================


    public void PanelFold() { StartCoroutine(Fold()); }
    public void PanelUnFold() { StartCoroutine(UnFold()); }
    private IEnumerator UnFold()
    {
        unFoldButton.SetActive(false);
        foldButton.SetActive(true);
        while (panelTransform.anchoredPosition.x < 350.0f)
        {
            panelTransform.anchoredPosition += movement * Time.deltaTime;
            yield return null;
        }

        panelTransform.anchoredPosition = new Vector2(350.0f, 0.0f);
    }
    private IEnumerator Fold()
    {
        unFoldButton.SetActive(true);
        foldButton.SetActive(false);
        while (panelTransform.anchoredPosition.x > -350.0f)
        {
            panelTransform.anchoredPosition -= movement * Time.deltaTime;
            yield return null;
        }

        panelTransform.anchoredPosition = new Vector2(-350.0f, 0.0f);
    }

    public void LocalMode(bool active)
    {
        resetButton.SetActive(active);
    }

    public void AfterGamePanelActive(bool isWin)
    {
        endGameText.text = isWin ? "½Â¸®" : "ÆÐ¹è";
        PanelUnFold();
        matchmakingPanel.SetActive(false);
        endGamePanel.SetActive(true);
        rematchText.SetActive(false);
    }

    public void MatchmakingPanelActive()
    {
        matchmakingPanel.SetActive(true);
        endGamePanel.SetActive(false);
        PanelUnFold();
    }

    // ==============================================================================
    // variable
    // ==============================================================================


    [SerializeField] private float moveSpeed = 5000.0f;
    [SerializeField] private GameObject foldButton = null;
    [SerializeField] private GameObject unFoldButton = null;
    [SerializeField] private GameObject resetButton = null;
    [SerializeField] private RectTransform panelTransform = null;

    [SerializeField] private GameObject matchmakingPanel = null;
    [SerializeField] private GameObject endGamePanel = null;
    [SerializeField] private GameObject rematchText = null;
    [SerializeField] private TextMeshProUGUI endGameText = null;


    private Vector2 movement = new Vector2(1.0f, 0.0f);
}
