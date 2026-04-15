using UnityEngine;
using TMPro;

public class Hp_number : MonoBehaviour
{
    public Enemy enemy;
    public TextMeshProUGUI hpText;

    void Update()
    {
        if (enemy == null) return;

        hpText.text = enemy.currentHealth + " / " + enemy.maxHealth;
    }
    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180f, 0);
    }
}