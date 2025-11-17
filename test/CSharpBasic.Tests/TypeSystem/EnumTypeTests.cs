using FileMode = CSharpBasic.TypeSystem.FileMode;

namespace CSharpBasic.Tests.TypeSystem;

internal sealed class EnumTypeTests
{
    [Test]
    public void EnumTypeTest()
    {
        int[] nums = [1, 2, 3, 4, 5, 6];
        FileMode[] modes =
        [
            FileMode.CreateNew, FileMode.Create, FileMode.Open, FileMode.OpenOrCreate, FileMode.Truncate,
            FileMode.Append
        ];
        for (var i = 0; i < nums.Length; i++)
        {
            var mode = (FileMode)Enum.ToObject(typeof(FileMode), nums[i]);
            Assert.That(mode, Is.EqualTo(modes[i]));
        }
    }
}