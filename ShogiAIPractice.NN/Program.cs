// See https://aka.ms/new-console-template for more information

using System;
using System.IO;
using System.Collections.Generic;
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
			// 出力する
			Console.WriteLine(psv[0]);
		}
	}
}