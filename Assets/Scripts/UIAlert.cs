using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAlert : MonoBehaviour
{
    public Sprite QuestionIcon;
    public Sprite AlertIcon;
    public Sprite DisabledIcon;
    public Color QuestionColor;
    public Color AlertColor;
    public Color DisabledColor;

    private Enemy monitorEnemy;
    private Image alertImage;

    void Awake()
    {
        alertImage = GetComponent<Image>();
    }

    void Start()
    {

    }

    void Update()
    {
        Vector3 monitor_vp_pos = 
            Camera.main.WorldToViewportPoint(monitorEnemy.transform.position);

        if (!(monitor_vp_pos.x < 0 || monitor_vp_pos.x > 1 ||
            monitor_vp_pos.y < 0 || monitor_vp_pos.y > 1))
        {
            transform.position = Camera.main.WorldToScreenPoint(
                monitorEnemy.transform.position + new Vector3(0.0f, 0.5f, 0.0f));

            if (monitorEnemy.Alerted)
                ActivateAlert();
            else if (monitorEnemy.Questioning)
                ActivateQuestion();
            else
                Deactivate();
        }
        else
        {
            Deactivate();
        }
    }

    public void Setup(Enemy monitor)
    {
        monitorEnemy = monitor;
        Deactivate();
    }

    public void ActivateQuestion()
    {
        alertImage.sprite = QuestionIcon;
        alertImage.color = QuestionColor;
    }

    public void ActivateAlert()
    {
        alertImage.sprite = AlertIcon;
        alertImage.color = AlertColor;
    }

    public void Deactivate()
    {
        alertImage.sprite = DisabledIcon;
        alertImage.color = DisabledColor;
    }
}
