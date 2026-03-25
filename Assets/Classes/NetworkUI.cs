using System.Collections;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================
    void Start()
    {
        movement *= moveSpeed;
    }


    // ==============================================================================
    // Methods
    // ==============================================================================


    public void PanelFold() { StartCoroutine(Fold()); }
    public void PanelUnFold() { StartCoroutine(UnFold()); }
    private IEnumerator UnFold()
    {
        UnFoldButton.SetActive(false);
        FoldButton.SetActive(true);
        while (panelTransform.anchoredPosition.x < 350.0f)
        {
            panelTransform.anchoredPosition += movement * Time.deltaTime;
            yield return null;
        }

        panelTransform.anchoredPosition = new Vector2(350.0f, 0.0f);
    }
    private IEnumerator Fold()
    {
        UnFoldButton.SetActive(true);
        FoldButton.SetActive(false);
        while (panelTransform.anchoredPosition.x > -350.0f)
        {
            panelTransform.anchoredPosition -= movement * Time.deltaTime;
            yield return null;
        }

        panelTransform.anchoredPosition = new Vector2(-350.0f, 0.0f);
    }


    // ==============================================================================
    // variable
    // ==============================================================================


    [SerializeField] private float moveSpeed = 5000.0f;
    [SerializeField] private GameObject FoldButton = null;
    [SerializeField] private GameObject UnFoldButton = null;
    [SerializeField] private RectTransform panelTransform = null;
    private Vector2 movement = new Vector2(1.0f, 0.0f);
}
