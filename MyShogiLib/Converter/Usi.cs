using MyShogi.Model.Shogi.Core;

namespace MyShogi.Model.Shogi.Converter
{
	public class Usi
	{
		public static Move ToMove(string usi)
		{
			if (usi == "none")
				return Move.NONE;
			if (usi == "resign")
				return Move.RESIGN;
			if (usi == "win")
				return Move.WIN;
			if (usi == "null")
				return Move.NULL;

			if (usi.Contains("*"))
			{
				// Drop move
				var parts = usi.Split('*');
				var piece = Util.FromUsiPiece(parts[0][0]);
				var to = Util.FromUsiSquare(parts[1][0], parts[1][1]);
				return Util.MakeMoveDrop(piece, to);
			}
			else
			{
				// Normal move
				var from = Util.FromUsiSquare(usi.Substring(0, 2)[0], usi.Substring(0, 2)[1]);
				var to = Util.FromUsiSquare(usi.Substring(2, 2)[0], usi.Substring(2, 2)[1]);
				var promote = usi.Length == 5 && usi[4] == '+';
				return promote ? Util.MakeMovePromote(from, to) : Util.MakeMove(from, to);
			}
		}
	}
}