using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Just listens to whatever event in PlayerInput and triggers the button this script is attached to. 
/// </summary>
public class ControllerButtonSupport : MonoBehaviour
{
    public enum InputEvent
    {
        Confirm,
        BButton
    }
    public InputEvent eventToListen;

    void Start()
    {
        switch (eventToListen)
        {
            case InputEvent.Confirm:
                PlayerInput.OnConfirm.AddListener(HandleInput);
                PlayerInput.EnableMenuControls();
                break;
            case InputEvent.BButton:
                PlayerInput.OnBButton.AddListener(HandleInput);
                break;
        }
    }

    void HandleInput()
    {
        GetComponent<Button>().onClick.Invoke();

        if (eventToListen == InputEvent.Confirm)
        {
            // these enable and disable calls should probably only happen in one or two places...
            PlayerInput.DisableMenuControls();
        }
    }

    private void OnDestroy()
    {
        switch (eventToListen)
        {
            case InputEvent.Confirm:
                PlayerInput.OnConfirm.RemoveListener(HandleInput);
                PlayerInput.EnableMenuControls();
                break;
            case InputEvent.BButton:
                PlayerInput.OnBButton.RemoveListener(HandleInput);
                break;
        }
    }
}
