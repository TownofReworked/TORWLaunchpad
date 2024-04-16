using Il2CppSystem;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Translations;
using LaunchpadReloaded.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace LaunchpadReloaded.API.Hud;

public abstract class CustomActionButton
{
    public abstract TranslationStringNames Name { get; }

    public abstract float Cooldown { get; }

    public abstract float EffectDuration { get; }

    public abstract int MaxUses { get; }

    public abstract LoadableAsset<Sprite> Sprite { get; }

    public bool HasEffect => EffectDuration > 0;

    public bool LimitedUses => MaxUses > 0;

    protected bool EffectActive;

    protected float Timer;

    protected int UsesLeft;

    protected ActionButton Button;

    protected DeadBody DeadBodyTarget;

    public void CreateButton(Transform parent)
    {
        if (Button)
        {
            return;
        }

        UsesLeft = MaxUses;
        Timer = 0;
        EffectActive = false;

        Button = GameObject.Instantiate(HudManager.Instance.AbilityButton, parent);
        Button.name = Name + "Button";
        Button.OverrideText(DestroyableSingleton<TranslationController>.Instance.GetString((StringNames)Name, Array.Empty<Il2CppSystem.Object>()));

        Button.graphic.sprite = Sprite.LoadAsset();

        Button.SetUsesRemaining(MaxUses);
        if (MaxUses <= 0)
        {
            Button.SetInfiniteUses();
        }

        var pb = Button.GetComponent<PassiveButton>();
        pb.OnClick.RemoveAllListeners();
        pb.OnClick.AddListener((UnityAction)ClickHandler);
    }

    public void OverrideSprite(Sprite sprite)
    {
        Button.graphic.sprite = sprite;
    }

    public void OverrideName(TranslationStringNames name)
    {
        Button.OverrideText(DestroyableSingleton<TranslationController>.Instance.GetString((StringNames)name, Array.Empty<Il2CppSystem.Object>()));
    }

    protected virtual void FixedUpdate(PlayerControl playerControl) { }

    protected abstract void OnClick();

    public abstract bool Enabled(RoleBehaviour role);

    protected virtual void OnEffectEnd() { }

    private bool CanUseHandler()
    {
        return Timer <= 0 && !EffectActive && (!LimitedUses || UsesLeft > 0) && CanUse();
    }

    public virtual bool CanUse()
    {
        return true;
    }

    public virtual void SetActive(bool visible, RoleBehaviour role)
    {
        Button.ToggleVisible(visible && Enabled(role));
    }

    private void ClickHandler()
    {
        if (!CanUseHandler())
        {
            return;
        }

        if (LimitedUses)
        {
            UsesLeft--;
            Button.SetUsesRemaining(UsesLeft);
        }

        OnClick();
        Button.SetDisabled();
        if (HasEffect)
        {
            EffectActive = true;
            Timer = EffectDuration;
        }
        else
        {
            Timer = Cooldown;
        }
    }

    public void UpdateHandler(PlayerControl playerControl)
    {
        if (Timer >= 0)
        {
            Timer -= Time.deltaTime;
        }
        else
        {
            if (HasEffect && EffectActive)
            {
                EffectEndHandler();
            }
        }

        if (CanUseHandler())
        {
            Button.SetEnabled();
        }
        else
        {
            Button.SetDisabled();
        }
        Button.SetCoolDown(Timer, EffectActive ? EffectDuration : Cooldown);

        playerControl.UpdateBodies(playerControl.Data.Role.TeamColor, ref DeadBodyTarget);
        FixedUpdate(playerControl);
    }

    private void EffectEndHandler()
    {
        EffectActive = false;
        Timer = Cooldown;
        OnEffectEnd();
    }
}