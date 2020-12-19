// ================= License ====================
//
// MovingCube_SetVelocity.cs
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

public class MovingCube_SetVelocity : UdonSharpBehaviour
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
    private Vector3 _previousPosition;
    private Vector3 _currentVelocity;
    private bool _isReverse;
    private bool _isPlayerAddForce;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _previousPosition = _rigidBody.position;
        _localPlayer = Networking.LocalPlayer;
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

        // 乗っているときは床の移動速度を計算してその分の力をプレイヤーに与える
        if (_isPlayerAddForce)
        {
            _currentVelocity = (_rigidBody.position - _previousPosition) / Time.deltaTime;
            _previousPosition = _rigidBody.position;
            _localPlayer.SetVelocity(_currentVelocity);
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        // 自分が乗ったら赤色に変える
        // 移動速度を変更する
        if (player == _localPlayer)
        {
            _isPlayerAddForce = true;
            _meshRenderer.material = _changedMaterial;
            player.SetRunSpeed(player.GetRunSpeed() * (10 * _speed));
            player.SetWalkSpeed(player.GetWalkSpeed() * (10 * _speed));
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        // 自分が降りたら元の色に戻す
        // 移動速度を元に戻す
        if (player == _localPlayer)
        {
            _isPlayerAddForce = false;
            _meshRenderer.material = _defaultMaterial;
            player.SetRunSpeed(player.GetRunSpeed() / (10 * _speed));
            player.SetWalkSpeed(player.GetWalkSpeed() / (10 * _speed));
        }
    }
}