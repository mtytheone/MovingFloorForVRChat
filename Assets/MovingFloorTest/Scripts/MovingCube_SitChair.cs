// ================= License ====================
//
// MovingCube_SitChair.cs
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

public class MovingCube_SitChair : UdonSharpBehaviour
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
    private BoxCollider _boxCollider;
    private VRCPlayerApi _localPlayer;
    private VRCStation _station;
    private bool _isReverse;
    private bool _isDelay;
    private float _timer;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _boxCollider = GetComponent<BoxCollider>();
        _localPlayer = Networking.LocalPlayer;
        _station = (VRCStation)GetComponent(typeof(VRCStation));
    }

    private void Update()
    {
        // 立ち上がった後少ししてからトリガーを戻す
        if (_isDelay)
        {
            _timer += Time.deltaTime;
            if (_timer > 3.0f)
            {
                _boxCollider.enabled = true;
                _isDelay = false;
            }
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
        // 座らせる
        if (player == _localPlayer)
        {
            _meshRenderer.material = _changedMaterial;
            player.UseAttachedStation();
            _boxCollider.enabled = false;
        }
    }

    public override void OnStationExited(VRCPlayerApi player)
    {
        // 自分が降りたら元の色に戻す
        if (player == _localPlayer)
        {
            _meshRenderer.material = _defaultMaterial;
            _isDelay = true;
        }
    }

    public void ExitStation()
    {
        _station.ExitStation(_localPlayer);
    }
}