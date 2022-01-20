using UnityEngine;
using System.Collections;

public enum GunStyles
{
  nonautomatic, automatic
}
public class GunScript : MonoBehaviour
{
  public GunStyles currentStyle;
  public MouseLookScript mls;
  public int walkingSpeed = 3;
  public int runningSpeed = 5;
  public float bulletsIHave = 1000;
  public float bulletsInTheGun = 5;
  public float amountOfBulletsPerLoad = 5;
  private Transform player;
  private Camera cameraComponent;

  private PlayerMovementScript pmS;

  //Collection the variables upon awake that we need.
  void Awake()
  {
    mls = GameObject.FindGameObjectWithTag("Player").GetComponent<MouseLookScript>();
    player = mls.transform;
    mainCamera = mls.myCamera;
    secondCamera = GameObject.FindGameObjectWithTag("SecondCamera").GetComponent<Camera>();
    cameraComponent = mainCamera.GetComponent<Camera>();
    pmS = player.GetComponent<PlayerMovementScript>();

    bulletSpawnPlace = GameObject.FindGameObjectWithTag("BulletSpawn");
    startLook = mouseSensitvity_notAiming;
    startAim = mouseSensitvity_aiming;
    startRun = mouseSensitvity_running;

    rotationLastY = mls.currentYRotation;
    rotationLastX = mls.currentCameraXRotation;
  }

  public Vector3 currentGunPosition;
  public Vector3 restPlacePosition;
  public Vector3 aimPlacePosition;
  public float gunAimTime = 0.1f;
  private Vector3 gunPosVelocity;
  private float cameraZoomVelocity;
  private float secondCameraZoomVelocity;

  //Update loop calling for methods
  void Update()
  {
    Animations();
    GiveCameraScriptMySensitvity();
    PositionGun();
    Shooting();
    Sprint();
    CrossHairExpansionWhenWalking();
  }

  //Calculation of weapon position when aiming or not aiming.
  void FixedUpdate()
  {
    RotationGun();

    //if we are aiming, the sensitivity, zoom, position of a weapon will change
    if (Input.GetAxis("Fire2") != 0)
    {
      gunPrecision = gunPrecision_aiming;
      recoilAmount_x = recoilAmount_x_;
      recoilAmount_y = recoilAmount_y_;
      recoilAmount_z = recoilAmount_z_;
      currentGunPosition = Vector3.SmoothDamp(currentGunPosition, aimPlacePosition, ref gunPosVelocity, gunAimTime);
      cameraComponent.fieldOfView = Mathf.SmoothDamp(cameraComponent.fieldOfView, cameraZoomRatio_aiming, ref cameraZoomVelocity, gunAimTime);
      secondCamera.fieldOfView = Mathf.SmoothDamp(secondCamera.fieldOfView, secondCameraZoomRatio_aiming, ref secondCameraZoomVelocity, gunAimTime);
    }
    //if not aiming
    else
    {
      gunPrecision = gunPrecision_notAiming;
      recoilAmount_x = recoilAmount_x_non;
      recoilAmount_y = recoilAmount_y_non;
      recoilAmount_z = recoilAmount_z_non;
      currentGunPosition = Vector3.SmoothDamp(currentGunPosition, restPlacePosition, ref gunPosVelocity, gunAimTime);
      cameraComponent.fieldOfView = Mathf.SmoothDamp(cameraComponent.fieldOfView, cameraZoomRatio_notAiming, ref cameraZoomVelocity, gunAimTime);
      secondCamera.fieldOfView = Mathf.SmoothDamp(secondCamera.fieldOfView, secondCameraZoomRatio_notAiming, ref secondCameraZoomVelocity, gunAimTime);
    }

  }

  //Sensitvity of the gun
  public float mouseSensitvity_notAiming = 10;
  public float mouseSensitvity_aiming = 5;
  public float mouseSensitvity_running = 4;

  //Uses for giving our main camera different sensivity options for each gun
  void GiveCameraScriptMySensitvity()
  {
    mls.mouseSensitvity_notAiming = mouseSensitvity_notAiming;
    mls.mouseSensitvity_aiming = mouseSensitvity_aiming;
  }

  //Uses for expanding position of the crosshair or make it dissapear when running
  void CrossHairExpansionWhenWalking()
  {

    if (player.GetComponent<Rigidbody>().velocity.magnitude > 1 && Input.GetAxis("Fire1") == 0)
    {//ifnot shooting

      expandValues_crosshair += new Vector2(20, 40) * Time.deltaTime;
      if (player.GetComponent<PlayerMovementScript>().maxSpeed < runningSpeed)
      { //not running
        expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 10), Mathf.Clamp(expandValues_crosshair.y, 0, 20));
        fadeout_value = Mathf.Lerp(fadeout_value, 1, Time.deltaTime * 2);
      }
      else
      {//running
        fadeout_value = Mathf.Lerp(fadeout_value, 0, Time.deltaTime * 10);
        expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 20), Mathf.Clamp(expandValues_crosshair.y, 0, 40));
      }
    }
    else
    {//if shooting
      expandValues_crosshair = Vector2.Lerp(expandValues_crosshair, Vector2.zero, Time.deltaTime * 5);
      expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 10), Mathf.Clamp(expandValues_crosshair.y, 0, 20));
      fadeout_value = Mathf.Lerp(fadeout_value, 1, Time.deltaTime * 2);

    }

  }

  //Changes the speed of the player and animation
  void Sprint()
  {
    if (Input.GetAxis("Vertical") > 0 && Input.GetAxisRaw("Fire2") == 0 && Input.GetAxisRaw("Fire1") == 0)
    {
      if (Input.GetKeyDown(KeyCode.LeftShift))
      {
        if (pmS.maxSpeed == walkingSpeed)
        {
          //Changes player movement's speed to max
          pmS.maxSpeed = runningSpeed;
        }
        else
        {
          pmS.maxSpeed = walkingSpeed;
        }
      }
    }
    else
    {
      pmS.maxSpeed = walkingSpeed;
    }

  }

  private float startLook, startAim, startRun;
  private Vector3 velV;
  public Transform mainCamera;
  private Camera secondCamera;
 
  //Calculates the weapon position accordingly to the player position and rotation
  //After calculation the recoil amount will be decreased
  void PositionGun()
  {
    transform.position = Vector3.SmoothDamp(
      transform.position,
      mainCamera.transform.position -
      (mainCamera.transform.right * (currentGunPosition.x + currentRecoilXPos)) +
      (mainCamera.transform.up * (currentGunPosition.y + currentRecoilYPos)) +
      (mainCamera.transform.forward * (currentGunPosition.z + currentRecoilZPos)), ref velV, 0
    );

    pmS.cameraPosition = new Vector3(currentRecoilXPos, currentRecoilYPos, 0);

    currentRecoilZPos = Mathf.SmoothDamp(currentRecoilZPos, 0, ref velocity_z_recoil, recoilOverTime_z);
    currentRecoilXPos = Mathf.SmoothDamp(currentRecoilXPos, 0, ref velocity_x_recoil, recoilOverTime_x);
    currentRecoilYPos = Mathf.SmoothDamp(currentRecoilYPos, 0, ref velocity_y_recoil, recoilOverTime_y);

  }

  private Vector2 velocityGunRotate;
  private float gunWeightX, gunWeightY;
  public float rotationLagTime = 0f;
  private float rotationLastY;
  private float rotationDeltaY;
  private float angularVelocityY;
  private float rotationLastX;
  private float rotationDeltaX;
  private float angularVelocityX;
  public Vector2 forwardRotationAmount = Vector2.one;
  
  //Rotates the weapon depending the rotation of mouse look
  //Calculates the forward rotation
  void RotationGun()
  {
    rotationDeltaY = mls.currentYRotation - rotationLastY;
    rotationDeltaX = mls.currentCameraXRotation - rotationLastX;

    rotationLastY = mls.currentYRotation;
    rotationLastX = mls.currentCameraXRotation;

    angularVelocityY = Mathf.Lerp(angularVelocityY, rotationDeltaY, Time.deltaTime * 5);
    angularVelocityX = Mathf.Lerp(angularVelocityX, rotationDeltaX, Time.deltaTime * 5);

    gunWeightX = Mathf.SmoothDamp(gunWeightX, mls.currentCameraXRotation, ref velocityGunRotate.x, rotationLagTime);
    gunWeightY = Mathf.SmoothDamp(gunWeightY, mls.currentYRotation, ref velocityGunRotate.y, rotationLagTime);

    transform.rotation = Quaternion.Euler(gunWeightX + (angularVelocityX * forwardRotationAmount.x), gunWeightY + (angularVelocityY * forwardRotationAmount.y), 0);
  }

  private float currentRecoilZPos;
  private float currentRecoilXPos;
  private float currentRecoilYPos;
  
  //Calls from ShootMethod();, upon shooting the recoil amount will increase.
  public void RecoilMath()
  {
    currentRecoilZPos -= recoilAmount_z;
    currentRecoilXPos -= (Random.value - 0.5f) * recoilAmount_x;
    currentRecoilYPos -= (Random.value - 0.5f) * recoilAmount_y;
    mls.wantedCameraXRotation -= Mathf.Abs(currentRecoilYPos * gunPrecision);
    mls.wantedYRotation -= (currentRecoilXPos * gunPrecision);

    expandValues_crosshair += new Vector2(6, 12);

  }
  public GameObject bulletSpawnPlace;
  public GameObject bullet;
  public float roundsPerSecond;
  private float waitTillNextFire;
  
  //Checking if the gun is automatic or nonautomatic and accordingly runs the ShootMethod();
  void Shooting()
  {
    if (currentStyle == GunStyles.nonautomatic)
    {
      if (Input.GetButtonDown("Fire1"))
      {
        ShootMethod();
      }
    }
    if (currentStyle == GunStyles.automatic)
    {
      if (Input.GetButton("Fire1"))
      {
        ShootMethod();
      }

    }
    waitTillNextFire -= roundsPerSecond * Time.deltaTime;
  }

  public float recoilAmount_z = 0.5f;
  public float recoilAmount_x = 0.5f;
  public float recoilAmount_y = 0.5f;
  public float recoilAmount_z_non = 0.5f;
  public float recoilAmount_x_non = 0.5f;
  public float recoilAmount_y_non = 0.5f;
  public float recoilAmount_z_ = 0.5f;
  public float recoilAmount_x_ = 0.5f;
  public float recoilAmount_y_ = 0.5f;
  public float velocity_z_recoil, velocity_x_recoil, velocity_y_recoil;
  public float recoilOverTime_z = 0.5f;
  public float recoilOverTime_x = 0.5f;
  public float recoilOverTime_y = 0.5f;
  public float gunPrecision_notAiming = 200.0f;
  public float gunPrecision_aiming = 100.0f;
  public float cameraZoomRatio_notAiming = 60;
  public float cameraZoomRatio_aiming = 40;
  public float secondCameraZoomRatio_notAiming = 60;
  public float secondCameraZoomRatio_aiming = 40;
  public float gunPrecision;
	
  //Calls Shooting and Recoil function
  //Creates bullets of the weapon
  private void ShootMethod()
  {
    if (waitTillNextFire <= 0 && pmS.maxSpeed < 5)
    {
      if (bulletsInTheGun > 0)
      {
        if (bullet)
          Instantiate(bullet, bulletSpawnPlace.transform.position, bulletSpawnPlace.transform.rotation);

        RecoilMath();

        waitTillNextFire = 1;
        bulletsInTheGun -= 1;
      }
    }
  }

  public Vector2 expandValues_crosshair;
  private float fadeout_value = 1;
  public Animator handsAnimator;
	void Animations()
  {
    if (handsAnimator)
    {
      handsAnimator.SetFloat("walkSpeed", pmS.currentSpeed);
      handsAnimator.SetBool("aiming", Input.GetButton("Fire2"));
      handsAnimator.SetInteger("maxSpeed", pmS.maxSpeed);
    }
  }
}
