public class Csomag
{
    public string Cimzett { get; }
    public string Meret { get; }
    public string Statusz { get; private set; }
    private string Nyitokod { get; }
    public DateTime FeladasIdo { get; }
    public string CsomagAzonosito { get; }
    private string KiszallitasiCim { get; }

    public Csomag(string cimzett, string meret, string statusz, string nyitokod, DateTime feladasIdo, string csomagAzonosito, string szallitasiCim)
    {
        Cimzett = cimzett;
        Meret = meret;
        Statusz = statusz;
        Nyitokod = nyitokod;
        FeladasIdo = feladasIdo;
        CsomagAzonosito = csomagAzonosito;
        KiszallitasiCim = szallitasiCim;
    }

    public bool NyitokodEllenorzes(string kod)
    {
        return Nyitokod == kod;
    }

    public string getNyitoKod()
    {
        return Nyitokod;
    }
    public string getSzallitasiCim()
    {
        return KiszallitasiCim;
    }

    public void StatuszFrissit(string ujStatusz)
    {
        string[] sorok = File.ReadAllLines("csomagok.csv");

        for (int i = 0; i < sorok.Length; i++)
        {
            string[] oszlopok = sorok[i].Split(';');

            if (oszlopok.Length >= 7 && oszlopok[5] == CsomagAzonosito)
            {
                Statusz = ujStatusz;
                sorok[i] = ToCsvSor();
                break;
            }
        }

        File.WriteAllLines("csomagok.csv", sorok);

        // TODO: HA A CSOMAG ATVETT ALAPPOTBA VAN NE TUDD FRISSITENI AZ ALLAPOTAT
    }

    // Egy sor a csv fájlba: cimzett;meret;statusz;nyitokod;feladasIdo;csomagAzonosito;szallitasiCim
    public string ToCsvSor()
    {
        string ido = FeladasIdo.ToString("yyyy-MM-dd HH:mm:ss");
        return $"{Cimzett};{Meret};{Statusz};{Nyitokod};{ido};{CsomagAzonosito};{KiszallitasiCim}";
    }
}