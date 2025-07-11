using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPBar : MonoBehaviour
{
    public Image fillImage;
    public Transform target;
    public Vector3 offset = Vector3.up * 1f;

    public float maxHP = 10f;
    public float currentHP = 10f;

    private RectTransform rectTransform;
    private Canvas canvas;

    public TMP_Text hpText;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (target != null && canvas != null)
        {
            Vector3 worldPos = target.position + offset;
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);

            Vector2 anchoredPos;
            RectTransform canvasRect = canvas.transform as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, canvas.worldCamera, out anchoredPos);

            rectTransform.anchoredPosition = anchoredPos;
        }

        if (fillImage != null)
            fillImage.fillAmount = currentHP / maxHP;
    }

    public void UpdatePositionImmediate()
    {
        if (target != null && canvas != null)
        {
            Vector3 worldPos = target.position + offset;
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);

            Vector2 anchoredPos;
            RectTransform canvasRect = canvas.transform as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, canvas.worldCamera, out anchoredPos);

            rectTransform.anchoredPosition = anchoredPos;
        }
    }

    public void SetHP(float hp, float max)
    {
        currentHP = hp;
        maxHP = max;

        if (hpText !=null)
        {
            hpText.text = $"{Mathf.CeilToInt(hp)}";
        }
    }
}
