using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour
{

  public float maxDistance = 1000000;
  RaycastHit hit;
  public GameObject decalHitWall;
  public float floatInfrontOfWall;
  public GameObject bloodEffect;
  public LayerMask ignoreLayer;

  //Uppon bullet creation with this script attatched
  void Update()
  {
    if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, ~ignoreLayer))
    {
      if (decalHitWall)
      {
        if (hit.transform.tag == "LevelPart")
        {
          Instantiate(decalHitWall, hit.point + hit.normal * floatInfrontOfWall, Quaternion.LookRotation(hit.normal));
          Destroy(gameObject);
        }
        if (hit.transform.tag == "Dummie")
        {
          Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
          Destroy(gameObject);
        }
      }
      Destroy(gameObject);
    }
    Destroy(gameObject, 0.1f);
  }
}
