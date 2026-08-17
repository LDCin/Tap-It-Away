using Cysharp.Threading.Tasks;

public abstract class BoosterBase
{
    protected int activeCount = 1;
    protected bool deactive = false;
    protected BoosterType boosterType;
    protected BoosterSO boosterSO;

    public async UniTask StartBooster()
    {
        await Active();
        if (deactive && this is IDeactivatableBooster deactivatableBooster)
        {
            deactivatableBooster.Deactive();
        }
    }

    public abstract UniTask Active();
    
    public BoosterBase(BoosterSO boosterSO)
    {
        this.boosterSO = boosterSO;
        activeCount = boosterSO.activeCount;
        deactive = boosterSO.deactive;
    }
}
