using Reactor.Utilities.Attributes;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Components;
[RegisterInIl2Cpp]
public class BulletComponent(IntPtr ptr) : MonoBehaviour(ptr)
{
    public float Speed = 20;
    public Vector2 Origin;
    public Vector2 Direction;
    public Vector2 Velocity => this.Direction * this.Speed;

    private Vector2 _direction;

    private BoxCollider2D _boxCollider2D;
    private Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        this._boxCollider2D = this.gameObject.AddComponent<BoxCollider2D>();
        this._boxCollider2D.isTrigger = true;

        this._rigidbody2D = this.gameObject.AddComponent<Rigidbody2D>();
        this._rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Start()
    {
        this.gameObject.transform.position = new Vector3(this.Origin.x, this.Origin.y, -500f);

        var angle = Mathf.Atan2(this.Velocity.y, this.Velocity.x) * 180 / MathF.PI;
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }


    private void Update()
    {
        this.transform.position += (Vector3)(this.Direction * this.Speed * Time.deltaTime);

        var angle = Mathf.Atan2(this.Velocity.y, this.Velocity.x) * 180 / MathF.PI;
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}