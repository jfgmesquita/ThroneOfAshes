using UnityEngine;
using UnityEngine.UI;

public class SoldierHealthBar : MonoBehaviour
{
    [SerializeField] Image healthbar;
    Camera cam;
    [SerializeField] float reduceSpeed = 2;
    float target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        healthbar.fillAmount = Mathf.MoveTowards(healthbar.fillAmount, target, reduceSpeed * Time.deltaTime);
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        target = currentHealth / maxHealth;
    }
}
