using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementScript : MonoBehaviour
{
  Rigidbody rb;
  public float currentSpeed;
  [HideInInspector] public Transform cameraMain;
  public float jumpForce = 750;
  [HideInInspector] public Vector3 cameraPosition;

  //Getting the Players rigidbody component.
  //And grabbing the mainCamera from Players child transform.
  void Awake()
  {
    rb = GetComponent<Rigidbody>();
    cameraMain = transform.Find("Main Camera").transform;

  }
  private Vector3 slowdownV;
  private Vector2 horizontalMovement;

  //Raycasting for meele attacks and input movement handling here.
  void FixedUpdate()
  {
    PlayerMovementLogic();
  }

  //Accordingly to input adds force and if magnitude is bigger it will clamp it.
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

    //Avoiding the slippery
    if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
    {
      deaccelerationSpeed = 0.5f;
    }
    else
    {
      deaccelerationSpeed = 0.1f;
    }
  }

  //Handles jumping and ads the force
  void Jumping()
  {
    if (Input.GetKeyDown(KeyCode.Space) && grounded)
    {
      rb.AddRelativeForce(Vector3.up * jumpForce);
    }
  }

  void Update()
  {
    Jumping();
    Crouching();
  }

  //If player toggle the crouch it will scale the player to appear that is crouching
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

  public int maxSpeed = 5;
  public float deaccelerationSpeed = 15.0f;
  public float accelerationSpeed = 50000.0f;
  public bool grounded;

  //Checks if our player is contacting the ground in the angle less than 60 degrees if it is, set grounded to true
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

}