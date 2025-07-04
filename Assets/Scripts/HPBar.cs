using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Image fillImage;
    public Transform target;
    public Vector3 offset = Vector3.up * 1f; //�������� ����, ����� ����� ��� �������
    public float maxHP = 10f;
    public float currentHP = 10f;

    void Update()
    {
        // ������ �� �������� ����
        if (target !=null)
        {
            transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
        }

        if (fillImage !=null)
        {
            fillImage.fillAmount = currentHP / maxHP;
        }
    }

    // ���������� ��
    public void SetHP(float hp, float max)
    {
        currentHP = hp;
        maxHP = max;
    }
}
