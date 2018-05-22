namespace CharacterMovementPro {
	using UnityEngine;
	using UnityEngine.Events;

	[DisallowMultipleComponent]
	public abstract class CharacterMovement : MonoBehaviour {



		#region --- SUB ---



		public enum GrabPose {
			Forward,
			Back,
			Top,
		}



		[System.Serializable]
		public class MovementData {

			[Header("Component")]
			public Transform LogicalRoot = null;
			public Transform RendererRoot = null;
			public CapsuleCollider2D Collider = null;
			public Rigidbody2D Rigidbody = null;

			[Header("Gravity")]
			public float RiseGravity = 10f;
			public float DropGravity = 15f;
			public float MaxDropSpeed = 30f;
			public LayerMask GroundLayer = new LayerMask() { value = -1, };

			[Header("Move")]
			public float MoveSpeed = 15f;
			public float GrabMoveSpeed = 2.5f;
			public float MoveDely = 0;
			public bool RotateOnMove = true;

			[Header("Squat")]
			public bool AllowSquat = true;
			public float SquatMoveSpeed = 5f;
			public float SquatJumpSpeed = 25;
			public float SquatDushSpeed = 30f;
			public float SquatHeightScale = 0.5f;

			[Header("Jump")]
			public float JumpCount = 2;
			public float JumpSpeed = 20;
			public float JumpDely = 0;

			[Header("Dush")]
			public float DushSpeed = 25f;
			public float GrabDushSpeed = 5f;
			public float DushDuration = 0.2f;
			public float DushGap = 0.1f;
			public float DushDely = 0;
			public bool BreakDushOnJump = true;
			public bool AllowDushOnJump = true;

			[Header("Grab")]
			public bool AllowForwardGrab = true;
			public bool AllowBackGrab = true;
			public bool AllowTopGrab = true;
			public bool FixPositionOnGrab = true;
			public bool FixRotationOnBackGrab = true;
			public bool StopMoveOnGrabTopEdge = true;
			public bool StopMoveOnGrabBottomEdge = true;
			public bool StopMoveOnGrabHorizontalEdge = true;
			public bool DropGrabOnJump = true;
			public bool DropGrabOnGround = true;
			public float GrabLengthTop = 0.1f;
			public float GrabLength = 0.5f;
			public string GrabableTag = "Untagged";

			[Header("Platform")]
			public bool AllowDropThroughPlatform = true;
			public bool MoveWithPlatform = true;

		}



		[System.Serializable]
		public class EventData {


			// SUB
			[System.Serializable] public class VoidEvent : UnityEvent { }

			[System.Serializable] public class PoseEvent : UnityEvent<GrabPose> { }



			// API
			public VoidEvent OnDush {
				get {
					return m_OnDush;
				}
			}

			public VoidEvent OnJump {
				get {
					return m_OnJump;
				}
			}

			public VoidEvent OnGround {
				get {
					return m_OnGround;
				}
			}


			// Ser
			[SerializeField] private VoidEvent m_OnDush;
			[SerializeField] private VoidEvent m_OnJump;
			[SerializeField] private VoidEvent m_OnGround;


		}


		#endregion



		#region --- VAR ---


		// Const
		private const int RAYCAST_CACHE_NUM = 16;
		private const float JUMP_REFRESH_GAP = 0.1f;
		private const float VELOCITY_GAP = 0.01f;
		private const float GROUND_CHECK_GAP = 0.05f;
		private const float DROP_PLATFORM_GAP = 0.01f;
		private const float GRAVITY_SCALE_GAP = 0.01f;
		private const float GRAB_RAY_SCALE_A = 0.25f;
		private const float GRAB_RAY_SCALE_B = 0.75f;


		// API
		public GrabPose GrabingPose {
			get {
				return _GrabingPose;
			}
			private set {
				_GrabingPose = value;
			}
		}

		public Vector2 AimVelocity {
			get {
				return _AimVelocity;
			}
		}

		public Vector2 Velocity {
			get {
				return Rig.velocity;
			}
		}

		public bool IsGrounded {
			get {
				return _IsGrounded;
			}
			protected set {
				_IsGrounded = value;
			}
		}

		public bool IsDushing {
			get {
				return Time.time < _LastDushTime + Data.DushDuration;
			}
		}

		public bool IsSquating {
			get {
				return Data.AllowSquat && (_ForceSquat || (_IsSquating && IsGrounded));
			}

			private set {
				_IsSquating = value;
			}
		}

		public bool IsGrabing {
			get {
				return _GrabHitUR.HasValue || _GrabHitDL.HasValue;
			}
		}

		public bool IsFacingRight {
			get {
				return _IsFacingRight;
			}
		}

		public bool IsCrossingPlatform {
			get {
				return _CrossingPlatform;
			}
		}

		public float MoveSpeedMuti {
			get {
				return _MoveSpeedMuti;
			}
			set {
				_MoveSpeedMuti = value;
			}
		}

		public float JumpSpeedMuti {
			get {
				return _JumpSpeedMuti;
			}

			set {
				_JumpSpeedMuti = value;
			}
		}

		public float DushSpeedMuti {
			get {
				return _DushSpeedMuti;
			}

			set {
				_DushSpeedMuti = value;
			}
		}

		public float Radius {
			get {
				return Col.size.x * 0.5f;
			}
		}

		public float Height {
			get {
				return Col.size.y;
			}
		}

		public MovementData Data {
			get {
				return m_Data;
			}
			set {
				m_Data = value;
			}
		}




		// Short Cut
		private EventData Event {
			get {
				return m_Event;
			}
		}

		private Rigidbody2D Rig {
			get {
				if (!Data.Rigidbody) {
					Data.Rigidbody = GetComponent<Rigidbody2D>();
				}
				return Data.Rigidbody;
			}
		}

		private CapsuleCollider2D Col {
			get {
				if (!Data.Collider) {
					Data.Collider = GetComponent<CapsuleCollider2D>();
				}
				return Data.Collider;
			}
		}

		private Transform RendererRoot {
			get {
				if (!Data.RendererRoot) {
					Data.RendererRoot = transform;
				}
				return Data.RendererRoot;
			}
		}

		private Transform LogicalRoot {
			get {
				if (!Data.LogicalRoot) {
					Data.LogicalRoot = transform;
				}
				return Data.LogicalRoot;
			}
		}

		private float FixedMoveSpeed {
			get {
				return (IsGrabing ? Data.GrabMoveSpeed : (IsSquating ? Data.SquatMoveSpeed : Data.MoveSpeed)) * MoveSpeedMuti;
			}
		}

		private float FixedJumpSpeed {
			get {
				return (IsSquating ? Data.SquatJumpSpeed : Data.JumpSpeed) * JumpSpeedMuti;
			}
		}

		private float FixedDushSpeed {
			get {
				return (IsGrabing ? Data.GrabDushSpeed : (IsSquating ? Data.SquatDushSpeed : Data.DushSpeed)) * DushSpeedMuti;
			}
		}

		private bool AimVelocityYIsZero {
			get {
				return Mathf.Abs(_AimVelocity.y) < VELOCITY_GAP;
			}
		}


		// Ser
		[SerializeField] private MovementData m_Data;
		[SerializeField] private EventData m_Event;


		// Data
		private GrabPose _GrabingPose = GrabPose.Forward;
		private Collider2D _CrossingPlatform = null;
		private Rigidbody2D _ContactingRig = null;
		private RaycastHit2D? _GroundHit = null;
		private RaycastHit2D? _GrabHitUR = null;
		private RaycastHit2D? _GrabHitDL = null;
		private Vector3 _CurrentVelocity = Vector3.zero;
		private Vector2 _AimVelocity = Vector2.zero;
		private float _BodyBasicScaleX = 1f;
		private float _MoveSpeedMuti = 1f;
		private float _JumpSpeedMuti = 1f;
		private float _DushSpeedMuti = 1f;
		private float _ColliderBasicHeight = 1f;
		private float _LastJumpTime = float.MinValue;
		private float _LastDushTime = float.MinValue;
		private bool _IsFacingRight = false;
		private bool _IsSquating = false;
		private bool _ForceSquat = false;
		private bool _IsGrounded = false;
		private int _GroundLayerInt = -1;
		private int _JumpCount = 0;
        private Animator animator;

        // Cache
        private RaycastHit2D[] c_GroundCheckHits = new RaycastHit2D[RAYCAST_CACHE_NUM];
		private RaycastHit2D[] c_GrabCheckHits = new RaycastHit2D[RAYCAST_CACHE_NUM];



		#endregion



		#region --- MSG ---



		private void Awake () {
			Rig.constraints = RigidbodyConstraints2D.FreezeRotation;
			_BodyBasicScaleX = RendererRoot.localScale.x;
			_ColliderBasicHeight = Height;
			_GroundLayerInt = Data.GroundLayer.value;
		}



		private void FixedUpdate () {
			GroundCheckUpdate();
			GrabCheckUpdate();
			JumpUpdate();
			MovementUpdate();
			PlatformUpdate();
		}




		// Fixed Updates
		private void GroundCheckUpdate () {
			_ContactingRig = null;
			_GroundHit = null;
			bool isGrounded = GroundCheck(new Vector2(
				LogicalRoot.position.x - Radius * 0.5f,
				LogicalRoot.position.y + GROUND_CHECK_GAP
			), Vector2.down, GROUND_CHECK_GAP * 2f, out _GroundHit) ||
			GroundCheck(new Vector2(
				LogicalRoot.position.x + Radius * 0.5f,
				LogicalRoot.position.y + GROUND_CHECK_GAP
			), Vector2.down, GROUND_CHECK_GAP * 2f, out _GroundHit);
			if (isGrounded && !IsGrounded) {
				Event.OnGround.Invoke();
			}
			IsGrounded = isGrounded;
		}



		private void GrabCheckUpdate () {

			_GrabHitUR = null;
			_GrabHitDL = null;

			if ((Data.DropGrabOnGround && IsGrounded) || IsSquating) {
				return;
			}

			float rot2D = IsFacingRight ? 1f : -1f;

			// Top
			if (!IsGrabing && Data.AllowTopGrab) {
				_GrabHitUR = GrabCheck((Vector2)LogicalRoot.position + new Vector2(GRAB_RAY_SCALE_B * Radius, Height), Vector2.up, Data.GrabLengthTop);
				_GrabHitDL = GrabCheck((Vector2)LogicalRoot.position + new Vector2(-GRAB_RAY_SCALE_B * Radius, Height), Vector2.up, Data.GrabLengthTop);
				if (IsGrabing) {
					GrabingPose = GrabPose.Top;
				}
			}

			// Forward
			if (!IsGrabing && Data.AllowForwardGrab) {
				_GrabHitUR = GrabCheck((Vector2)LogicalRoot.position + new Vector2(rot2D * Radius, Height * GRAB_RAY_SCALE_B), rot2D * Vector2.right, Data.GrabLength);
				_GrabHitDL = GrabCheck((Vector2)LogicalRoot.position + new Vector2(rot2D * Radius, Height * GRAB_RAY_SCALE_A), rot2D * Vector2.right, Data.GrabLength);
				if (IsGrabing) {
					GrabingPose = GrabPose.Forward;
				}
			}

			// Back
			if (!IsGrabing && Data.AllowBackGrab) {
				_GrabHitUR = GrabCheck((Vector2)LogicalRoot.position + new Vector2(-rot2D * Radius, Height * GRAB_RAY_SCALE_B), rot2D * Vector2.left, Data.GrabLength);
				_GrabHitDL = GrabCheck((Vector2)LogicalRoot.position + new Vector2(-rot2D * Radius, Height * GRAB_RAY_SCALE_A), rot2D * Vector2.left, Data.GrabLength);
				if (IsGrabing) {
					if (Data.FixRotationOnBackGrab && !IsDushing) {
						Rotate(!IsFacingRight);
						GrabingPose = GrabPose.Forward;
					} else {
						GrabingPose = GrabPose.Back;
					}
				}
			}

		}



		private void JumpUpdate () {
			if ((IsGrounded || IsGrabing) && Time.time > _LastJumpTime + JUMP_REFRESH_GAP) {
				_JumpCount = 0;
			}
		}



		private void MovementUpdate () {

			// Drop
			_CurrentVelocity.y = Mathf.Clamp(Rig.velocity.y, -Data.MaxDropSpeed, Data.MaxDropSpeed);

			// Grab
			if (_CurrentVelocity.y <= 0f) {
				if (IsGrabing) {

					SetGravityScale(0f);

					// Check Able to Move
					if (GrabingPose == GrabPose.Top) {
						if (Data.StopMoveOnGrabHorizontalEdge) {
							if ((!_GrabHitDL.HasValue && _CurrentVelocity.x < 0f) ||
								(!_GrabHitUR.HasValue && _CurrentVelocity.x > 0f)
							) {
								_CurrentVelocity.x = 0f;
							}
						}
					} else {
						if (Data.StopMoveOnGrabTopEdge) {
							if (!_GrabHitUR.HasValue && _AimVelocity.y > 0) {
								_AimVelocity.y = 0f;
							}
						}
						if (Data.StopMoveOnGrabBottomEdge) {
							if (!_GrabHitDL.HasValue && _AimVelocity.y < 0) {
								_AimVelocity.y = 0f;
							}
						}
					}
					// Movement for Grab
					_CurrentVelocity = new Vector3(
						GrabingPose == GrabPose.Top ? _CurrentVelocity.x : Mathf.Clamp(
							_CurrentVelocity.x,
							IsFacingRight == (GrabingPose == GrabPose.Forward) ? 0f : float.MinValue,
							IsFacingRight == (GrabingPose == GrabPose.Forward) ? float.MaxValue : 0f
						),
						GrabingPose == GrabPose.Top ? Mathf.Max(_CurrentVelocity.y, 0f) :
							(AimVelocityYIsZero ? Mathf.Max(Rig.velocity.y, 0f) : (FixedMoveSpeed * _AimVelocity.y)),
						0f
					);
					// Fix Position for Grab
					if (Data.FixPositionOnGrab) {
						Vector3 v = LogicalRoot.position;
						Vector2 point = _GrabHitUR.HasValue ? _GrabHitUR.Value.point :
							_GrabHitDL.HasValue ? _GrabHitDL.Value.point : Vector2.zero;
						if (GrabingPose == GrabPose.Top) {
							v.y = point.y - Height;
						} else {
							v.x = (GrabingPose == GrabPose.Forward) == IsFacingRight ? point.x - Radius : point.x + Radius;
						}
						LogicalRoot.position = v;
					}
				} else {
					SetGravityScale(Data.DropGravity);
				}
			}

			// Dush
			if (IsDushing) {
				Vector3 newVelocity = (IsFacingRight ? Vector2.right : Vector2.left) * FixedDushSpeed;
				_CurrentVelocity.x = newVelocity.x;
				_CurrentVelocity.z = newVelocity.z;
			}

			// Move
			Rig.velocity = _CurrentVelocity;

			// Squat
			_ForceSquat = IsGrounded && GroundCheck(
				(Vector2)LogicalRoot.position + Vector2.up * GROUND_CHECK_GAP,
				Vector2.up, _ColliderBasicHeight - GROUND_CHECK_GAP, false
			);

			if (Data.SquatHeightScale != 1f) {
				Col.size = new Vector2(Col.size.x, IsSquating ? _ColliderBasicHeight * Data.SquatHeightScale : _ColliderBasicHeight);
				Col.offset = Vector2.up * (Height * 0.5f);
			}
		}



		private void PlatformUpdate () {
			// Move With  Platform
			if (_ContactingRig) {
				Rig.velocity += _ContactingRig.velocity;
			}
			// Crossing Platform
			if (_CrossingPlatform) {
				Bounds b0 = Col.bounds;
				Bounds b1 = _CrossingPlatform.bounds;
				b0.center = new Vector3(b0.center.x, b0.center.y, b1.center.z);
				if (!b0.Intersects(b1)) {
					Physics2D.IgnoreCollision(Col, _CrossingPlatform, false);
					_CrossingPlatform = null;
				}
			}
		}



		#endregion



		#region --- API ---



		// Move
		public void Move (float x, float y) {
            animator = GetComponentInChildren<Animator>();
            x = Mathf.Clamp(x, -1f, 1f);
			y = Mathf.Clamp(y, -1f, 1f);
			_AimVelocity.x = Mathf.Abs(x) > VELOCITY_GAP ? (x > 0f ? 1f : -1f) : 0f;
			_AimVelocity.y = Mathf.Abs(y) > VELOCITY_GAP ? (y > 0f ? 1f : -1f) : 0f;
			if (Mathf.Abs(x) > VELOCITY_GAP) {
				if (Data.RotateOnMove && (!IsGrabing || _GrabingPose == GrabPose.Top)) {
					Rotate(x > 0f);
				}
			}
			float t = Data.MoveDely <= 0 ? 1f : Mathf.Clamp01(Time.deltaTime / Data.MoveDely);

			_CurrentVelocity.x = Mathf.Lerp(_CurrentVelocity.x, x * FixedMoveSpeed, t);

          
        }



		public void Rotate (bool right) {
			_IsFacingRight = right;
			Vector3 v = RendererRoot.localScale;
			v.x = _BodyBasicScaleX * (right ? 1f : -1f);
			RendererRoot.localScale = v;
		}




		// Action
		public void Jump () {
			if (Data.JumpDely > 0 && IsGrounded) {
				Invoke("JumpInvoke", Data.JumpDely);
			} else {
				JumpInvoke();
			}
		}



		public void Dush () {
			if (Data.DushDely > 0) {
				Invoke("DushInvoke", Data.DushDely);
			} else {
				DushInvoke();
			}
		}



		public void BreakDush () {
			_LastDushTime = float.MinValue;
		}



		public void Squat (bool squat) {
			IsSquating = Data.AllowSquat && squat;
		}



		public bool TryDropPlatform () {
			if (Data.AllowDropThroughPlatform && _GroundHit.HasValue) {
				var col = _GroundHit.Value.collider;
				if (col && col.usedByEffector) {
					var eff = col.GetComponent<PlatformEffector2D>();
					if (eff) {
						if (_CrossingPlatform) {
							Physics2D.IgnoreCollision(Col, _CrossingPlatform, false);
						}
						Physics2D.IgnoreCollision(Col, col, true);
						_CrossingPlatform = col;
						LogicalRoot.position = LogicalRoot.position + Vector3.down * DROP_PLATFORM_GAP;
						return true;
					}
				}
			}
			return false;
		}



		public bool DropGrab () {
			return DropTopGrab() || DropSideGrab();
		}



		public bool DropTopGrab () {
			if (IsGrabing && GrabingPose == GrabPose.Top) {
				Vector3 v = LogicalRoot.position;
				v.y -= Data.GrabLengthTop + DROP_PLATFORM_GAP;
				LogicalRoot.position = v;
				return true;
			}
			return false;
		}



		public bool DropSideGrab () {
			if (IsGrabing && GrabingPose != GrabPose.Top) {
				Vector3 v = LogicalRoot.position;
				float delta = Data.GrabLength + GROUND_CHECK_GAP;
				v.x += IsFacingRight == (GrabingPose == GrabPose.Forward) ? -delta : delta;
				LogicalRoot.position = v;
				return true;
			}
			return false;
		}



		#endregion



		#region --- LGC ---



		protected void SetGravityScale (float scale) {
			Rig.gravityScale = scale;
		}



		// Action Invoke
		private void JumpInvoke () {
			if (_ForceSquat) {
				return;
			}
			if (Data.DropGrabOnJump) {
				if (DropTopGrab()) {
					return;
				} else {
					DropSideGrab();
				}
			}
			if (_JumpCount < Data.JumpCount) {
				SetGravityScale(Data.RiseGravity);
				_CurrentVelocity.y = FixedJumpSpeed;
				Rig.velocity = _CurrentVelocity;
				_LastJumpTime = Time.time;
				_JumpCount++;
				Event.OnJump.Invoke();
				if (Data.BreakDushOnJump) {
					BreakDush();
				}
			}
		}



		private void DushInvoke () {
			if (Time.time > _LastDushTime + Data.DushGap + Data.DushDuration && (IsGrounded || Data.AllowDushOnJump) && Mathf.Abs(DushSpeedMuti) > VELOCITY_GAP) {
				_LastDushTime = Time.time;
				Event.OnDush.Invoke();
			}
		}



		// Raycast Check
		private bool GroundCheck (Vector3 from, Vector2 dir, float length, bool allowEffecter = true) {
			RaycastHit2D? hit;
			return GroundCheck(from, dir, length, out hit, allowEffecter);
		}



		private bool GroundCheck (Vector3 from, Vector2 dir, float length, out RaycastHit2D? hit, bool allowEffecter = true) {
			bool isGrounded = false;
			hit = null;
			int len = Physics2D.RaycastNonAlloc(from, dir, c_GroundCheckHits, length, _GroundLayerInt);
			for (int i = 0; i < len; i++) {
				if (c_GroundCheckHits[i].transform != LogicalRoot &&
					IsSolidPlatform(c_GroundCheckHits[i].collider, allowEffecter)
				) {
					hit = c_GroundCheckHits[i];
					if (c_GroundCheckHits[i].collider != _CrossingPlatform) {
						isGrounded = true;
					}
					if (c_GroundCheckHits[i].rigidbody) {
						_ContactingRig = c_GroundCheckHits[i].rigidbody;
					}
				}
			}
			return isGrounded;
		}



		private RaycastHit2D? GrabCheck (Vector2 from, Vector2 dir, float length) {
			int len = Physics2D.RaycastNonAlloc(from, dir, c_GrabCheckHits, length, _GroundLayerInt);
			for (int i = 0; i < len; i++) {
				if (c_GrabCheckHits[i].transform != LogicalRoot &&
					IsGrabablePaltform(c_GrabCheckHits[i].collider) &&
					IsSolidPlatform(c_GrabCheckHits[i].collider, false)
				) {
					if (c_GrabCheckHits[i].rigidbody) {
						_ContactingRig = c_GrabCheckHits[i].rigidbody;
					}
					return c_GrabCheckHits[i];
				}
			}
			return null;
		}




		#endregion



		#region --- UTL ---



		private bool IsSolidPlatform (Collider2D col, bool allowEffecter = true) {
			return col && !col.isTrigger && (allowEffecter || !col.usedByEffector);
		}


		private bool IsGrabablePaltform (Collider2D col) {
			return col && !string.IsNullOrEmpty(Data.GrabableTag) && col.transform.CompareTag(Data.GrabableTag);
		}



		#endregion


	}
}