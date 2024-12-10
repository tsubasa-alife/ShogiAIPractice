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

public class SfenPacker
{

}