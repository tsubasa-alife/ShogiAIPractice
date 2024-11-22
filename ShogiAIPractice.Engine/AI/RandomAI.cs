using MyShogi.Model.Shogi.Core;

namespace ShogiAIPractice.Engine;

public class RandomAI : ISearcher
{
	public async Task<Move> GetBestMove(Position position)
	{
		// 合法手をランダムに選ぶ
		var moves = new Move[(int)Move.MAX_MOVES];
		var endIndex = MoveGen.LegalAll(position, moves, 0);
		var index = new Random().Next(0, endIndex);
		// 1秒待つ
		await Task.Delay(1000);
		return moves[index];
	}
}