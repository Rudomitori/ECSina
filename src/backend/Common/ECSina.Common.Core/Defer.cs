namespace ECSina.Common.Core;

public sealed class Defer : IDisposable
{
    private readonly Action _deferFunc;

    public Defer(Action deferFunc)
    {
        _deferFunc = deferFunc;
    }

    public void Dispose()
    {
        _deferFunc();
    }
}
