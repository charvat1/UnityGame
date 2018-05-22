namespace CharacterMovementPro {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;


	public class EventTrigger : MonoBehaviour {


		private Collider2D Col {
			get {
				return _Col ?? (_Col = GetComponent<Collider2D>());
			}
		}


		private Collider2D _Col = null;


		[SerializeField] private string m_Action;
		[SerializeField] private float m_Param;
		[SerializeField] private Transform m_Target;
		[SerializeField] private bool m_Delete = false;
		[SerializeField] private float m_Dely = 0f;
		[SerializeField] private AudioClip m_Audio;
		[SerializeField] private bool m_Enter = true;
		[SerializeField] private bool m_Stay = true;
		[SerializeField] private bool m_Exit;
		[SerializeField] private bool m_Awake;


		private Vector2? ContentPoint;


		private void Awake () {
			if (m_Awake) {
				Trigger(null);
			}
		}

		private void OnCollisionEnter2D (Collision2D col) {
			if (m_Enter) {
				var cons = col.contacts;
				if (cons.Length > 0) {
					ContentPoint = cons[0].point;
				}
				Trigger(col.transform);
			}
		}
		private void OnCollisionStay2D (Collision2D col) {
			if (m_Stay) {
				var cons = col.contacts;
				if (cons.Length > 0) {
					ContentPoint = cons[0].point;
				}
				Trigger(col.transform);
			}
		}
		private void OnCollisionExit2D (Collision2D col) {
			if (m_Exit) {
				var cons = col.contacts;
				if (cons.Length > 0) {
					ContentPoint = cons[0].point;
				}
				Trigger(col.transform);
			}
		}


		private void OnTriggerEnter2D (Collider2D col) {
			if (m_Enter) {
				Trigger(col.transform);
			}
		}
		private void OnTriggerStay2D (Collider2D col) {
			if (m_Stay) {
				Trigger(col.transform);
			}
		}
		private void OnTriggerExit2D (Collider2D col) {
			if (m_Exit) {
				Trigger(col.transform);
			}
		}



		private void Trigger (Transform tf) {
			if (!tf || (DemoGame.Main && tf == DemoGame.Main.Player)) {
				if (!IsInvoking("InvokeTrigger")) {
					Invoke("InvokeTrigger", m_Dely);
				}
			}
		}



		private void InvokeTrigger () {
			switch (m_Action.ToLower()) {

				// Null
				default:
					Debug.LogWarning("No Action: " + m_Action);
					break;
				case "":
				case null:
					break;

				// Object
				case "active":
					if (m_Target) {
						m_Target.gameObject.SetActive(true);
					}
					break;
				case "inactive":
					if (m_Target) {
						m_Target.gameObject.SetActive(false);
					}
					break;
				case "killplayer":
					if (DemoGame.Main) {
						DemoGame.Main.PlayerDied((Vector3)ContentPoint);
					}
					break;


				// Target
				case "target.move.up":
				case "target.move.down":
				case "target.move.left":
				case "target.move.right":
				case "target.moveto.x":
				case "target.moveto.y":
					RigAction();
					break;
				case "target.scale.x":
				case "target.scale.y":
					TransformAction();
					break;
				case "target.delete":
					if (m_Target) {
						Destroy(m_Target.gameObject);
					}
					break;

				// Game
				case "allowdush":
					if (DemoGame.Main) {
						DemoGame.Main.SetDushable(true);
					}
					break;
				case "notallowdush":
					if (DemoGame.Main) {
						DemoGame.Main.SetDushable(false);
					}
					break;
				case "savegame":
					if (DemoGame.Main) {
						DemoGame.Main.Save(transform.position);
					}
					break;

				// Level
				case "gotolevel":
					if (DemoGame.Main) {
						DemoGame.Main.GotoLevel((int)m_Param, true, true);
						DemoGame.Main.Save(DemoGame.Main.CurrentLevel.EntrancePosition, false, true);
						DemoGame.Main.DisableKillerPoint();
					}
					break;
				case "backtolevel":
					if (DemoGame.Main) {
						DemoGame.Main.GotoLevel((int)m_Param, false, true);
						DemoGame.Main.Save(DemoGame.Main.CurrentLevel.ExitPosition, false, true);
						DemoGame.Main.DisableKillerPoint();
					}
					break;
			}
			if (m_Audio) {
				AudioSource.PlayClipAtPoint(m_Audio, Vector3.zero);
			}
			if (m_Delete) {
				DestroyImmediate(this, false);
			}
			ContentPoint = null;
		}



		private void RigAction () {
			if (m_Target) {
				// Init Component
				var rig = m_Target.GetComponent<Rigidbody2D>();
				if (rig) {
					// Action
					switch (m_Action.ToLower()) {
						default:
							break;
						case "target.move.up":
							rig.velocity = Vector2.up * m_Param;
							break;
						case "target.move.down":
							rig.velocity = Vector2.down * m_Param;
							break;
						case "target.move.left":
							rig.velocity = Vector2.left * m_Param;
							break;
						case "target.move.right":
							rig.velocity = Vector2.right * m_Param;
							break;
						case "target.moveto.x":
							rig.MovePosition(new Vector2(m_Param, rig.position.y));
							break;
						case "target.moveto.y":
							rig.MovePosition(new Vector2(rig.position.x, m_Param));
							break;
					}
				}
			}
		}



		private void TransformAction () {
			if (m_Target) {
				switch (m_Action.ToLower()) {
					default:
					case "":
						break;
					case "target.scale.x":
						m_Target.localScale = new Vector3(m_Param, m_Target.localScale.y, m_Target.localScale.z);
						break;
					case "target.scale.y":
						m_Target.localScale = new Vector3(m_Target.localScale.x, m_Param, m_Target.localScale.z);
						break;
				}
			}
		}

	}
}