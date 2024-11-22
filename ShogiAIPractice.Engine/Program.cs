using MyShogi.Model.Shogi.Core;
using MyShogi.Model.Shogi.Converter;
using ShogiAIPractice.Engine;

Initializer.Init();
bool onGame = true;
Position position = new Position();
position.InitBoard();
ISearcher searcher = new RandomAI();
Console.WriteLine(position.Pretty());

// メインループ
while (onGame)
{
    var command = Console.ReadLine();
    await ProcessCommand(command);
}

// 入力されたコマンドを処理する
async Task ProcessCommand(string? command)
{
    // コマンドがnullの場合は処理しない
    if (command == null)
    {
        return;
    }
    
    switch (command)
    {
        case "end":
            Console.Clear();
            Console.WriteLine("GameEnd");
            onGame = false;
            break;
        default:
            // USI形式の手かどうか判定する
            if (Usi.TryParseMove(command, out var move))
            {
                // 手が合法かどうか判定する
                if (!position.IsLegal(move))
                {
                    Console.WriteLine("Illegal move");
                    return;
                }
                
                // 手を指す
                position.DoMove(move);
                Console.Clear();
                Console.WriteLine(position.Pretty());
                Console.WriteLine(move.ToUsi());
                Console.WriteLine("AI思考中...");
                var bestMove = await searcher.GetBestMove(position);
                Console.WriteLine("AIの手: " + bestMove.ToUsi());
                position.DoMove(bestMove);
                Console.Clear();
                Console.WriteLine(position.Pretty());
                Console.WriteLine("AIの手: " + bestMove.ToUsi());
            }
            else
            {
                Console.WriteLine("Invalid command");
            }
            break;
    }
    
    
}