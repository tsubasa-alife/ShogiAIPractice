using System.Runtime.InteropServices;

namespace MyShogi.Model.Shogi.Core
{
	public struct PackedSfenValue
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public int[] packedSfen;
		public UInt16 move;
		public Int16 score;
		public UInt16 gamePly;
		public sbyte gameResult;
	}
}