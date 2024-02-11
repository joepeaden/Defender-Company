using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSprite : MonoBehaviour
{
    private WeaponData weaponData;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator gunBlastAnim;
    [SerializeField] private Animator gunMoveAnim;

    private bool isFacingLeft = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetData(WeaponData newWeaponData)
    {
        weaponData = newWeaponData;
    }

    public void PlayFireAnim()
    {
        gunBlastAnim.Play("HumanGunBlast", -1, 0);

        if (isFacingLeft)
        {
            gunMoveAnim.Play("GunKickingLeft", -1, 0);
        }
        else
        {
            gunMoveAnim.Play("GunKicking", -1, 0);
        }
    }

    public void FaceLeft()
    {
        if (weaponData != null && spriteRenderer.sprite != null)
        {
            Quaternion q = Quaternion.Euler(new Vector3(0, 0, 270));
            transform.localRotation = q;
            spriteRenderer.sprite = weaponData.leftSprite;
            isFacingLeft = true;
            gunMoveAnim.SetBool("isFacingLeft", true);
        }
    }

    public void FaceRight()
    {
        if (weaponData != null && spriteRenderer.sprite != null)
        {
            Quaternion q = Quaternion.Euler(new Vector3(0, 0, 90));
            transform.localRotation = q;
            spriteRenderer.sprite = weaponData.rightSprite;
            isFacingLeft = false;
            gunMoveAnim.SetBool("isFacingLeft", false);
        }
    }
}
