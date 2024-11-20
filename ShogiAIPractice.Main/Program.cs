using MyShogi.Model.Shogi.Core;
using MyShogi.Model.Shogi.Converter;

Initializer.Init();
bool onGame = true;
Position position = new Position();
position.InitBoard();
Console.WriteLine(position.Pretty());

// メインループ
while (onGame)
{
    var command = Console.ReadLine();
    ProcessCommand(command);
}

Console.WriteLine("GameEnd");

// 入力されたコマンドを処理する
void ProcessCommand(string? command)
{
    // コマンドがnullの場合は処理しない
    if (command == null)
    {
        return;
    }
    
    switch (command)
    {
        case "end":
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
            }
            else
            {
                Console.WriteLine("Invalid command");
            }
            break;
    }
    
    
}