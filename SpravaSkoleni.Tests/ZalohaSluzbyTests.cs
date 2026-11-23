using SpravaSkoleni.Zaloha;
using System.IO;
using System.Runtime.Versioning;
using Xunit;

namespace SpravaSkoleni.Tests;

[SupportedOSPlatform("windows")]
[Collection("PracovniAdresarZalohy")]
public sealed class ZalohaSluzbyTests : IDisposable
{
    private readonly string _puvodniAdresar;
    private readonly string _testAdresar;

    public ZalohaSluzbyTests()
    {
        _puvodniAdresar = Directory.GetCurrentDirectory();
        _testAdresar = Path.Combine(Path.GetTempPath(), "SpravaSkoleniTests_Zaloha_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testAdresar);
        Directory.SetCurrentDirectory(_testAdresar);
    }

    [Fact]
    public async Task ZkontrolujRocniArchivaciAsync_VytvoriArchivProPredchoziRok()
    {
        Directory.CreateDirectory("Skoleni");
        Directory.CreateDirectory("Studenti");
        Directory.CreateDirectory("Lektori");

        await File.WriteAllTextAsync("Skoleni/skoleni.json", """[{"id":1}]""");
        await File.WriteAllTextAsync("Studenti/studenti.json", """[{"id":1}]""");
        await File.WriteAllTextAsync("Lektori/lektori.json", """[{"id":1}]""");

        await ArchivacniSluzba.ZkontrolujRocniArchivaciAsync();

        int predchoziRok = DateTime.Now.Year - 1;
        Assert.True(File.Exists(Path.Combine("Archiv", $"skoleni_{predchoziRok}.json")));
        Assert.True(File.Exists(Path.Combine("Archiv", $"studenti_{predchoziRok}.json")));
        Assert.True(File.Exists(Path.Combine("Archiv", $"lektori_{predchoziRok}.json")));
    }

    [Fact]
    public async Task ZkontrolujRocniArchivaciAsync_SmazeArchivStarsiNezPetLet()
    {
        Directory.CreateDirectory("Archiv");
        int aktualniRok = DateTime.Now.Year;
        string staryArchiv = Path.Combine("Archiv", $"skoleni_{aktualniRok - 6}.json");
        string aktualniArchiv = Path.Combine("Archiv", $"skoleni_{aktualniRok - 1}.json");

        await File.WriteAllTextAsync(staryArchiv, "[]");
        await File.WriteAllTextAsync(aktualniArchiv, "[]");

        await ArchivacniSluzba.ZkontrolujRocniArchivaciAsync();

        Assert.False(File.Exists(staryArchiv));
        Assert.True(File.Exists(aktualniArchiv));
    }

    [Fact]
    public async Task ProvedAutomatickouZalohu_SFirmou_VytvoriZalohuVSlozceZalohy()
    {
        string zdroj = Path.Combine(_testAdresar, "studenti.json");
        await File.WriteAllTextAsync(zdroj, """[{"jmeno":"Jan"}]""");

        ZalohovaciSluzba.ProvedAutomatickouZalohu(zdroj, "FirmaA");

        string cil = Path.Combine(_testAdresar, "Zalohy", "studenti_FirmaA.bak.json");
        bool vytvoren = await PockejNaSouborAsync(cil);

        Assert.True(vytvoren);
        Assert.Equal("""[{"jmeno":"Jan"}]""", await File.ReadAllTextAsync(cil));
    }

    [Fact]
    public async Task ProvedAutomatickouZalohu_Globalni_VytvoriBakJson()
    {
        string zdroj = Path.Combine(_testAdresar, "skoleni.json");
        await File.WriteAllTextAsync(zdroj, """[{"nazev":"BOZP"}]""");

        ZalohovaciSluzba.ProvedAutomatickouZalohu(zdroj);

        string cil = Path.Combine(_testAdresar, "Zalohy", "skoleni.bak.json");
        bool vytvoren = await PockejNaSouborAsync(cil);

        Assert.True(vytvoren);
        Assert.Equal("""[{"nazev":"BOZP"}]""", await File.ReadAllTextAsync(cil));
    }

    private static async Task<bool> PockejNaSouborAsync(string cesta)
    {
        for (int i = 0; i < 40; i++)
        {
            if (File.Exists(cesta))
            {
                try
                {
                    using var _ = File.Open(cesta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    return true;
                }
                catch (IOException)
                {
                    // Soubor se ještě zapisuje
                }
            }

            await Task.Delay(50);
        }

        return false;
    }

    public void Dispose()
    {
        try
        {
            Directory.SetCurrentDirectory(_puvodniAdresar);
        }
        catch
        {
            // Ignorovat při úklidu
        }

        try
        {
            if (Directory.Exists(_testAdresar))
            {
                Directory.Delete(_testAdresar, recursive: true);
            }
        }
        catch
        {
            // Ignorovat při úklidu
        }
    }
}

[CollectionDefinition("PracovniAdresarZalohy", DisableParallelization = true)]
public sealed class PracovniAdresarZalohyCollection;