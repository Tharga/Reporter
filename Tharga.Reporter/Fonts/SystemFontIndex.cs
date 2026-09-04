using System.Buffers.Binary;
using System.Text;

namespace Tharga.Reporter.Fonts;

internal static class SystemFontIndex
{
    private const int NameTableFamilyId = 1;
    private const int NameTableSubfamilyId = 2;
    private const uint TrueTypeCollectionTag = 0x74746366;

    private static readonly Lazy<IReadOnlyDictionary<string, string>> Index = new(Build);

    internal static bool TryGetFontFile(string familyName, bool isBold, bool isItalic, out string path)
    {
        path = null;
        if (string.IsNullOrWhiteSpace(familyName)) return false;

        foreach (var key in GetCandidateKeys(familyName, isBold, isItalic))
        {
            if (Index.Value.TryGetValue(key, out path)) return true;
        }

        return false;
    }

    private static IEnumerable<string> GetCandidateKeys(string familyName, bool isBold, bool isItalic)
    {
        yield return BuildKey(familyName, isBold, isItalic);

        if (isBold && isItalic)
        {
            yield return BuildKey(familyName, true, false);
            yield return BuildKey(familyName, false, true);
        }

        yield return BuildKey(familyName, false, false);
    }

    private static string BuildKey(string familyName, bool isBold, bool isItalic)
    {
        return $"{familyName.Trim().ToLowerInvariant()}|{(isBold ? 1 : 0)}|{(isItalic ? 1 : 0)}";
    }

    private static IReadOnlyDictionary<string, string> Build()
    {
        var index = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var file in EnumerateFontFiles())
        {
            try
            {
                if (!TryReadFamily(file, out var family, out var isBold, out var isItalic)) continue;

                var key = BuildKey(family, isBold, isItalic);
                if (!index.ContainsKey(key))
                {
                    index.Add(key, file);
                }
            }
            catch (Exception)
            {
                // A font file that cannot be parsed is skipped rather than failing the whole index.
            }
        }

        return index;
    }

    private static IEnumerable<string> EnumerateFontFiles()
    {
        foreach (var directory in GetFontDirectories())
        {
            if (!Directory.Exists(directory)) continue;

            IEnumerable<string> files;
            try
            {
                files = Directory.EnumerateFiles(directory, "*.tt*", SearchOption.AllDirectories);
            }
            catch (Exception)
            {
                continue;
            }

            foreach (var file in files)
            {
                yield return file;
            }
        }
    }

    private static IEnumerable<string> GetFontDirectories()
    {
        yield return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts");
        yield return "/usr/share/fonts";
        yield return "/usr/local/share/fonts";
        yield return "/Library/Fonts";
        yield return "/System/Library/Fonts";

        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (!string.IsNullOrEmpty(home))
        {
            yield return Path.Combine(home, ".fonts");
            yield return Path.Combine(home, ".local", "share", "fonts");
            yield return Path.Combine(home, "Library", "Fonts");
        }
    }

    private static bool TryReadFamily(string path, out string family, out bool isBold, out bool isItalic)
    {
        family = null;
        isBold = false;
        isItalic = false;

        var data = File.ReadAllBytes(path);
        if (data.Length < 12) return false;

        var offset = 0u;
        if (BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(0, 4)) == TrueTypeCollectionTag)
        {
            if (data.Length < 16) return false;
            offset = BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(12, 4));
        }

        if (!TryFindNameTable(data, offset, out var nameOffset)) return false;
        if (!TryReadNames(data, nameOffset, out family, out var subFamily)) return false;

        var style = (subFamily ?? string.Empty).ToLowerInvariant();
        isBold = style.Contains("bold");
        isItalic = style.Contains("italic") || style.Contains("oblique");

        return !string.IsNullOrWhiteSpace(family);
    }

    private static bool TryFindNameTable(byte[] data, uint offset, out uint nameOffset)
    {
        nameOffset = 0;

        if (data.Length < offset + 12) return false;

        var tableCount = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan((int)offset + 4, 2));

        for (var i = 0; i < tableCount; i++)
        {
            var entry = (int)offset + 12 + i * 16;
            if (data.Length < entry + 16) return false;

            if (Encoding.ASCII.GetString(data, entry, 4) != "name") continue;

            nameOffset = BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(entry + 8, 4));
            return nameOffset + 6 <= data.Length;
        }

        return false;
    }

    private static bool TryReadNames(byte[] data, uint nameOffset, out string family, out string subFamily)
    {
        family = null;
        subFamily = null;

        var count = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan((int)nameOffset + 2, 2));
        var stringOffset = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan((int)nameOffset + 4, 2));

        for (var i = 0; i < count; i++)
        {
            var record = (int)nameOffset + 6 + i * 12;
            if (data.Length < record + 12) break;

            var platformId = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(record, 2));
            var nameId = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(record + 6, 2));
            var length = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(record + 8, 2));
            var valueOffset = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(record + 10, 2));

            if (nameId != NameTableFamilyId && nameId != NameTableSubfamilyId) continue;

            var start = (int)nameOffset + stringOffset + valueOffset;
            if (start + length > data.Length) continue;

            var value = platformId == 1
                ? Encoding.ASCII.GetString(data, start, length)
                : Encoding.BigEndianUnicode.GetString(data, start, length);

            if (nameId == NameTableFamilyId)
            {
                family ??= value;
            }
            else
            {
                subFamily ??= value;
            }
        }

        return family != null;
    }
}
