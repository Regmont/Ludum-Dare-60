using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    private Image staminaImage;

    private void Awake()
    {
        staminaImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (playerMovement == null || staminaImage == null)
        {
            return;
        }

        staminaImage.fillAmount = playerMovement.CurrentStamina / playerMovement.MaxStamina;
    }
}
