namespace MyShogi.Model.Shogi.Core;

public struct HuffmanCode
{
	public int Value;
	public int NumOfBit;

	public HuffmanCode(int value, int numOfBit)
	{
		Value = value;
		NumOfBit = numOfBit;
	}
}