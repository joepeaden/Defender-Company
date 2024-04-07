using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSprite : MonoBehaviour
{
    private WeaponData weaponData;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator gunBlastAnim;
    [SerializeField] private Animator gunMoveAnim;
    //[SerializeField] private Animator shellCasingAnim;
    //[SerializeField] private Animator shellCasingAnim2;
    //[SerializeField] private Animator shellCasingAnim3;
    public GameObject shellCasing;

    private bool isFacingLeft = false;

    public Transform weaponParent;

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

        PlayShellCasingAnim();

        if (isFacingLeft)
        {
            gunMoveAnim.Play("GunKickingLeft", -1, 0);
        }
        else
        {
            gunMoveAnim.Play("GunKicking", -1, 0);
        }
    }


    private void PlayShellCasingAnim()
    {
        GameObject casing = Instantiate(shellCasing, transform.position, transform.rotation);
        casing.GetComponent<Rigidbody2D>().AddForce(-weaponParent.up * Random.Range(200f, 500f));
        if (isFacingLeft)
        {
            casing.GetComponent<Rigidbody2D>().AddForce(-weaponParent.right * Random.Range(150f, 200f));
        }
        else
        {
            casing.GetComponent<Rigidbody2D>().AddForce(weaponParent.right * Random.Range(150f, 200f));
        }

        casing.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-500f, 500f));
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
