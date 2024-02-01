using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;

/// <summary>
/// Directly interacts with PlayerControls and sends out events / stores input info.
/// </summary>
public static class PlayerInput
{
	public static UnityEvent<bool> OnSprint = new UnityEvent<bool>();
	public static UnityEvent<bool> OnAim = new UnityEvent<bool>();
	public static UnityEvent<bool> OnFire = new UnityEvent<bool>();
	public static UnityEvent OnSwitchWeapons = new UnityEvent();
	public static UnityEvent OnReload = new UnityEvent();
	public static UnityEvent OnUseEquipment= new UnityEvent();
	public static UnityEvent OnInteract = new UnityEvent();
	public static UnityEvent OnConfirm = new UnityEvent();
	public static UnityEvent OnSelect = new UnityEvent();
	public static UnityEvent OnBButton = new UnityEvent();
	public static UnityEvent<Vector2> OnNavigate = new UnityEvent<Vector2>();
	public static UnityEvent OnDragStarted = new UnityEvent();
	public static UnityEvent OnDragEnded = new UnityEvent();
	public static UnityEvent OnCommandModeEnter = new UnityEvent();
	public static UnityEvent OnCommandModeExit = new UnityEvent();

	public static Vector2 MovementInput => movementInput;
	private static Vector2 movementInput;
	public static Vector2 RotationInput => rotationInput;
	private static Vector2 rotationInput;
	public static bool UsingMouseForRotation => usingMouseForRotation;
	private static bool usingMouseForRotation;

	private static List<FriendlyActorController> selectedFriendlies = new List<FriendlyActorController>();
	private static AIActorController targetedActor;

	public static Vector3 dragStart;
	public static Vector3 dragEnd;

	private static PlayerControls controls;

	public static void InitializeControls()
    {
		if (controls == null)
		{
			controls = new PlayerControls();
		}

		controls.Gameplay.EnterCommandMode.performed += HandleCommandModeEnter;
		controls.Gameplay.Move.performed += HandleMovementInput;
        controls.Gameplay.Move.canceled += ZeroMovementInput;
        controls.Gameplay.Sprint.performed += HandleSprintPerformedInput;
        controls.Gameplay.Sprint.canceled += HandleSprintStopInput;
        controls.Gameplay.Rotate.performed += HandleRotationInput;
        controls.Gameplay.RotateMouse.performed += HandleRotationInputMouse;
        controls.Gameplay.Aim.performed += HandleAimBeginInput;
        controls.Gameplay.Aim.canceled += HandleAimEndInput;
        controls.Gameplay.Fire.started += HandleFireStartInput;
        controls.Gameplay.Fire.canceled += HandleFireStopInput;
        controls.Gameplay.SwitchWeapons.performed += HandleSwitchWeaponsInput;
        controls.Gameplay.SwitchWeaponsMouse.performed += HandleSwitchWeaponsInput;
        controls.Gameplay.Reload.performed += HandleReloadInput;
        controls.Gameplay.UseEquipment.performed += HandleUseEquipmentInput;
        controls.Gameplay.Interact.performed += HandleInteractInput;
		controls.Gameplay.ReleasePawn.started += HandleReleasePawn;
		controls.UI.Confirm.performed += HandleConfirmInput;
		controls.UI.Select.performed += HandleSelectInput;
		controls.UI.Navigate.started += HandleNavigationInput;
		controls.UI.BButton.performed += HandleBButtonInput;
		controls.Command.ExitCommandMode.performed += HandleCommandModeExit;
		controls.Command.Select.performed += HandleCommandSelect;
		controls.Command.RightClick.performed += HandleCommandRightClick;
		controls.Command.Drag.started += HandleCommandDragStart;
		controls.Command.Drag.canceled += HandleCommandDragEnd;
		controls.Command.FollowMe.performed += HandleCommandFollow;
		controls.Command.ControlPawn.started += HandleControlPawn;

		MissionManager.OnMissionEnd.AddListener(HandleMissionEnd);
	}

	private static void HandleControlPawn(InputAction.CallbackContext cntxt)
	{
		if (selectedFriendlies.Count > 0)
		{
			FriendlyActorController controlledActor = selectedFriendlies[0];
			MissionManager.Instance.Player.ControlledActor = controlledActor;
			controlledActor.GetActor().IsPlayer = true;
			controlledActor.SetActorControlled(true);
			controlledActor.OnActorDeath.AddListener(HandleControlledActorDeath);
			HandleCommandModeExit(cntxt);
		}
	}

	private static void HandleReleasePawn(InputAction.CallbackContext cntxt)
    {
		FriendlyActorController controlledActor = MissionManager.Instance.Player.ControlledActor as FriendlyActorController;
		controlledActor.SetActorControlled(false);
		controlledActor.GetActor().IsPlayer = false;
		MissionManager.Instance.Player.ControlledActor = null;
		controlledActor.OnActorDeath.RemoveListener(HandleControlledActorDeath);
		HandleCommandModeEnter(cntxt);
	}

	private static void HandleControlledActorDeath()
    {
		// no need for the context but it's a param, whatever
		HandleReleasePawn(new InputAction.CallbackContext());
	}

	private static void HandleCommandFollow(InputAction.CallbackContext cntxt)
	{
		if (MissionManager.Instance.Player.ControlledActor != null)
		{
			selectedFriendlies.ForEach(x => x.FollowThisThing(MissionManager.Instance.Player.ControlledActor.transform));
		}
	}

	private static void HandleCommandSelect(InputAction.CallbackContext cntxt)
    {
		DeselectFriendlies();

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1000f, LayerMask.GetMask("SelectionTriggers"));

		// I actually don't know why this conditional works. Interesting. I wonder if there's an implemented boolean comparision in the type...
		if (hit)
		{
			// only select friendly actors for now.
			AIActorController actorCont = hit.transform.gameObject.GetComponent<AIActorController>();
			if (actorCont != null && actorCont.GetActor().team == Actor.ActorTeam.Friendly)
			{
				DeselectFriendlies();

				selectedFriendlies.Add(hit.transform.gameObject.GetComponent<FriendlyActorController>());
				hit.transform.gameObject.GetComponent<FriendlyActorController>().UpdateSelection(true);
			}
		}
	}

	private static void HandleMissionEnd(bool playerWon)
    {
		selectedFriendlies.Clear();
	}

	private static void DeselectFriendlies()
    {
		foreach (FriendlyActorController friendly in selectedFriendlies)
		{
			friendly.UpdateSelection(false);
		}

		selectedFriendlies.Clear();
	}

	private static void HandleCommandDragStart(InputAction.CallbackContext cntxt)
	{
		dragStart = Input.mousePosition;
		OnDragStarted.Invoke();
    }

	private static void HandleCommandDragEnd(InputAction.CallbackContext cntxt)
	{
        dragEnd = Input.mousePosition;

		Vector2 dragStartWorldPos = Camera.main.ScreenToWorldPoint(dragStart);
		Vector2 dragEndWorldPos = Camera.main.ScreenToWorldPoint(dragEnd);

		// make sure the UI knows drag has stopped (or that there was no drag)
		OnDragEnded.Invoke();

		// don't consider it a drag unless 
		if ((dragEndWorldPos - dragStartWorldPos).magnitude < 2f)
        {
			return;
        }

		DeselectFriendlies();

		// get the four corners of the selelection rectangle
		float rightmostValue = dragStartWorldPos.x > dragEndWorldPos.x ? dragStartWorldPos.x : dragEndWorldPos.x;
        float leftmostValue = dragStartWorldPos.x == rightmostValue ? dragEndWorldPos.x : dragStartWorldPos.x;
        float topmostValue = dragStartWorldPos.y > dragEndWorldPos.y ? dragStartWorldPos.y : dragEndWorldPos.y;
        float bottommostValue = dragStartWorldPos.y == topmostValue ? dragEndWorldPos.y : dragStartWorldPos.y;

		// select friendlies within that rectangle
        foreach (FriendlyActorController friendly in MissionManager.Instance.friendlyActors)
        {
            if (friendly != null)
            {
                // if friendly within selection box
                if (friendly.transform.position.x > leftmostValue && friendly.transform.position.x < rightmostValue
                    && friendly.transform.position.y > bottommostValue && friendly.transform.position.y < topmostValue)
                {
                    friendly.UpdateSelection(true);
                    selectedFriendlies.Add(friendly);
                }
            }
        }
    }

    private static void HandleCommandRightClick(InputAction.CallbackContext cntxt)
	{
		// now for the record it would be good to have a "Selection Manager" probably. Or maybe a "commands manager".

		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//CoverZone coverZone = null;
		bool isTargetingSomeone = false;
		Debug.DrawRay(worldPoint, Vector3.down, Color.red);
		RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector3.forward);
		foreach (RaycastHit2D hit in hits)
		{
			// it's important that we check the collider of the hit not the transform. For some reason, the transform
			// is a reference to teh actual actor transform not the collider transform.
			AIActorController actorTarget = hit.collider.gameObject.GetComponent<AIActorController>();
			if (actorTarget != null && actorTarget.GetActor().team == Actor.ActorTeam.Enemy)
			{
				targetedActor = hit.collider.gameObject.GetComponent<AIActorController>();
				isTargetingSomeone = true;	
			}

			// need to get cover position for each actor.
			// also idk if it's super good idea to do all these raycasts. would it not be better to just detect the click on the gameobject with OnPointerDown or something?
			//coverZone = hit.collider.gameObject.GetComponent<CoverZone>();
		}

		if (!isTargetingSomeone)
        {
			targetedActor = null;
        }

		if (selectedFriendlies != null)
		{
			if (isTargetingSomeone)
			{
				selectedFriendlies.ForEach((friendly) => friendly.SetAttackTarget(targetedActor.gameObject));
			}
			else
			{
				for (int i = 0; i < selectedFriendlies.Count; i++)
                {
					Vector3 newPos;
					//if (coverZone == null)
					//{
					newPos = worldPoint;
					//newPos.y = 0f;
					// first guy should go exactly on mouse click
					if (i != 0)
					{
						// makes this kinda shape
						// x x x
						// x x x
						// x x x
						// yeah yeah yeah I know I know I know this could be done with some modulo stuff or whatever but I don't feel like it.

						if (i == 1)
						{
							newPos.x += 1;
						}
						else if (i == 2)
						{
							newPos.y += 1;
						}
						else if (i == 3)
						{
							newPos.x += 1;
							newPos.y += 1;
						}
						else if (i == 4)
						{
							newPos.x += 2;
						}
						else if (i == 5)
						{
							newPos.x += 2;
							newPos.y += 1;
						}
						else if (i == 6)
						{
							newPos.y += 2;
						}
						else if (i == 7)
						{
							newPos.x += 1;
							newPos.y += 2;
						}
						else if (i == 8)
						{
							newPos.x += 2;
							newPos.y += 2;
						}
					}

                    selectedFriendlies[i].GoToPosition(newPos);
				}

								
			}
		}
	}

	private static void HandleCommandModeExit(InputAction.CallbackContext cntxt)
	{
		if (MissionManager.Instance.Player.ControlledActor != null)
        {
            controls.Command.Disable();
			controls.Gameplay.Enable();
			OnCommandModeExit.Invoke();
        }
    }

	private static void HandleCommandModeEnter(InputAction.CallbackContext cntxt)
	{
		controls.Gameplay.Disable();
		controls.Command.Enable();

		OnCommandModeEnter.Invoke();
	}

	private static void HandleBButtonInput(InputAction.CallbackContext cntxt)
	{
		OnBButton.Invoke();
	}

	private static void HandleMovementInput(InputAction.CallbackContext cntxt)
	{
		movementInput = cntxt.ReadValue<Vector2>();
	}

	private static void ZeroMovementInput(InputAction.CallbackContext cntxt)
	{
		movementInput = Vector2.zero;
	}

	private static void HandleSprintPerformedInput(InputAction.CallbackContext cntxt)
	{
		OnSprint.Invoke(true);
	}

	private static void HandleSprintStopInput(InputAction.CallbackContext cntxt)
	{
		OnSprint.Invoke(false);
		// not really sure this is necessary any more...
		// since rotation is based on the movement input when sprinting, clean it up so we don't pop back to previous rot input value.
		rotationInput = MovementInput;
	}

	private static void HandleRotationInput(InputAction.CallbackContext cntxt)
	{
		rotationInput = cntxt.ReadValue<Vector2>();
		usingMouseForRotation = false;
	}

	private static void HandleRotationInputMouse(InputAction.CallbackContext cntxt)
	{
		// generally I don't like having more processing in here, but it seemed appropriate.
		Vector3 mousePosition = cntxt.ReadValue<Vector2>();
		Vector3 playerPositionScreen = Camera.main.WorldToScreenPoint(MissionManager.Instance.Player.ControlledActor.transform.position);
		Vector3 normalizedMouseInput = (mousePosition - playerPositionScreen).normalized;
		rotationInput = normalizedMouseInput;
		usingMouseForRotation = true;
	}

	private static void HandleAimBeginInput(InputAction.CallbackContext cntxt)
	{
		OnAim.Invoke(true);
	}

	private static void HandleAimEndInput(InputAction.CallbackContext cntxt)
	{
		OnAim.Invoke(false);
	}

	private static void HandleFireStartInput(InputAction.CallbackContext cntxt)
	{
		OnFire.Invoke(true);
	}

	private static void HandleFireStopInput(InputAction.CallbackContext cntxt)
	{
		OnFire.Invoke(false);
	}

	private static void HandleSwitchWeaponsInput(InputAction.CallbackContext cntxt)
	{
		OnSwitchWeapons.Invoke();
	}

	private static void HandleReloadInput(InputAction.CallbackContext cntxt)
	{
		OnReload.Invoke();
	}

	private static void HandleUseEquipmentInput(InputAction.CallbackContext cntxt)
	{
		OnUseEquipment.Invoke();
	}

	private static void HandleInteractInput(InputAction.CallbackContext cntxt)
	{
		OnInteract.Invoke();
	}

	private static void HandleConfirmInput(InputAction.CallbackContext cntxt)
	{
		OnConfirm.Invoke();
	}

	private static void HandleSelectInput(InputAction.CallbackContext cntxt)
	{
		OnSelect.Invoke();
	}

	private static void HandleNavigationInput(InputAction.CallbackContext cntxt)
	{
		OnNavigate.Invoke(cntxt.ReadValue<Vector2>());
	}

	/// <summary>
	/// Enable gameplay controls.
	/// </summary>
	public static void EnableGameplayControls()
	{
		if (controls == null)
		{
			controls = new PlayerControls();
		}
		controls.Gameplay.Enable();
	}

	/// <summary>
	/// Disable gameplay controls.
	/// </summary>
	public static void DisableGameplayControls()
	{
		if (controls == null)
		{
			controls = new PlayerControls();
		}
		controls.Gameplay.Disable();
	}

    /// <summary>
    /// Disable gameplay controls.
    /// </summary>
    public static void EnableMenuControls()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
        }
        controls.UI.Enable();
    }

    //	/// <summary>
    ///// Disable gameplay controls.
    ///// </summary>
    public static void DisableMenuControls()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
        }
        controls.UI.Disable();
	}

	public static void EnableCommandControls()
	{
		if (controls == null)
		{
			controls = new PlayerControls();
		}

		controls.Command.Enable();
	}

	public static void DisableCommandControls()
	{
		if (controls == null)
		{
			controls = new PlayerControls();
		}

		controls.Command.Disable();
	}
}
