using MyShogi.Model.Shogi.Core;
using MyShogi.Model.Shogi.Converter;

Initializer.Init();
bool onGame = true;
Position position = new Position();
position.InitBoard();

while (onGame)
{
    var command = Console.ReadLine();
    
    Console.Clear();
    Console.WriteLine(position.Pretty());
    
    switch (command)
    {
        case "end":
            onGame = false;
            break;
        default:
            Console.WriteLine("not supported command");
            break;
    }
}

Console.WriteLine("GameEnd");