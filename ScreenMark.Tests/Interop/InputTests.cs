using System.Runtime.InteropServices;
using ScreenMark.Interop;

namespace ScreenMark.Tests.Interop;

public class InputTests
{
    private const uint MouseType = 0;
    private const uint WheelFlag = 0x0800;

    [Fact]
    public void Layout_MatchesNativeInputStructure()
    {
        var expectedSize = nint.Size == 8 ? 40 : 28;

        Assert.Equal(expectedSize, Marshal.SizeOf<Input>());
        Assert.Equal(nint.Size, (int)Marshal.OffsetOf<Input>(nameof(Input.Mouse)));
    }

    [Theory]
    [InlineData(120)]
    [InlineData(-120)]
    [InlineData(30)]
    public void Wheel_CreatesWheelEventWithSignedDelta(int delta)
    {
        var input = Input.Wheel(delta);

        Assert.Equal(MouseType, input.Type);
        Assert.Equal(WheelFlag, input.Mouse.Flags);
        Assert.Equal(delta, (int)input.Mouse.MouseData);
    }
}
