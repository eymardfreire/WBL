using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    public TMP_Text currentAngleText; // Text to display the current angle
    public TMP_Text lastAngleText;    // Text to display the last angle used
    public Slider movementSlider;     // Slider to display movement amount
    public Slider castingPowerSlider; // Slider to display casting power
    public PlayerMovement player;
    public MagicalWeapon magicalWeapon; // Reference to the MagicalWeapon script
    public SpellCasting spellCasting; // Reference to the SpellCasting script


    void Update()
    {

        // Update the casting power slider value
        if (spellCasting.IsCharging)
        {
            castingPowerSlider.value = spellCasting.CurrentCastingPower / spellCasting.maxCastingPower;
        }

        movementSlider.value = player.movementAmount / player.maxMovementAmount;

        if (currentAngleText != null)
        {
            currentAngleText.text = $"{magicalWeapon.CurrentAngle:0}";
        }

        if (lastAngleText != null)
        {
            lastAngleText.text = $"{magicalWeapon.LastAngle:0}";
        }

    }

}
