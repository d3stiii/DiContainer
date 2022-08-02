using System.Reflection;

namespace DiContainer;

public class Container {
    public Container() =>
        Register(this);

    public T? GetInstance<T>() =>
        Implementation<T>.ServiceInstance;

    public void Register<T>(T instance) =>
        Implementation<T>.ServiceInstance = instance;

    public void ConfigureInstallers() {
        Type parentType = typeof(Installer);
        var installers = Assembly.GetCallingAssembly()
            .GetTypes()
            .Where(type => parentType.IsAssignableFrom(type) && parentType != type)
            .Select(type => {
                var installer = (Installer)Activator.CreateInstance(type)!;
                Inject(installer);
                installer.Install();
                return installer;
            });
    }

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

    private void InjectIntoField<T>(T instance) {
        var injectionFields = typeof(T)
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(field => field.GetCustomAttributes(typeof(InjectAttribute)) != null);

        foreach (var field in injectionFields) {
            var dependency = typeof(Container).GetMethod(nameof(GetInstance))
                ?.MakeGenericMethod(field.FieldType)
                .Invoke(this, null);

            field.SetValue(instance, dependency);
        }
    }

    private void InjectIntoMethod<T>(T instance) {
        var injectionsMethods = typeof(T)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
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

    private void Inject<T>(T instance) {
        InjectIntoMethod(instance);
        InjectIntoField(instance);
    }

    private static class Implementation<T> {
        public static T? ServiceInstance;
    }
}