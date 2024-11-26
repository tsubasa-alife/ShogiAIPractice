using Xunit;
using MyShogi.Model.Shogi.Core;
using MyShogi.Model.Shogi.Converter;
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

		/// <summary>
		/// USI形式の指し手とMoveクラスの変換テスト
		/// </summary>
		[Fact]
		public void TestUsi()
		{
			var move = Usi.ToMove("7g7f");
			Assert.Equal("7g7f", move.ToUsi());
			var move2 = Usi.ToMove("7g7f+");
			Assert.Equal("7g7f+", move2.ToUsi());
			var move3 = Usi.ToMove("P*7f");
			Assert.Equal("P*7f", move3.ToUsi());
			Usi.TryParseMove("test*", out var move4);
			Assert.Equal(Move.NONE, move4);
		}
		
		[Fact]
		public void TestPackedSfen()
		{
			Position position = new Position();
			position.InitBoard();
			_testOutputHelper.WriteLine(position.ToSfen());
			
			// 平手盤面
			Assert.Equal(Sfens.HIRATE, position.ToSfen());
			
			var packedSfen = SfenConverter.Pack(position.ToSfen());
			var position2 = new Position();
			var sfen = SfenConverter.Unpack(packedSfen);
			_testOutputHelper.WriteLine(sfen);
			position2.SetSfen(sfen);
			
			// 正しく復元できているか？
			Assert.Equal(Sfens.HIRATE, position2.ToSfen());
		}
	}
}