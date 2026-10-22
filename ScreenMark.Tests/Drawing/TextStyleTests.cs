using System.Windows.Media;
using ScreenMark.Drawing;

namespace ScreenMark.Tests.Drawing;

public class TextStyleTests
{
    [Fact]
    public void Family_IsInstalledOnThisSystem() => StaThread.Run(() =>
        Assert.Contains(Fonts.SystemFontFamilies, family => family.Source == TextStyle.Family.Source));
}
