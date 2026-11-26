using System.IO;
using System.Linq;

// Hashez
using System.Security.Cryptography;
using System.Text;

const string hasheltJelszo = "E42A9291CC848FF6B4322B2A2D16412254B0CE14980772775C47A535554A6112";

string hash(string szoveg)
{
    byte[] bajtok = Rfc2898DeriveBytes.Pbkdf2(szoveg, Encoding.UTF8.GetBytes("csomagautomata-futar"), 600000, HashAlgorithmName.SHA256, 32);
    return Convert.ToHexString(bajtok);
}

string jelszoBeolvasas()
{
    string jelszo = "";
    ConsoleKeyInfo billentyu;

    while ((billentyu = Console.ReadKey(true)).Key != ConsoleKey.Enter)
    {
        if (billentyu.Key == ConsoleKey.Backspace)
        {
            if (jelszo.Length > 0)
            {
                jelszo = jelszo.Substring(0, jelszo.Length - 1);
                Console.Write("\b \b");
            }
        }
        else if (!char.IsControl(billentyu.KeyChar))
        {
            jelszo += billentyu.KeyChar;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("*");
            Console.ResetColor();
        }
    }

    Console.WriteLine();
    return jelszo;
}

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
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Hibás sor: {sor}");
        Console.ResetColor();
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
    while (x != "1" && x != "2" && x != "3" && x != "4" && x != "0")
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Helytelen bemenet! ");
        Console.ResetColor();
        Console.Write("Helyes bemenetek: 0. Kilépés | 1. Új csomag feladása | 2. Csomag átvétele | 3. Csomagkövetés | 4. Csomag állapotának megváltoztatása (Futár művelet)\n");
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

void visszaOpcio()
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("0. ");
    Console.ResetColor();
    Console.WriteLine("Vissza a főmenübe");
}

void visszaDobas()
{
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine("Visszatérés a főmenübe...");
    Console.ResetColor();
}

// Kiírja a kérdést, beolvas egy sort. Ha "0"-t írnak, null-t ad vissza.
string bekeres(string felirat)
{
    Console.Write(felirat);
    string be = Console.ReadLine();

    if (be == null || be.Trim() == "0")
    {
        return null;
    }

    return be;
}

belepeskor();

while (true)
{
    Console.WriteLine("----------------------------------------------");
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("0. ");
    Console.ResetColor();
    Console.Write("Kilépés\n");

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("1. ");
    Console.ResetColor();
    Console.Write("Új csomag feladása\n");

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("2. ");
    Console.ResetColor();
    Console.Write("Csomag átvétele\n");

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("3. ");
    Console.ResetColor();
    Console.Write("Csomagkövetés illetve adatainak kiírása\n");

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("4. ");
    Console.ResetColor();
    Console.Write("Csomag állapotának megváltoztatása (Futár művelet)\n");
    Console.WriteLine("----------------------------------------------");

    Console.WriteLine("Válasszon a műveletek közül egy szám beírásával: ");

    string valasztas = muveletValasztas();
    if (valasztas == "0")
    {
        kilepes();
    }
    else if (valasztas == "1")
    {
        // csomagfeladas
        visszaOpcio();

        string cimzett = bekeres("Címzett neve: ");
        while (cimzett != null && !checkNev(cimzett))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Helytelen bemenet! ");
            Console.ResetColor();
            cimzett = bekeres("Címzett neve: ");
        }
        if (cimzett == null) { visszaDobas(); continue; }

        string csomagMeret = bekeres("Csomag mérete (S/M/L): ");
        while (csomagMeret != null && !checkMeret(csomagMeret))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Helytelen bemenet! ");
            Console.ResetColor();
            csomagMeret = bekeres("Csomag mérete (S/M/L): ");
        }
        if (csomagMeret == null) { visszaDobas(); continue; }

        string cim = bekeres("Kiszállítási cím: ");
        while (cim != null && !checkCim(cim))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Helytelen bemenet! ");
            Console.ResetColor();
            cim = bekeres("Kiszállítási cím: ");
        }
        if (cim == null) { visszaDobas(); continue; }

        DateTime feladasiIdo = DateTime.Now;
        string csomagAzonosito = ujCsomagAzonosito();
        string nyitokod = ujNyitokod();

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
        visszaOpcio();

        string azonosito = bekeres("Adja meg a csomag azonosítóját: ");
        if (azonosito == null) { visszaDobas(); continue; }
        azonosito = azonosito.ToUpper();

        Csomag talaltCsomag = csomagok.FirstOrDefault(c => c.CsomagAzonosito == azonosito);

        if (talaltCsomag == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Nincs ilyen csomag!");
            Console.ResetColor();
        }
        else
        {
            string nyitokod = bekeres("Adja meg a csomag nyitókódját: ");
            if (nyitokod == null) { visszaDobas(); continue; }

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
    }
    else if (valasztas == "3")
    {
        visszaOpcio();

        string azonosito = bekeres("Adja meg a csomag azonosítóját: ");
        if (azonosito == null) { visszaDobas(); continue; }
        azonosito = azonosito.ToUpper();

        Csomag talaltCsomag = csomagok.FirstOrDefault(c => c.CsomagAzonosito == azonosito);
        if (talaltCsomag == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Nincs ilyen csomag!");
            Console.ResetColor();
        }
        else
        {
            string nyitokod = bekeres("Adja meg a csomag nyitókódját: ");
            if (nyitokod == null) { visszaDobas(); continue; }

            if (talaltCsomag.NyitokodEllenorzes(nyitokod))
            {
                // Egy sor a csv fájlba: cimzett;meret;statusz;nyitokod;feladasIdo;csomagAzonosito;szallitasiCim

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"========================");
                Console.WriteLine($"A csomag adatai:");
                Console.ResetColor();

                Console.Write($"Címzett: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{talaltCsomag.Cimzett}\n");
                Console.ResetColor();

                Console.Write($"Mérete: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{talaltCsomag.Meret}\n");
                Console.ResetColor();

                Console.Write($"Státusza: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{talaltCsomag.Statusz}\n");
                Console.ResetColor();

                Console.Write($"Nyitókódja: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{talaltCsomag.getNyitoKod()}\n");
                Console.ResetColor();

                Console.Write($"A csomagot ekkor adták fel: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{talaltCsomag.FeladasIdo.ToString("yyyy.MM.dd HH:mm:ss")}\n");
                Console.ResetColor();

                Console.Write($"Azonosítója: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{talaltCsomag.CsomagAzonosito}\n");
                Console.ResetColor();

                Console.Write($"Kiszállítási címe: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{talaltCsomag.getSzallitasiCim()}\n");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"========================");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Helytelen nyitókód!");
                Console.ResetColor();
            }
        }
    }
    else if (valasztas == "4")
    {
        // futarDolgok
        visszaOpcio();

        Console.Write("Kérem adja meg a jelszót: ");
        string megadottJelszo = jelszoBeolvasas();

        if (megadottJelszo == "0") { visszaDobas(); continue; }

        if (hash(megadottJelszo) != hasheltJelszo)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Helytelen jelszó!");
            Console.ResetColor();
            continue;
        }

        string azonosito = bekeres("Adja meg a csomag azonosítóját: ");
        if (azonosito == null) { visszaDobas(); continue; }
        azonosito = azonosito.ToUpper();

        Csomag talaltCsomag = csomagok.FirstOrDefault(c => c.CsomagAzonosito == azonosito);

        if (talaltCsomag == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Nincs ilyen csomag!");
            Console.ResetColor();
        }
        else if (talaltCsomag.Statusz == "Átvett")
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Az átvett csomag állapota már nem módosítható!");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine("----------------------------------------------");

            Console.Write("Jelenlegi állapot: ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"{talaltCsomag.Statusz}\n");
            Console.ResetColor();

            Console.WriteLine("Válasszon a műveletek közül egy szám beírásával: ");
            Console.WriteLine("Új állapot:");

            visszaOpcio();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("1. ");
            Console.ResetColor();
            Console.Write("A futár elindult a csomaggal\n");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("2. ");
            Console.ResetColor();
            Console.Write("A futár kiszállította a csomagot\n");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("3. ");
            Console.ResetColor();
            Console.Write("Hiba miatt a csomag el lett utasítva és vissza lett küldve.\n");
            Console.WriteLine("----------------------------------------------");

            string allapot = Console.ReadLine();
            while (allapot != "0" && allapot != "1" && allapot != "2" && allapot != "3")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Helytelen bemenet! ");
                Console.ResetColor();
                Console.Write("Helyes bemenetek: 0. Vissza | 1. A futár elindult a csomaggal | 2. A futár kiszállította a csomagot | 3. Csomag elutasítva\n");
                allapot = Console.ReadLine();
            }

            if (allapot == "0") { visszaDobas(); continue; }

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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Az állapot módosítva: {ujStatusz}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ez az állapotváltás nem engedélyezett (jelenlegi állapot: {talaltCsomag.Statusz})!");
                Console.ResetColor();
            }
        }
    }
}