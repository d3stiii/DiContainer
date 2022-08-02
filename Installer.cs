namespace DiContainer; 

public class Installer {
    protected Container Container = null!;

    public virtual void Install() =>
        throw new NotImplementedException();

    [Inject]
    public void Construct(Container container) =>
        Container = container;
}