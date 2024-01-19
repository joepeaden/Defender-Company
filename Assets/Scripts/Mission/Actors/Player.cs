using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Controller for the player, also handles a few player specific things like death.
/// </summary>
public class Player : ActorController
{
	[HideInInspector] public UnityEvent<InventoryWeapon> OnSwitchWeapons = new UnityEvent<InventoryWeapon>();
	[HideInInspector] public UnityEvent<Equipment> OnUpdateEquipment = new UnityEvent<Equipment>();

	private bool targetInSights;
	private bool attemptingToFire;

	///////////////
	#region Unity Event Methods

	private new void Awake()
	{
		base.Awake();

        PlayerInput.OnSprint.AddListener(HandleSprintInput);
		PlayerInput.OnAim.AddListener(HandleAimInput);
		PlayerInput.OnFire.AddListener(HandleFireInput);
		PlayerInput.OnSwitchWeapons.AddListener(HandleSwitchWeaponsInput);
        PlayerInput.OnReload.AddListener(HandleReloadInput);
        PlayerInput.OnUseEquipment.AddListener(HandleUseEquipmentInput);
        PlayerInput.OnInteract.AddListener(HandleInteractInput);

		MissionManager.OnAttackStart.AddListener(EnablePlayerControls);
	}

	private new void Start()
	{
		actor.OnHeal.AddListener(HandleHeal);

		StartCoroutine(SetupGear());

		string[] nameOptions =
		{
			"Rourke",
			"Niels",
			"Smith",
			"Danson",
			"Peters",
			"Wang",
			"O'Malley",
			"Bauer",
			"Rochefort",
			"Dumas",
			"Garcia",
			"Vargas",
			"Anderson",
			"Thomas",
			"Brown",
			"Grey",
			"Benson",
			"Al-Hilli",
			"Cohen",
			"Rosenberg",
			"Goldstein"
		};
		int randomIndex = Random.Range(0, nameOptions.Length - 1);
		MissionUI.Instance.AddEntityMarker(this, nameOptions[randomIndex]);

		StartCoroutine(AttackCoroutine());
	}

	private IEnumerator AttackCoroutine()
	{
		while (true)
		{
			if (attemptingToFire)
			{
				if (!pauseFurtherAttacks)
				{
					StartCoroutine(FireBurst(actor.GetEquippedWeapon().data.projPerBurst));
				}

				yield return new WaitForSeconds(data.timeBetweenBursts);
			}
			yield return null;
		}
	}

	private IEnumerator SetupGear()
    {
		yield return new WaitUntil(GameManager.Instance.IsInitialized);

		// add weapons the player owns - for now. This needs to be cleaned up where you can just AttemptAddItem and pass in the GearData.
		foreach (GearData gear in GameManager.Instance.Company.GetOwnedGear().Values)
		{
			if (gear as WeaponData)
			{
				actor.GetInventory().AttemptAddItem(new InventoryWeapon((WeaponData)gear));
			}
			else if (gear as MedkitData)
			{
				actor.GetInventory().AttemptAddItem(new MedkitEquipment((MedkitData)gear));
			}
			else if (gear as ExplosiveData)
			{
				actor.GetInventory().AttemptAddItem(new ExplosiveEquipment((ExplosiveData)gear));
			}
		}
	}

    private void EnablePlayerControls()
    {
        PlayerInput.EnableGameplayControls();
    }

	private void OnDisable()
	{
		PlayerInput.DisableGameplayControls();
	}

	private void Update()
	{
		if (!MissionUI.Instance || !MissionUI.Instance.InMenu())
		{
            if (Input.GetKeyDown(KeyCode.C))
            {
                actor.ToggleCrouch();
            }
        }
	}

	private void FixedUpdate()
	{
		if (!MissionUI.Instance || !MissionUI.Instance.InMenu())
		{
			// rotation is based on movement when sprinting and rotation input when otherwise. So need this.
			//Vector2 rotationInputToUse;
			//if (!actor.state[Actor.State.Sprinting])
			//{
				//// Movement ////
				// get the move direction
				Vector2 moveDir = new Vector2(PlayerInput.MovementInput.x, PlayerInput.MovementInput.y);

				moveDir = Vector2.ClampMagnitude(moveDir, 1f);

				actor.Move(moveDir, false);

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

				float rotationDelta = Mathf.Abs(newRotationYAngle - transform.rotation.eulerAngles.z);

				// fix if we're going from 360 to 0 or the other way; this is confusing but don't stress it.
				// basically just need to remember that transform.Rotate tatkes a number of degrees to rotate as a param. So going from 359 -> 0  degree rotation should not be -359 degrees, but should be 1 degree. Ya feel me?
				if (rotationDelta >= 180f)
				{
					rotationDelta -= 359f;
				}

				float controllerAimRotationSensitivity = data.baseControllerAimRotaitonSensitivity;
				if (!PlayerInput.UsingMouseForRotation && targetInSights)
				{
					controllerAimRotationSensitivity = .01f;
				}

				float stratifiedRotation = rotationDelta / data.controllerMaxRotationSensitivity;
				float adjustedRotationDelta = stratifiedRotation * (actor.state[Actor.State.Aiming] ? controllerAimRotationSensitivity : data.controllerRotationSensitivity);
				float adjustedRotationValue = transform.rotation.eulerAngles.z > newRotationYAngle ? -adjustedRotationDelta : adjustedRotationDelta;

				Vector3 finalNewEulers = transform.rotation.eulerAngles + new Vector3(0f, 0f, adjustedRotationValue);
				Quaternion finalNewRotation = new Quaternion();
				finalNewRotation.eulerAngles = finalNewEulers;
				transform.rotation = finalNewRotation;
            //}
		}
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		actor.OnHeal.RemoveListener(HandleHeal);
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

	private void HandleSprintInput(bool starting)
	{
		if (starting)
		{
			actor.SetState(Actor.State.Sprinting);
		}
		else
        {
			actor.SetState(Actor.State.Walking);
		}
	}

	private void HandleAimInput(bool starting)
	{
		if (starting)
		{
			actor.BeginAiming();
		}
		else
        {
			actor.EndAiming();
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
		if (actor.GetInventory().weaponCount > 1)
		{
			bool result = actor.AttemptSwitchWeapons();
			if (result == true)
			{
				OnSwitchWeapons.Invoke(actor.GetEquippedWeapon());
			}
		}
	}

    private void HandleReloadInput()
	{
		actor.AttemptReload();
	}

	private void HandleUseEquipmentInput()
	{
		actor.AttemptUseEquipment();
		OnUpdateEquipment.Invoke(actor.GetInventory().GetEquipment());
	}

	private void HandleInteractInput()
	{
		actor.AttemptInteraction();
		// hmmmmm I haven't figured out how to handle swapping equipment, etc. Or even if I will allow that.
		OnUpdateEquipment.Invoke(actor.GetInventory().GetEquipment());
	}

	#endregion

	public void HandleTargetInSights(bool inSights)
    {
		targetInSights = inSights;
	}

	public Inventory GetInventory()
	{
		return actor.GetInventory();
	}

	/// <summary>
	/// Get the ammo and max ammo of the player's weapon
	/// </summary>
	/// <returns>2 integers: the weapon's loaded ammo, and the total backup ammo. If infinite backup, backup val will be int.MinValue</returns>
	public (int, int) GetAmmo()
	{
		int totalAmmo = actor.GetEquippedWeapon().data.hasInfiniteBackupAmmo ? int.MinValue : actor.GetEquippedWeapon().amount;
		int currentAmmo = actor.GetEquippedWeaponAmmo();

		return (currentAmmo, totalAmmo);
	}

	/// <summary>
	/// Don't need params; just update the health UI.
	/// </summary>
	protected override void HandleGetHit()
	{
		UpdateHealthUI();
	}

	private void HandleHeal()
    {
		MissionUI.Instance.HealthFlash();
		UpdateHealthUI();
    }

	private void UpdateHealthUI()
    {
		MissionUI.Instance.SetVignette(1f - ((float) actor.HitPoints) / ((float) actor.MaxHitPoints));
	}
}
