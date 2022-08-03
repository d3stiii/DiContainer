namespace DiContainer;

public abstract class Installer {
    protected Container Container => _container;

    [Inject] private Container _container = null!;

    public abstract void Install();
}