
using Random = UnityEngine.Random;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPerson : MonoBehaviour {

    Camera m_Camera;
    Vector2 m_Input;
    Vector3 m_MoveDir = Vector3.zero;
    [SerializeField] Transform playerTransform;

    private void Start(){
        m_Camera = Camera.main;
        MouseLook.Instance.Init(transform , m_Camera.transform);
    }

    private void Update(){
        RotateView();
    }

    private void FixedUpdate(){
        GetInput();
        MouseLook.Instance.UpdateCursorLock();
    }

    private void GetInput(){
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
    }

    private void RotateView(){
        MouseLook.Instance.LookRotation (playerTransform, m_Camera.transform);
    }

}
