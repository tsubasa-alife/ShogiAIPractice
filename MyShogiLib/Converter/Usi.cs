using MyShogi.Model.Shogi.Core;

namespace MyShogi.Model.Shogi.Converter
{
	public class Usi
	{
		/// <summary>
		/// USI形式の指し手をMoveクラスに変換する（失敗した時はMove.NONEを返す）
		/// </summary>
		/// <param name="usi"></param>
		/// <returns></returns>
		public static Move ToMove(string usi)
		{
			if (usi == "resign")
				return Move.RESIGN;
			if (usi == "win")
				return Move.WIN;
			if (usi == "null")
				return Move.NULL;

			if (usi.Contains("*"))
			{
				var parts = usi.Split('*');
				
				// そもそも文字の長さが足りない場合はMove.NONEを返す
				if (parts.Length < 2)
				{
					return Move.NONE;
				}
				
				if (parts[1].Length < 2)
				{
					return Move.NONE;
				}
				
				var piece = Util.FromUsiPiece(parts[0][0]);
				var to = Util.FromUsiSquare(parts[1][0], parts[1][1]);

				// 変換できない場合はMove.NONEを返す
				if (piece == Piece.NO_PIECE || to == Square.NB)
				{
					return Move.NONE;
				}
				
				return Util.MakeMoveDrop(piece, to);
			}
			else
			{
				// そもそも文字の長さが足りない場合はMove.NONEを返す
				if (usi.Length < 4)
				{
					return Move.NONE;
				}
				
				var from = Util.FromUsiSquare(usi.Substring(0, 2)[0], usi.Substring(0, 2)[1]);
				var to = Util.FromUsiSquare(usi.Substring(2, 2)[0], usi.Substring(2, 2)[1]);
				
				// 変換できない場合はMove.NONEを返す
				if (from == Square.NB || to == Square.NB)
				{
					return Move.NONE;
				}
				
				var promote = usi.Length == 5 && usi[4] == '+';
				return promote ? Util.MakeMovePromote(from, to) : Util.MakeMove(from, to);
			}
		}
		
		/// <summary>
		/// USI形式の指し手のMoveへの変換を試す（失敗した時はfalseを返す）
		/// </summary>
		public static bool TryParseMove(string usi, out Move move)
		{
			move = ToMove(usi);
			return move != Move.NONE;
		}
	}
}