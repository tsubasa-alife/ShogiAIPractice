using System.Runtime.InteropServices;

namespace MyShogi.Model.Shogi.Core
{
	public struct PackedSfenValue
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public byte[] sfen;
		public ushort move;
		public int score;
		public ushort gamePly;
		public sbyte game_result;
	}
}