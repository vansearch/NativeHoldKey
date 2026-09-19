namespace HoldKeyPlugin;

public sealed class HoldKeyController : IHoldKeyController
{
    private readonly IKeyEmitter emitter;
    private readonly IRepeatTimer timer;
    private readonly object sync = new();
    private string[] heldKeys = [];
    private bool disposed;

    public HoldKeyController(IKeyEmitter emitter, IRepeatTimer timer)
    {
        this.emitter = emitter;
        this.timer = timer;
    }

    public bool IsPressed
    {
        get
        {
            lock (sync)
                return heldKeys.Length > 0;
        }
    }

    public void Press(string combination, TimeSpan repeatInterval, TimeSpan maximumHold)
    {
        var keys = Parse(combination);

        lock (sync)
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            if (heldKeys.Length > 0)
                return;

            var pressedKeys = new List<string>(keys.Length);
            try
            {
                foreach (var key in keys)
                {
                    emitter.KeyDown(key, false);
                    pressedKeys.Add(key);
                }

                heldKeys = keys;
                timer.Start(repeatInterval, maximumHold, Repeat, Release);
            }
            catch
            {
                ReleaseKeys(pressedKeys);
                heldKeys = [];
                throw;
            }
        }
    }

    public void Release()
    {
        lock (sync)
        {
            if (heldKeys.Length == 0)
                return;

            var keysToRelease = heldKeys;
            heldKeys = [];
            timer.Stop();
            ReleaseKeys(keysToRelease);
        }
    }

    private void ReleaseKeys(IReadOnlyList<string> keys)
    {
        for (var index = keys.Count - 1; index >= 0; index--)
        {
            try
            {
                emitter.KeyUp(keys[index]);
            }
            catch
            {
                // Best effort: one failed release must not prevent the remaining keys from being released.
            }
        }
    }

    private void Repeat()
    {
        lock (sync)
        {
            foreach (var key in heldKeys)
            {
                if (!emitter.IsModifier(key))
                    emitter.KeyDown(key, true);
            }
        }
    }

    private string[] Parse(string combination)
    {
        if (string.IsNullOrWhiteSpace(combination))
            throw new ArgumentException("A key or key combination is required.", nameof(combination));

        var keys = combination
            .Split('+', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (keys.Length == 0 || keys.Any(key => !emitter.IsSupported(key)))
            throw new ArgumentException($"Unsupported key combination: {combination}", nameof(combination));

        return keys;
    }

    public void Dispose()
    {
        lock (sync)
        {
            if (disposed)
                return;

            Release();
            disposed = true;
            timer.Dispose();
        }
    }
}
