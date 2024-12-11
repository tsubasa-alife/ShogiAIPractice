using System.Diagnostics;
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
	
	public static void Pack(Position pos)
	{
		// Aperyの駒の並び順
		Piece[] toAperyPieces = { Piece.NO_PIECE, Piece.PAWN, Piece.LANCE, Piece.KNIGHT, Piece.SILVER, Piece.GOLD, Piece.BISHOP, Piece.ROOK };

		// 駒箱枚数
		int[] hpCount = { 0, 18, 4, 4, 4, 4, 2, 2 };

		Array.Clear(data, 0, data.Length);
		bitStream.SetData(data);

		// 手番
		bitStream.WriteOneBit((int)pos.sideToMove);

		// 先手玉、後手玉の位置、それぞれ7bit
		foreach (Color c in Enum.GetValues(typeof(Color)))
		{
			bitStream.WriteNBit((int)pos.KingSquare(c), 7);
		}

		// 盤面の駒は王以外はそのまま書き出して良し！
		foreach (Square sq in Enum.GetValues(typeof(Square)))
		{
			Piece pc = pos.PieceOn(sq);
			if (pc.PieceType() == Piece.KING)
				continue;
			WriteBoardPieceToStream(pc);

			// 駒箱から減らす
			hpCount[(int)pc.RawPieceType()]--;
		}

		// 手駒をハフマン符号化して書き出し
		foreach (Color c in Enum.GetValues(typeof(Color)))
		{
			for (Piece pr = Piece.PAWN; pr < Piece.KING; pr++)
			{
				Piece pr2 = toAperyPieces[(int)pr];
				int n = pos.Hand(c).Count(pr2);

				for (int i = 0; i < n; i++)
				{
					WriteHandPieceToStream(Util.MakePiece(c, pr2));
				}

				// 駒箱から減らす
				hpCount[(int)pr2] -= n;
			}
		}

		// 最後に駒箱の分を出力
		for (Piece pr = Piece.PAWN; pr < Piece.KING; pr++)
		{
			Piece pr2 = toAperyPieces[(int)pr];
			int n = hpCount[(int)pr2];

			for (int i = 0; i < n; i++)
			{
				WritePieceBoxPieceToStream(pr2);
			}
		}

		// 全部で256bitのはず。(普通の盤面であれば)
		Debug.Assert(bitStream.GetCursor() == 256);
	}
	
	public static string Unpack()
	{
		bitStream.SetData(data);

		// 盤上の81升
		Piece[] board = new Piece[81];
		Array.Fill(board, Piece.NO_PIECE);

		// 手番
		Color turn = (Color)bitStream.ReadOneBit();

		// まず玉の位置
		foreach (Color c in Enum.GetValues(typeof(Color)))
		{
			board[bitStream.ReadNBit(7)] = Util.MakePiece(c, Piece.KING);
		}

		// 盤上の駒
		foreach (Square sq in Enum.GetValues(typeof(Square)))
		{
			// すでに玉がいるようだ
			if (board[(int)sq].PieceType() == Piece.KING)
				continue;

			board[(int)sq] = ReadBoardPieceFromStream();

			Debug.Assert(bitStream.GetCursor() <= 256);
		}

		// 手駒
		Hand[] hand = { Hand.ZERO, Hand.ZERO };
		while (bitStream.GetCursor() != 256)
		{
			// 256になるまで手駒か駒箱の駒が格納されているはず
			Piece pc = ReadHandPieceFromStream();

			// 成り駒が返ってきたら、これは駒箱の駒。
			if (pc.IsPromote())
				continue;

			hand[(int)pc.PieceColor()].Add(pc.RawPieceType());
		}

		// boardとhandが確定した。これで局面を構築できる…かも。
		// Position::sfen()は、board,hand,side_to_move,game_plyしか参照しないので
		// 無理やり代入してしまえば、sfen()で文字列化できるはず。

		return Position.SfenFromRawdata(board, hand, turn, 0);
	}
	
	/// <summary>
	/// 盤面の駒をstreamに出力する
	/// </summary>
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
	
	/// <summary>
	/// 手駒をstreamに出力する
	/// </summary>
	public static void WriteHandPieceToStream(Piece pc)
	{
		if (pc == Piece.NO_PIECE)
		{
			throw new ArgumentException("Piece cannot be NO_PIECE", nameof(pc));
		}

		// Piece type
		Piece pr = pc.RawPieceType();
		HuffmanedPiece c = HuffmanTables.HuffmanTable[(int)pr];
		bitStream.WriteNBit(c.Code >> 1, c.Bits - 1);

		// For pieces other than GOLD, output the promotion flag (unpromoted) to maintain the same bit count as board pieces
		if (pr != Piece.GOLD)
		{
			bitStream.WriteOneBit(0);
		}

		// Color flag
		bitStream.WriteOneBit((int)pc.PieceColor());
	}
	
	/// <summary>
	/// 駒箱の駒をstreamに出力する
	/// </summary>
	public static void WritePieceBoxPieceToStream(Piece pr)
	{
		if (pr == Piece.NO_PIECE)
		{
			throw new ArgumentException("PieceType cannot be NO_PIECE_TYPE", nameof(pr));
		}

		// Piece type
		HuffmanedPiece c = HuffmanTables.HuffmanTablePieceBox[(int)pr];
		bitStream.WriteNBit(c.Code, c.Bits);

		// Output the promotion flag, so this ends the promotion part.

		// Write the color flag as 0. For GOLD, this flag is consumed (treated as a piece of the opponent), so this ends here.
		if (pr != Piece.GOLD)
		{
			bitStream.WriteOneBit(0);
		}
	}
	
	/// <summary>
	/// Streamから盤面の駒を読み込む
	/// </summary>
	public static Piece ReadBoardPieceFromStream()
	{
		Piece pr = Piece.NO_PIECE;
		int code = 0, bits = 0;
		while (true)
		{
			code |= bitStream.ReadOneBit() << bits;
			++bits;

			if (bits > 6)
			{
				throw new InvalidOperationException("Bits exceeded the maximum allowed value.");
			}

			for (pr = Piece.NO_PIECE; pr < Piece.KING; ++pr)
			{
				if (HuffmanTables.HuffmanTable[(int)pr].Code == code &&
				    HuffmanTables.HuffmanTable[(int)pr].Bits == bits)
				{
					goto Found;
				}
			}
		}
		Found:
		if (pr == Piece.NO_PIECE)
		{
			return Piece.NO_PIECE;
		}

		// Promotion flag
		bool promote = (pr == Piece.GOLD) ? false : bitStream.ReadOneBit() == 1;

		// Color flag
		Color c = (Color)bitStream.ReadOneBit();

		return Util.MakePiece(c, pr + (int)(promote ? Piece.PROMOTE : Piece.NO_PIECE));
	}
	
	/// <summary>
	/// Streamから手駒の駒を読み込む
	/// </summary>
	public static Piece ReadHandPieceFromStream()
	{
		Piece pr = Piece.NO_PIECE;
		int code = 0, bits = 0;
		while (true)
		{
			code |= bitStream.ReadOneBit() << bits;
			++bits;

			if (bits > 6)
			{
				throw new InvalidOperationException("Bits exceeded the maximum allowed value.");
			}

			for (pr = Piece.PAWN; pr < Piece.KING; ++pr)
			{
				if ((HuffmanTables.HuffmanTable[(int)pr].Code >> 1) == code &&
				    (HuffmanTables.HuffmanTable[(int)pr].Bits - 1) == bits)
				{
					goto Found;
				}
			}
		}
		Found:
		if (pr == Piece.NO_PIECE)
		{
			throw new InvalidOperationException("Piece type cannot be NO_PIECE.");
		}

		// For pieces other than GOLD, discard the promotion flag (if this is 1, it is a piece from the piece box, so return a promoted piece)
		if (pr != Piece.GOLD)
		{
			bool promote = bitStream.ReadOneBit() == 1;
			if (promote)
			{
				pr = pr.ToPromotePiece();
			}
		}

		// Color flag
		Color c = (Color)bitStream.ReadOneBit();

		return Util.MakePiece(c, pr);
	}
}