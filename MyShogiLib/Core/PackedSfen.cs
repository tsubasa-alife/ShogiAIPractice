namespace MyShogi.Model.Shogi.Core;

public static class PackedSfen
{
    public static readonly HuffmanCode[] BoardCodeTable = new HuffmanCode[]
    {
        new HuffmanCode(0, 1),    // Empty
        new HuffmanCode(1, 4),    // BPawn
        new HuffmanCode(3, 6),    // BLance
        new HuffmanCode(11, 6),   // BKnight
        new HuffmanCode(7, 6),    // BSilver
        new HuffmanCode(31, 8),   // BBishop
        new HuffmanCode(63, 8),   // BRook
        new HuffmanCode(15, 6),   // BGold
        new HuffmanCode(0, 0),    // BKing
        new HuffmanCode(5, 4),    // BProPawn
        new HuffmanCode(19, 6),   // BProLance
        new HuffmanCode(27, 6),   // BProKnight
        new HuffmanCode(23, 6),   // BProSilver
        new HuffmanCode(95, 8),   // BHorse
        new HuffmanCode(127, 8),  // BDragon
        new HuffmanCode(0, 0),    // Unused
        new HuffmanCode(0, 0),    // Unused
        new HuffmanCode(9, 4),    // WPawn
        new HuffmanCode(35, 6),   // WLance
        new HuffmanCode(43, 6),   // WKnight
        new HuffmanCode(39, 6),   // WSilver
        new HuffmanCode(159, 8),  // WBishop
        new HuffmanCode(191, 8),  // WRook
        new HuffmanCode(47, 6),   // WGold
        new HuffmanCode(0, 0),    // WKing
        new HuffmanCode(13, 4),   // WProPawn
        new HuffmanCode(51, 6),   // WProLance
        new HuffmanCode(59, 6),   // WProKnight
        new HuffmanCode(55, 6),   // WProSilver
        new HuffmanCode(223, 8),  // WHorse
        new HuffmanCode(255, 8)   // WDragon
    };

    public static readonly HuffmanCode[,] HandCodeTable = new HuffmanCode[7, 2]
    {
        { new HuffmanCode(0, 3), new HuffmanCode(4, 3) },       // HPawn
        { new HuffmanCode(1, 5), new HuffmanCode(17, 5) },      // HLance
        { new HuffmanCode(5, 5), new HuffmanCode(21, 5) },      // HKnight
        { new HuffmanCode(3, 5), new HuffmanCode(19, 5) },      // HSilver
        { new HuffmanCode(7, 5), new HuffmanCode(23, 5) },      // HGold
        { new HuffmanCode(15, 7), new HuffmanCode(79, 7) },     // HBishop
        { new HuffmanCode(31, 7), new HuffmanCode(95, 7) }      // HRook
    };
}