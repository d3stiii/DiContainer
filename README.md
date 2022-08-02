# DiContainer
## Injection
- **Injection into methods**
```cs
public class Client {
    private Writer _writer;
    private ISaveService _saveService;

    [Inject]
    public void Construct(Writer writer, ISaveService saveService) {
        _writer = writer;
        _saveService = saveService;
    }
}
```
- **Injection into fields**
```cs
public class Client {
    [Inject] private Writer _writer;
    [Inject] private ISaveService _saveService;
}
```
## Registration
To register your dependency in the container, use ``` Container.Register ``` method. In the method parameters, you need to pass the instance of the dependency
```cs
Container.Register<IWriter>(new Writer());
```

## Instances creation
To create instance with injected dependencies, use ``` Container.CreateInstance ``` method.

- **Simple classes**
```cs
Client client = Container.CreateInstance<Client>();
```

- **Interfaces**
```cs
ISaveService saveService = Container.CreateInstance<ISaveService, SaveService>();
```

## Usage
- Create a new ``` Container ``` and configure installers.
```cs
Container container = new Container();
container.ConfigureInstallers();
```

- Create your own installer. Example:
```cs
public class ExampleInstaller : Installer {
    public override void Install() {
        Container.Register(Container.CreateInstance<IWriter, Writer>());
        Container.Register(Container.CreateInstance<ISaveService, SaveService>());
        Client client = Container.CreateInstance<Client>();
        client.DoWork();
    }
}
```
