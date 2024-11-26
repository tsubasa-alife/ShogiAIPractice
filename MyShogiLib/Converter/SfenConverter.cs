using System;
using System.Collections.Generic;

namespace MyShogi.Model.Shogi.Converter;

public class SfenConverter
{
    // Bitstreams
    public static IEnumerable<int> Int2Bits(int x, int n)
    {
        for (int i = 0; i < n; i++)
        {
            yield return (x >> i) & 1;
        }
    }

    public static IEnumerable<int> Bits2Int8(IEnumerable<int> bits)
    {
        int y = 0;
        int j = 0;
        foreach (var (i, x) in bits.Select((x, i) => (i, x)))
        {
            j = i & 7;
            y |= x << j;
            if (j == 7)
            {
                yield return y;
                y = 0;
            }
        }
        if (j != 7)
        {
            yield return y;
        }
    }

    public static IEnumerable<int> Int8s2Bits(IEnumerable<int> int8s)
    {
        foreach (var i in int8s)
        {
            foreach (var bit in Int2Bits(i, 8))
            {
                yield return bit;
            }
        }
    }

    public static int Bits2Int(IEnumerator<int> gen, int n)
    {
        int x = 0;
        for (int i = 0; i < n; i++)
        {
            gen.MoveNext();
            x |= gen.Current << i;
        }
        return x;
    }

    // Converter from sfen to packed sfen
    private static readonly Dictionary<char, (int, int)> SfenDict = new Dictionary<char, (int, int)>
    {
        {'P', (1, 0)}, {'L', (2, 0)}, {'N', (3, 0)}, {'S', (4, 0)}, {'G', (5, 0)}, {'B', (6, 0)}, {'R', (7, 0)}, {'K', (8, 0)},
        {'p', (1, 1)}, {'l', (2, 1)}, {'n', (3, 1)}, {'s', (4, 1)}, {'g', (5, 1)}, {'b', (6, 1)}, {'r', (7, 1)}, {'k', (8, 1)}
    };

    private static readonly int[][] Huffman = new int[][]
    {
        new int[] {0},               // PAWN
        new int[] {1, 0, 0},         // LANCE
        new int[] {1, 0, 1},         // KNIGHT
        new int[] {1, 1, 0},         // SILVER
        new int[] {1, 1, 1, 0},      // GOLD
        new int[] {1, 1, 1, 1, 0},   // BISHOP
        new int[] {1, 1, 1, 1, 1}    // ROOK
    };

    public static IEnumerable<int> PackBoard(string board)
    {
        var l = new List<int>();
        var king = new int[2];
        int promoted = 0;
        var rows = board.Split('/');
        for (int y = 0; y < rows.Length; y++)
        {
            var rank = rows[y];
            int x = 0;
            foreach (var code in rank)
            {
                if (code == '+')
                {
                    promoted = 1;
                    continue;
                }
                if (char.IsDigit(code))
                {
                    for (int i = 0; i < int.Parse(code.ToString()); i++)
                    {
                        l.Add(0);
                        x++;
                    }
                }
                else
                {
                    var (p, c) = SfenDict[code];
                    if (p == 8)
                    {
                        king[c] = y * 9 + x;
                    }
                    else
                    {
                        l.Add(1);
                        l.AddRange(Huffman[p - 1]);
                        if (p != 5)
                        {
                            l.Add(promoted);
                        }
                        promoted = 0;
                        l.Add(c);
                    }
                    x++;
                }
            }
        }
        foreach (var bit in Int2Bits(king[0], 7)) yield return bit;
        foreach (var bit in Int2Bits(king[1], 7)) yield return bit;
        foreach (var bit in l) yield return bit;
    }

    public static IEnumerable<int> PackTurn(char turn)
    {
        yield return turn == 'b' ? 0 : 1;
    }

    public static IEnumerable<int> PackHands(string hands)
    {
        if (hands == "-") yield break;
        int n = 1;
        foreach (var code in hands)
        {
            if (char.IsDigit(code))
            {
                n = n * 10 + int.Parse(code.ToString());
            }
            else
            {
                var (p, c) = SfenDict[code];
                for (int i = 0; i < n; i++)
                {
                    foreach (var bit in Huffman[p - 1]) yield return bit;
                    if (p != 5) yield return 0;
                    yield return c;
                }
                n = 1;
            }
        }
    }

    public static IEnumerable<int> Pack2Bits(string sfen)
    {
        var parts = sfen.Split();
        var board = parts[0];
        var turn = parts[1][0];
        var hands = parts[2];
        yield return PackTurn(turn).First();
        foreach (var bit in PackBoard(board)) yield return bit;
        foreach (var bit in PackHands(hands)) yield return bit;
    }

    public static IEnumerable<int> Pack(string sfen)
    {
        foreach (var b in Bits2Int8(Pack2Bits(sfen))) yield return b;
    }

    // Converter from packed sfen to sfen
    private static readonly Dictionary<int, string> HuffmanInvert = new Dictionary<int, string>
    {
        {0, "Pp"}, {4, "Ll"}, {5, "Nn"}, {6, "Ss"}, {14, "Gg"}, {30, "Bb"}, {31, "Rr"}
    };

    public static string Bits2Piece(IEnumerator<int> gen)
    {
        string sym = null;
        int x = 0;
        for (int i = 0; i < 32; i++)
        {
            gen.MoveNext();
            x = (x << 1) | gen.Current;
            if (HuffmanInvert.TryGetValue(x, out sym))
            {
                break;
            }
        }
        if (sym == null) throw new InvalidOperationException("Invalid piece encoding");
        bool promoted = sym == "Gg" ? false : gen.MoveNext() && gen.Current == 1;
        char piece = sym[gen.MoveNext() && gen.Current == 1 ? 1 : 0];
        return promoted ? "+" + piece : piece.ToString();
    }

    public static IEnumerable<string> UnpackCompactRow(IEnumerable<int> row)
    {
        int zrl = 0;
        foreach (var s in row)
        {
            if (s == 0)
            {
                zrl++;
            }
            else
            {
                if (zrl > 0)
                {
                    yield return zrl.ToString();
                    zrl = 0;
                }
                yield return s.ToString();
            }
        }
        if (zrl > 0) yield return zrl.ToString();
    }

    public static string UnpackBoard(IEnumerable<int> flatboard)
    {
        var rows = new List<string>();
        for (int y = 0; y < 9; y++)
        {
            var row = flatboard.Skip(y * 9).Take(9);
            rows.Add(string.Join("", UnpackCompactRow(row)));
        }
        return string.Join("/", rows);
    }

    public static IEnumerable<string> UnpackCompactHands(IEnumerable<string> hands)
    {
        if (!hands.Any())
        {
            yield return "-";
        }
        else
        {
            int n = 0;
            string q = null;
            foreach (var p in hands)
            {
                if (q == null)
                {
                    n = 1;
                    q = p;
                }
                else if (q == p)
                {
                    n++;
                }
                else
                {
                    if (n > 1) yield return n.ToString();
                    yield return q;
                    n = 1;
                    q = p;
                }
            }
            if (q != null)
            {
                if (n > 1) yield return n.ToString();
                yield return q;
            }
        }
    }

    public static string UnpackHands(IEnumerable<string> hands)
    {
        return string.Join("", UnpackCompactHands(hands));
    }

    public static string Unpack(IEnumerable<int> psfen, int ply = 1)
    {
        var gen = psfen.GetEnumerator();
        gen.MoveNext();
        char turn = gen.Current == 0 ? 'b' : 'w';
        int[] king = { Bits2Int(gen, 7), Bits2Int(gen, 7) };
        Array.Sort(king);
        var board = new List<int>();
        for (int i = 0; i < 79; i++)
        {
            gen.MoveNext();
            board.Add(gen.Current == 1 ? Bits2Piece(gen).First() : 0);
        }
        foreach (var k in king)
        {
            if (k >= 0 && k <= board.Count)
            {
                board.Insert(k, k == king[0] ? 'K' : 'k');
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(k), "King position is out of bounds");
            }
        }
        var hands = new List<string>();
        while (gen.MoveNext())
        {
            hands.Add(Bits2Piece(gen));
        }
        return $"{UnpackBoard(board)} {turn} {UnpackHands(hands)} {ply}";
    }
}