// ================= License ====================
//
// MovingCube_ColliderOnly.cs
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

public class MovingCube_ColliderOnly : UdonSharpBehaviour
{
    [Header("General")]
    [SerializeField] private Rigidbody _rigidBody;
    [Header("Settings")]
    [SerializeField] private BoxCollider[] _colliders;
    [SerializeField] private Transform _startPosition;
    [SerializeField] private Transform _endPosition;
    [SerializeField] private float _speed = 1.0f;
    [Header("Materials")]
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _changedMaterial;

    private MeshRenderer _meshRenderer;
    private VRCPlayerApi _localPlayer;
    private bool _isReverse;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
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
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        // 自分が乗ったら赤色に変える
        // コライダーを出す
        if (player == _localPlayer)
        {
            _meshRenderer.material = _changedMaterial;
            SetCollider(true);
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        // 自分が降りたら元の色に戻す
        // コライダーを消す
        if (player == _localPlayer)
        {
            _meshRenderer.material = _defaultMaterial;
            SetCollider(false);
        }
    }

    // コライダーの出し入れをする関数
    private void SetCollider(bool isActive)
    {
        for (int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].enabled = isActive;
        }
    }
}