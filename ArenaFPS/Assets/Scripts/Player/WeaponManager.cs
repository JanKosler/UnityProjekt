using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private int selectedWeapon = 0;
    [SerializeField] private Camera playerCam;
    [SerializeField] private float grabRange;
    [SerializeField] private GameObject weaponHolder;

    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        WeaponSwitch();
        WeaponGrab();
    }


    private void WeaponGrab()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, grabRange))
        {
            if (hit.transform.CompareTag("Weapon"))
            {
                Debug.Log("We hit a weapon");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Grabbing weapon");
                    GameObject weapon = hit.transform.gameObject;
                    weapon.gameObject.transform.parent = weaponHolder.transform;
                    weapon.gameObject.transform.localPosition = new Vector3(0, 0, 0);
                    weapon.gameObject.transform.localRotation = Quaternion.identity;
                }
            }
        }
    }
    private void WeaponSwitch()
    {
        int previousSelectedWeapon = selectedWeapon;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
                selectedWeapon = transform.childCount - 1;
            else
                selectedWeapon--;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
        {
            selectedWeapon = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
        {
            selectedWeapon = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && transform.childCount >= 4)
        {
            selectedWeapon = 3;
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }
    private void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(playerCam.transform.position, grabRange);
    }
}
