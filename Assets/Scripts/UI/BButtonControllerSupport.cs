using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Just listens to the PlayerControls.Confirm event and triggers the button this script is attached to. 
/// </summary>
/// <remarks>
/// this class is the pinnacle of laziness x2 man!!! Look at that class name. Good god.
/// </remarks>
public class BButtonControllerButtonSupport : MonoBehaviour
{

    // yeah yeah yeah, these do not belong every-freakin-where, these belong in a single input class with an even tahat says "HandleConfirmPressed" or something, then everything subscribes, and problem solved.
    // I'm just not feelin it Jack. Just not feelin it today. Nah. Not gonna do it.
    // Just letting you know I know. Just so you know that I know. Now you know that I know, and I know that you know, so we know that I know.
    private PlayerControls controls;

    // Start is called before the first frame update
    void Start()
    {
        controls = new PlayerControls();
        controls.UI.BButton.performed += HandleConfirmInput;
        controls.UI.Enable();
    }

    void HandleConfirmInput(InputAction.CallbackContext cntxt)
    {
        GetComponent<Button>().onClick.Invoke();
        controls.UI.Disable();
    }

    private void OnDestroy()
    {
        controls.UI.Confirm.performed -= HandleConfirmInput;
    }
}
