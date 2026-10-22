using System.Diagnostics.CodeAnalysis;

namespace ScreenMark.Drawing;

internal sealed class UndoHistory<T>
{
    private readonly Stack<T> done = new();
    private readonly Stack<T> undone = new();

    public void Record(T item)
    {
        done.Push(item);
        undone.Clear();
    }

    public bool TryUndo([MaybeNullWhen(false)] out T item)
    {
        if (!done.TryPop(out item))
        {
            return false;
        }

        undone.Push(item);
        return true;
    }

    public bool TryRedo([MaybeNullWhen(false)] out T item)
    {
        if (!undone.TryPop(out item))
        {
            return false;
        }

        done.Push(item);
        return true;
    }

    public void Clear()
    {
        done.Clear();
        undone.Clear();
    }
}
