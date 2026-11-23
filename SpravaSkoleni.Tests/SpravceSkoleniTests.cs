using System.Runtime.Versioning;
using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Dto;
using SpravaSkoleni.Logika;
using Xunit;

namespace SpravaSkoleni.Tests;

[SupportedOSPlatform("windows")]
public sealed class SpravceSkoleniTests : IDisposable
{
    private const int PocetPokusuMazani = 10;
    private const int ProdlevaMazaniMs = 50;

    private readonly string _testAdresar;

    public SpravceSkoleniTests()
    {
        _testAdresar = Path.Combine(Path.GetTempPath(), $"SpravaSkoleniTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testAdresar);
        Directory.CreateDirectory(Path.Combine(_testAdresar, "Skoleni"));
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

    [Fact]
    public async Task PridatSkoleniAsync_KoncoveDatumPredPocatecnim_VyhodiArgumentException()
    {
        var spravce = new SpravceSkoleni("FirmaA");
        var skoleni = new Skoleni(
            "Kurz C#",
            "Popis",
            new DateTime(2026, 12, 10),
            new DateTime(2026, 12, 5),
            doporucenyPocetUcastniku: 10,
            firma: "");

        await Assert.ThrowsAsync<ArgumentException>(() => spravce.PridatSkoleniAsync(skoleni));
    }

    [Fact]
    public async Task PrihlasStudentaAsync_PrekroceniKapacity_VyhodiInvalidOperationException()
    {
        var spravce = new SpravceSkoleni("FirmaA");
        var skoleni = new Skoleni(
            "Bezpečnost",
            "Popis",
            new DateTime(2026, 12, 1),
            new DateTime(2026, 12, 2),
            doporucenyPocetUcastniku: 1,
            firma: "FirmaA");

        await spravce.PridatSkoleniAsync(skoleni);

        var student1 = new Student("Jan", "Novák", "S1", "Operátor", "FirmaA");
        var student2 = new Student("Petr", "Svoboda", "S2", "Operátor", "FirmaA");

        await spravce.PrihlasStudentaAsync(skoleni, student1);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            spravce.PrihlasStudentaAsync(skoleni, student2));
    }

    [Fact]
    public async Task PrihlasStudentaAsync_PrekryvTerminu_VyhodiInvalidOperationException()
    {
        var spravce = new SpravceSkoleni("FirmaA");
        var skoleni1 = new Skoleni(
            "Kurz 1",
            "Popis 1",
            new DateTime(2026, 12, 1),
            new DateTime(2026, 12, 5),
            doporucenyPocetUcastniku: 10,
            firma: "FirmaA");

        var skoleni2 = new Skoleni(
            "Kurz 2",
            "Popis 2",
            new DateTime(2026, 12, 4),
            new DateTime(2026, 12, 8),
            doporucenyPocetUcastniku: 10,
            firma: "FirmaA");

        await spravce.PridatSkoleniAsync(skoleni1);
        await spravce.PridatSkoleniAsync(skoleni2);

        var student = new Student("Jan", "Novák", "S1", "Operátor", "FirmaA");

        await spravce.PrihlasStudentaAsync(skoleni1, student);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            spravce.PrihlasStudentaAsync(skoleni2, student));
    }

    [Fact]
    public async Task PrevedNaStudentyAsync_FiltrujePrazdnyKodADoplniNeuvedeno()
    {
        var spravce = new SpravceSkoleni("FirmaA");
        var dtoList = new[]
        {
            new ImportovanyStudentDto("  Eva ", " Malá ", " K10 ", "   "),
            new ImportovanyStudentDto("Karel", "Velký", "", "Specialista")
        };

        var studenti = await spravce.PrevedNaStudentyAsync(dtoList, ulozitNoveDoKmenu: false);

        Assert.Single(studenti);
        Assert.Equal("K10", studenti[0].Kod);
        Assert.Equal("Neuvedeno", studenti[0].PracovniPozice);
    }
}