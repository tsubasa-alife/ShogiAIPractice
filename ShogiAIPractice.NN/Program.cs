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
			var psv = PsvUtility.ReadPsv("../../../DataSet/shuffled.bin");
			// 読み込んだデータを表示
			foreach (var i in psv)
			{
				Console.WriteLine(i);
			}

			var sfen = SfenConverter.Unpack(psv);
			Console.WriteLine(sfen);
			Initializer.Init();
			var position = new Position();
			position.InitBoard();
			position.SetSfen(sfen);
			Console.WriteLine(position.Pretty());
		}
	}
}