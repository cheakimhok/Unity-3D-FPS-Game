using UnityEngine;
using System.Collections;

public class MouseLookScript : MonoBehaviour
{
  public Transform myCamera;

  //Hiding the cursor
  void Awake()
  {
    Cursor.lockState = CursorLockMode.Locked;
    myCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
  }

  void Update()
  {
    MouseInputMovement();
    if (GetComponent<PlayerMovementScript>().currentSpeed > 1)
    HeadMovement();
  }

  public float timer;
  public int int_timer;
  public float zRotation;
  public float wantedZ;
  public float timeSpeed = 2;
  public float timerToRotateZ;

  //Switching Z rotation and applying to camera in camera Rotation().
  void HeadMovement()
  {
    timer += timeSpeed * Time.deltaTime;
    int_timer = Mathf.RoundToInt(timer);
    if (int_timer % 2 == 0)
    {
      wantedZ = -1;
    }
    else
    {
      wantedZ = 1;
    }

    zRotation = Mathf.Lerp(zRotation, wantedZ, Time.deltaTime * timerToRotateZ);
  }

  public float mouseSensitvity = 0;
  public float mouseSensitvity_notAiming = 300;
  public float mouseSensitvity_aiming = 50;

  //Reduxing mouse sensitvity if we are aiming.
  void FixedUpdate()
  {
    if (Input.GetAxis("Fire2") != 0)
    {
      mouseSensitvity = mouseSensitvity_aiming;
    }
    else if (GetComponent<PlayerMovementScript>().maxSpeed > 5)
    {
      mouseSensitvity = mouseSensitvity_notAiming;
    }
    else
    {
      mouseSensitvity = mouseSensitvity_notAiming;
    }

    ApplyingStuff();
  }

  private float rotationYVelocity;
  private float cameraXVelocity;
  public float yRotationSpeed;
  public float xCameraSpeed;
  public float wantedYRotation;
  public float currentYRotation;
  public float wantedCameraXRotation;
  public float currentCameraXRotation;
  public float maxAngleView = 60;
  public float minAngleView = -45;

  //Clamping the camera rotation X to maximum and minimum angles.
  void MouseInputMovement()
  {
    wantedYRotation += Input.GetAxis("Mouse X") * mouseSensitvity;
    wantedCameraXRotation -= Input.GetAxis("Mouse Y") * mouseSensitvity;
    wantedCameraXRotation = Mathf.Clamp(wantedCameraXRotation, minAngleView, maxAngleView);
  }

  //Applying Smooth movement.
  //Applying the rotation of camera
  void ApplyingStuff()
  {

    currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, yRotationSpeed);
    currentCameraXRotation = Mathf.SmoothDamp(currentCameraXRotation, wantedCameraXRotation, ref cameraXVelocity, xCameraSpeed);

    WeaponRotation();

    transform.rotation = Quaternion.Euler(0, currentYRotation, 0);
    myCamera.localRotation = Quaternion.Euler(currentCameraXRotation, 0, zRotation);

  }

  private Vector2 velocityGunFollow;
  private float gunWeightX;
  private float gunWeightY;
  public GameObject weapon;
  private GunScript gun;

  //Rotating current weapon from here
  //Checking if player has a weapon or not
  void WeaponRotation()
  {
    if (!weapon)
    {
      if (weapon)
      {
        if (weapon.GetComponent<GunScript>())
        {
          try
          {
            gun = GameObject.FindGameObjectWithTag("Weapon").GetComponent<GunScript>();
          }
          catch (System.Exception ex)
          {
            print("The gun is not found->" + ex.StackTrace.ToString());
          }
        }
      }
    }
  }

}
