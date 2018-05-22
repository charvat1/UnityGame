namespace CharacterMovementPro {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class DemoGame : MonoBehaviour {



		#region --- VAR ---


		// Global
		public static DemoGame Main;
		private const float SAVE_GAP = 0.1f;


		// API
		public Transform Player {
			get {
				return m_Player;
			}
		}



		// Short Cut
		private Camera Camera {
			get {
				return _Camera ?? (_Camera = Camera.main ?? FindObjectOfType<Camera>());
			}
		}

		public DemoLevel CurrentLevel {
			get {
				return _CurrentLevel;
			}

			private set {
				_CurrentLevel = value;
			}
		}


		// Ser
		[SerializeField] private Transform m_Player;
		[SerializeField] private Transform m_KillerPoint;
		[SerializeField] private bool m_GodMod = false;
		[SerializeField] private float m_PlayerRebornWait = 1f;
		[SerializeField] private Transform[] m_ADCanvas;
		[Header("Particle"), SerializeField] private Transform m_BloodParticle;
		[SerializeField] private Transform m_SaveParticle;
		[SerializeField] private Transform m_RebornParticle;
		[Header("Audio"), SerializeField] private AudioClip m_DieSound;
		[SerializeField] private AudioClip m_RebornSound;
		[SerializeField] private AudioClip m_SaveSound;
		[Space, SerializeField] private List<DemoLevel> m_Levels;


		// Data
		private Vector3 PlayerRebornPoint;
		private DemoLevel _CurrentLevel = null;
		private Transform BloodParticle = null;
		private Camera _Camera = null;
		private float LastSaveTime = float.MinValue;
		private int CurrentLevelIndex = 0;



		#endregion




		#region --- MSG ---



		private void Awake () {
			Main = this;
		}



		private void Start () {
			GotoLevel(0, true, true);
			Save(CurrentLevel.EntrancePosition, false, true);
			DisableKillerPoint();
			Screen.SetResolution(1024, 768, false);
		}



		private void Update () {
			if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(2)) {
				Reborn();
			}
			if (m_GodMod) {
				if (Input.GetKeyDown(KeyCode.Alpha1)) {
					GotoLevel(Mathf.Clamp(CurrentLevelIndex - 1, 0, m_Levels.Count - 1), true, true);
					Save(CurrentLevel.EntrancePosition, false, true);
				}
				if (Input.GetKeyDown(KeyCode.Alpha2)) {
					GotoLevel(Mathf.Clamp(CurrentLevelIndex + 1, 0, m_Levels.Count - 1), true, true);
					Save(CurrentLevel.EntrancePosition, false, true);
				}
			}
		}



		#endregion




		#region --- API ---



		public void UI_GotoLink (string url) {
			Application.OpenURL(url);
		}



		public void PlayerDied (Vector3 killerPosition) {
			m_Player.gameObject.SetActive(false);
			if (CurrentLevel) {
				CurrentLevel.DisableAllTriggers();
			}
			Invoke("Reborn", m_PlayerRebornWait);
			BloodEffect();
			// Killer Point
			killerPosition.z = -0.2f;
			m_KillerPoint.gameObject.SetActive(true);
			m_KillerPoint.position = killerPosition;
		}




		public void DisableKillerPoint () {
			m_KillerPoint.gameObject.SetActive(false);
		}



		public void Reborn () {
			if (IsInvoking("Reborn")) {
				CancelInvoke("Reborn");
			}
			// Reset Level
			GotoLevel(CurrentLevelIndex, true, false);
			// Reset Player
			m_Player.position = PlayerRebornPoint;
			m_Player.gameObject.SetActive(true);
			var movement = m_Player.GetComponent<CharacterMovement>();
			movement.BreakDush();
			movement.Move(0, 0);
			// Effect
			RebornEffect();
			LastSaveTime = Time.time;
		}



		public void Save (Vector3 pos, bool useEffect = true, bool forceSave = false) {
			if (forceSave || Time.time > LastSaveTime + SAVE_GAP) {
				LastSaveTime = Time.time;
				// Logic
				PlayerRebornPoint = pos;
				if (useEffect) {
					SaveEffect(pos);
				}
			}
		}



		public void UI_GotoLevel (int index) {
			GotoLevel(index, true, true);
			Save(CurrentLevel.EntrancePosition, false, true);
		}


		public void GotoLevel (int index, bool toEntrance = true, bool useEffect = true) {
			// Destroy Current Level
			if (CurrentLevel) {
				Destroy(CurrentLevel.gameObject);
			}
			// Spawn New Level
			CurrentLevel = Instantiate(m_Levels[index], Vector3.zero, Quaternion.identity);
			CurrentLevelIndex = index;
			// Reset Player
			m_Player.gameObject.SetActive(true);
			m_Player.position = toEntrance ? CurrentLevel.EntrancePosition : CurrentLevel.ExitPosition;
			if (useEffect) {
				RebornEffect();
			}
			// Fix Movement
			CurrentLevel.FixMovement(m_Player.GetComponent<CharacterMovement>());
			// Reset Camera
			var range = CurrentLevel.Range;
			Camera.transform.position = new Vector3(range.center.x, range.center.y, Camera.transform.position.z);
			Camera.orthographicSize = Mathf.Max(
				range.size.y * 0.5f,
				range.size.x * 0.5f / Camera.aspect
			);
			// Delete Blood
			if (BloodParticle) {
				Destroy(BloodParticle.gameObject);
			}
			// End Canvas
			for (int i = 0; i < m_ADCanvas.Length; i++) {
				m_ADCanvas[i].gameObject.SetActive(index == i);
			}
		}



		public void SetDushable (bool dushable) {
			var m = m_Player.GetComponent<CharacterMovement>();
			if (m) {
				m.DushSpeedMuti = dushable ? 1f : 0f;
			}
		}



		#endregion




		#region --- LGC ---



		private void RebornEffect () {
			// Particle
			var particle = Instantiate(m_RebornParticle, m_Player.position, Quaternion.identity);
			Destroy(particle.gameObject, 3f);
			// Audio
			AudioSource.PlayClipAtPoint(m_RebornSound, Vector3.zero, 0.25f);
		}




		private void SaveEffect (Vector3 pos) {
			// Audio
			AudioSource.PlayClipAtPoint(m_SaveSound, Vector3.zero, 0.5f);
			// Particle
			pos.z += 0.01f;
			Transform tf = Instantiate(m_SaveParticle, pos, Quaternion.identity);
			Destroy(tf.gameObject, 3f);
		}



		private void BloodEffect () {
			// Delete Blood
			if (BloodParticle) {
				Destroy(BloodParticle.gameObject);
			}
			// Blood
			BloodParticle = Instantiate(m_BloodParticle, m_Player.position, Quaternion.identity);
			Destroy(BloodParticle.gameObject, 10f);
			// Audio
			AudioSource.PlayClipAtPoint(m_DieSound, Vector3.zero);
		}

		#endregion


	}
}
