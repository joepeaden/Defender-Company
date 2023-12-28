using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSprite : MonoBehaviour
{
    public WeaponData weaponData;

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
        Quaternion q = Quaternion.Euler(new Vector3(90, 180, 0));
        transform.localRotation = q;
        spriteRenderer.sprite = weaponData.downSprite;
    }

    public void FaceLeft()
    {
        Quaternion q = Quaternion.Euler(new Vector3(90, 90, 0));
        transform.localRotation = q;
        spriteRenderer.sprite = weaponData.leftSprite;
    }

    public void FaceBack()
    {
        Quaternion q = Quaternion.Euler(new Vector3(90, 0, 0));
        transform.localRotation = q;
        spriteRenderer.sprite = weaponData.upSprite;
    }

    public void FaceRight()
    {
        Quaternion q = Quaternion.Euler(new Vector3(90, 270, 0));
        transform.localRotation = q;
        spriteRenderer.sprite = weaponData.rightSprite;
    }
}
