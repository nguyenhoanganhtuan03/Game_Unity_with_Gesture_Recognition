using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController _controller;
    private PlayerInput _playerInput;
    private InputAction _m_Jump;
    private InputAction _m_Left;
    private InputAction _m_Right;
    private InputAction _m_Crounch;
    private float _laneDistance;
    private float _jumpForce;
    public float _gravity;
    private Vector3 direction;
    private float _startSpeed;
    private float _forwardSpeed;
    private float _trackedDistance;
    private int _currentLane = 1;
    private float _initialHeight;
    public int gestureIndex;
    private bool isSitting = false;
    private bool isSlowingDown = false; 
    private float slowDownFactor = 0.5f; 
    private float originalSpeed; 

    private float trackedDistance = 0;

    public int CurrentLane
    {
        get { return _currentLane; }
        set { if (value < 0) _currentLane = 0; else if (value > 2) _currentLane = 2; else _currentLane = value; }
    }

    public delegate void OnTileGenerated();
    public static event OnTileGenerated onTileGenerated;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _m_Jump = _playerInput.actions["Jump"];
        _m_Left = _playerInput.actions["Left"];
        _m_Right = _playerInput.actions["Right"];
        _m_Crounch = _playerInput.actions["Crounch"];
        isSitting = false;
        _initialHeight = _controller.height; ;

        _laneDistance = 2.5f;
        _jumpForce = 14f;
        _gravity = 30f;

        switch (LevelSelection.currentLevel)
        {
            case LevelSelector.Easy:
                _startSpeed = 5;
                break;
            case LevelSelector.Medium:
                _startSpeed = 10;
                break;
            case LevelSelector.Hard:
                _startSpeed = 15;
                break;
            case LevelSelector.Infinite:
                _startSpeed = 8;
                break;
        }
    }

    public void ControlCharacter(int gestureIndex)
    {
        Debug.Log("Gesture Signal: " + gestureIndex);

        // Xử lý tín hiệu sang trái
        if (gestureIndex == 0)
        {
            CurrentLane--;
            Debug.Log("Nhân vật sang trái");
        }

        // Xử lý tín hiệu sang phải
        if (gestureIndex == 1)
        {
            CurrentLane++; 
            Debug.Log("Nhân vật sang phải");
        }

        // Xử lý tín hiệu ngồi
        if (gestureIndex == 2 && !isSitting)
        {
            isSitting = true;
            Debug.Log("Nhân vật ngồi");
        }

        // Xử lý tín hiệu nhảy
        if (gestureIndex == 3)
        {
            PlayerJump();
            Debug.Log("Nhân vật nhảy");
        }

        // Xử lý tín hiệu other aciton
        if (gestureIndex == 5)
        {
            Debug.Log("Không có hành động");
        }

        // Xử lý tín hiệu stop
        if (gestureIndex == 4)
        {
            if (isSitting)
            {
                isSitting = false;
                if (!isSlowingDown)
                {
                    isSlowingDown = true;
                    originalSpeed = _forwardSpeed;
                    _forwardSpeed *= slowDownFactor;
                    Debug.Log("Nhân vật giảm tốc độ và thoát trạng thái ngồi");
                }
            }
            else
            {
                if (!isSlowingDown)
                {
                    isSlowingDown = true;
                    originalSpeed = _forwardSpeed;
                    _forwardSpeed *= slowDownFactor;
                    Debug.Log("Nhân vật giảm tốc độ");
                }
            }
        }
        else if (isSlowingDown)
        {
            // Khi không còn tín hiệu stop, tốc độ trở lại bình thường
            isSlowingDown = false;
            _forwardSpeed = originalSpeed;
            Debug.Log("Nhân vật trở lại tốc độ bình thường");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelSelection.currentLevel == LevelSelector.Infinite)
        {
            _forwardSpeed = isSlowingDown ? originalSpeed * slowDownFactor : (float)(_startSpeed + (int)(trackedDistance / 100) * 0.5);
            trackedDistance += _forwardSpeed * Time.deltaTime;
        }
        else
        {
            _forwardSpeed = isSlowingDown ? originalSpeed * slowDownFactor : _startSpeed;
        }

        direction.z = _forwardSpeed;

        // Kiểm tra trạng thái ngồi
        if (isSitting)
        {
            _controller.height = 0.5f * _initialHeight; 
        }
        else
        {
            _controller.height = _initialHeight; 
        }

        if (_controller.isGrounded)
        {
            // Kiểm tra jump
            if (_m_Jump != null && _m_Jump.WasPressedThisFrame()) 
            {
                PlayerJump();
            }

            // Kiểm tra crouch
            if (_m_Crounch != null && _m_Crounch.IsPressed())
            {
                _controller.height = 0.5f * _initialHeight;
            }
            else
            {
                _controller.height = Mathf.Lerp(_controller.height, _initialHeight, 5.0f * Time.deltaTime);
            }
        }
        else
        {
            direction.y -= _gravity * Time.deltaTime;
        }

        // Di chuyển trái
        if (_m_Left != null && _m_Left.WasPressedThisFrame())  
        {
            CurrentLane--;
        }

        // Di chuyển phải
        if (_m_Right != null && _m_Right.WasPressedThisFrame()) 
        {
            CurrentLane++;
        }

        Vector3 targetPosition = (transform.position.z * transform.forward) + (transform.position.y * transform.up);

        switch (CurrentLane)
        {
            case 0:
                targetPosition += Vector3.left * _laneDistance;
                break;
            case 2:
                targetPosition += Vector3.right * _laneDistance;
                break;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, 60 * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        _controller.Move(direction * Time.fixedDeltaTime);
    }

    void attributeScheme()
    {
        _playerInput.SwitchCurrentControlScheme(SettingsUI.instance.GetCurrentControlScheme(), Keyboard.current);
    }

    private void PlayerJump()
    {
        direction.y = _jumpForce;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.CompareTag("Obstacle"))
        {
            if (GameManager.restEsquive > 0)
            {
                GameManager.restEsquive--;
                StartCoroutine(Flasher());
                hit.collider.enabled = false;
            }
            else
            {
                GameManager.playerIsDied = true;
            }
        }

        if (hit.transform.CompareTag("Finish"))
        {
            GameManager.playerReachedFinishLine = true;
        }
    }

    IEnumerator Flasher()
    {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

        Color normalColor = meshRenderer.material.color;
        Color collideColor = Color.clear;

        for (int i = 0; i < 5; i++)
        {
            meshRenderer.material.color = collideColor;
            yield return new WaitForSeconds(.1f);
            meshRenderer.material.color = normalColor;
            yield return new WaitForSeconds(.1f);
        }
    }
}