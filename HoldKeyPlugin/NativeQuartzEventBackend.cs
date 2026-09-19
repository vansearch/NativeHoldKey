using System.Runtime.InteropServices;

namespace HoldKeyPlugin;

public sealed class NativeQuartzEventBackend : IQuartzEventBackend
{
    private const string ApplicationServices =
        "/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices";
    private const string CoreFoundation =
        "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

    public IntPtr CreateKeyboardEvent(ushort keyCode, bool isDown) =>
        CGEventCreateKeyboardEvent(IntPtr.Zero, keyCode, isDown);

    public void SetFlags(IntPtr keyEvent, ulong flags) => CGEventSetFlags(keyEvent, flags);
    public void SetIntegerField(IntPtr keyEvent, int field, long value) =>
        CGEventSetIntegerValueField(keyEvent, field, value);
    public void Post(uint tap, IntPtr keyEvent) => CGEventPost(tap, keyEvent);
    public void Release(IntPtr keyEvent) => CFRelease(keyEvent);

    [DllImport(ApplicationServices)]
    private static extern IntPtr CGEventCreateKeyboardEvent(IntPtr source, ushort virtualKey, bool keyDown);

    [DllImport(ApplicationServices)]
    private static extern void CGEventSetFlags(IntPtr keyEvent, ulong flags);

    [DllImport(ApplicationServices)]
    private static extern void CGEventSetIntegerValueField(IntPtr keyEvent, int field, long value);

    [DllImport(ApplicationServices)]
    private static extern void CGEventPost(uint tap, IntPtr keyEvent);

    [DllImport(CoreFoundation)]
    private static extern void CFRelease(IntPtr value);
}
