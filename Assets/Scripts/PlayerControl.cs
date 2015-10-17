using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	public int PlayerNumber = 1;
	Transform enemy;

	Rigidbody2D rig2d;
	Animator anim;

	float horizontal;
	float vertical;
	public float maxSpeed = 25;
	Vector3 movement;
	bool crouch;

	public float JumpForce = 20;
	public float JumpDuration = .1f;
	float jmpDuration;
	float jmpForce;
	bool jumpKey;
	bool falling;
	bool onGround;

	public float AttackRate = 0.3f;
	bool[] attack = new bool[2];
	float[] attackTimer = new float[2];
	int[] timesPressed = new int[2];

	public bool damage;
	public float noDamage = 1;
	float noDamageTimer;

	public bool specialAttack;
	public GameObject projectile;
	
	void Start () {
		rig2d = GetComponent<Rigidbody2D> ();
		anim = GetComponentInChildren<Animator> ();
		jmpForce = JumpForce;

		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject pl in players) {
			if(pl.transform != this.transform){
				enemy = pl.transform;
			}
		}
	}
		
	void Update(){
		OnGroundCheck ();
		AttackInput();
		ScaleCheck ();
		Damage ();
		SpecialAttack ();
		UpdateAnimator ();
	}

	void FixedUpdate () {
		horizontal = Input.GetAxis ("Horizontal" + PlayerNumber.ToString());
		vertical = Input.GetAxis ("Vertical" + PlayerNumber.ToString());
		
		Vector3 movement = new Vector3(horizontal, 0, 0);
		
		crouch = (vertical < -0.1f);
		
		if (vertical > 0.1f) {
			if(!jumpKey){
				jmpDuration += Time.deltaTime;
				jmpForce += Time.deltaTime;
				
				if(jmpDuration < JumpDuration){
					rig2d.velocity = new Vector2(rig2d.velocity.x, jmpForce);
				} else {
					jumpKey = true;
				}
			}
		}
		
		if (!onGround && vertical < 0.1f) {
			falling = true;
		}
		
		if (attack [0] && !jumpKey || attack [1] && !jumpKey) {
			movement = Vector3.zero;
		}
		
		if (!crouch) {
			rig2d.AddForce (movement * maxSpeed);
		} else {
			rig2d.velocity = Vector3.zero;
		}
	}

	void UpdateAnimator(){
		anim.SetBool ("OnGround", this.onGround);
		anim.SetBool ("Falling", this.falling);
		anim.SetBool ("Crouch", this.crouch);
		anim.SetFloat ("Movement", Mathf.Abs (horizontal));
		anim.SetBool ("Attack1", attack [0]);
		anim.SetBool ("Attack2", attack [1]);
	}

	void AttackInput(){
		if (Input.GetButtonDown ("Attack1" + PlayerNumber.ToString ())) {
			attack[0] = true;
			attackTimer[0] = 0;
			timesPressed[0]++;
		}

		if (attack [0]) {
			attackTimer[0] += Time.deltaTime;
			if(attackTimer[0] > AttackRate || timesPressed[0] >= 4){
				attackTimer[0] = 0;
				attack[0] = false;
				timesPressed[0] = 0;
			}
		}

		if (Input.GetButtonDown ("Attack2" + PlayerNumber.ToString ())) {
			attack[1] = true;
			attackTimer[1] = 0;
			timesPressed[1]++;
		}
		
		if (attack [1]) {
			attackTimer[1] += Time.deltaTime;
			if(attackTimer[1] > AttackRate || timesPressed[1] >= 4){
				attackTimer[1] = 0;
				attack[1] = false;
				timesPressed[1] = 0;
			}
		}
	}

	void OnGroundCheck(){
		if (!onGround) {
			rig2d.gravityScale = 5;
		} else {
			rig2d.gravityScale = 1;
		}
	}

	void ScaleCheck(){
		if (transform.position.x < enemy.position.x) {
			transform.localScale = new Vector3 (-1, 1, 1);
		} else {
			transform.localScale = Vector3.one;
		}
	}

	void OnCollisionEnter2D(Collision2D col2D){
		if (col2D.collider.tag == "Ground") {
			onGround = true;
			jumpKey = false;
			jmpDuration = 0;
			jmpForce = JumpForce;
			falling = false;
		}
	}
	
	void OnCollisionExit2D(Collision2D col2D){
		if (col2D.collider.tag == "Ground") {
			onGround = false;
		}
	}

	void Damage(){
		if (damage) {
			noDamageTimer += Time.deltaTime;

			if(noDamageTimer > noDamage){
				damage = false;
				noDamageTimer = 0;
			}
		}

		/*
		if (!onGround) {

		}
		*/
	}

	void SpecialAttack(){
		if (specialAttack) {
			GameObject pr = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
			Vector3 nrDir = new Vector3(enemy.position.x, transform.position.y, 0);
			Vector3 dir = nrDir - transform.position;
			pr.GetComponent<Rigidbody2D>().AddForce(dir * 10, ForceMode2D.Impulse);

			specialAttack = false;
		}
	}
}
