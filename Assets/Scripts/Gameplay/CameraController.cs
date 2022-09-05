using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _camera;
        [SerializeField] private float _rotateSpeed = .3f;
        [SerializeField] private float _zoomSpeed = .3f;
        [SerializeField] private float _minZoom = 0.5f;
        [SerializeField] private float _maxZoom = 3f;
        [SerializeField] private Vector3 _cameraOffset = new Vector3(0f, 3f, -5f);
        [SerializeField] private Vector3 _targetOffset = new Vector3(0f, 1f, 0f);

        private Vector3 _mousePosition;
        private float _zoom = 1f;
        
        private bool _isInitialized = false;

        private void LateUpdate()
        {
            if (PlayerCharacter.Local == null)
                return;

            if (PlayerCharacter.Local != null && !_isInitialized)
                Initialize();

            transform.position = PlayerCharacter.Local.transform.position + _targetOffset;

            if (Input.GetMouseButtonDown(2))
            {
                _mousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(2))
            {
                Vector2 delta = Input.mousePosition - _mousePosition;
                _mousePosition = Input.mousePosition;

                if (delta.x != 0f)
                    transform.Rotate(Vector3.up, delta.x * _rotateSpeed);

                if (delta.y != 0f)
                    _cameraOffset.y = Mathf.Clamp(_cameraOffset.y - delta.y * 0.01f, 1f, 5f);
            }
            else
            {
                if (Input.mouseScrollDelta.y != 0f)
                {
                    _zoom = Mathf.Clamp(_zoom -= Input.mouseScrollDelta.y * _zoomSpeed, _minZoom, _maxZoom);
                }
            }

            Vector3 offset = _cameraOffset;
            offset *= _zoom;

            _camera.transform.localPosition = offset;
            _camera.LookAt(transform.position);
        }

        private void Initialize()
        {
            _camera.transform.position = _cameraOffset;
            _camera.LookAt(transform.position);

            _isInitialized = true;
        }
    }
}


