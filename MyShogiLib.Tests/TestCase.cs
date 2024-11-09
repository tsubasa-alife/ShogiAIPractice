using Xunit;
using MyShogi.Model.Shogi.Core;
using Xunit.Abstractions;

namespace MyShogiLib.Tests
{
	public class TestCase
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public TestCase(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
			Initializer.Init();
		}
		
		[Fact]
		public void Test()
		{
			Position position = new Position();
			position.InitBoard();
			_testOutputHelper.WriteLine(position.Pretty());
			
			// 平手盤面
			Assert.Equal(Sfens.HIRATE, position.ToSfen());
		}
	}
}