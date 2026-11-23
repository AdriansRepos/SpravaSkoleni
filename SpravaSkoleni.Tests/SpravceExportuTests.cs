using System.Runtime.Versioning;
using System.Text;
using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Logika;
using Xunit;

namespace SpravaSkoleni.Tests;

[SupportedOSPlatform("windows")]
public sealed class SpravceExportuTests : IDisposable
{
    private const int PocetPokusuMazani = 10;
    private const int ProdlevaMazaniMs = 50;

    private readonly string _testAdresar;

    public SpravceExportuTests()
    {
        _testAdresar = Path.Combine(Path.GetTempPath(), $"SpravaSkoleni_ExportTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testAdresar);
    }

    [Fact]
    public void SestavPrezencniListinu_NullSkoleni_VyhodiArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SpravceExportu.SestavPrezencniListinu(null!));
    }

    [Fact]
    public void SestavPrezencniListinu_BezLektoruAStudentu_ObsahujeVychoziStavy()
    {
        var skoleni = new Skoleni(
            "BOZP",
            "Školení BOZP",
            new DateTime(2026, 12, 1),
            new DateTime(2026, 12, 2),
            15,
            "Firma A");

        string vysledek = SpravceExportu.SestavPrezencniListinu(skoleni);

        Assert.Contains("Školení BOZP", vysledek);
        Assert.Contains("Není přiřazen", vysledek);
        Assert.Contains("(Žádní studenti nejsou přihlášeni)", vysledek);
    }

    [Fact]
    public async Task ExportujPrezencniListinuAsync_VytvoriSouborAUloziObsah()
    {
        string slozkaExportu = Path.Combine(_testAdresar, "Exporty");
        var skoleni = new Skoleni(
            "PO",
            "PO",
            new DateTime(2026, 12, 5),
            new DateTime(2026, 12, 5),
            20,
            "Firma A");

        string cestaKSouboru = await SpravceExportu.ExportujPrezencniListinuAsync(skoleni, slozkaExportu);

        Assert.True(File.Exists(cestaKSouboru));
        Assert.Contains("Firma A", Path.GetFileName(cestaKSouboru));
        Assert.Contains("PO", Path.GetFileName(cestaKSouboru));

        string obsah = await File.ReadAllTextAsync(cestaKSouboru, Encoding.UTF8);
        Assert.Contains("PO", obsah);
    }

    [Fact]
    public async Task ExportujPrezencniListinuAsync_NullSkoleni_VyhodiArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            SpravceExportu.ExportujPrezencniListinuAsync(null!, _testAdresar));
    }

    public void Dispose()
    {
        if (!Directory.Exists(_testAdresar))
        {
            return;
        }

        for (int i = 0; i < PocetPokusuMazani; i++)
        {
            try
            {
                Directory.Delete(_testAdresar, recursive: true);
                return;
            }
            catch (IOException) when (i < PocetPokusuMazani - 1)
            {
                Thread.Sleep(ProdlevaMazaniMs);
            }
            catch (UnauthorizedAccessException) when (i < PocetPokusuMazani - 1)
            {
                Thread.Sleep(ProdlevaMazaniMs);
            }
        }
    }
}