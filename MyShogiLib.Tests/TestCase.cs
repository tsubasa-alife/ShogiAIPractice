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
			SfenPacker.Pack(position);
			var position2 = new Position();
			var sfen = SfenPacker.Unpack();
			position2.SetSfen(sfen);
			
			// 正しく復元できているか？
			Assert.Equal(Sfens.HIRATE, position2.ToSfen());
			
			// 1手指す
			position2.DoMove(Usi.ToMove("7g7f"));
			
			// 1手指した局面についてもpackしてunpackしてみる
			SfenPacker.Pack(position2);
			var position3 = new Position();
			var sfen2 = SfenPacker.Unpack();
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
			
			var testPackedSfen = new int[]
			{
				141, 143, 241, 23, 175, 245, 11, 231, 251, 105, 76, 82, 58, 17, 82, 38, 242, 128, 132, 136, 32, 26, 135, 128, 159, 199, 99, 88, 192, 50, 20, 146, 36, 73, 146, 36
			};
			
			var testSfen = "lr3g1n+B/3sgbk1l/p2p1p1s1/2P1p1pp1/2pS4p/4PPP2/P+p3S1P1/3RG1GK1/LN5NL w N3p 1";
			var testPackedSfenStr = string.Join(" ", testPackedSfen);
			
			// 盤面をpack/unpackしてみる
			var packedSfen = SfenConverter.Pack(testSfen);
			var sfen = SfenConverter.Unpack(testPackedSfen);
			_testOutputHelper.WriteLine($"testPackedSfenStr:\n{testPackedSfenStr}");
			_testOutputHelper.WriteLine($"packedSfen:\n{string.Join(" ", packedSfen)}");
			_testOutputHelper.WriteLine($"testSfen:\n{testSfen}");
			_testOutputHelper.WriteLine($"sfen:\n{sfen}");
			
			// 正しくpackできているか？
			Assert.Equal(testPackedSfenStr, string.Join(" ", packedSfen));
			// 正しくunpackできているか？
			Assert.Equal(testSfen, sfen);
			
			position.SetSfen(sfen);
			
			// 正しく復元できているか？
			Assert.Equal(testSfen, position.ToSfen());
		}

		/// <summary>
		/// 局面「8r/1P1+P1k3/l3ppG1g/6p1p/p2s1P3/4P1P1P/1p1+p1S1L+n/5G1p1/LNS2KR1L w 1S1N2P2b1g1n2p 1」のpack、unpackテスト
		/// </summary>
		[Fact]
		public void TestPackedSfen3()
		{
			// 盤面の初期化
			Position position = new Position();
			position.InitBoard();
			
			// 平手盤面になっているかどうか？
			Assert.Equal(Sfens.HIRATE, position.ToSfen());
			
			// 155 14 128 95 161 96 68 230 227 5 82 38 167 0 132 16 82 59 12 59 120 164 97 57 248 49 140 2 0 0 0 0 207 231 243 249 124 62 159 207 231 243 121 43 73 146 36 73 2
			var testPackedSfen = new int[]
			{
				155, 14, 128, 95, 161, 96, 68, 230, 227, 5, 82, 38, 167, 0, 132, 16, 82, 59, 12, 59, 120, 164, 97, 57, 248, 49, 140, 2, 0, 0, 0, 0, 207, 231, 243, 249, 124, 62, 159, 207, 231, 243, 121, 43, 73, 146, 36, 73, 2
			};
			
			var testSfen = "8r/1P1+P1k3/l3ppG1g/6p1p/p2s1P3/4P1P1P/1p1+p1S1L+n/5G1p1/LNS2KR1L w SN2P2bgn2p 1";
			var testPackedSfenStr = string.Join(" ", testPackedSfen);
			
			// 盤面をpack/unpackしてみる
			var packedSfen = SfenConverter.Pack(testSfen);
			var sfen = SfenConverter.Unpack(testPackedSfen);
			_testOutputHelper.WriteLine($"testPackedSfenStr:\n{testPackedSfenStr}");
			_testOutputHelper.WriteLine($"packedSfen:\n{string.Join(" ", packedSfen)}");
			_testOutputHelper.WriteLine($"testSfen:\n{testSfen}");
			_testOutputHelper.WriteLine($"sfen:\n{sfen}");
			
			// 正しくpackできているか？
			Assert.Equal(testPackedSfenStr, string.Join(" ", packedSfen));
			// 正しくunpackできているか？
			Assert.Equal(testSfen, sfen);
			
			position.SetSfen(sfen);
			
			// 正しく復元できているか？
			Assert.Equal(testSfen, position.ToSfen());
		}
		
		/// <summary>
		/// 局面「8r/1P1+P5/l3ppk1g/2p3pPp/p1bs1P3/2g1P1P1P/Lp1+p1S1L+n/5G1p1/1NS2KR1L b SNPbgnp 1」のpack、unpackテスト
		/// ※持ち駒重複がないパターン
		/// </summary>
		[Fact]
		public void TestPackedSfen4()
		{
			// 盤面の初期化
			Position position = new Position();
			position.InitBoard();
			
			// 平手盤面になっているかどうか？
			Assert.Equal(Sfens.HIRATE, position.ToSfen());
			
			// 154 24 128 95 161 192 136 76 47 137 140 76 159 167 0 175 16 98 72 237 48 236 224 145 44 7 63 134 81 240 188 149
			var testPackedSfen = new int[]
			{
				154, 24, 128, 95, 161, 192, 136, 76, 47, 137, 140, 76, 159, 167, 0, 175, 16, 98, 72, 237, 48, 236, 224, 145, 44, 7, 63, 134, 81, 240, 188, 149
			};
			
			var testSfen = "8r/1P1+P5/l3ppk1g/2p3pPp/p1bs1P3/2g1P1P1P/Lp1+p1S1L+n/5G1p1/1NS2KR1L b SNPbgnp 1";
			var testPackedSfenStr = string.Join(" ", testPackedSfen);
			
			// 盤面をpack/unpackしてみる
			var packedSfen = SfenConverter.Pack(testSfen);
			var sfen = SfenConverter.Unpack(testPackedSfen);
			_testOutputHelper.WriteLine($"testPackedSfenStr:\n{testPackedSfenStr}");
			_testOutputHelper.WriteLine($"packedSfen:\n{string.Join(" ", packedSfen)}");
			_testOutputHelper.WriteLine($"testSfen:\n{testSfen}");
			_testOutputHelper.WriteLine($"sfen:\n{sfen}");
			
			// 正しくpackできているか？
			Assert.Equal(testPackedSfenStr, string.Join(" ", packedSfen));
			// 正しくunpackできているか？
			Assert.Equal(testSfen, sfen);
			
			position.SetSfen(sfen);
			
			// 正しく復元できているか？
			Assert.Equal(testSfen, position.ToSfen());
		}
	}
}