// ================= License ====================
//
// MovingCube_Teleport.cs
//
// Copyright (c) 2020 hatuxes
//
// Released under the MIT license.
// Check README.md when you use this script.
//
// ==============================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class MovingCube_Teleport : UdonSharpBehaviour
{
    [Header("General")]
    [SerializeField] private Rigidbody _rigidBody;
    [Header("Settings")]
    [SerializeField] private Transform _startPosition;
    [SerializeField] private Transform _endPosition;
    [SerializeField] private float _speed = 1.0f;
    [Header("Materials")]
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _changedMaterial;

    private MeshRenderer _meshRenderer;
    private VRCPlayerApi _localPlayer;
    private bool _isReverse;
    private bool _isPlayerMoving;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _localPlayer = Networking.LocalPlayer;
    }

    private void Update()
    {
        if (_isPlayerMoving)
        {
            _localPlayer.TeleportTo(transform.position, transform.rotation);

            // 移動で抜け出したい場合はコメントアウトを外す
            /*var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
            if (horizontal != 0 || vertical != 0)
            {
                _isPlayerMoving = false;
            }*/
        }
    }

    private void FixedUpdate()
    {
        // 床の移動
        if (_isReverse)
        {
            Vector3 toVector = Vector3.MoveTowards(transform.position, _startPosition.position, _speed * Time.deltaTime);
            
            _rigidBody.MovePosition(toVector);

            if (Vector3.Distance(transform.position, _startPosition.position) < 0.1f)
            {
                _isReverse = false;
            }
        }
        else
        {
            Vector3 toVector = Vector3.MoveTowards(transform.position, _endPosition.position, _speed * Time.deltaTime);
            
            _rigidBody.MovePosition(toVector);
            
            if (Vector3.Distance(transform.position, _endPosition.position) < 0.1f)
            {
                _isReverse = true;
            }
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        // 自分が乗ったら赤色に変える
        // テレポート開始
        if (player == _localPlayer)
        {
            _isPlayerMoving = true;
            _meshRenderer.material = _changedMaterial;
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        // 自分が降りたら元の色に戻す
        if (player == _localPlayer)
        {
            _isPlayerMoving = false;
            _meshRenderer.material = _defaultMaterial;
        }
    }

    // 強制的にテレポートを終えるpublic関数
    public void ResetPlayerMoving()
    {
        _isPlayerMoving = false;
    }
}