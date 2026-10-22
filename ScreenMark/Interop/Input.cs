using System.Runtime.InteropServices;

namespace ScreenMark.Interop;

[StructLayout(LayoutKind.Sequential)]
internal struct Input
{
    private const uint MouseType = 0;
    private const uint WheelFlag = 0x0800;

    public uint Type;
    public MouseInput Mouse;

    public static Input Wheel(int delta) => new()
    {
        Type = MouseType,
        Mouse = new MouseInput { MouseData = (uint)delta, Flags = WheelFlag },
    };
}

[StructLayout(LayoutKind.Sequential)]
internal struct MouseInput
{
    public int X;
    public int Y;
    public uint MouseData;
    public uint Flags;
    public uint Time;
    public nuint ExtraInfo;
}
