using ScreenMark.Drawing;

namespace ScreenMark.Tests.Drawing;

public class UndoHistoryTests
{
    [Fact]
    public void Undo_ReturnsItemsNewestFirst()
    {
        var history = new UndoHistory<int>();
        history.Record(1);
        history.Record(2);

        Assert.True(history.TryUndo(out var first));
        Assert.True(history.TryUndo(out var second));
        Assert.Equal([2, 1], [first, second]);
        Assert.False(history.TryUndo(out _));
    }

    [Fact]
    public void Redo_RestoresUndoneItemsInOriginalOrder()
    {
        var history = new UndoHistory<int>();
        history.Record(1);
        history.Record(2);
        history.TryUndo(out _);
        history.TryUndo(out _);

        Assert.True(history.TryRedo(out var first));
        Assert.True(history.TryRedo(out var second));
        Assert.Equal([1, 2], [first, second]);
        Assert.False(history.TryRedo(out _));
    }

    [Fact]
    public void Record_DiscardsRedoableItems()
    {
        var history = new UndoHistory<int>();
        history.Record(1);
        history.TryUndo(out _);

        history.Record(2);

        Assert.False(history.TryRedo(out _));
    }

    [Fact]
    public void Clear_DropsUndoAndRedoItems()
    {
        var history = new UndoHistory<int>();
        history.Record(1);
        history.Record(2);
        history.TryUndo(out _);

        history.Clear();

        Assert.False(history.TryUndo(out _));
        Assert.False(history.TryRedo(out _));
    }

    [Fact]
    public void UndoAndRedo_AreNoOpsWhenEmpty()
    {
        var history = new UndoHistory<string>();

        Assert.False(history.TryUndo(out var undone));
        Assert.False(history.TryRedo(out var redone));
        Assert.Null(undone);
        Assert.Null(redone);
    }
}
