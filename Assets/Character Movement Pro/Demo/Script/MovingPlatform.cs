namespace CharacterMovementPro {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;


	public class MovingPlatform : MonoBehaviour {



		private Rigidbody2D Rig {
			get {
				return _Rig ?? (_Rig = GetComponent<Rigidbody2D>());
			}
		}



		// Ser
		[SerializeField] private float Distance = 10f;
		[SerializeField] private float Speed = 10f;


		// Data
		private Rigidbody2D _Rig = null;
		private Vector3 _StartPosition;


		private void Awake () {
			_StartPosition = transform.position;
			Rig.velocity = Speed * Vector3.right;
		}


		private void FixedUpdate () {

			if (Rig.velocity.x > 0f) {
				if (transform.position.x > _StartPosition.x + Distance) {
					Rig.velocity = Speed * Vector3.left;
				}
			} else {
				if (transform.position.x < _StartPosition.x - Distance) {
					Rig.velocity = Speed * Vector3.right;
				}
			}



		}




	}
}