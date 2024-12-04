namespace MyShogi.Model.Shogi.Converter;

/// <summary>
/// PackedSfenAndValueの読み書きを行うクラス
/// note:
/// PacedSfenAndValueは以下のような構造になっている
/// 「PackedSfen、評価値、指し手、手数、勝敗＋Padding（32 + 2 + 2 + 2 + 1 + 1 = 40bytes）」
/// </summary>
public class PsvUtility
{
    public static int[] ReadPsv(string path)
    {
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
        {
            // 256bit分だけ読み込む
            byte[] buffer = new byte[32];
            fs.Read(buffer, 0, 32);
            var result = new int[32];
            for (int i = 0; i < 32; i++)
            {
                result[i] = buffer[i];
            }
            return result;
        }
    }
    
    public static void WritePsv(string path, string[] lines)
    {
        throw new NotImplementedException();
    }
}