using MyShogi.Model.Shogi.Core;
namespace MyShogi.Model.Shogi.Converter;
	
// ビットストリームを扱うクラス
// 局面の符号化を行なうときに、これがあると便利
public class BitStream
{
	private int bitCursor;
	private byte[] data;

	// データを格納するメモリを事前にセットする。
	// そのメモリは0クリアされているものとする。
	public void SetData(byte[] data)
	{
		this.data = data;
		Reset();
	}

	// set_data()で渡されたポインタの取得。
	public byte[] GetData()
	{
		return data;
	}

	// カーソルの取得。
	public int GetCursor()
	{
		return bitCursor;
	}

	// カーソルのリセット
	public void Reset()
	{
		bitCursor = 0;
	}

	// ストリームに1bit書き出す。
	// bは非0なら1を書き出す。0なら0を書き出す。
	public void WriteOneBit(int b)
	{
		if (b != 0)
		{
			data[bitCursor / 8] |= (byte)(1 << (bitCursor & 7));
		}

		bitCursor++;
	}

	// ストリームから1ビット取り出す。
	public int ReadOneBit()
	{
		int b = (data[bitCursor / 8] >> (bitCursor & 7)) & 1;
		bitCursor++;
		return b;
	}

	// nビットのデータを書き出す
	// データはdの下位から順に書き出されるものとする。
	public void WriteNBit(int d, int n)
	{
		for (int i = 0; i < n; i++)
		{
			WriteOneBit(d & (1 << i));
		}
	}

	// nビットのデータを読み込む
	// write_n_bit()の逆変換。
	public int ReadNBit(int n)
	{
		int result = 0;
		for (int i = 0; i < n; i++)
		{
			if (ReadOneBit() != 0)
			{
				result |= (1 << i);
			}
		}
		return result;
	}
}

public struct HuffmanedPiece
{
	public int Code; // どうコード化されるか
	public int Bits; // 何bit専有するのか

	public HuffmanedPiece(int code, int bits)
	{
		Code = code;
		Bits = bits;
	}
}

public static class HuffmanTables
{
	public static readonly HuffmanedPiece[] HuffmanTable = new HuffmanedPiece[]
	{
		new HuffmanedPiece(0x00, 1), // NO_PIECE
		new HuffmanedPiece(0x01, 2), // PAWN
		new HuffmanedPiece(0x03, 4), // LANCE
		new HuffmanedPiece(0x0b, 4), // KNIGHT
		new HuffmanedPiece(0x07, 4), // SILVER
		new HuffmanedPiece(0x1f, 6), // BISHOP
		new HuffmanedPiece(0x3f, 6), // ROOK
		new HuffmanedPiece(0x0f, 5), // GOLD
	};

	public static readonly HuffmanedPiece[] HuffmanTablePieceBox = new HuffmanedPiece[]
	{
		new HuffmanedPiece(0x00, 1), // not use
		new HuffmanedPiece(0x02, 2), // PAWN
		new HuffmanedPiece(0x09, 4), // LANCE
		new HuffmanedPiece(0x0d, 4), // KNIGHT
		new HuffmanedPiece(0x0b, 4), // SILVER
		new HuffmanedPiece(0x2f, 6), // BISHOP
		new HuffmanedPiece(0x3f, 6), // ROOK
		new HuffmanedPiece(0x1b, 5), // GOLD
	};
}

public class SfenPacker
{
	public static byte[] data = new byte[32];
	public static BitStream bitStream = new BitStream();
	
	// 盤面の駒をstreamに出力する。
	public static void WriteBoardPieceToStream(Piece pc)
	{
		// 駒種
		Piece pr = pc.RawPieceType();
		HuffmanedPiece c = HuffmanTables.HuffmanTable[(int)pr];
		bitStream.WriteNBit(c.Code, c.Bits);

		if (pc == Piece.NO_PIECE)
			return;

		// 成りフラグ
		// (金はこのフラグはない)
		if (pr != Piece.GOLD)
			bitStream.WriteOneBit((pc & Piece.PROMOTE) != 0 ? 1 : 0);

		// 先後フラグ
		bitStream.WriteOneBit((int)pc.PieceColor());
	}
}