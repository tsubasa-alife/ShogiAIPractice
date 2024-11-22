using MyShogi.Model.Shogi.Core;

namespace ShogiAIPractice;

public interface ISearcher
{
	Task<Move> GetBestMove(Position position);
}