namespace CharacterMovementPro {
	using UnityEngine;

	public class PlayerMovement : CharacterMovement {


		[System.Serializable]
		public class KeyInputData {
			public KeyCode Left = KeyCode.LeftArrow;
			public KeyCode Right = KeyCode.RightArrow;
			public KeyCode Up = KeyCode.UpArrow;
			public KeyCode Down = KeyCode.DownArrow;
			public KeyCode Jump = KeyCode.Space;
			public KeyCode Dush = KeyCode.LeftShift;
			public KeyCode Squat = KeyCode.DownArrow;
			public KeyCode DropGrab = KeyCode.DownArrow;
		}



		#region --- VAR ---


		// Ser
		[SerializeField] private KeyInputData m_KeyData;
		[SerializeField] private KeyInputData m_KeyDataAlt;

		// Data
		private float LastKeyPressedTime_L = float.MinValue;
		private float LastKeyPressedTime_R = float.MinValue;
		private float LastKeyPressedTime_U = float.MinValue;
		private float LastKeyPressedTime_D = float.MinValue;


		#endregion



		#region --- EDT ---
#if UNITY_EDITOR

		private void Reset () {
			m_KeyData = new KeyInputData() {
				Left = KeyCode.LeftArrow,
				Right = KeyCode.RightArrow,
				Up = KeyCode.UpArrow,
				Down = KeyCode.DownArrow,
				Jump = KeyCode.Space,
				Dush = KeyCode.LeftShift,
				Squat = KeyCode.DownArrow,
				DropGrab = KeyCode.DownArrow,
			};
			m_KeyDataAlt = new KeyInputData() {
				Left = KeyCode.A,
				Right = KeyCode.D,
				Up = KeyCode.W,
				Down = KeyCode.S,
				Jump = KeyCode.Space,
				Dush = KeyCode.LeftShift,
				Squat = KeyCode.S,
				DropGrab = KeyCode.S,
			};
		}

#endif
        #endregion



        #region --- MSG ---


        private Animator animator;

        private void Update () {
            animator = GetComponentInChildren<Animator>();
            MovementUpdate();
			ActionUpdate();
		}



		private void MovementUpdate () {

			float moveH = 0f;
			float moveV = 0f;

			if (CheckKey(m_KeyData.Left, m_KeyDataAlt.Left)) {
				if (LastKeyPressedTime_L < 0f) {
					LastKeyPressedTime_L = Time.time;
				}
				if (LastKeyPressedTime_L > LastKeyPressedTime_R) {
					moveH = -1f;
                    animator.SetBool("isRunning", true);


                }
			} else {
				LastKeyPressedTime_L = -1f;
                animator.SetBool("isRunning", false);
            }

			if (CheckKey(m_KeyData.Right, m_KeyDataAlt.Right)) {
				if (LastKeyPressedTime_R < 0f) {
					LastKeyPressedTime_R = Time.time;
				}
				if (LastKeyPressedTime_R > LastKeyPressedTime_L) {
					moveH = 1f;
                    animator.SetBool("isRunning", true);
                }
			} else {
				LastKeyPressedTime_R = -1f;
                animator.SetBool("isRunning", false);
            }

			if (CheckKey(m_KeyData.Down, m_KeyDataAlt.Down)) {
				if (LastKeyPressedTime_D < 0f) {
					LastKeyPressedTime_D = Time.time;
				}
				if (LastKeyPressedTime_D > LastKeyPressedTime_U) {
					moveV = -1f;
				}
			} else {
				LastKeyPressedTime_D = -1f;
			}

			if (CheckKey(m_KeyData.Up, m_KeyDataAlt.Up)) {
				if (LastKeyPressedTime_U < 0f) {
					LastKeyPressedTime_U = Time.time;
				}
				if (LastKeyPressedTime_U > LastKeyPressedTime_D) {
					moveV = 1f;
				}
			} else {
				LastKeyPressedTime_U = -1f;
			}

			Move(moveH, moveV);

		}



		public void ActionUpdate () {

			// Squat
			Squat(CheckKey(m_KeyData.Squat, m_KeyDataAlt.Squat));

			// Jump
			if (CheckKeyDown(m_KeyData.Jump, m_KeyDataAlt.Jump) &&
				(!CheckKey(m_KeyData.Down, m_KeyDataAlt.Down) ||
				(!TryDropPlatform() && !IsCrossingPlatform))
			) {
				Jump();
			}
			if (!CheckKey(m_KeyData.Jump, m_KeyDataAlt.Jump)) {
				SetGravityScale(Data.DropGravity);
			}

			// Dush
			if (CheckKeyDown(m_KeyData.Dush, m_KeyDataAlt.Dush)) {
				Dush();
			}

			// Drop Grab
			if (CheckKeyDown(m_KeyData.DropGrab, m_KeyDataAlt.DropGrab)) {
				DropTopGrab();
			}

		}




		#endregion



		private bool CheckKey (KeyCode key, KeyCode keyAlt) {
			return Input.GetKey(key) || Input.GetKey(keyAlt) ||
				((key == KeyCode.LeftShift || keyAlt == KeyCode.LeftShift) && Input.GetKey(KeyCode.RightShift)) ||
				((key == KeyCode.LeftControl || keyAlt == KeyCode.LeftControl) && Input.GetKey(KeyCode.RightControl)) ||
				((key == KeyCode.LeftAlt || keyAlt == KeyCode.LeftAlt) && Input.GetKey(KeyCode.RightAlt));
		}


		private bool CheckKeyDown (KeyCode key, KeyCode keyAlt) {
			return Input.GetKeyDown(key) || Input.GetKeyDown(keyAlt) ||
				((key == KeyCode.LeftShift || keyAlt == KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.RightShift)) ||
				((key == KeyCode.LeftControl || keyAlt == KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.RightControl)) ||
				((key == KeyCode.LeftAlt || keyAlt == KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.RightAlt));
		}


	}
}