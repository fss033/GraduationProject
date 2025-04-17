using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public CharacterHealth characterHealth;
    public Slider healthSlider;

    void Start()
    {
        healthSlider.maxValue = characterHealth.maxHealth;
        healthSlider.value = characterHealth.currentHealth;
    }

    void Update()
    {
        healthSlider.value = characterHealth.currentHealth;
    }
}