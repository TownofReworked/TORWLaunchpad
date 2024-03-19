using LaunchpadReloaded.Features;
using LaunchpadReloaded.Utilities;
using Reactor.Utilities.Attributes;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Components;

[RegisterInIl2Cpp]
public class GunComponent(IntPtr ptr) : MonoBehaviour(ptr)
{
    public virtual string Name => throw new NotImplementedException();
    public virtual LoadableAsset<Sprite> Sprite => throw new NotImplementedException();
    public virtual float Range => throw new NotImplementedException();
    public virtual float Cooldown => 2f;
    public virtual int DefaultBullets => 1;
    public int CurrentBullets { get; set; }
    public virtual Vector2 FlipOffset => new Vector2(-0.7f, 0);
    public virtual Vector2 NormalOffset => new Vector2(0.7f, 0);

    public PlayerControl Owner;
    private SpriteRenderer _rend;
    private float _timer;
    public Vector3 TargetPosition;

    private void Start()
    {
        _rend = gameObject.GetComponent<SpriteRenderer>();
        CurrentBullets = DefaultBullets;
    }

    public void FireBullet(Vector2 clampedMousePos)
    {
        GameObject bullet = new GameObject("Bullet");
        bullet.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        bullet.layer = LayerMask.NameToLayer("Objects");
        bullet.transform.position = transform.position + new Vector3(0.2f, 0, 0);

        SpriteRenderer rend = bullet.AddComponent<SpriteRenderer>();
        rend.sprite = LaunchpadAssets.BulletSprite.LoadAsset();

        BulletComponent bulletComp = bullet.AddComponent<BulletComponent>();
        bulletComp.Origin = transform.position;
        bulletComp.Direction = clampedMousePos;
    }

    private void FixedUpdate()
    {
        Vector3 clamped = Vector3.zero;
        bool flipX = Owner.cosmetics.FlipX;
        Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = flipX ? ((Vector2)transform.position - mouseScreenPosition) : (mouseScreenPosition - (Vector2)transform.position);
        clamped = new Vector2(Mathf.Clamp(direction.normalized.x, 1f, -1f), direction.normalized.y);

        if (Input.GetMouseButton(0))
        {
            FireBullet(mouseScreenPosition);
        }

        if (Owner.AmOwner)
        {
            _timer += Time.deltaTime;
            if (_timer > 0.5f)
            {
                _timer = 0;
                WeaponManager.RpcSyncPosition(Owner, clamped.x, clamped.y, clamped.z);
            }
            TargetPosition = clamped;
        }

        transform.right = Vector3.Lerp(transform.right, TargetPosition, Time.deltaTime * 5f);
        _rend.flipX = flipX;
        transform.localPosition = flipX ? FlipOffset : NormalOffset;
    }
}