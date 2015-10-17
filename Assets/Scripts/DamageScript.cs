using UnityEngine;
using System.Collections;

public class DamageScript : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col){
		if (col.transform.root != transform.root && col.tag != "Ground" && !col.isTrigger) {
			if(!col.transform.GetComponent<PlayerControl>().damage){
				col.transform.GetComponent<PlayerControl>().damage = true;

				col.transform.root.GetComponentInChildren<Animator>().SetTrigger("Damage");
			}
		}
	}

}
