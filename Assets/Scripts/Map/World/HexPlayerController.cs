using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPlayerController : MonoBehaviour {
	Rigidbody rigidbody;
	Transform trans;
	GameObject player;
	Vector3 gravityDir;
	Vector3 moveDir;
	WorldManager wM;
	World aW;
	Vector3 origin;
	float currentHeight = 0;
	float testH = 0;
	public float gravityScale = 4f;
	public float runSpeed = 1;
	public float rotateSpeed = 2.4f;
	public float jumpHeight = 24f;
	public bool canJump;
	public Camera cam;
	public float camSens = .5f;
	// Use this for initialization
	void Start () {
		player = this.gameObject;
		trans = player.transform;
		rigidbody = GetComponent<Rigidbody>();
		rigidbody.useGravity = false;
		rigidbody.freezeRotation = true;
		cam = Camera.main;
		wM = GameObject.Find("WorldManager").GetComponent<WorldManager>();
		aW = wM.activeWorld;
		origin = new Vector3(aW.origin.x, aW.origin.y, aW.origin.z);
	}
	void Update()
	{
		//trans.up = -gravityDir/gravityDir.magnitude;
	}
	// Update is called once per frame
	void FixedUpdate () { 
		gravityDir = (origin - trans.position).normalized;
		trans.rotation = Quaternion.FromToRotation(trans.up, -gravityDir) * trans.rotation;
		rigidbody.AddForce(gravityDir * gravityScale * rigidbody.mass, ForceMode.Acceleration);
		
		if(Input.GetKey(KeyCode.W))
		{
			rigidbody.velocity += trans.forward * runSpeed;
		}
		if(Input.GetKey(KeyCode.S))
		{
			rigidbody.velocity += -trans.forward * runSpeed;
		}
		if(Input.GetKey(KeyCode.D))
		{
			if(gravityDir.y > 0){trans.Rotate(gravityDir, rotateSpeed);}
			else{trans.Rotate(gravityDir, -rotateSpeed);}
		}
		if(Input.GetKey(KeyCode.A))
		{
			if(gravityDir.y > 0){trans.Rotate(gravityDir, -rotateSpeed);}
			else{trans.Rotate(gravityDir, rotateSpeed);}
		}
		if(Input.GetKeyDown(KeyCode.Space) && canJump)
		{
			rigidbody.AddForce(-gravityDir * jumpHeight);
			canJump = false;
		}
	}
	
	void OnCollisionEnter(Collision collision)
    {
		canJump = true;

		/* 
		Vector3 setPoint = new Vector3();
		if(currentHeight == 0)
			{
				currentHeight = collision.contacts[0].point.sqrMagnitude;
			}
        foreach (ContactPoint contact in collision.contacts)
        {
			float t = (contact.point - origin).sqrMagnitude;
            if(t > testH && (Mathf.Abs(t - testH) < .1f || testH == 0)) //getting biggest, only checking ones that aren't too big
			{
				testH = t;
				setPoint = contact.point;
			}
			if(testH > currentHeight)
			{
				rigidbody.MovePosition(trans.position, trans.position*(Mathf.Abs(testH - currentHeight)));
				currentHeight = testH;
			}
        }
		
        //if (collision.relativeVelocity.magnitude > 2)  {audioSource.Play();}
		*/
    }
}
