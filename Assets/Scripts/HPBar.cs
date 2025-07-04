using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Image fillImage;
    public Transform target;
    public Vector3 offset = Vector3.up * 1f; //Смещение бара, чтобы висел над головой
    public float maxHP = 10f;
    public float currentHP = 10f;

    void Update()
    {
        // Следим за позицией цели
        if (target !=null)
        {
            transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
        }

        if (fillImage !=null)
        {
            fillImage.fillAmount = currentHP / maxHP;
        }
    }

    // Обновление НР
    public void SetHP(float hp, float max)
    {
        currentHP = hp;
        maxHP = max;
    }
}
