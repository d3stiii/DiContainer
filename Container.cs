using System.Reflection;

namespace DiContainer; 

public class Container {
    public Container() =>
        Register(this);

    public TConcrete CreateInstance<TConcrete>() {
        var instanceType = typeof(TConcrete);
        var instance = (TConcrete)Activator.CreateInstance(instanceType)!;
        Inject(instance);
        return instance;
    }

    public TAbstract CreateInstance<TAbstract, TConcrete>() {
        var instanceType = typeof(TConcrete);
        var instance = (TAbstract)Activator.CreateInstance(instanceType)!;
        Inject(instance);
        return instance;
    }

    private void Inject<T>(T instance) {
        var injectionsMethods = typeof(T)
            .GetMethods()
            .Where(method => method.GetCustomAttribute(typeof(InjectAttribute)) != null);
        foreach (var method in injectionsMethods) {
            object[] parameters = method.GetParameters()
                .Select(parameter => typeof(Container)
                    .GetMethod(nameof(GetInstance))
                    ?.MakeGenericMethod(parameter.ParameterType)
                    .Invoke(this, null))
                .ToArray()!;

            method.Invoke(instance, parameters);
        }
    }

    public void ConfigureInstallers() {
        Type parentType = typeof(Installer);
        var installers = Assembly.GetCallingAssembly()
            .GetTypes()
            .Where(type => parentType.IsAssignableFrom(type) && parentType != type)
            .Select(type => (Installer)Activator.CreateInstance(type)!);

        foreach (var installer in installers) {
            Inject(installer);
            installer.Install();
        }
    }

    public void Register<T>(T instance) =>
        Implementation<T>.ServiceInstance = instance;

    public T? GetInstance<T>() =>
        Implementation<T>.ServiceInstance;

    private static class Implementation<T> {
        public static T? ServiceInstance;
    }
}