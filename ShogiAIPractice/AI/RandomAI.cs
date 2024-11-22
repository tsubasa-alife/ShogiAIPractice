using MyShogi.Model.Shogi.Core;

namespace ShogiAIPractice;

public class RandomAI : ISearcher
{
	public Move GetBestMove(Position position)
	{
		// 合法手をランダムに選ぶ
		var moves = new Move[(int)Move.MAX_MOVES];
		var endIndex = MoveGen.LegalAll(position, moves, 0);
		var index = new Random().Next(0, endIndex);
		var bestMove = moves[index];
		return position.IsLegal(bestMove) ? bestMove : Move.RESIGN;
	}
}