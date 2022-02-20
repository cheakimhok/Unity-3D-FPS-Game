using UnityEngine;
using System.Collections;

public class MouseLookScript : MonoBehaviour
{
  public Transform myCamera;

  //Hiding the cursor.
  void Awake()
  {
    myCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
  }

  //Locking the mouse if pressing L.
  void Update()
  {
    MouseInputMovement();

    // if (Input.GetKeyDown(KeyCode.L))
    // {
    //   Cursor.lockState = CursorLockMode.Locked;
    // }
    deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

    if (GetComponent<PlayerMovementScript>().currentSpeed > 1)
      HeadMovement();
  }

  private float timer;
  private int int_timer;
  private float zRotation;
  private float wantedZ;
  private float timeSpeed = 2;
  private float timerToRotateZ;

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

  private float mouseSensitvity = 0;
  public float mouseSensitvity_notAiming = 300;
  public float mouseSensitvity_aiming = 50;

  //If aiming set the mouse sensitvity from our variables and vice versa
  void FixedUpdate()
  {
    //Reduxing mouse sensitvity if we are aiming.
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

  private float rotationYVelocity, cameraXVelocity;
  private float yRotationSpeed, xCameraSpeed;
  public float wantedYRotation;
  public float currentYRotation;
  public float wantedCameraXRotation;
  public float currentCameraXRotation;
  private float topAngleView = 60;
  private float bottomAngleView = -45;

  //Upon mouse movenet it increases/decreased wanted value
  //Clamping the camera rotation X to top and bottom angles
  void MouseInputMovement()
  {
    wantedYRotation += Input.GetAxis("Mouse X") * mouseSensitvity;
    wantedCameraXRotation -= Input.GetAxis("Mouse Y") * mouseSensitvity;
    wantedCameraXRotation = Mathf.Clamp(wantedCameraXRotation, bottomAngleView, topAngleView);
  }

  //Smooothing the movement
  //Applying the camera rotation
  void ApplyingStuff()
  {
    currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, yRotationSpeed);
    currentCameraXRotation = Mathf.SmoothDamp(currentCameraXRotation, wantedCameraXRotation, ref cameraXVelocity, xCameraSpeed);

    WeaponRotation();

    transform.rotation = Quaternion.Euler(0, currentYRotation, 0);
    myCamera.localRotation = Quaternion.Euler(currentCameraXRotation, 0, zRotation);
  }

  float deltaTime = 0.0f;
  private GameObject weapon;
  private GunScript gun;

  //Rotating the current weapon
  //Checking if the player has a weapon or not 
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
            print("gun not found->" + ex.StackTrace.ToString());
          }
        }
      }
    }
  }
}
