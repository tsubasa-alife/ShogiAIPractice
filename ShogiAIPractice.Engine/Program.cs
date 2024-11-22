﻿using System.Diagnostics;
using MyShogi.Model.Shogi.Core;
using MyShogi.Model.Shogi.Converter;

namespace ShogiAIPractice.Engine
{
    public class Program
    {
        public const string USI_Ponder = "USI_Ponder";
        public const string USI_Hash = "USI_Hash";
        public const string EvalDir = "EvalDir";
        public const string BookFile = "BookFile";
        public const string IgnoreBookPlay = "IgnoreBookPlay";
        
        public static void Initialize()
        {
            Initializer.Init();
        }

        void Run()
        {
            Initialize();
            var options = new Dictionary<string, string>();
            var position = new Position();

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                var split = line.Split();
                if (split.Length == 0)
                {
                    continue;
                }

                var command = split[0];
                switch (command)
                {
                    case "usi":
                        Console.WriteLine("id name Hina");
                        Console.WriteLine("id author Tsubasa Hizono");
                        Console.WriteLine($"option name {USI_Ponder} type check default true");
                        Console.WriteLine($"option name {USI_Hash} type spin default 256");
                        Console.WriteLine($"option name {EvalDir} type string default eval");
                        Console.WriteLine($"option name {BookFile} type string default user_book1.db");
                        Console.WriteLine($"option name {IgnoreBookPlay} type check default true");
                        Console.WriteLine("usiok");
                        Console.Out.Flush();
                        break;

                    case "isready":
                        Console.WriteLine("readyok");
                        Console.Out.Flush();
                        break;

                    case "usinewgame":
                        break;

                    case "setoption":
                        Debug.Assert(split.Length == 5);
                        Debug.Assert(split[1] == "name");
                        Debug.Assert(split[3] == "value");
                        var id = split[2];
                        var x = split[4];
                        options[id] = x;
                        break;

                    case "position":
                        Debug.Assert(split.Length >= 2);
                        Debug.Assert(split[1] == "sfen" || split[1] == "startpos");
                        int nextIndex;
                        if (split[1] == "sfen")
                        {
                            Debug.Assert(command.Length >= 6);
                            var sfen = string.Join(" ", split.Skip(2).Take(4));
                            position.SetSfen(sfen);
                            nextIndex = 6;
                        }
                        else if (split[1] == "startpos")
                        {
                            position.SetSfen(Position.SFEN_HIRATE);
                            nextIndex = 2;
                        }
                        else
                        {
                            throw new Exception($"不正なpositionコマンドを受信しました。 line={line}");
                        }

                        foreach (var moveString in split.Skip(nextIndex))
                        {
                            if (moveString == "moves")
                            {
                                continue;
                            }

                            var move = Usi.ToMove(moveString);
                            position.DoMove(move);
                        }

                        break;

                    case "go":
                        bool ponder = false;
                        int btime = 0;
                        int wtime = 0;
                        int byoyomi = 0;
                        int binc = 0;
                        int winc = 0;
                        bool infinite = false;
                        for (int index = 1; index < split.Length; ++index)
                        {
                            var option = split[index];
                            switch (option)
                            {
                                case "ponder":
                                    ponder = true;
                                    break;

                                case "btime":
                                    btime = int.Parse(split[++index]);
                                    break;

                                case "wtime":
                                    wtime = int.Parse(split[++index]);
                                    break;

                                case "byoyomi":
                                    byoyomi = int.Parse(split[++index]);
                                    break;

                                case "binc":
                                    binc = int.Parse(split[++index]);
                                    break;

                                case "winc":
                                    winc = int.Parse(split[++index]);
                                    break;

                                case "infinite":
                                    infinite = true;
                                    break;

                                default:
                                    throw new Exception($"Unsupported go option: option={option}");
                            }
                        }
                        break;

                    case "stop":
                        break;

                    case "ponderhit":
                        break;

                    case "quit":
                        return;

                    case "gameover":
                        break;

                    // 以下デバッグ用コマンド
                    case "d":
                        Console.WriteLine(position);
                        break;
                    default:
                        Console.WriteLine($"info string Unsupported command: command={command}");
                        Console.Out.Flush();
                        break;
                }
            }
        }
        
        static void Main(string[] args)
        {
            new Program().Run();
        }
    }
}