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
    public static PackedSfenValue ReadPsv(string path)
    {
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            using (BinaryReader reader = new BinaryReader(fs))
            {
                PackedSfenValue psv = ReadPackedSfenValue(reader);
                return psv;
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
        psv.sfen = reader.ReadBytes(32);
        psv.move = reader.ReadUInt16();
        psv.score = reader.ReadInt32();
        psv.gamePly = reader.ReadUInt16();
        psv.game_result = reader.ReadSByte();
        return psv;
    }
}