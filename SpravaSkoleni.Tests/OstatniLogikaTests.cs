using System.Runtime.Versioning;
using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Dto;
using SpravaSkoleni.Enumy;
using SpravaSkoleni.Logika;
using SpravaSkoleni.Logika.ImportExport;
using SpravaSkoleni.Menu;
using SpravaSkoleni.Rozhrani.ImportExport;
using Xunit;

namespace SpravaSkoleni.Tests;

[SupportedOSPlatform("windows")]
public sealed class OstatniLogikaTests
{
    private sealed class FakeStudentReader(IReadOnlyList<ImportovanyStudentDto> data) : IFormatStudentReader
    {
        public bool PodporujePriponu(string pripona)
            => string.Equals(pripona, ".fake", StringComparison.OrdinalIgnoreCase);

        public Task<IReadOnlyList<ImportovanyStudentDto>> NactiAsync(string cestaKSouboru)
            => Task.FromResult(data);
    }

    [Fact]
    public async Task ImportujZeSouboruAsync_ChybnySoubor_VyhodiFileNotFoundException()
    {
        var service = new StudentImportService([new FakeStudentReader([])]);

        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            service.ImportujZeSouboruAsync("neexistujici_soubor.fake"));
    }

    [Fact]
    public async Task ImportujZeSouboruAsync_CistiDataAOrezavaMezery()
    {
        string tempFile = Path.GetTempFileName();
        string fakeFile = Path.ChangeExtension(tempFile, ".fake");
        File.Move(tempFile, fakeFile, overwrite: true);

        try
        {
            var fakeData = new[]
            {
                new ImportovanyStudentDto(" Jan ", " Novák ", " 101 ", ""),
                new ImportovanyStudentDto("", "Bezejména", "102", "Pozice")
            };

            var service = new StudentImportService([new FakeStudentReader(fakeData)]);
            var vysledek = await service.ImportujZeSouboruAsync(fakeFile);

            Assert.Single(vysledek);
            Assert.Equal("Jan", vysledek[0].Jmeno);
            Assert.Equal("Novák", vysledek[0].Prijmeni);
            Assert.Equal("101", vysledek[0].Kod);
            Assert.Equal("Neuvedeno", vysledek[0].PracovniPozice);
        }
        finally
        {
            if (File.Exists(fakeFile))
            {
                File.Delete(fakeFile);
            }
        }
    }

    [Fact]
    public void SestavPrezencniListinu_BezStudentuALektoru_ObsahujeVychoziTexty()
    {
        var skoleni = new Skoleni(
            "BOZP",
            "Základní školení",
            new DateTime(2026, 12, 1),
            new DateTime(2026, 12, 1),
            doporucenyPocetUcastniku: 15,
            firma: "FirmaA");

        string listina = SpravceExportu.SestavPrezencniListinu(skoleni);

        Assert.Contains("BOZP", listina);
        Assert.Contains("Není přiřazen", listina);
        Assert.Contains("(Žádní studenti nejsou přihlášeni)", listina);
        Assert.Contains("Kapacita: 0 / 15", listina);
    }

    [Theory]
    [InlineData("1", HlavniMenuVolba.SpravaSkoleni)]
    [InlineData(" 2 ", HlavniMenuVolba.ExportPrezencniListiny)]
    [InlineData("K", HlavniMenuVolba.KonecProgramu)]
    [InlineData("x", HlavniMenuVolba.NeplatnaVolba)]
    public void HlavniMenu_ZpracujVolbu_VraciOcekavanyEnum(string vstup, HlavniMenuVolba ocekavanaVolba)
    {
        var vysledek = HlavniMenu.ZpracujVolbu(vstup);
        Assert.Equal(ocekavanaVolba, vysledek);
    }
}