using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;

public class LogUI : MonoBehaviour
{
    // ==============================================================================
    // constructor/Initializer
    // ==============================================================================

    void Start()
    {
        textPool = new ObjectPool<GameObject>(createFunc: () => Instantiate(textObject), actionOnGet: obj => obj.SetActive(true), actionOnRelease: obj => obj.SetActive(false), actionOnDestroy: obj => Destroy(obj), maxSize: 100);
        CreateBasicPoolObject(textPool, 10);
    }
    public void CreateBasicPoolObject(ObjectPool<GameObject> pool, int number)
    {
        GameObject[] preCreationObjects = new GameObject[number];

        for (int i = 0; i < number; i++)
            preCreationObjects[i] = pool.Get();

        for (int i = 0; i < number; i++)
            pool.Release(preCreationObjects[i]);
    }

    // ==============================================================================
    // Unity FrameCycle Methods
    // ==============================================================================

    void Update()
    {
        if (isMouseOver)
        {
            // 마우스 올라가 있으면 바로 보이게
            canvasGroup.alpha = 1f;
            fadeTimer = 0f;
        }
        else if(canvasGroup.alpha > 0)
        {
            fadeTimer += Time.deltaTime;

            if (fadeTimer > fadeDelay)
                canvasGroup.alpha = Mathf.Clamp01(canvasGroup.alpha - Time.deltaTime * fadeSpeed);
        }
    }


    // ==============================================================================
    // Methods
    // ==============================================================================

    public void Log(string text)
    {
        if (MessagePanel.transform.childCount >= maxLogs)
        {
            GameObject target = MessagePanel.transform.GetChild(0).gameObject;
            target.transform.SetParent(null);
            textPool.Release(target);
        }

        GameObject newInstance = textPool.Get();
        newInstance.transform.SetParent(MessagePanel.transform);
        newInstance.GetComponent<TextMeshProUGUI>().text = text;

        canvasGroup.alpha = 1.0f;
        fadeTimer = 0.0f;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }



    public void OnPointerEnter() { fadeTimer = 0.0f; isMouseOver = true; }
    public void OnPointerExit() { fadeTimer = 0.0f; isMouseOver = false; }

    // ==============================================================================
    // variables
    // ==============================================================================

    private bool isMouseOver = false;
    private float fadeTimer = 0.0f;

    [SerializeField] private float fadeDelay = 3.0f;
    [SerializeField] private float fadeSpeed = 2.0f;

    [SerializeField] private GameObject textObject = null;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject MessagePanel = null;
    [SerializeField] private ScrollRect scrollRect = null;
    [SerializeField] private int maxLogs = 100;
    ObjectPool<GameObject> textPool;
}
