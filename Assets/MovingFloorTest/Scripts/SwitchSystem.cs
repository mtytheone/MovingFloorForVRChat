// ================= License ====================
//
// SwitchSystem.cs
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
using VRC.Udon;

public class SwitchSystem : UdonSharpBehaviour
{
    [SerializeField] private Transform _teleportPoint;
    [SerializeField]  private UdonBehaviour _sitBehavior;
    [SerializeField] private UdonBehaviour _teleportBehavior;

    private bool _isEnable;
    private float _interval;

    private void Update()
    {
        // タイマー
        // 3秒後に全員リスポーンする
        if (_isEnable)
        {
            _interval += Time.deltaTime;

            if (_interval > 3)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(AllTeleport));
                _interval = 0;
                _isEnable = false;
            }
        }
    }

    public override void Interact()
    {
        // 全員椅子から降りる
        _sitBehavior.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ExitStation");
        // 全員テレポート床の動きを止める
        _teleportBehavior.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetPlayerMoving");
        _isEnable = true;
    }

    // テレポートするための関数
    public void AllTeleport()
    {
        Networking.LocalPlayer.TeleportTo(_teleportPoint.position, _teleportPoint.rotation);
    }
}