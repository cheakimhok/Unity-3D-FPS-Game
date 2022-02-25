using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementScript : MonoBehaviour
{
  Rigidbody rb;
  public float currentSpeed = 100.0f;
  private Transform cameraMain;
  private float jumpForce = 20000;
  public Vector3 cameraPosition;

  //Getting the Players rigidbody component
  //And grabbing the mainCamera from Players child transform
  void Awake()
  {
    rb = GetComponent<Rigidbody>();
    cameraMain = transform.Find("Main Camera").transform;
    bulletSpawn = cameraMain.Find("BulletSpawn").transform;
    ignoreLayer = 1 << LayerMask.NameToLayer("Player");

  }
  private Vector3 slowdownV;
  private Vector2 horizontalMovement;

  //Raycasting for meele attacks and input movement handling here.
  void FixedUpdate()
  {
    PlayerMovementLogic();
  }

  //Accordingly to input adds force and if magnitude is bigger it will clamp it
  //If player leaves keys it will deaccelerate
  void PlayerMovementLogic()
  {
    currentSpeed = rb.velocity.magnitude;
    horizontalMovement = new Vector2(rb.velocity.x, rb.velocity.z);
    if (horizontalMovement.magnitude > maxSpeed)
    {
      horizontalMovement = horizontalMovement.normalized;
      horizontalMovement *= maxSpeed;
    }
    rb.velocity = new Vector3(
      horizontalMovement.x,
      rb.velocity.y,
      horizontalMovement.y
    );
    if (grounded)
    {
      rb.velocity = Vector3.SmoothDamp(rb.velocity,
        new Vector3(0, rb.velocity.y, 0),
        ref slowdownV,
        deaccelerationSpeed);
    }

    if (grounded)
    {
      rb.AddRelativeForce(Input.GetAxis("Horizontal") * accelerationSpeed * Time.deltaTime, 0, Input.GetAxis("Vertical") * accelerationSpeed * Time.deltaTime);
    }
    else
    {
      rb.AddRelativeForce(Input.GetAxis("Horizontal") * accelerationSpeed / 2 * Time.deltaTime, 0, Input.GetAxis("Vertical") * accelerationSpeed / 2 * Time.deltaTime);

    }

    //Slippery issues fixed here
    if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
    {
      deaccelerationSpeed = 0.5f;
    }
    else
    {
      deaccelerationSpeed = 0.1f;
    }
  }

  //Handles jumping and ads the force and sounds
  void Jumping()
  {
    if (Input.GetKeyDown(KeyCode.Space) && grounded)
    {
      rb.AddRelativeForce(Vector3.up * jumpForce);
      if (_jumpSound)
        _jumpSound.Play();
      else
        print("Missig jump sound.");
      _walkSound.Stop();
      _runSound.Stop();
    }
  }

  //Update loop calling other stuff
  void Update()
  {
    Jumping();
    Crouching();
    WalkingSound();
  }

  //Checks if player is grounded and plays the sound accorindlgy to his speed
  void WalkingSound()
  {
    if (_walkSound && _runSound)
    {
      if (RayCastGrounded())
      {
        if (currentSpeed > 1)
        {
          if (maxSpeed == 3)
          {
            if (!_walkSound.isPlaying)
            {
              _walkSound.Play();
              _runSound.Stop();
            }
          }
          else if (maxSpeed == 5)
          {
            if (!_runSound.isPlaying)
            {
              _walkSound.Stop();
              _runSound.Play();
            }
          }
        }
        else
        {
          _walkSound.Stop();
          _runSound.Stop();
        }
      }
      else
      {
        _walkSound.Stop();
        _runSound.Stop();
      }
    }
    else
    {
      print("Missing walk and running sounds.");
    }
  }

  //Raycasts down to check if we are grounded along the gorunded method()
  private bool RayCastGrounded()
  {
    RaycastHit groundedInfo;
    if (Physics.Raycast(transform.position, transform.up * -1f, out groundedInfo, 1, ~ignoreLayer))
    {
      Debug.DrawRay(transform.position, transform.up * -1f, Color.red, 0.0f);
      if (groundedInfo.transform != null)
      {
        return true;
      }
      else
      {
        return false;
      }
    }
    return false;
  }

  //Crouching method
  void Crouching()
  {
    if (Input.GetKey(KeyCode.C))
    {
      transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 0.6f, 1), Time.deltaTime * 15);
    }
    else
    {
      transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * 15);

    }
  }

  public int maxSpeed = 100;
  private float deaccelerationSpeed = 15.0f;
  private float accelerationSpeed = 100000.0f;
  private bool grounded;

  //Checking if the player is contacting the ground
  void OnCollisionStay(Collision other)
  {
    foreach (ContactPoint contact in other.contacts)
    {
      if (Vector2.Angle(contact.normal, Vector3.up) < 60)
      {
        grounded = true;
      }
    }
  }

  //On collision exit set grounded to false
  void OnCollisionExit()
  {
    grounded = false;
  }

  //to ignore player layer
  private LayerMask ignoreLayer;
  private Transform bulletSpawn;
  public AudioSource _jumpSound;
  public AudioSource _freakingZombiesSound;
  public AudioSource _walkSound;
  public AudioSource _runSound;
}