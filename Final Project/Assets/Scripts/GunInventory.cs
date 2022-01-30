using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MenuStyle
{
  horizontal, vertical
}

public class GunInventory : MonoBehaviour
{
  private GameObject currentGun;
  private Animator currentHAndsAnimator;
  private int currentGunCounter = 0;
  public List<string> gunsIHave = new List<string>();
  private float switchWeaponCooldown;

	//Calling the method that will update the icons of our guns if we carry any upon start
  void Awake()
  {
		//Starting with a weapon
    StartCoroutine("SpawnWeaponUponStart");

    if (gunsIHave.Count == 0)
      print("No guns in the inventory");
  }

	//Waiting then calls for a waepon spawn
  IEnumerator SpawnWeaponUponStart()
  {
    yield return new WaitForSeconds(0.5f);
    StartCoroutine("Spawn", 0);
  }

	//Calculation switchWeaponCoolDown so it does not allow us to change weapons millions of times per second
  void Update()
  {
    switchWeaponCooldown += 1 * Time.deltaTime;
    if (switchWeaponCooldown > 1.2f && Input.GetKey(KeyCode.LeftShift) == false)
    {
      Create_Weapon();
    }
  }

	//If used scroll mousewheel or arrows up and down the player will change weapon
	//GunPlaceSpawner is child of Player gameObject, where the gun is going to spawn and transition
  void Create_Weapon()
  {
		//Scrolling wheel waepons changing
    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Mouse ScrollWheel") > 0)
    {
      switchWeaponCooldown = 0;

      currentGunCounter++;
      if (currentGunCounter > gunsIHave.Count - 1)
      {
        currentGunCounter = 0;
      }
      StartCoroutine("Spawn", currentGunCounter);
    }
    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("Mouse ScrollWheel") < 0)
    {
      switchWeaponCooldown = 0;

      currentGunCounter--;
      if (currentGunCounter < 0)
      {
        currentGunCounter = gunsIHave.Count - 1;
      }
      StartCoroutine("Spawn", currentGunCounter);
    }
  }

	//Called from Create_Weapon() upon pressing arrow up/down or scrolling the mouse wheel
  IEnumerator Spawn(int _redniBroj)
  {
    if (currentGun)
    {
      if (currentGun.name.Contains("Gun"))
      {
        yield return new WaitForSeconds(0.8f);
        Destroy(currentGun);

        GameObject resource = (GameObject)Resources.Load(gunsIHave[_redniBroj].ToString());
        currentGun = (GameObject)Instantiate(resource, transform.position, /*gameObject.transform.rotation*/Quaternion.identity);
        AssignHandsAnimator(currentGun);
      }
    }
    else
    {
      GameObject resource = (GameObject)Resources.Load(gunsIHave[_redniBroj].ToString());
      currentGun = (GameObject)Instantiate(resource, transform.position, /*gameObject.transform.rotation*/Quaternion.identity);

      AssignHandsAnimator(currentGun);
    }
  }

  //Assigns Animator to the script so we can use it in other scripts of a current gun
  void AssignHandsAnimator(GameObject _currentGun)
  {
    if (_currentGun.name.Contains("Gun"))
    {
      currentHAndsAnimator = currentGun.GetComponent<GunScript>().handsAnimator;
    }
  }

	//Call this method when player dies
  public void DeadMethod()
  {
    Destroy(currentGun);
    Destroy(this);
  }
}
