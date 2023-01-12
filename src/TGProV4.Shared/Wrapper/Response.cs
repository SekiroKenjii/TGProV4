using TGProV4.Shared.Wrapper.Base;

namespace TGProV4.Shared.Wrapper;

public class Response<T> : BaseResponse
{
    public T? Data { get; set; }
}
