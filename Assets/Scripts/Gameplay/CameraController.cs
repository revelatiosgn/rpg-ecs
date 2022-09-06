using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPGGame.Gameplay
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineFreeLook _cmCamera;
        [SerializeField] private float _scrollSens = 0.001f;

        private void Start()
        {
            _cmCamera.m_YAxis.Value = 1f;
        }

        private void LateUpdate()
        {
            if (PlayerCharacter.Local == null)
                return;

            transform.position = PlayerCharacter.Local.transform.position;

            _cmCamera.m_XAxis.m_MaxSpeed = 0f;

            Mouse mouse = Mouse.current;
			if (mouse != null && mouse.middleButton.isPressed)
			{
                _cmCamera.m_XAxis.m_MaxSpeed = 300f;
            }

            if (mouse != null)
            {
                Vector2 scroll = mouse.scroll.ReadValue();
                _cmCamera.m_YAxis.Value = Mathf.Clamp01(_cmCamera.m_YAxis.Value - scroll.y * _scrollSens);
            }
        }
    }
}


