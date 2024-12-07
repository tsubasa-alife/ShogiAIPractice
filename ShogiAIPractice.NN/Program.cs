// See https://aka.ms/new-console-template for more information

using System;
using System.IO;
using System.Collections.Generic;
using MyShogi.Model.Shogi.Core;
using MyShogi.Model.Shogi.Converter;

namespace ShogiAIPractice.NN
{
	class Program
	{
		static void Main(string[] args)
		{
			// 現在のディレクトリを表示
			Console.WriteLine(Directory.GetCurrentDirectory());
			// DataSetフォルダーの中にあるbinファイルを読み込む
			var psvList = PsvUtility.ReadPsv("../../../DataSet/shuffled.bin");
			Console.WriteLine($"psvList.Count: {psvList.Count}");
			var psvData = psvList[0];
			// 読み込んだデータを表示
			Console.WriteLine(psvData.sfen);
			Console.WriteLine(psvData.move);
			Console.WriteLine(psvData.score);
			Console.WriteLine(psvData.gamePly);
			Console.WriteLine(psvData.gameResult);
			// psv.sfenをint配列に変換
			var packedSfen = new int[32];
			for (int i = 0; i < 32; i++)
			{
				packedSfen[i] = psvData.sfen[i];
			}

			var sfen = SfenConverter.Unpack(packedSfen);
			Console.WriteLine(sfen);
			Initializer.Init();
			var position = new Position();
			position.InitBoard();
			position.SetSfen(sfen);
			Console.WriteLine(position.Pretty());
		}
	}
}