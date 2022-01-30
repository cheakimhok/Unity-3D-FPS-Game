using System.Collections;
using UnityEngine;

public class DestroyAfterTimeParticle : MonoBehaviour 
{
	public float timeToDestroy = 0.8f;
	//Destroys gameobject after its created on scene.
	//This is used for particles and flashes.
	void Start () 
	{
		Destroy (gameObject, timeToDestroy);
	}

}
