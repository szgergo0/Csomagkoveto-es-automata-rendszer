# 📦 Csomagautomata

Egyszerű, konzolos csomagkövető rendszer C#-ban. Csomagot lehet feladni, követni és átvenni, a futár pedig módosíthatja a csomag állapotát.

Az adatok a `csomagok.csv` fájlba kerülnek, így a program újraindítás után is megtartja őket.

**Fejlesztők:** Fodor Benedek és Szabó Gergő

---

## ✨ Funkciók

| Menü | Funkció | Leírás |
|:---:|---|---|
| `0` | Kilépés | Kilépés a programból |
| `1` | Új csomag feladása | Címzett, méret és kiszállítási cím megadása, a program azonosítót és nyitókódot generál |
| `2` | Csomag átvétele | Átvétel az azonosító és a nyitókód megadásával |
| `3` | Csomagkövetés | A csomag adatainak és állapotának megtekintése (azonosító és nyitókód kell hozzá) |
| `4` | Állapotmódosítás (futár) | A futár frissíti a csomag állapotát az azonosító alapján |

---

## 🧭 Használat

Indítás után ez a menü jelenik meg:

```
----------------------------------------------
0. Kilépés
1. Új csomag feladása
2. Csomag átvétele
3. Csomagkövetés
4. Csomag állapotának megváltoztatása (Futár művelet)
----------------------------------------------
Válasszon a műveletek közül egy szám beírásával:
```

### 1. Új csomag feladása

Meg kell adni a címzett nevét (nem lehet üres, nem tartalmazhat számot és `;` jelet), a csomag méretét (`S`, `M` vagy `L`) és a kiszállítási címet (nem lehet üres, nem tartalmazhat `;` jelet). Hibás adatnál a program újra megkérdezi az értéket.

```
Címzett neve: Kiss Anna
Csomag mérete (S/M/L): M
Kiszállítási cím: Szeged, Fő utca 1.
----------------------------------------------
Csomag sikeresen feladva!
Követési szám: AB-12
A csomag nyitókódja: 04829
----------------------------------------------
```

A **követési szám** két betűből és két számjegyből áll (pl. `AB-12`), a **nyitókód** ötjegyű. A nyitókódot a címzettnek kell megkapnia, mert ez kell az átvételhez.

### 2. Csomag átvétele

A csomag csak akkor vehető át, ha a státusza **Kiszállított**, és a nyitókód helyes.

```
Adja meg a csomag azonosítóját: AB-12
Adja meg a csomag nyitókódját: 04829
Helyes nyitókód, vegye át a csomagot!
```

Ha a csomag még nincs kiszállítva, már át lett véve vagy el lett utasítva, a program ezt külön jelzi.

### 3. Csomagkövetés

Az azonosító és a nyitókód megadása után a program kiírja a csomag adatait: címzett, méret, státusz, feladás ideje, azonosító és kiszállítási cím.

### 4. Állapotmódosítás (futár)

A futár megadja az azonosítót, a program kiírja a jelenlegi állapotot, majd a futár választhat:

```
Jelenlegi állapot: Feladva
Új állapot:
1. A futár elindult a csomaggal
2. A futár kiszállította a csomagot
3. Hiba miatt a csomag el lett utasítva és vissza lett küldve.
```

Ha az állapotváltás nem engedélyezett (például `Feladva` → `Kiszállított`), a program hibaüzenetet ad, és nem módosít semmit.

---

## 🔄 Csomag állapotai

```
Feladva  →  Kiszállítás alatt  →  Kiszállított  →  Átvett
   │                │                   │
   └────────────────┴───────────────────┴──→  Elutasított
```

- A csomag csak a fenti sorrendben haladhat.
- **Elutasított** állapotba átvétel előtt bármikor kerülhet.
- Az **Átvett** és az **Elutasított** csomag állapota már nem módosítható.
- Az **Átvett** állapotot csak a címzett állíthatja be az átvétel menüpontban, a futár nem.

---

## 🗂️ Projekt felépítése

| Fájl | Mire való |
|---|---|
| `Program.cs` | A menü, a bemenetek ellenőrzése és a program fő ciklusa |
| `Csomag.cs` | A csomag osztály: adatok, állapotváltás szabályai, CSV-be mentés |
| `csomagok.csv` | A csomagok adatai (automatikusan jön létre) |