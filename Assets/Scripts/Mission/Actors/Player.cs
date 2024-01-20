using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Controller for the player, also handles a few player specific things like death.
/// </summary>
public class Player : MonoBehaviour//ActorController
{
	[HideInInspector] public UnityEvent<InventoryWeapon> OnSwitchWeapons = new UnityEvent<InventoryWeapon>();
	[HideInInspector] public UnityEvent<Equipment> OnUpdateEquipment = new UnityEvent<Equipment>();

	private bool targetInSights;
	private bool attemptingToFire;
	public AIActorController ControlledActor { get; set; }

	///////////////
	#region Unity Event Methods

	private void Awake()
	{
        PlayerInput.OnSprint.AddListener(HandleSprintInput);
		PlayerInput.OnAim.AddListener(HandleAimInput);
		PlayerInput.OnFire.AddListener(HandleFireInput);
		PlayerInput.OnSwitchWeapons.AddListener(HandleSwitchWeaponsInput);
        PlayerInput.OnReload.AddListener(HandleReloadInput);
        PlayerInput.OnUseEquipment.AddListener(HandleUseEquipmentInput);
        PlayerInput.OnInteract.AddListener(HandleInteractInput);

		MissionManager.OnAttackStart.AddListener(EnablePlayerControls);
	}

	private void Start()
	{
		//actor.OnHeal.AddListener(HandleHeal);

		StartCoroutine(AttackCoroutine());
	}

	private void OnDisable()
	{
		PlayerInput.DisableGameplayControls();
	}

	private void Update()
	{
		if (ControlledActor != null && (!MissionUI.Instance || !MissionUI.Instance.InMenu()))
		{
            if (Input.GetKeyDown(KeyCode.C))
            {
                ControlledActor.GetActor().ToggleCrouch();
            }
        }
	}

	private void FixedUpdate()
	{
		if (ControlledActor != null && (!MissionUI.Instance || !MissionUI.Instance.InMenu()))
		{
			Transform actorTransform = ControlledActor.transform;
			// rotation is based on movement when sprinting and rotation input when otherwise. So need this.
			//Vector2 rotationInputToUse;
			//if (!actor.state[Actor.State.Sprinting])
			//{
				//// Movement ////
				// get the move direction
				Vector2 moveDir = new Vector2(PlayerInput.MovementInput.x, PlayerInput.MovementInput.y);

				moveDir = Vector2.ClampMagnitude(moveDir, 1f);

				ControlledActor.GetActor().Move(moveDir, false);

				//rotationInputToUse = PlayerInput.RotationInput;
			//}
			//else
			//{
			//	// only go forward if sprinting
			//	actor.Move(transform.forward, false);
			//	rotationInputToUse = PlayerInput.UsingMouseForRotation ? PlayerInput.RotationInput : PlayerInput.MovementInput;
			//}

			//if (rotationInputToUse != Vector2.zero)
			//{
				// Get the angle of rotation based on the controller input (look, math! I did it!)
				float newRotationYAngle = Mathf.Atan(-PlayerInput.RotationInput.x / PlayerInput.RotationInput.y) * Mathf.Rad2Deg;

				// handle the wierd problem with negative y values (idk why man it works ok?)
				if (PlayerInput.RotationInput.y < 0)
				{
					newRotationYAngle -= 180;
				}

				float rotationDelta = Mathf.Abs(newRotationYAngle - actorTransform.rotation.eulerAngles.z);

				// fix if we're going from 360 to 0 or the other way; this is confusing but don't stress it.
				// basically just need to remember that transform.Rotate tatkes a number of degrees to rotate as a param. So going from 359 -> 0  degree rotation should not be -359 degrees, but should be 1 degree. Ya feel me?
				if (rotationDelta >= 180f)
				{
					rotationDelta -= 359f;
				}

				float controllerAimRotationSensitivity = ControlledActor.Data.baseControllerAimRotaitonSensitivity;
				if (!PlayerInput.UsingMouseForRotation && targetInSights)
				{
					controllerAimRotationSensitivity = .01f;
				}

				// just FYI - the controller stuff really should not be an actor thing. That is only for the player.
				// so need to create a seperate Data object for that stuff.
				float stratifiedRotation = rotationDelta / ControlledActor.Data.controllerMaxRotationSensitivity;
				float adjustedRotationDelta = stratifiedRotation * (ControlledActor.GetActor().state[Actor.State.Aiming] ? controllerAimRotationSensitivity : ControlledActor.Data.controllerRotationSensitivity);
				float adjustedRotationValue = actorTransform.rotation.eulerAngles.z > newRotationYAngle ? -adjustedRotationDelta : adjustedRotationDelta;

				Vector3 finalNewEulers = actorTransform.rotation.eulerAngles + new Vector3(0f, 0f, adjustedRotationValue);
				Quaternion finalNewRotation = new Quaternion();
				finalNewRotation.eulerAngles = finalNewEulers;
				actorTransform.rotation = finalNewRotation;
            //}
		}
	}

	private void OnDestroy()
	{
		//base.OnDestroy();
		ControlledActor.GetActor().OnHeal.RemoveListener(HandleHeal);
		PlayerInput.OnSprint.RemoveListener(HandleSprintInput);
		PlayerInput.OnAim.RemoveListener(HandleAimInput);
		PlayerInput.OnFire.RemoveListener(HandleFireInput);
		PlayerInput.OnSwitchWeapons.RemoveListener(HandleSwitchWeaponsInput);
		PlayerInput.OnReload.RemoveListener(HandleReloadInput);
		PlayerInput.OnUseEquipment.RemoveListener(HandleUseEquipmentInput);
		PlayerInput.OnInteract.RemoveListener(HandleInteractInput);
		MissionManager.OnAttackStart.RemoveListener(EnablePlayerControls);
	}

	#endregion

	///////////////
	#region Input
	///////////////

	private void EnablePlayerControls()
	{
		PlayerInput.EnableGameplayControls();
	}

	private void HandleSprintInput(bool starting)
	{
		if (ControlledActor != null)
		{
			if (starting)
			{
				ControlledActor.GetActor().SetState(Actor.State.Sprinting);
			}
			else
			{
				ControlledActor.GetActor().SetState(Actor.State.Walking);
			}
		}
	}

	private void HandleAimInput(bool starting)
	{
		if (ControlledActor != null)
		{
			if (starting)
			{
				ControlledActor.GetActor().BeginAiming();
			}
			else
			{
				ControlledActor.GetActor().EndAiming();
			}
		}
	}

	private void HandleFireInput(bool starting)
	{
		if (starting)
		{
            if (!attemptingToFire)
            {
				attemptingToFire = true;
            }
        }
		else
        {
            attemptingToFire = false;
		}
    }

	private void HandleSwitchWeaponsInput()
	{
		if (ControlledActor != null &&  ControlledActor.GetActor().GetInventory().weaponCount > 1)
		{
			bool result = ControlledActor.GetActor().AttemptSwitchWeapons();
			if (result == true)
			{
				OnSwitchWeapons.Invoke(ControlledActor.GetActor().GetEquippedWeapon());
			}
		}
	}

	private void HandleReloadInput()
	{
		if (ControlledActor != null)
		{
			ControlledActor.GetActor().AttemptReload();
		}
	}

	private void HandleUseEquipmentInput()
	{
		if (ControlledActor != null)
		{
			ControlledActor.GetActor().AttemptUseEquipment();
			OnUpdateEquipment.Invoke(ControlledActor.GetActor().GetInventory().GetEquipment());
		}
	}

	private void HandleInteractInput()
	{
		if (ControlledActor != null)
		{
			ControlledActor.GetActor().AttemptInteraction();
			// hmmmmm I haven't figured out how to handle swapping equipment, etc. Or even if I will allow that.
			OnUpdateEquipment.Invoke(ControlledActor.GetActor().GetInventory().GetEquipment());
		}
	}

	#endregion


	private IEnumerator AttackCoroutine()
	{
		while (true)
		{
			if (ControlledActor != null && attemptingToFire)
			{
				if (!ControlledActor.PauseFurtherAttacks)
				{
					StartCoroutine(ControlledActor.FireBurst(ControlledActor.GetActor().GetEquippedWeapon().data.projPerBurst));
				}

				yield return new WaitForSeconds(ControlledActor.Data.timeBetweenBursts);
			}
			yield return null;
		}
	}

	public void HandleTargetInSights(bool inSights)
    {
		targetInSights = inSights;
	}

	public Inventory GetInventory()
	{
		return ControlledActor != null ? ControlledActor.GetActor().GetInventory() : null;
	}

	/// <summary>
	/// Get the ammo and max ammo of the player's weapon
	/// </summary>
	/// <returns>2 integers: the weapon's loaded ammo, and the total backup ammo. If infinite backup, backup val will be int.MinValue</returns>
	public (int, int) GetAmmo()
	{
		if (ControlledActor != null)
		{
			//int totalAmmo = ControlledActor.GetActor().GetEquippedWeapon().data.hasInfiniteBackupAmmo ? int.MinValue : ControlledActor.GetActor().GetEquippedWeapon().amount;
			//int currentAmmo = ControlledActor.GetActor().GetEquippedWeaponAmmo();
			//return (currentAmmo, totalAmmo);
		}

		return (0, 0);
	}

	/// <summary>
	/// Don't need params; just update the health UI.
	/// </summary>
	//protected override void HandleGetHit()
	//{
	//	UpdateHealthUI();
	//}

	private void HandleHeal()
    {
		MissionUI.Instance.HealthFlash();
		UpdateHealthUI();
    }

	private void UpdateHealthUI()
	{
		if (ControlledActor != null)
		{
			MissionUI.Instance.SetVignette(1f - ((float)ControlledActor.GetActor().HitPoints) / ((float)ControlledActor.GetActor().MaxHitPoints));
		}
	}
}
