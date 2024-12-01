namespace MyShogi.Model.Shogi.Converter;

/// <summary>
/// PackedSfenAndValueの読み書きを行うクラス
/// </summary>
public class PsvUtility
{
    public static int[] ReadPsv(string path)
    {
        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
        {
            byte[] bufr = new byte[100];
            fs.Read(bufr, 0, 10);
            // 格納先はbufrのインデックス0～9。
            int[] result = new int[10];
            for (int i = 0; i < 10; i++)
            {
                result[i] = bufr[i];
            }
            return result;
        }
    }

    public static void WritePsv(string path, string[] lines)
    {
        throw new NotImplementedException();
    }
}