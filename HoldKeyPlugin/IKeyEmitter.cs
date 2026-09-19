namespace HoldKeyPlugin;

public interface IKeyEmitter
{
    bool IsSupported(string key);
    bool IsModifier(string key);
    void KeyDown(string key, bool isRepeat);
    void KeyUp(string key);
}
