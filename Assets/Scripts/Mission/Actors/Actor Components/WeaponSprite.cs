using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSprite : MonoBehaviour
{
    private WeaponData weaponData;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetData(WeaponData newWeaponData)
    {
        weaponData = newWeaponData;
    }

    public void FaceFront()
    {
        if (weaponData != null)
        {
            Quaternion q = Quaternion.Euler(new Vector3(0, 0, 180));
            transform.localRotation = q;
            spriteRenderer.sprite = weaponData.downSprite;
        }
    }

    public void FaceLeft()
    {
        if (weaponData != null)
        {
            Quaternion q = Quaternion.Euler(new Vector3(0, 0, 270));
            transform.localRotation = q;
            spriteRenderer.sprite = weaponData.leftSprite;
        }
    }

    public void FaceBack()
    {
        if (weaponData != null)
        {
            Quaternion q = Quaternion.Euler(new Vector3(0, 0, 0));
            transform.localRotation = q;
            spriteRenderer.sprite = weaponData.upSprite;
        }
    }

    public void FaceRight()
    {
        if (weaponData != null)
        {
            Quaternion q = Quaternion.Euler(new Vector3(0, 0, 90));
            transform.localRotation = q;
            spriteRenderer.sprite = weaponData.rightSprite;
        }
    }
}
