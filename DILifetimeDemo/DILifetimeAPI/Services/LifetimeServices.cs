namespace DILifetimeAPI.Services;

public interface ITransientService { Guid GetOperationId(); }
public interface IScopedService { Guid GetOperationId(); }
public interface ISingletonService { Guid GetOperationId(); }

public class OperationService : ITransientService, IScopedService, ISingletonService
{
    private readonly Guid _id;

    public OperationService()
    {
        _id = Guid.NewGuid();
    }

    public Guid GetOperationId() => _id;
}
