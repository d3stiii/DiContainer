namespace DiContainer;

public class Installer {
    protected Container Container => _container;

    [Inject] private Container _container = null!;

    public virtual void Install() =>
        throw new NotImplementedException();
}