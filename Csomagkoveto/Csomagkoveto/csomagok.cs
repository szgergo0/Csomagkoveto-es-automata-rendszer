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


    public bool AtmenetEngedelyezett(string ujStatusz)
    {
        if (Statusz == "Feladva" && ujStatusz == "Kiszállítás alatt")
        {
            return true;
        }

        if (Statusz == "Kiszállítás alatt" && ujStatusz == "Kiszállított")
        {
            return true;
        }

        if (Statusz != "Átvett" && Statusz != "Elutasított" && ujStatusz == "Elutasított")
        {
            return true;
        }

        if (Statusz == "Kiszállított" && ujStatusz == "Átvett")
        {
            return true;
        }

        return false;
    }

    public bool StatuszFrissit(string ujStatusz)
    {
        if (!AtmenetEngedelyezett(ujStatusz)) return false;

        string[] sorok = File.ReadAllLines("csomagok.csv");

        for (int i = 0; i < sorok.Length; i++)
        {
            string[] oszlopok = sorok[i].Split(';');

            if (oszlopok.Length >= 7 && oszlopok[5] == CsomagAzonosito)
            {
                Statusz = ujStatusz;
                sorok[i] = ToCsvSor();
                File.WriteAllLines("csomagok.csv", sorok);
                return true;
            }
        }
        return false;
    }
    
    // Egy sor a csv fájlba: cimzett;meret;statusz;nyitokod;feladasIdo;csomagAzonosito;szallitasiCim
    public string ToCsvSor()
    {
        string ido = FeladasIdo.ToString("yyyy-MM-dd HH:mm:ss");
        return $"{Cimzett};{Meret};{Statusz};{Nyitokod};{ido};{CsomagAzonosito};{KiszallitasiCim}";
    }
}