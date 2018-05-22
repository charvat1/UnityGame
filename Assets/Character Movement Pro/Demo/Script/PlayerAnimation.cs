namespace CharacterMovementPro {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class PlayerAnimation : MonoBehaviour {



        #region --- VAR ---
        
        // Short Cur
        private Animator Ani {
			get {
				return _Ani ?? (_Ani = GetComponent<Animator>());
			}
		}

		public CharacterMovement Movement {
			get {
				return _Movement ?? (_Movement = GetComponentInParent<CharacterMovement>());
			}
		}


		// Ser
		[SerializeField] private float m_FootstepGap = 0.05f;
		[SerializeField] private Transform m_FootstepParticleRoot;
		[SerializeField] private AudioSource m_FootstepSource;


		// Data
		private Animator _Ani = null;
		private CharacterMovement _Movement = null;
		private float _LastFootstepTime = float.MinValue;



		#endregion



		private void Update () {

			Ani.SetFloat("SquatInex", Movement.IsSquating ? 1f : 0f);
			Ani.SetFloat("GrabIndex", Movement.GrabingPose == CharacterMovement.GrabPose.Top ? 1f : 0f);
			Ani.SetBool("IsGrounded", Movement.IsGrounded);
			Ani.SetBool("IsDushing", Movement.IsDushing && Movement.DushSpeedMuti > 0f);
			Ani.SetBool("IsGrabing", Movement.IsGrabing);
			Ani.SetFloat("Speed", Mathf.Abs(Movement.IsGrabing && Movement.GrabingPose != CharacterMovement.GrabPose.Top ? Movement.AimVelocity.y : Movement.AimVelocity.x));
			Ani.SetFloat("SpeedY", Movement.Velocity.y);


			if (Movement.IsGrounded && Mathf.Abs(Movement.AimVelocity.x) > 0.1f) {
				m_FootstepParticleRoot.gameObject.SetActive(true);
				if (Time.time > _LastFootstepTime + m_FootstepGap) {
					_LastFootstepTime = Time.time;
					m_FootstepSource.volume = Random.Range(0.25f, 0.5f);
					m_FootstepSource.pitch = Random.Range(0.7f, 1f);
					if (Movement.IsSquating) {
						m_FootstepSource.volume *= 0.1f;
					}
					m_FootstepSource.Play();
                    
                }
			} else {
				m_FootstepParticleRoot.gameObject.SetActive(false);
			}



		}




	}
}
