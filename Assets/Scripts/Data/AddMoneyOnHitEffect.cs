using UnityEngine;

[CreateAssetMenu(menuName = "Game/Modifiers/Effects/Add Money On Hit")]
public class AddMoneyOnHitEffect : PlaceableModifierEffect
{
    [SerializeField] private int money = 1;

    public override void OnHit(
        PlaceableRuntimeData placeable,
        BallRuntimeData ball,
        HitEventData hitData)
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Economy.AddMoney(money);
        Debug.Log("Test on  addmoney");
    }
}