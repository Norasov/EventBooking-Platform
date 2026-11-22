namespace Application.StatusCodes;

public class ServiceStatusCode<T>
{
    public StatusCode Type {get;set;}
    public T? Data {get;set;}
    public string Error {get;set;}=null!;

    public static ServiceStatusCode<T> Ok(T? data)
        => new ServiceStatusCode<T>{Type = StatusCode.Ok, Data = data};

    public static ServiceStatusCode<T> Created(T data)
        => new ServiceStatusCode<T>{Type = StatusCode.Created, Data = data};

    public static ServiceStatusCode<T> NoContent()
        => new ServiceStatusCode<T>{Type = StatusCode.NoContent};

    public static ServiceStatusCode<T> NotFound(string error)
        => new ServiceStatusCode<T>{Type = StatusCode.NotFound, Error = error};

    public static ServiceStatusCode<T> BadRequest(string error)
        => new ServiceStatusCode<T>{Type = StatusCode.BadRequest, Error = error};

    public static ServiceStatusCode<T> Conflict(string error)
     => new ServiceStatusCode<T>{Type = StatusCode.Conflict, Error = error};
}
