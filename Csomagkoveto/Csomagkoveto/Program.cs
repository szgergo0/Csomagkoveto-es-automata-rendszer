using System.IO;
using System.Linq;

if (!File.Exists("csomagok.csv"))
{
    File.WriteAllText("csomagok.csv", "");
}

List<Csomag> csomagok = new();

foreach (string sor in File.ReadLines("csomagok.csv"))
{
    if (string.IsNullOrWhiteSpace(sor)) continue;

    string[] mezok = sor.Split(';');
    if (mezok.Length != 7 || !DateTime.TryParse(mezok[4], out DateTime ido))
    {
        Console.WriteLine($"Hibás sor: {sor}");
        continue;
    }

    csomagok.Add(new Csomag(mezok[0], mezok[1], mezok[2], mezok[3], ido, mezok[5], mezok[6]));
}

bool checkNev(string nev)
{

    foreach (char karakter in nev)
    {

        if (char.IsDigit(karakter))
        {
            return false;
        }

    }

    if (string.IsNullOrWhiteSpace(nev) || nev.Contains(';')) return false;

    return true;
}

bool checkMeret(string meret)
{
    meret = meret.ToLower();
    if (meret == "s" || meret == "m" || meret == "l")
    {
        return true;
    }

    return false;
}

string muveletValasztas()
{
    string x = Console.ReadLine();
    while (x != "1" && x != "2" && x != "3" && x != "4" && x != "0" )
    {
        Console.WriteLine("Helytelen bemenet! Helyes bemenetek: 0. Kilépés | 1. Új csomag feladása | 2. Csomag átvétele | 3. Csomagkövetés | 4. Csomag állapotának megváltoztatása (Futár művelet) ");
        x = Console.ReadLine();
    }
    return x;
}

void kilepes()
{
    Environment.Exit(0);
}

string ujCsomagAzonosito()
{
    string azonosito;
    do
    {
        char betu1 = (char)Random.Shared.Next('A', 'Z' + 1);
        char betu2 = (char)Random.Shared.Next('A', 'Z' + 1);
        int szam = Random.Shared.Next(0, 100);

        azonosito = $"{betu1}{betu2}-{szam:D2}";
    } while (csomagok.Any(c => c.CsomagAzonosito == azonosito));

    return azonosito;
}

string ujNyitokod()
{
    return Random.Shared.Next(0, 100000).ToString("D5");
}

Console.WriteLine("==============================================");
Console.WriteLine($"Csomagautomata rendszer | {DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")}");
Console.WriteLine("==============================================");

while (true)
{
    Console.WriteLine("0. Kilépés");
    Console.WriteLine("1. Új csomag feladása");
    Console.WriteLine("2. Csomag átvétele");
    Console.WriteLine("3. Csomagkövetés");
    Console.WriteLine("4. Csomag állapotának megváltoztatása (Futár művelet)");
    Console.WriteLine("Válasszon a műveletek közül egy szám beírásával: ");
    
    string valasztas = muveletValasztas();
    if (valasztas == "0")
    {
        kilepes();
    }else if (valasztas == "1")
    {
        // csomagfeladas
            Console.Write("Címzett neve: ");
            string cimzett = Console.ReadLine();
            while (checkNev(cimzett) == false)
            {
                Console.Write("Helytelen bemenet! Címzett neve: ");
                cimzett = Console.ReadLine();
            }

            Console.Write("Csomag mérete (S/M/L): ");
            string csomagMeret = Console.ReadLine();
            while (checkMeret(csomagMeret) == false)
            {
                Console.Write("Helytelen bemenet! Csomag mérete (S/M/L): ");
                csomagMeret = Console.ReadLine();
            }

            DateTime feladasiIdo = DateTime.Now;

            string csomagAzonosito = ujCsomagAzonosito();

            string nyitokod = ujNyitokod();

            Console.Write("Kiszállítási cím: ");
            string cim = Console.ReadLine();

            Csomag uj = new(cimzett, csomagMeret.ToUpper(), "Feladva", nyitokod, feladasiIdo, csomagAzonosito, cim);

            csomagok.Add(uj);

            File.AppendAllText("csomagok.csv", uj.ToCsvSor() + Environment.NewLine);

            Console.WriteLine($"{feladasiIdo} Csomag sikeresen feladva!");
            Console.WriteLine($"Követési szám: {uj.CsomagAzonosito}");

    }
    else if (valasztas == "2")
    {
        Console.Write("Adja meg a csomag azonosítóját: ");
        string azonosito = Console.ReadLine();

        Csomag talaltCsomag = csomagok.FirstOrDefault(
            c => c.CsomagAzonosito == azonosito
        );

        if (talaltCsomag == null)
        {
            Console.WriteLine("Nincs ilyen csomag!");
        } else
        {
            Console.Write("Adja meg a csomag nyitókódját: ");
            string nyitokod = Console.ReadLine();
            if (talaltCsomag.NyitokodEllenorzes(nyitokod))
            {
                talaltCsomag.StatuszFrissit("Átvett");
            }
            else
            {
                Console.WriteLine("Helytelen nyitókód!");
            }

        }
    } else if (valasztas == "3")
    {
        Console.Write("Adja meg a csomag azonosítóját: ");
        string azonosito = Console.ReadLine();

        Csomag talaltCsomag = csomagok.FirstOrDefault(
            c => c.CsomagAzonosito == azonosito
        );

        if (talaltCsomag == null)
        {
            Console.WriteLine("Nincs ilyen csomag!");
        }

        // Egy sor a csv fájlba: cimzett;meret;statusz;nyitokod;feladasIdo;csomagAzonosito;szallitasiCim

        Console.WriteLine($"========================");
        Console.WriteLine($"A csomag adatai:");
        Console.WriteLine($"Címzett: {talaltCsomag.Cimzett}");
        Console.WriteLine($"Mérete: {talaltCsomag.Meret}");
        Console.WriteLine($"Státusza: {talaltCsomag.Statusz}");
        Console.WriteLine($"Myitókódja: {talaltCsomag.getNyitoKod()}");
        Console.WriteLine($"A csomagot ekkor adták fel: {talaltCsomag.FeladasIdo}");
        Console.WriteLine($"Azonosítója: {talaltCsomag.CsomagAzonosito}");
        Console.WriteLine($"Kiszállítási címe: {talaltCsomag.getSzallitasiCim()}");
        Console.WriteLine($"========================");
    }
    else if (valasztas == "4")
    {
        // futarDolgok

        Console.Write("Adja meg a csomag azonosítóját: ");
        string azonosito = Console.ReadLine();

        Csomag talaltCsomag = csomagok.FirstOrDefault(
            c => c.CsomagAzonosito == azonosito
        );

        if (talaltCsomag == null)
        {
            Console.WriteLine("Nincs ilyen csomag!");
        }
        else
        {
            Console.WriteLine("Válasszon a műveletek közül egy szám beírásával: ");
            Console.Write("Új állapot:\n1. A futár elindult a csomaggal\n2. A futár kiszállította a csomagot\n3. Hiba miatt a csomag el lett utasítva/vissza lett küldve");
            string allapot = Console.ReadLine();
            while (allapot != "1" && allapot != "2" && allapot != "3")
            {
                Console.WriteLine("Helytelen bemenet! Helyes bemenetek: 1. A futár elindult a csomaggal | 2. A futár kiszállította a csomagot | 3. Csomag elutasítva\n");
                allapot = Console.ReadLine();
            }

            if (allapot == "1")
            {
                talaltCsomag.StatuszFrissit("Kiszállítás alatt");
            }else if (allapot == "2")
            {
                talaltCsomag.StatuszFrissit("Kiszállított");
            } else if (allapot == "3")
            {
                talaltCsomag.StatuszFrissit("Elutasított");
            }
        }

    }
    
}