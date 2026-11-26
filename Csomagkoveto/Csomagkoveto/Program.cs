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

bool checkCim(string cim)
{
    if (string.IsNullOrWhiteSpace(cim) || cim.Contains(';'))
    {
        return false;
    }

    return true;
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

void belepeskor()
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("==============================================");
    Console.WriteLine($"Csomagautomata rendszer | {DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")}");
    Console.WriteLine("==============================================");
    Console.ResetColor();

}

belepeskor();

while (true)
{
    Console.WriteLine("----------------------------------------------");
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write("0. ");
    Console.ResetColor();
    Console.Write("Kilépés\n");
    
    Console.WriteLine("1. Új csomag feladása");
    Console.WriteLine("2. Csomag átvétele");
    Console.WriteLine("3. Csomagkövetés");
    Console.WriteLine("4. Csomag állapotának megváltoztatása (Futár művelet)");
    Console.WriteLine("----------------------------------------------");
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
            while (checkCim(cim) == false)
            {
                Console.Write("Helytelen bemenet! Kiszállítási cím: ");
                cim = Console.ReadLine();
            }

            Csomag uj = new(cimzett, csomagMeret.ToUpper(), "Feladva", nyitokod, feladasiIdo, csomagAzonosito, cim);

            csomagok.Add(uj);

            File.AppendAllText("csomagok.csv", uj.ToCsvSor() + Environment.NewLine);
            
            Console.WriteLine("----------------------------------------------");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{feladasiIdo.ToString("yyyy.MM.dd HH:mm:ss")} Csomag sikeresen feladva!");
            Console.ResetColor();
            Console.Write($"Követési szám: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{uj.CsomagAzonosito}\n");
            Console.ResetColor();
            Console.Write($"A csomag nyitókódja: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{nyitokod}\n");
            Console.ResetColor();
            Console.WriteLine("----------------------------------------------");

    }
    else if (valasztas == "2")
    {
        Console.Write("Adja meg a csomag azonosítóját: ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        string azonosito = Console.ReadLine().ToUpper();
        Console.ResetColor();

        Csomag talaltCsomag = csomagok.FirstOrDefault(c => c.CsomagAzonosito == azonosito);

        if (talaltCsomag == null)
        {
            Console.WriteLine("Nincs ilyen csomag!");
        } else
        {
            Console.Write("Adja meg a csomag nyitókódját: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            string nyitokod = Console.ReadLine();
            Console.ResetColor();
            if (talaltCsomag.NyitokodEllenorzes(nyitokod))
            {
                if (talaltCsomag.Statusz == "Átvett")
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Ez a csomag már át lett véve!");
                    Console.ResetColor();
                }
                else if (talaltCsomag.Statusz == "Kiszállított")
                {
                    talaltCsomag.StatuszFrissit("Átvett");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Helyes nyitókód, vegye át a csomagot!");
                    Console.ResetColor();
                }
                else if (talaltCsomag.Statusz == "Elutasított")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ez a csomag el lett utasítva, nem vehető át!");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Ez a csomag még nem lett kiszállítva!");
                    Console.ResetColor();
                }
                
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Helytelen nyitókód!");
                Console.ResetColor();
            }

        }
    } else if (valasztas == "3")
    {
        Console.Write("Adja meg a csomag azonosítóját: ");
        string azonosito = Console.ReadLine().ToUpper();

        Csomag talaltCsomag = csomagok.FirstOrDefault(c => c.CsomagAzonosito == azonosito);
        if (talaltCsomag == null)
        {
            Console.WriteLine("Nincs ilyen csomag!");
        }
        else
        {
            Console.Write("Adja meg a csomag nyitókódját: ");
            string nyitokod = Console.ReadLine();
            if (talaltCsomag.NyitokodEllenorzes(nyitokod))
            {
                // Egy sor a csv fájlba: cimzett;meret;statusz;nyitokod;feladasIdo;csomagAzonosito;szallitasiCim

                Console.WriteLine($"========================");
                Console.WriteLine($"A csomag adatai:");
                Console.WriteLine($"Címzett: {talaltCsomag.Cimzett}");
                Console.WriteLine($"Mérete: {talaltCsomag.Meret}");
                Console.WriteLine($"Státusza: {talaltCsomag.Statusz}");
                Console.WriteLine($"Nyitókódja: {talaltCsomag.getNyitoKod()}");
                Console.WriteLine($"A csomagot ekkor adták fel: {talaltCsomag.FeladasIdo}");
                Console.WriteLine($"Azonosítója: {talaltCsomag.CsomagAzonosito}");
                Console.WriteLine($"Kiszállítási címe: {talaltCsomag.getSzallitasiCim()}");
                Console.WriteLine($"========================");
            }
            else
            {
                Console.WriteLine("Helytelen nyitókód!");
            }
        }
    }
    else if (valasztas == "4")
    {
        // futarDolgok

        Console.Write("Adja meg a csomag azonosítóját: ");
        string azonosito = Console.ReadLine();
        azonosito = azonosito.ToUpper();

        Csomag talaltCsomag = csomagok.FirstOrDefault(c => c.CsomagAzonosito == azonosito);

        if (talaltCsomag == null)
        {
            Console.WriteLine("Nincs ilyen csomag!");
        }
        else if (talaltCsomag.Statusz == "Átvett")
        {
            Console.WriteLine("Az átvett csomag állapota már nem módosítható!");
        }
        else
        {
            Console.WriteLine($"Jelenlegi állapot: {talaltCsomag.Statusz}");
            Console.WriteLine("Válasszon a műveletek közül egy szám beírásával: ");
            Console.Write("Új állapot:\n1. A futár elindult a csomaggal\n2. A futár kiszállította a csomagot\n3. Hiba miatt a csomag el lett utasítva és vissza lett küldve.\n");
            string allapot = Console.ReadLine();
            while (allapot != "1" && allapot != "2" && allapot != "3")
            {
                Console.WriteLine("Helytelen bemenet! Helyes bemenetek: 1. A futár elindult a csomaggal | 2. A futár kiszállította a csomagot | 3. Csomag elutasítva\n");
                allapot = Console.ReadLine();
            }

            string ujStatusz;
            if (allapot == "1")
            {
                ujStatusz = "Kiszállítás alatt";
            }
            else if (allapot == "2")
            {
                ujStatusz = "Kiszállított";
            }
            else
            {
                ujStatusz = "Elutasított";
            }


            if (talaltCsomag.StatuszFrissit(ujStatusz))
            {
                Console.WriteLine($"Az állapot módosítva: {ujStatusz}");
            }
            else
            {
                Console.WriteLine($"Ez az állapotváltás nem engedélyezett (jelenlegi állapot: {talaltCsomag.Statusz})!");
            }
        }

    }

}