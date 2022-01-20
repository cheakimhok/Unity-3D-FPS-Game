using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunInventory : MonoBehaviour
{
  private GameObject currentGun;
  private Animator currentHAndsAnimator;
  private int currentGunCounter = 0;
  public List<string> gunsIHave = new List<string>();

  //Carry a weapon from the start
  void Awake()
  {
    StartCoroutine("SpawnWeaponUponStart");

    if (gunsIHave.Count == 0)
      print("No guns in the inventory");
  }

  //Waiting a bit of time to spawn a weapon
  IEnumerator SpawnWeaponUponStart()
  {
    yield return new WaitForSeconds(0.5f);
    StartCoroutine("Spawn", 0);
  }

  void Update()
  {

  }

  //Scrolling wheel weapons changing
  void Create_Weapon()
  {

    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Mouse ScrollWheel") > 0)
    {
      currentGunCounter++;
      if (currentGunCounter > gunsIHave.Count - 1)
      {
        currentGunCounter = 0;
      }
      StartCoroutine("Spawn", currentGunCounter);
    }
    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("Mouse ScrollWheel") < 0)
    {
      currentGunCounter--;
      if (currentGunCounter < 0)
      {
        currentGunCounter = gunsIHave.Count - 1;
      }
      StartCoroutine("Spawn", currentGunCounter);
    }

  }

  //This method is called from Create_Weapon() upon pressing arrow up/arrow down or scrolling the mouse wheel
  //Checks if we carry a gun and destroy it, and its then going to load a gun prefab from our Resources Folder
  IEnumerator Spawn(int _redniBroj)
  {
    if (currentGun)
    {
      if (currentGun.name.Contains("Gun"))
      {
        currentHAndsAnimator.SetBool("changingWeapon", true);
        
        //0.8 time to change waepon, but since there is no change weapon animation there is no need to wait fo weapon taken down
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

  //Assigns animator to the script
  void AssignHandsAnimator(GameObject _currentGun)
  {
    if (_currentGun.name.Contains("Gun"))
    {
      currentHAndsAnimator = currentGun.GetComponent<GunScript>().handsAnimator;
    }
  }
}
