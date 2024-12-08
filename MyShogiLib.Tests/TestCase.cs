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
			// 盤面の初期化
			Position position = new Position();
			position.InitBoard();
			
			// 平手盤面になっているかどうか？
			Assert.Equal(Sfens.HIRATE, position.ToSfen());
			
			// 平手盤面をpackしてunpackしてみる
			var packedSfen = SfenConverter.Pack(position.ToSfen());
			var position2 = new Position();
			var sfen = SfenConverter.Unpack(packedSfen);
			position2.SetSfen(sfen);
			
			// 正しく復元できているか？
			Assert.Equal(Sfens.HIRATE, position2.ToSfen());
			
			// 1手指す
			position2.DoMove(Usi.ToMove("7g7f"));
			
			// 1手指した局面についてもpackしてunpackしてみる
			var packedSfen2 = SfenConverter.Pack(position2.ToSfen());
			var position3 = new Position();
			var sfen2 = SfenConverter.Unpack(packedSfen2);
			position3.SetSfen(sfen2);
			
			// 正しく復元できているか？
			Assert.Equal(position2.ToSfen(), position3.ToSfen());
		}
		
		/// <summary>
		/// 局面「lr3g1n+B/3sgbk1l/p2p1p1s1/2P1p1pp1/2pS4p/4PPP2/P+p3S1P1/3RG1GK1/LN5NL w N3p 1」のpack、unpackテスト
		/// </summary>
		[Fact]
		public void TestPackedSfen2()
		{
			// 盤面の初期化
			Position position = new Position();
			position.InitBoard();
			
			// 平手盤面になっているかどうか？
			Assert.Equal(Sfens.HIRATE, position.ToSfen());
			
			var packedSfen = new int[]
			{
				141, 143, 241, 23, 175, 245, 11, 231, 251, 105, 76, 82, 58, 17, 82, 38, 242, 128, 132, 136, 32, 26, 135, 128, 159, 199, 99, 88, 192, 50, 20, 146, 36, 73, 146, 36
			};
			
			var testSfen = "lr3g1n+B/3sgbk1l/p2p1p1s1/2P1p1pp1/2pS4p/4PPP2/P+p3S1P1/3RG1GK1/LN5NL w N3p 1";
			var testPackedSfenStr = string.Join(" ", packedSfen);
			
			// 盤面をunpackしてみる
			
			var sfen = SfenConverter.Unpack(packedSfen);
			position.SetSfen(sfen);
			
			// 正しく復元できているか？
			Assert.Equal(testSfen, position.ToSfen());
		}
	}
}