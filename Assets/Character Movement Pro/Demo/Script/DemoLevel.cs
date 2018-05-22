namespace CharacterMovementPro {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[RequireComponent(typeof(BoxCollider2D))]
	public class DemoLevel : MonoBehaviour {






		// API
		public Vector3 EntrancePosition {
			get {
				return m_Entrance.position;
			}
		}

		public Vector3 ExitPosition {
			get {
				return m_Exit.position;
			}
		}

		public Bounds Range {
			get {
				RangeCol.enabled = true;
				var b = RangeCol.bounds;
				RangeCol.enabled = false;
				return b;
			}
		}


		// Short Cut
		private BoxCollider2D RangeCol {
			get {
				return _RangeCol ?? (_RangeCol = GetComponent<BoxCollider2D>());
			}
		}


		// Ser
		[SerializeField] private Transform m_Entrance;
		[SerializeField] private Transform m_Exit;
		[SerializeField] private bool m_AllowMove = true;
		[SerializeField] private bool m_AllowJump = true;
		[SerializeField] private bool m_AllowDush = true;
		[SerializeField] private bool m_AllowSquat = true;
		[SerializeField] private bool m_AllowGrab = true;


		// Data
		private BoxCollider2D _RangeCol = null;




		private void Awake () {
			RangeCol.enabled = false;
		}



		public void FixMovement (CharacterMovement movement) {
			movement.MoveSpeedMuti = m_AllowMove ? 1f : 0f;
			movement.DushSpeedMuti = m_AllowDush ? 1f : 0f;
			movement.Data.AllowSquat = m_AllowSquat;
			movement.Data.JumpCount = m_AllowJump ? 2 : 0;
			movement.Data.AllowForwardGrab = m_AllowGrab;
			movement.Data.AllowBackGrab = m_AllowGrab;
			movement.Data.AllowTopGrab = m_AllowGrab;
		}


		public void DisableAllTriggers () {
			var cols = GetComponentsInChildren<Collider2D>();
			for (int i = 0; i < cols.Length; i++) {
				if (cols[i].isTrigger) {
					cols[i].enabled = false;
				}
			}
		}


	}
}