using LaunchpadReloaded.API.Hud;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Translations;
using LaunchpadReloaded.Roles;
using LaunchpadReloaded.Utilities;
using Reactor.Utilities;
using System.Collections;
using UnityEngine;

namespace LaunchpadReloaded.Buttons;

public class ZoomButton : CustomActionButton
{
    public override TranslationStringNames Name => TranslationStringNames.CaptainZoom;

    public override float Cooldown => CaptainRole.ZoomCooldown.Value;

    public override float EffectDuration => CaptainRole.ZoomDuration.Value;

    public override int MaxUses => 0;

    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.ZoomButton;

    public static bool IsZoom { get; private set; }

    public override bool Enabled(RoleBehaviour role)
    {
        return role is CaptainRole;
    }

    public override bool CanUse() => !PlayerControl.LocalPlayer.Data.IsHacked();

    protected override void OnClick()
    {
        Coroutines.Start(ZoomOutCoroutine());
    }

    protected override void OnEffectEnd()
    {
        Coroutines.Start(ZoomInCoroutine());
    }

    private static IEnumerator ZoomOutCoroutine()
    {
        HudManager.Instance.ShadowQuad.gameObject.SetActive(false);
        IsZoom = true;
        for (var ft = Camera.main!.orthographicSize; ft < CaptainRole.ZoomDistance.Value; ft += 0.3f)
        {
            Camera.main.orthographicSize = MeetingHud.Instance ? 3f : ft;
            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
            foreach (var cam in Camera.allCameras) cam.orthographicSize = Camera.main.orthographicSize;
            yield return null;
        }

        foreach (var cam in Camera.allCameras) cam.orthographicSize = CaptainRole.ZoomDistance.Value;
        ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
    }

    private static IEnumerator ZoomInCoroutine()
    {
        for (var ft = Camera.main!.orthographicSize; ft > 3f; ft -= 0.3f)
        {
            Camera.main.orthographicSize = MeetingHud.Instance ? 3f : ft;
            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
            foreach (var cam in Camera.allCameras) cam.orthographicSize = Camera.main.orthographicSize;

            yield return null;
        }

        foreach (var cam in Camera.allCameras) cam.orthographicSize = 3f;
        HudManager.Instance.ShadowQuad.gameObject.SetActive(true);
        IsZoom = false;

        ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
    }
}