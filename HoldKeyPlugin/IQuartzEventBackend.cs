namespace HoldKeyPlugin;

public interface IQuartzEventBackend
{
    IntPtr CreateKeyboardEvent(ushort keyCode, bool isDown);
    void SetFlags(IntPtr keyEvent, ulong flags);
    void SetIntegerField(IntPtr keyEvent, int field, long value);
    void Post(uint tap, IntPtr keyEvent);
    void Release(IntPtr keyEvent);
}
