using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controller for the player, also handles a few player specific things like death.
/// </summary>
/// <remarks>
/// Honestly I kind of feel like input should be handled in its own class. Because then it would be very easy to find, understand, debug.
/// But I don't feel like it at the moment.
/// </remarks>
public class Player : MonoBehaviour
{
	public UnityEvent<InventoryWeapon> OnSwitchWeapons = new UnityEvent<InventoryWeapon>();
	public UnityEvent<Equipment> OnUpdateEquipment = new UnityEvent<Equipment>();
	public UnityEvent OnPlayerDeath = new UnityEvent();

	// these should be in a SO
	public float baseControllerAimRotaitonSensitivity;
	public float controllerMaxRotationSensitivity;
	public float controllerRotationSensitivity;
	public WeaponData startWeapon;

	private Actor actor;

	private bool targetInSights;
	private bool attemptingToFire;
	private bool triggerPull;

	///////////////
	#region Unity Event Methods

	private void Awake()
	{
		actor = GetComponent<Actor>();
		actor.team = Actor.ActorTeam.Friendly;

        PlayerInput.OnSprint.AddListener(HandleSprintInput);
		PlayerInput.OnAim.AddListener(HandleAimInput);
		PlayerInput.OnFire.AddListener(HandleFireInput);
		PlayerInput.OnSwitchWeapons.AddListener(HandleSwitchWeaponsInput);
        PlayerInput.OnReload.AddListener(HandleReloadInput);
        PlayerInput.OnUseEquipment.AddListener(HandleUseEquipmentInput);
        PlayerInput.OnInteract.AddListener(HandleInteractInput);
	}

	private void Start()
	{
		actor.OnDeath.AddListener(HandlePlayerDeath);
		actor.OnGetHit.AddListener(HandleGetHit);
		actor.OnHeal.AddListener(HandleHeal);
		actor.SetWeaponFromData(startWeapon);
	}

    private void OnEnable()
    {
		PlayerInput.EnableControls();
    }

	private void OnDisable()
	{
		PlayerInput.DisableControls();
	}

	private void Update()
	{
		if (!GameplayUI.Instance || !GameplayUI.Instance.InMenu())
		{
			if (attemptingToFire)
			{
				actor.AttemptAttack(triggerPull);
				triggerPull = false;
			}

			//if (Input.GetKeyDown(KeyCode.C))
			//{
			//	actor.ToggleCrouch();
			//}
		}
	}

	private void FixedUpdate()
	{
		if (!GameplayUI.Instance || !GameplayUI.Instance.InMenu())
		{
			// rotation is based on movement when sprinting and rotation input when otherwise. So need this.
			Vector2 rotationInputToUse;
			if (!actor.state[Actor.State.Sprinting])
			{
				//// Movement ////
				// get the move direction
				Vector3 moveDir = new Vector3(PlayerInput.MovementInput.x, 0f, PlayerInput.MovementInput.y);

				moveDir = Vector3.ClampMagnitude(moveDir, 1f);

				actor.Move(moveDir, false);

				rotationInputToUse = PlayerInput.RotationInput;
			}
			else
			{
				// only go forward if sprinting
				actor.Move(transform.forward, false);
				rotationInputToUse = PlayerInput.UsingMouseForRotation ? PlayerInput.RotationInput : PlayerInput.MovementInput;
			}

			if (rotationInputToUse != Vector2.zero)
			{
				// Get the angle of rotation based on the controller input (look, math! I did it!)
				float newRotationYAngle = Mathf.Atan(rotationInputToUse.x / rotationInputToUse.y) * Mathf.Rad2Deg;

				// handle the wierd problem with negative y values (idk why man it works ok?)
				if (rotationInputToUse.y < 0)
				{
					newRotationYAngle -= 180;
				}

				float rotationDelta = Mathf.Abs(newRotationYAngle - transform.rotation.eulerAngles.y);

				// fix if we're going from 360 to 0 or the other way; this is confusing but don't stress it.
				// basically just need to remember that transform.Rotate tatkes a number of degrees to rotate as a param. So going from 359 -> 0  degree rotation should not be -359 degrees, but should be 1 degree. Ya feel me?
				if (rotationDelta >= 180f)
				{
					rotationDelta -= 359f;
				}

				float controllerAimRotationSensitivity = baseControllerAimRotaitonSensitivity;
				if (!PlayerInput.UsingMouseForRotation && targetInSights)
				{
					controllerAimRotationSensitivity = .01f;
				}

				float stratifiedRotation = rotationDelta / controllerMaxRotationSensitivity;
				float adjustedRotationDelta = stratifiedRotation * (actor.state[Actor.State.Aiming] ? controllerAimRotationSensitivity : controllerRotationSensitivity);
				float adjustedRotationValue = transform.rotation.eulerAngles.y > newRotationYAngle ? -adjustedRotationDelta : adjustedRotationDelta;

				Vector3 finalNewEulers = transform.rotation.eulerAngles + new Vector3(0f, adjustedRotationValue, 0f);
				Quaternion finalNewRotation = new Quaternion();
				finalNewRotation.eulerAngles = finalNewEulers;
				transform.rotation = finalNewRotation;
            }
		}
	}

	private void OnDestroy()
	{
		actor.OnDeath.RemoveListener(HandlePlayerDeath);
		actor.OnGetHit.RemoveListener(HandleGetHit);
		actor.OnHeal.RemoveListener(HandleHeal);
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
				triggerPull = true;
				attemptingToFire = true;
			}
		}
		else
        {
			triggerPull = false;
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

	private void HandlePlayerDeath()
	{
		enabled = false;
		OnPlayerDeath.Invoke();
	}

	/// <summary>
	/// Don't need params; just update the health UI.
	/// </summary>
	/// <param name="hitLocation"></param>
	/// <param name="hitDirection"></param>
	private void HandleGetHit(Projectile proj)
	{
		UpdateHealthUI();
	}

	private void HandleHeal()
    {
		GameplayUI.Instance.HealthFlash();
		UpdateHealthUI();
    }

	private void UpdateHealthUI()
    {
		GameplayUI.Instance.SetVignette(1f - ((float) actor.HitPoints) / ((float) actor.MaxHitPoints));
	}
}
