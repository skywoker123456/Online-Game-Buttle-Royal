﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace CoverShooter
{

    [RequireComponent(typeof(CharacterMotor))]
    [RequireComponent(typeof(ThirdPersonController))]
    [RequireComponent(typeof(Actor))]
    public class MyAim : MonoBehaviour, ICharacterController
    {
        /// <summary>
        /// Input is ignored when a disabler is active.
        /// </summary>
        [Tooltip("Input is ignored when a disabler is active.")]
        public GameObject Disabler;

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
        // Use this for initialization

        public void UpdateAfterCamera()
        {
                    Update();
        }

        private void Awake()
        {
            _controller = GetComponent<ThirdPersonController>();
            _motor = GetComponent<CharacterMotor>();
            _actor = GetComponent<Actor>();
            _controller.WaitForUpdateCall = true;
            _inventory = GetComponent<CharacterInventory>();


        }
        void Start()
        {
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update() {

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

                          
                            groundPosition = targetActor.transform.position;

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
    }
}

