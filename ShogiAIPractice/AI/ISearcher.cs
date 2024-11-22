using MyShogi.Model.Shogi.Core;

namespace ShogiAIPractice;

public interface ISearcher
{
	Move GetBestMove(Position position);
}