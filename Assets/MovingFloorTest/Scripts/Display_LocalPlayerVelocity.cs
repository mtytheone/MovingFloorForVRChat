using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class Display_LocalPlayerVelocity : UdonSharpBehaviour
{
    [SerializeField] Text _xVelocity;
    [SerializeField] Text _yVelocity;
    [SerializeField] Text _zVelocity;

    private void FixedUpdate()
    {
        var localPlayer = Networking.LocalPlayer;
        var velocity = localPlayer.GetVelocity();
        _xVelocity.text = $"X: {velocity.x.ToString("F2")}";
        _yVelocity.text = $"Y: {velocity.y.ToString("F2")}";
        _zVelocity.text = $"Z: {velocity.z.ToString("F2")}";
    }
}