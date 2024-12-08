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
			Initializer.Init();
			// 現在のディレクトリを表示
			Console.WriteLine(Directory.GetCurrentDirectory());
			// DataSetフォルダーの中にあるbinファイルを読み込む
			var psvList = PsvUtility.ReadPsv("../../../DataSet/shuffled.bin");
			Console.WriteLine("データ読み込み完了！");
			Console.WriteLine($"psvList.Count: {psvList.Count}");
			
			var psvData = psvList[0];
			
			// packedSfenをスペース区切りの文字列に変換
			var packedSfenStr = string.Join(" ", psvData.packedSfen);
			// 読み込んだデータを表示
			Console.WriteLine($"packedSfen: {packedSfenStr}");
			Console.WriteLine($"psvData.move: {psvData.move}");
			Console.WriteLine($"psvData.score: {psvData.score}");
			Console.WriteLine($"psvData.gamePly: {psvData.gamePly}");
			Console.WriteLine($"psvData.gameResult: {psvData.gameResult}");

			var sfen = SfenConverter.Unpack(psvData.packedSfen);
			Console.WriteLine($"sfen: {sfen}");
			var position = new Position();
			position.InitBoard();
			position.SetSfen(sfen);
			Console.WriteLine(position.Pretty());
		}
	}
}