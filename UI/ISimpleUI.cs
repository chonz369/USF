public interface ISimpleUI
{
    ISimpleUI PrevUI { get; }

    bool IsOpened { get; }

    bool IsClosed { get; }

    void Open(ISimpleUI prevUI = null);

    void Back();

    void Close();
}