using System.Runtime.Versioning;
using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Data.Repozitare;
using Xunit;

namespace SpravaSkoleni.Tests;

[SupportedOSPlatform("windows")]
public sealed class RepozitareTests : IDisposable
{
    private const int PocetPokusuMazani = 10;
    private const int ProdlevaMazaniMs = 50;

    private readonly string _testAdresar;

    public RepozitareTests()
    {
        _testAdresar = Path.Combine(Path.GetTempPath(), $"SpravaSkoleni_RepoTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testAdresar);
    }

    [Fact]
    public async Task LektoriRepository_UlozitANacist_ZachovaSeznam()
    {
        var repo = new LektoriRepository(Path.Combine(_testAdresar, "Lektori"));
        var lektori = new List<Lektor>
        {
            new("Petr", "Dvořák", "L01", "Školitel BOZP")
        };

        await repo.UlozitAsync(lektori);
        var nacteni = await repo.NacistVsechnyAsync();

        Assert.Single(nacteni);
        Assert.Equal("L01", nacteni[0].Kod);
        Assert.Equal("Petr", nacteni[0].Jmeno);
    }

    [Fact]
    public async Task StudentiRepository_ChybnyHlavniSoubor_ObnoviZeZalohy()
    {
        string slozkaStudenti = Path.Combine(_testAdresar, "Studenti");
        string slozkaZalohy = Path.Combine(_testAdresar, "Zalohy");
        Directory.CreateDirectory(slozkaZalohy);

        var repo = new StudentiRepository(slozkaStudenti);
        await repo.UlozitAsync([new Student("Jana", "Nová", "S01", "Operátorka", "Firma A")]);

        string cestaStudenti = Path.Combine(slozkaStudenti, "studenti.json");
        File.Copy(cestaStudenti, Path.Combine(slozkaZalohy, "studenti.bak.json"), overwrite: true);

        for (int i = 0; i < PocetPokusuMazani; i++)
        {
            try
            {
                File.Delete(cestaStudenti);
                break;
            }
            catch (IOException) when (i < PocetPokusuMazani - 1)
            {
                await Task.Delay(ProdlevaMazaniMs);
            }
            catch (UnauthorizedAccessException) when (i < PocetPokusuMazani - 1)
            {
                await Task.Delay(ProdlevaMazaniMs);
            }
        }

        var obnoveni = await repo.NacistVsechnyAsync();

        Assert.Single(obnoveni);
        Assert.Equal("S01", obnoveni[0].Kod);
        Assert.Equal("Firma A", obnoveni[0].Firma);
    }

    [Fact]
    public async Task SkoleniRepository_MultiTenancy_ZachovaOstatniFirmyAVynutiAktualniFirmu()
    {
        string slozkaSkoleni = Path.Combine(_testAdresar, "Skoleni");

        var repoFirmaA = new SkoleniRepository("Firma A", slozkaSkoleni);
        var repoFirmaB = new SkoleniRepository("Firma B", slozkaSkoleni);

        var skoleniA = new Skoleni(
            "BOZP",
            "Školení A",
            new DateTime(2026, 12, 1),
            new DateTime(2026, 12, 1),
            10,
            "JinaFirmaBudePrepsana");

        var skoleniB = new Skoleni(
            "PO",
            "Školení B",
            new DateTime(2026, 12, 2),
            new DateTime(2026, 12, 2),
            12,
            "Firma B");

        await repoFirmaA.UlozitAsync([skoleniA]);
        await repoFirmaB.UlozitAsync([skoleniB]);

        var nactenaA = await repoFirmaA.NacistAsync();
        var nactenaB = await repoFirmaB.NacistAsync();

        Assert.Single(nactenaA);
        Assert.Equal("Firma A", nactenaA[0].Firma);
        Assert.Equal("BOZP", nactenaA[0].Nazev);

        Assert.Single(nactenaB);
        Assert.Equal("Firma B", nactenaB[0].Firma);
        Assert.Equal("PO", nactenaB[0].Nazev);
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