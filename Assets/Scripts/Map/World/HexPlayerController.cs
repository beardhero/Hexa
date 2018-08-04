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
	Animator animator;
	float currentHeight = 0;
	float testH = 0;
	public float gravityScale = 4f;
	public float walkSpeed = 1.33f;
	public float runSpeed = 1.33f;
	public float rotateSpeed = 2.4f;
	public float jumpHeight = 24f;
	public bool canJump;
	public bool jumped;
	public Camera cam;
	//public float camZoomStep = .3f;
	public float camRotateSpeed = 4.2f;
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
		animator = player.GetComponent<Animator>();
	}
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space) && canJump)
		{
			jumped = true;
		}
		if(Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.W))
		{
			animator.Play("Idle");
		}
	}
	// Update is called once per frame
	void FixedUpdate () { 
		gravityDir = (origin - trans.position).normalized;
		trans.rotation = Quaternion.FromToRotation(trans.up, -gravityDir) * trans.rotation;
		rigidbody.AddForce(gravityDir * gravityScale * rigidbody.mass, ForceMode.Acceleration);

		if(Input.GetKey(KeyCode.W))
		{
			animator.enabled = true;
			if(Input.GetKey(KeyCode.LeftShift))
			{
				rigidbody.velocity += trans.forward * runSpeed;
				{animator.Play("Run");}
			}
			else{
				rigidbody.velocity += trans.forward * walkSpeed;
				{animator.Play("Walk");}
			}
		}
		if(Input.GetKey(KeyCode.S))
		{
			animator.enabled = true;
			if(Input.GetKey(KeyCode.LeftShift))
			{
				rigidbody.velocity += trans.forward * runSpeed;
				{animator.Play("Run");}
			}
			else{
				rigidbody.velocity += -trans.forward * walkSpeed;
				{animator.Play("Walk");}
			}
		}
		
		if(Input.GetKey(KeyCode.D))
		{
			trans.RotateAround(trans.position, gravityDir, -rotateSpeed);
		}
		if(Input.GetKey(KeyCode.A))
		{
			trans.RotateAround(trans.position, gravityDir, rotateSpeed);
		}
		if(jumped)
		{
			rigidbody.AddForce(-gravityDir * jumpHeight);
			canJump = false;
			jumped = false;
		}
		if(Input.GetKey(KeyCode.Mouse1))
		{
			cam.transform.RotateAround(trans.position, gravityDir, -camRotateSpeed*Input.GetAxis("Mouse X"));
			cam.transform.RotateAround(trans.position, cam.transform.right, -camRotateSpeed*Input.GetAxis("Mouse Y"));
		}
	}
	void OnCollisionStay(Collision collision)
	{
		canJump = true;
	}
}
