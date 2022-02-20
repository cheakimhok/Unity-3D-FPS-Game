using UnityEngine;
using System.Collections;
//using UnityStandardAssets.ImageEffects;

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


  public float bulletsIHave = 50;
  public float bulletsInTheGun = 25;
  public float amountOfBulletsPerLoad = 25;

  private Transform player;
  private Camera cameraComponent;
  private Transform gunPlaceHolder;

  private PlayerMovementScript pmS;

  /*
	 * Collection the variables upon awake that we need.
	 */
  void Awake()
  {
    mls = GameObject.FindGameObjectWithTag("Player").GetComponent<MouseLookScript>();
    player = mls.transform;
    mainCamera = mls.myCamera;
    secondCamera = GameObject.FindGameObjectWithTag("SecondCamera").GetComponent<Camera>();
    cameraComponent = mainCamera.GetComponent<Camera>();
    pmS = player.GetComponent<PlayerMovementScript>();
    bulletSpawnPlace = GameObject.FindGameObjectWithTag("BulletSpawn");
    hitMarker = transform.Find("hitMarkerSound").GetComponent<AudioSource>();
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
  public bool reloading;
  private Vector3 gunPosVelocity;
  private float cameraZoomVelocity;
  private float secondCameraZoomVelocity;

  //Updating the functions
  void Update()
  {
    Animations();
    GiveCameraScriptMySensitvity();
    PositionGun();
    Shooting();
    Sprint();
    CrossHairExpansionWhenWalking();
  }

  //Calculating the position of the weapon when the player aims and not aims
  void FixedUpdate()
  {
    RotationGun();
    //Changing the value if the player is aiming. (sensitivity, zoom, position)
    //If the player is aiming
    if (Input.GetAxis("Fire2") != 0 && !reloading)
    {
      gunPrecision = gunPrecision_aiming;
      recoilAmount_x = recoilAmount_x_;
      recoilAmount_y = recoilAmount_y_;
      recoilAmount_z = recoilAmount_z_;
      currentGunPosition = Vector3.SmoothDamp(currentGunPosition, aimPlacePosition, ref gunPosVelocity, gunAimTime);
      cameraComponent.fieldOfView = Mathf.SmoothDamp(cameraComponent.fieldOfView, cameraZoomRatio_aiming, ref cameraZoomVelocity, gunAimTime);
      secondCamera.fieldOfView = Mathf.SmoothDamp(secondCamera.fieldOfView, secondCameraZoomRatio_aiming, ref secondCameraZoomVelocity, gunAimTime);
    }
    //If the player is not aiming
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
  public float mouseSensitvity_notAiming = 10;
  public float mouseSensitvity_aiming = 5;
  public float mouseSensitvity_running = 4;

  //Set the main camera into different sensitivity 
  void GiveCameraScriptMySensitvity()
  {
    mls.mouseSensitvity_notAiming = mouseSensitvity_notAiming;
    mls.mouseSensitvity_aiming = mouseSensitvity_aiming;
  }

  //Expands the position of the crosshair, and make it disappear when the player is running
  void CrossHairExpansionWhenWalking()
  {
    if (player.GetComponent<Rigidbody>().velocity.magnitude > 1 && Input.GetAxis("Fire1") == 0)
    {
      //If not shooting
      expandValues_crosshair += new Vector2(20, 40) * Time.deltaTime;
      if (player.GetComponent<PlayerMovementScript>().maxSpeed < runningSpeed)
      {
        //If not running
        expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 10), Mathf.Clamp(expandValues_crosshair.y, 0, 20));
        fadeout_value = Mathf.Lerp(fadeout_value, 1, Time.deltaTime * 2);
      }
      else
      {
        //If running
        fadeout_value = Mathf.Lerp(fadeout_value, 0, Time.deltaTime * 10);
        expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 20), Mathf.Clamp(expandValues_crosshair.y, 0, 40));
      }
    }
    else
    {
      //If shooting
      expandValues_crosshair = Vector2.Lerp(expandValues_crosshair, Vector2.zero, Time.deltaTime * 5);
      expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 10), Mathf.Clamp(expandValues_crosshair.y, 0, 20));
      fadeout_value = Mathf.Lerp(fadeout_value, 1, Time.deltaTime * 2);
    }
  }

  //Changes player's speed and animation
  void Sprint()
  {
    if (Input.GetAxis("Vertical") > 0 && Input.GetAxisRaw("Fire2") == 0 && Input.GetAxisRaw("Fire1") == 0)
    {
      if (Input.GetKeyDown(KeyCode.LeftShift))
      {
        if (pmS.maxSpeed == walkingSpeed)
        {
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

  public bool aiming;
  private float startLook, startAim, startRun;
  private Vector3 velV;
  public Transform mainCamera;
  private Camera secondCamera;

  //Calculates the position of the weapon depending on player's position and rotation
  void PositionGun()
  {
    transform.position = Vector3.SmoothDamp(transform.position,
      mainCamera.transform.position -
      (mainCamera.transform.right * (currentGunPosition.x + currentRecoilXPos)) +
      (mainCamera.transform.up * (currentGunPosition.y + currentRecoilYPos)) +
      (mainCamera.transform.forward * (currentGunPosition.z + currentRecoilZPos)), ref velV, 0);
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

  //Rotates the weapon depending on mouse's rotation
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

  //Calculates the recoil when the player is shooting
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
  public AudioSource shoot_sound_source, reloadSound_source;
  public static AudioSource hitMarker;

  //Target hit sound
  public static void HitMarkerSound()
  {
    hitMarker.Play();
  }

  public GameObject[] muzzelFlash;
  public GameObject muzzelSpawn;
  private GameObject holdFlash;
  private GameObject holdSmoke;

  //Creates bullets of the weapon, calls for flashes and recoil
  private void ShootMethod()
  {
    RaycastHit hit;

    if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 100000))
    {
      TargetX target = hit.transform.GetComponent<TargetX>();
      if (target != null)
      {
        UnityEngine.Debug.Log(hit.transform.name);
        target.takeDamage(5);
      }

    }

    if (waitTillNextFire <= 0 && !reloading && pmS.maxSpeed < 5)
    {
      if (bulletsInTheGun > 0)
      {
        int randomNumberForMuzzelFlash = Random.Range(0, 5);
        if (bullet)
          Instantiate(bullet, bulletSpawnPlace.transform.position, bulletSpawnPlace.transform.rotation);
        else
          print("Missing the bullet prefab");
        holdFlash = Instantiate(muzzelFlash[randomNumberForMuzzelFlash], muzzelSpawn.transform.position /*- muzzelPosition*/, muzzelSpawn.transform.rotation * Quaternion.Euler(0, 0, 90)) as GameObject;
        holdFlash.transform.parent = muzzelSpawn.transform;
        if (shoot_sound_source)
          shoot_sound_source.Play();
        else
          print("Missing 'Shoot Sound Source'.");

        RecoilMath();

        waitTillNextFire = 1;
        bulletsInTheGun -= 1;
      }
      else
      {
        StartCoroutine("Reload_Animation");
      }
    }
  }

  //Reloading the weapon, animator of reloading
  public float reloadChangeBulletsTime;
  public string reloadAnimationName = "Player_Reload";
  IEnumerator Reload_Animation()
  {
    if (bulletsIHave > 0 && bulletsInTheGun < amountOfBulletsPerLoad && !reloading)
    {
      if (reloadSound_source.isPlaying == false && reloadSound_source != null)
      {
        if (reloadSound_source)
          reloadSound_source.Play();
        else
          print("'Reload Sound Source' missing.");
      }
      handsAnimator.SetBool("reloading", true);
      yield return new WaitForSeconds(0.5f);
      handsAnimator.SetBool("reloading", false);

      yield return new WaitForSeconds(reloadChangeBulletsTime - 0.5f);
      if (player.GetComponent<PlayerMovementScript>()._freakingZombiesSound)
        player.GetComponent<PlayerMovementScript>()._freakingZombiesSound.Play();
      else
        print("Missing Freaking Zombies Sound");

      if (bulletsIHave - amountOfBulletsPerLoad >= 0)
      {
        bulletsIHave -= amountOfBulletsPerLoad - bulletsInTheGun;
        bulletsInTheGun = amountOfBulletsPerLoad;
      }
      else if (bulletsIHave - amountOfBulletsPerLoad < 0)
      {
        float valueForBoth = amountOfBulletsPerLoad - bulletsInTheGun;
        if (bulletsIHave - valueForBoth < 0)
        {
          bulletsInTheGun += bulletsIHave;
          bulletsIHave = 0;
        }
        else
        {
          bulletsIHave -= valueForBoth;
          bulletsInTheGun += valueForBoth;
        }
      }
    }
  }

  //Crosshair
  public TextMesh HUD_bullets;
  void OnGUI()
  {
    if (mls && HUD_bullets)
      HUD_bullets.text = bulletsIHave.ToString() + " - " + bulletsInTheGun.ToString();

    DrawCrosshair();
  }

  public Texture horizontal_crosshair, vertical_crosshair;
  public Vector2 top_pos_crosshair, bottom_pos_crosshair, left_pos_crosshair, right_pos_crosshair;
  public Vector2 size_crosshair_vertical = new Vector2(1, 1);
  public Vector2 size_crosshair_horizontal = new Vector2(1, 1);
  public Vector2 expandValues_crosshair;
  private float fadeout_value = 1;

  //Draws the crosshair
  void DrawCrosshair()
  {
    GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, fadeout_value);
    if (Input.GetAxis("Fire2") == 0)
    {
      //Crosshair if not aiming
      GUI.DrawTexture(new Rect(vec2(left_pos_crosshair).x + position_x(-expandValues_crosshair.x) + Screen.width / 2, Screen.height / 2 + vec2(left_pos_crosshair).y, vec2(size_crosshair_horizontal).x, vec2(size_crosshair_horizontal).y), vertical_crosshair);//left
      GUI.DrawTexture(new Rect(vec2(right_pos_crosshair).x + position_x(expandValues_crosshair.x) + Screen.width / 2, Screen.height / 2 + vec2(right_pos_crosshair).y, vec2(size_crosshair_horizontal).x, vec2(size_crosshair_horizontal).y), vertical_crosshair);//right

      GUI.DrawTexture(new Rect(vec2(top_pos_crosshair).x + Screen.width / 2, Screen.height / 2 + vec2(top_pos_crosshair).y + position_y(-expandValues_crosshair.y), vec2(size_crosshair_vertical).x, vec2(size_crosshair_vertical).y), horizontal_crosshair);//top
      GUI.DrawTexture(new Rect(vec2(bottom_pos_crosshair).x + Screen.width / 2, Screen.height / 2 + vec2(bottom_pos_crosshair).y + position_y(expandValues_crosshair.y), vec2(size_crosshair_vertical).x, vec2(size_crosshair_vertical).y), horizontal_crosshair);//bottom
    }

  }

  //Return the size and position for GUI images
  private float position_x(float var)
  {
    return Screen.width * var / 100;
  }
  private float position_y(float var)
  {
    return Screen.height * var / 100;
  }
  private Vector2 vec2(Vector2 _vec2)
  {
    return new Vector2(Screen.width * _vec2.x / 100, Screen.height * _vec2.y / 100);
  }
  //#

  public Animator handsAnimator;
  /*
	* Fetching if any current animation is running.
	* Setting the reload animation upon pressing R.
	*/
  void Animations()
  {
    if (handsAnimator)
    {
      reloading = handsAnimator.GetCurrentAnimatorStateInfo(0).IsName(reloadAnimationName);
      handsAnimator.SetFloat("walkSpeed", pmS.currentSpeed);
      handsAnimator.SetBool("aiming", Input.GetButton("Fire2"));
      handsAnimator.SetInteger("maxSpeed", pmS.maxSpeed);
      if (Input.GetKeyDown(KeyCode.R) && pmS.maxSpeed < 5 && !reloading)
      {
        StartCoroutine("Reload_Animation");
      }
    }
  }
}
