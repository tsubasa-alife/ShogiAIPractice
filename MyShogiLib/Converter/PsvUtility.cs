using MyShogi.Model.Shogi.Core;
namespace MyShogi.Model.Shogi.Converter;

/// <summary>
/// PackedSfenAndValueの読み書きを行うクラス
/// note:
/// PacedSfenAndValueは以下のような構造になっている
/// 「PackedSfen、評価値、指し手、手数、勝敗＋Padding（32 + 2 + 2 + 2 + 1 + 1 = 40bytes）」
/// </summary>
public class PsvUtility
{
    public static List<PackedSfenValue> ReadPsv(string path)
    {
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            using (BinaryReader reader = new BinaryReader(fs))
            {
                List<PackedSfenValue> psvList = new List<PackedSfenValue>();
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    psvList.Add(ReadPackedSfenValue(reader));
                }
                return psvList;
            }
        }
    }
    
    public static void WritePsv(string path, string[] lines)
    {
        throw new NotImplementedException();
    }
    
    public static PackedSfenValue ReadPackedSfenValue(BinaryReader reader)
    {
        PackedSfenValue psv = new PackedSfenValue();
        psv.packedSfen = new byte[32];
        for (int i = 0; i < 32; i++)
        {
            psv.packedSfen[i] = reader.ReadByte();
        }
        psv.move = reader.ReadUInt16();
        psv.score = reader.ReadInt16();
        psv.gamePly = reader.ReadUInt16();
        psv.gameResult = reader.ReadSByte();
        reader.ReadSByte(); // Paddingを読み飛ばす
        return psv;
    }
}