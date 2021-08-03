using UnityEngine;
using UnityEngine.EventSystems;


namespace CoverShooter
{
    /// <summary>
    /// Takes player input and transform it to commands to ThirdPersonController.

    [RequireComponent(typeof(CharacterMotor))]
    [RequireComponent(typeof(ThirdPersonController))]
    [RequireComponent(typeof(Actor))]
    public class TopDownInput : MonoBehaviour, ICharacterController
    {

        /// <summary>
        /// Input is ignored when a disabler is active.
        /// </summary>
        [Tooltip("Input is ignored when a disabler is active.")]
        public GameObject Disabler;
        public AudioClip GiveBullets;
        /// <summary>
        /// Marker that is placed on the ground to mark the target the character is aiming at.
        /// </summary>
        [Tooltip("Marker that is placed on the ground to mark the target the character is aiming at.")]
        public GameObject Marker;

        /// <summary>
        /// Should the character walk by default and run, or run by default and sprint when needed.
        /// </summary>
        [Tooltip("Should the character walk by default and run, or run by default and sprint when needed.")]
        public bool WalkByDefault = false;

        /// <summary>
        /// Should the character sprint towards the target instead of relative screen up direction.
        /// </summary>
        [Tooltip("Should the character sprint towards the target instead of relative screen up direction.")]
        public bool SprintTowardsTarget = false;

        /// <summary>
        /// Marker should be enabled and disabled together with this component.
        /// </summary>
        [Tooltip("Marker should be enabled and disabled together with this component.")]
        public bool ManageMarkerVisibility = true;

        /// <summary>
        /// Will the marker be constantly displayed on the ground.
        /// </summary>
        [Tooltip("Will the marker be constantly displayed on the ground.")]
        public bool MarkerAlwaysOnGround = true;

        /// <summary>
        /// Height off the ground the marker is displayed at.
        /// </summary>
        [Tooltip("Height off the ground the marker is displayed at.")]
        public float GroundLift = 0.218f;
        [Tooltip("Maximum time in seconds to wait for a second tap to active rolling.")]
        public float DoubleTapDelay = 0.3f;
        private Actor _actor;
        private CharacterMotor _motor;
        private ThirdPersonController _controller;
        private CharacterInventory _inventory;
        private bool _isAimingFriendly = false;
        private bool _isFireDown;
        private bool _isZoomDown;
        private bool _isSprinting;

        private float _angle;

        private float[] _snapWork = new float[_snaps.Length];
        private static float[] _snaps = new float[] { 0, -45, 45, -90, 90, 135, -135, 180 };
        private float _timeW;
        private float _timeA;
        private float _timeS;
        private float _timeD;
        public Transform crosshairs;
        private float _leftMoveIntensity = 1;
        private float _rightMoveIntensity = 1;
        private float _backMoveIntensity = 1;
        private float _frontMoveIntensity = 1;
        public int get;

        public void UpdateAfterCamera()
        {
            UpdateTarget();
        }

        private void Awake()
        {
            _controller = GetComponent<ThirdPersonController>();
            _motor = GetComponent<CharacterMotor>();
            _actor = GetComponent<Actor>();
            _controller.WaitForUpdateCall = true; 
            _inventory = GetComponent<CharacterInventory>();
            Cursor.visible = false;

        }
        void Start()
        {
            
        }
        
        private void OnDisable()
        {
            if (Marker != null && ManageMarkerVisibility)
                Marker.SetActive(false);
        }

        private void Update()
        {
           

            if (Disabler != null && Disabler.activeSelf)
                return;

            if (Marker != null && ManageMarkerVisibility)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    if (Marker.activeSelf)
                        Marker.SetActive(false);
                }
                else if (!Marker.activeSelf)
                    Marker.SetActive(true);
            }

            var camera = Camera.main;
            var updateAfterCamera = false;

            if (camera != null)
            {
                var comp = camera.GetComponent<CharacterCamera>();

                if (comp != null)
                {
                    updateAfterCamera = true;
                    comp.DeferUpdate(this);
                }
            }

            if (!updateAfterCamera)
                UpdateAfterCamera();





            //        _controller.LookAt(point);
            //         crosshairs.transform.position = point;
            //    _controller.AimTargetInput = lookPosition;



                             UpdateTarget();
            
            UpdateMovement();
          //  UpdateWeapons();
            UpdateReload();
            UpdateRolling();
            UpdateAttack();

            UpdateCrouching();
            UpdateClimbing();



            _controller.ManualUpdate();
        }

        protected virtual void UpdateClimbing()
        {
            if (Input.GetButtonDown("Climb"))
            {
                var direction = Input.GetAxis("Horizontal") * Vector3.right +
                    Input.GetAxis("Vertical") * Vector3.forward;

                if (direction.magnitude > float.Epsilon)
                {
                    direction = Quaternion.Euler(0, aimAngle, 0) * direction.normalized;

                    var cover = _motor.GetClimbableInDirection(direction);

                    if (cover != null)
                        _controller.InputClimbOrVault(cover);
                }
            }
        }


        protected virtual void UpdateWeapons()
        {
            if (Input.GetKey(KeyCode.Alpha1)) { _motor.InputCancelGrenade(); inputWeapon(0); }
            if (Input.GetKey(KeyCode.Alpha2)) { _motor.InputCancelGrenade(); inputWeapon(1); }
            if (Input.GetKey(KeyCode.Alpha3)) { _motor.InputCancelGrenade(); inputWeapon(2); }
            if (Input.GetKey(KeyCode.Alpha4)) { _motor.InputCancelGrenade(); inputWeapon(3); }
            if (Input.GetKey(KeyCode.Alpha5)) { _motor.InputCancelGrenade(); inputWeapon(4); }
            if (Input.GetKey(KeyCode.Alpha6)) { _motor.InputCancelGrenade(); inputWeapon(5); }
            if (Input.GetKey(KeyCode.Alpha7)) { _motor.InputCancelGrenade(); inputWeapon(6); }
            if (Input.GetKey(KeyCode.Alpha8)) { _motor.InputCancelGrenade(); inputWeapon(7); }
            if (Input.GetKey(KeyCode.Alpha9)) { _motor.InputCancelGrenade(); inputWeapon(8); }
            if (Input.GetKey(KeyCode.Alpha0)) { _motor.InputCancelGrenade(); inputWeapon(9); }


           
        }


        private int currentWeapon
		{
			get
			{
				if (_inventory == null || !_motor.IsEquipped)
					return 0;

				for (int i = 0; i < _inventory.Weapons.Length; i++)
					if (_inventory.Weapons[i].IsTheSame(ref _motor.Weapon))
						return i + 1;

				return 0;
			}
		}

		private void inputWeapon(int index)
		{
			if (_inventory == null && index > 0)
				return;

			if (index <= 0 || (_inventory != null && index > _inventory.Weapons.Length))
				_controller.InputUnequip();
			else if (_inventory != null && index <= _inventory.Weapons.Length)
				_controller.InputEquip(_inventory.Weapons[index - 1]);
		}


        protected virtual void UpdateMovement()
        {
            var movement = new CharacterMovement();

            var direction = Input.GetAxis("Horizontal") * Vector3.right +
                            Input.GetAxis("Vertical") * Vector3.forward;

            if (WalkByDefault)
            {
                _isSprinting = false;
                movement.Magnitude = 0.5f;

                if (!_isZoomDown && Input.GetButton("Run"))
                    movement.Magnitude = 1.0f;
            }
            else
            {
                movement.Magnitude = 1.0f;

                if (_isZoomDown)
                {
                    _isSprinting = false;
                    movement.Magnitude = 0.5f;
                }
                else
                {
                    _isSprinting = Input.GetButton("Run");

                    if (_isSprinting)
                        movement.Magnitude = 1.0f;
                }
            }

            var camera = Camera.main;
            var lookAngle = 0f;

            if (camera == null || (_isSprinting && SprintTowardsTarget))
                lookAngle = Util.HorizontalAngle(_controller.AimTargetInput - transform.position);
            else
                lookAngle = Util.HorizontalAngle(camera.transform.forward);

            if (direction.magnitude > float.Epsilon)
                movement.Direction = Quaternion.Euler(0, lookAngle, 0) * direction.normalized;

            _controller.MovementInput = movement;
        }
        void OnTriggerEnter(Collider other)
        {

            if (other.tag == "Player" && other.gameObject.layer == 10 )
            {
                if (Input.GetButtonDown("GiveBullet") && GetComponent<CoverShooter.CharacterMotor>().EquippedWeapon.Gun.GetComponent<CoverShooter.Gun>().BulletInventory > get)
                {
                    other.GetComponent<CoverShooter.CharacterMotor>().EquippedWeapon.Gun.GetComponent<CoverShooter.Gun>().BulletInventory += get;
                    GetComponent<CoverShooter.CharacterMotor>().EquippedWeapon.Gun.GetComponent<CoverShooter.Gun>().BulletInventory -= get;                  
                    AudioSource.PlayClipAtPoint(GiveBullets, transform.position);
                } 
            }
        }
        protected virtual void UpdateAttack()
		{
			if (Input.GetButtonDown("Fire"))
				_controller.FireInput = true;

			if (Input.GetButtonUp("Fire"))
				_controller.FireInput = false;

			if (Input.GetButtonDown("Melee"))
				_controller.InputMelee();

			if (Input.GetButtonDown("Zoom"))
				_controller.ZoomInput = true;

			if (Input.GetButtonUp("Zoom"))
				_controller.ZoomInput = false;

			if (Input.GetButtonDown("Block"))
				_controller.BlockInput = true;

			if (Input.GetButtonUp("Block"))
				_controller.BlockInput = false;

			if (_controller.IsZooming)
			{
				if (Input.GetButtonDown("Scope"))
					_controller.ScopeInput = !_controller.ScopeInput;
			}
			else
				_controller.ScopeInput = false;
		}
        protected virtual void UpdateFire()
        {
            if (Input.GetButtonDown("Fire") && !EventSystem.current.IsPointerOverGameObject())
                _isFireDown = true;

            if (Input.GetButtonUp("Fire"))
                _isFireDown = false;

            if (Input.GetButtonDown("Zoom") && !EventSystem.current.IsPointerOverGameObject())
                _isZoomDown = true;

			if (Input.GetButtonUp("Block"))
				_controller.BlockInput = false;
			
            if (Input.GetButtonDown("Melee"))
                _motor.InputMelee();

            if (_isFireDown && !_isAimingFriendly)
                _controller.FireInput = true;
            else
                _controller.FireInput = false;

            _controller.ZoomInput = _isZoomDown;
        }
		protected virtual void UpdateCrouching()
		{
			if (Input.GetButton("Crouch"))
				_controller.InputCrouch();
		}
		protected virtual void UpdateReload()
		{
			if (Input.GetButton("Reload"))
				_controller.InputReload();
		}

		protected virtual void UpdateRolling()
		{
			if (_timeW > 0) _timeW -= Time.deltaTime;
			if (_timeA > 0) _timeA -= Time.deltaTime;
			if (_timeS > 0) _timeS -= Time.deltaTime;
			if (_timeD > 0) _timeD -= Time.deltaTime;

			if (Input.GetButtonDown("RollForward"))
			{
				if (_timeW > float.Epsilon)
				{
					var cover = _motor.GetClimbambleInDirection(aimAngle);

					if (cover != null)
						_controller.InputClimbOrVault(cover);
					else
						roll(Vector3.forward);
				}
				else
					_timeW = DoubleTapDelay;
			}

			if (Input.GetButtonDown("RollLeft"))
			{
				if (_timeA > float.Epsilon)
				{
					var cover = _motor.GetClimbambleInDirection(aimAngle - 90);

					if (cover != null)
						_controller.InputClimbOrVault(cover);
					else
						roll(-Vector3.right);
				}
				else
					_timeA = DoubleTapDelay;
			}

			if (Input.GetButtonDown("RollBackward"))
			{
				if (_timeS > float.Epsilon)
				{
					var cover = _motor.GetClimbambleInDirection(aimAngle + 180);

					if (cover != null)
						_controller.InputClimbOrVault(cover);
					else
						roll(-Vector3.forward);
				}
				else
					_timeS = DoubleTapDelay;
			}

			if (Input.GetButtonDown("RollRight"))
			{
				if (_timeD > float.Epsilon)
				{
					var cover = _motor.GetClimbambleInDirection(aimAngle + 90);

					if (cover != null)
						_controller.InputClimbOrVault(cover);
					else
						roll(Vector3.right);
				}
				else
					_timeD = DoubleTapDelay;
			}
		}

        protected virtual void UpdateTarget()
        {
            if (_controller == null || (!_isFireDown && EventSystem.current.IsPointerOverGameObject()))
                return;

            var camera = Camera.main;
            if (camera == null) return;

            var mouse = Input.mousePosition;
            mouse.z = camera.nearClipPlane;

            var near = camera.ScreenToWorldPoint(mouse);

            mouse.z = camera.farClipPlane;
            var far = camera.ScreenToWorldPoint(mouse);

            Vector3 lookNormal;
          var lookPosition = Util.GetClosestHitIgnoreSide(near, far, 1, _actor.Side, out lookNormal);
            var groundPosition = lookPosition;

           var targetActor = AIUtil.FindClosestActor(lookPosition, 0.7f, _actor);
           
       //                if (targetActor != null  0.7f, _actor)
       //                 groundPosition = targetActor.transform.position;

            var ground = _actor.transform.position.y + 1.5f;

            if (_motor.ActiveWeapon.Gun != null)
                ground = _motor.GunOrigin.y;

           var displayMarkerOnGround = MarkerAlwaysOnGround || targetActor != null;
           

                       if (targetActor != null && targetActor.Side != _actor.Side)
                           lookPosition.y = Vector3.Lerp(targetActor.transform.position, targetActor.TopPosition, 0.75f).y;
                      else if (lookPosition.y > _actor.transform.position.y - 0.5f && lookPosition.y <= ground)
            {
                var plane = new Plane(Vector3.up, -ground);
                var direction = (far - near).normalized;
                var ray = new Ray(near, direction);

                float enter;
                if (plane.Raycast(ray, out enter))
                {
                    lookPosition = near + direction * enter;
                    displayMarkerOnGround = MarkerAlwaysOnGround;
               }
            }

            if (Marker != null)
            {
               Vector3 up;

                if (displayMarkerOnGround)
                {
                    up = targetActor == null ? lookNormal : Vector3.up;
                    Marker.transform.position = groundPosition + GroundLift * up;
                }
                else
                {
                    Marker.transform.position = lookPosition;
                    up = -_motor.GunDirection;
                }

               Vector3 right;
 
             if (Vector3.Distance(up, Vector3.forward) > 0.01f)
                 right = Vector3.right;
             else
                    right = Vector3.Cross(up, Vector3.forward);

               var forward = Vector3.Cross(right, up);

                Marker.transform.LookAt(Marker.transform.position + forward, up);
            }

            {
                var axis = Util.HorizontalAngle(camera.transform.forward);
                var top = _actor.transform.position + 1.5f * Vector3.up;
                var vector = lookPosition - top;
                vector.y = 0;

                var angle = Util.HorizontalAngle(vector);

                var distance = vector.magnitude;

                if (distance < 2 && distance > float.Epsilon)
                {
                    var height = lookPosition.y;

                    if (distance < 1)
                        Util.LerpAngle(ref _angle, angle, distance * 2);
                    else
                        _angle = angle;

                    lookPosition = top + Util.HorizontalVector(_angle) * 2;
                    lookPosition.y = height;
                }
                else
               _angle = angle;
            }

            {
                var axis = Util.HorizontalAngle(camera.transform.forward);

               for (int i = 0; i < _snaps.Length; i++)
                  _snapWork[i] = Mathf.Abs(Mathf.DeltaAngle(axis + _snaps[i], _angle));

                var angle = axis;

                for (int i = 0; i < _snaps.Length; i++)
                {
                    var isOk = true;

                    for (int j = i + 1; j < _snaps.Length; j++)
                        if (_snapWork[j] < _snapWork[i])
                        {
                            isOk = false;
                            break;
                        }

                    if (isOk)
                    {
                        angle = axis + _snaps[i];
                        break;
                    }
                }

                _controller.BodyTargetInput = _actor.transform.position + Util.HorizontalVector(angle) * 10;
            }
           
            _controller.AimTargetInput = lookPosition;

            if (!_isSprinting && Mathf.Abs(Mathf.DeltaAngle(Util.HorizontalAngle(_motor.transform.forward), Util.HorizontalAngle(_controller.BodyTargetInput - _motor.transform.position))) > 90)
                _motor.InputPossibleImmediateTurn();
        }
		private Vector3 getMovementDirection(Vector3 local)
		{
			var forward = _controller.BodyTargetInput - transform.position;
			forward.y = 0;
			forward.Normalize();

			float angle;

			if (_motor.IsInCover)
			{
				angle = Util.HorizontalAngle(forward);

				if (_motor.Cover.IsLeft(angle, 45))
					angle = Util.HorizontalAngle(_motor.Cover.Left);
				else if (_motor.Cover.IsRight(angle, 45))
					angle = Util.HorizontalAngle(_motor.Cover.Right);
				else if (_motor.Cover.IsBack(angle, 45))
					angle = Util.HorizontalAngle(-_motor.Cover.Forward);
				else
					angle = Util.HorizontalAngle(_motor.Cover.Forward);

				forward = Util.HorizontalVector(angle);
			}
			else
				angle = Util.HorizontalAngle(forward);

			var right = Vector3.Cross(Vector3.up, forward);

			Util.Lerp(ref _leftMoveIntensity, _motor.IsFreeToMove(-right) ? 1.0f : 0.0f, 4);
			Util.Lerp(ref _rightMoveIntensity, _motor.IsFreeToMove(right) ? 1.0f : 0.0f, 4);
			Util.Lerp(ref _backMoveIntensity, _motor.IsFreeToMove(-forward) ? 1.0f : 0.0f, 4);
			Util.Lerp(ref _frontMoveIntensity, _motor.IsFreeToMove(forward) ? 1.0f : 0.0f, 4);

			if (local.x < -float.Epsilon) local.x *= _leftMoveIntensity;
			if (local.x > float.Epsilon) local.x *= _rightMoveIntensity;
			if (local.z < -float.Epsilon) local.z *= _backMoveIntensity;
			if (local.z > float.Epsilon) local.z *= _frontMoveIntensity;

			return Quaternion.Euler(0, angle, 0) * local;
		}

		private void roll(Vector3 local)
		{
			var direction = getMovementDirection(local);

			if (direction.sqrMagnitude > float.Epsilon)
				_controller.InputRoll(Util.HorizontalAngle(direction));
		}
		private float aimAngle
		{
			get { return Util.HorizontalAngle(_controller.BodyTargetInput - transform.position); }
		}
    }

}