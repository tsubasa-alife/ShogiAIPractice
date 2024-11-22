using MyShogi.Model.Shogi.Core;

namespace ShogiAIPractice.Engine;

public interface ISearcher
{
	Task<Move> GetBestMove(Position position);
}