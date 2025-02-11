-- ustawienie formatu dat
ALTER SESSION SET NLS_DATE_FORMAT = 'YYYY-MM-DD';


/* Zad. 17. Wyświetlić pseudonimy, przydziały myszy oraz nazwy band dla kotów operujących
na terenie POLE posiadających przydział myszy większy od 50. Uwzględnić fakt, że są w
stadzie koty posiadające prawo do polowań na całym „obsługiwanym” przez stado terenie.
Nie stosować podzapytań. */
SELECT
    pseudo AS "POLUJE W POLU",
    przydzial_myszy AS "PRZYDZIAL MYSZY",
    nazwa AS "BANDA"
FROM kocury k
INNER JOIN bandy b ON b.nr_bandy = k.nr_bandy
WHERE przydzial_myszy > 50 AND teren IN ('POLE', 'CALOSC')
ORDER BY przydzial_myszy DESC;


/* Zad. 18. Wyświetlić bez stosowania podzapytania imiona i daty przystąpienia do stada
kotów, które przystąpiły do stada przed kotem o imieniu ’JACEK’. Wyniki uporządkować
malejąco wg daty przystąpienia do stadka. */
SELECT
    k1.imie,
    k1.w_stadku_od AS "POLUJE OD"
FROM kocury k1, kocury k2
WHERE k2.imie = 'JACEK' AND k1.w_stadku_od < k2.w_stadku_od
ORDER BY k1.w_stadku_od DESC;


/* Zad. 19. Dla kotów pełniących funkcję KOT i MILUSIA wyświetlić w kolejności hierarchii
imiona wszystkich ich szefów. Zadanie rozwiązać na trzy sposoby:
a. z wykorzystaniem tylko złączeń,
b. z wykorzystaniem drzewa, operatora CONNECT_BY_ROOT i tabel przestawnych,
c. z wykorzystaniem drzewa, funkcji SYS_CONNECT_BY_PATH i operatora CONNECT_BY_ROOT. */

-- 19 A
SELECT
    k1.imie AS "Imie",
    k1.funkcja AS "Funkcja",
    NVL(k2.imie, ' ') AS "Szef 1",
    NVL(k3.imie, '  ') AS "Szef 2",
    NVL(k4.imie, '   ') AS "Szef 3"
FROM kocury k1
LEFT JOIN kocury k2 ON k1.szef = k2.pseudo
LEFT JOIN kocury k3 ON k2.szef = k3.pseudo
LEFT JOIN kocury k4 ON k3.szef = k4.pseudo
WHERE k1.funkcja IN ('KOT', 'MILUSIA');

-- 19 B
SELECT
    "Imie",
    "Funkcja",
    NVL("Szef 1", ' ') AS "Szef 1",
    NVL("Szef 2", ' ') AS "Szef 2",
    NVL("Szef 3", ' ') AS "Szef 3"
FROM (
    SELECT 
        CONNECT_BY_ROOT imie AS "Imie",
        CONNECT_BY_ROOT funkcja AS "Funkcja",
        imie AS "Szef",
        LEVEL AS "lvl"
    FROM kocury
    START WITH funkcja IN ('MILUSIA', 'KOT')
    CONNECT BY PRIOR szef = pseudo )
PIVOT (
    MAX("Szef") FOR "lvl" IN (2 AS "Szef 1", 3 AS "Szef 2", 4 AS "Szef 3") 
);

-- 19 C
SELECT
    CONNECT_BY_ROOT imie AS "Imie",
    CONNECT_BY_ROOT funkcja AS "Funkcja",
    SUBSTR(SYS_CONNECT_BY_PATH(RPAD(imie, 13, ' '), ' | '), 17) AS "Imiona kolejnych szefów"
FROM kocury
WHERE szef IS NULL
START WITH funkcja IN ('KOT', 'MILUSIA')
CONNECT BY PRIOR szef = pseudo;


/* Zad. 20. Wyświetlić imiona wszystkich kotek, które uczestniczyły w incydentach po
01.01.2007. Dodatkowo wyświetlić nazwy band do których należą kotki, imiona ich wrogów
wraz ze stopniem wrogości oraz datę incydentu. */
SELECT
    k.imie AS "Imie kotki",
    b.nazwa AS "Nazwa bandy",
    w.imie_wroga AS "Imie wroga",
    w.stopien_wrogosci AS "Ocena wroga",
    wk.data_incydentu AS "Data inc."
FROM kocury k
INNER JOIN bandy b ON b.nr_bandy = k.nr_bandy
INNER JOIN wrogowie_kocurow wk ON wk.pseudo = k.pseudo
INNER JOIN wrogowie w ON w.imie_wroga = wk.imie_wroga
WHERE data_incydentu > '2007-01-01' AND plec = 'D'
ORDER BY imie ASC, w.imie_wroga ASC;


/* Zad. 21. Określić ile kotów w każdej z band posiada wrogów. */
SELECT
    nazwa AS "Nazwa bandy",
    COUNT(DISTINCT k.pseudo) AS "Koty z wrogami"
FROM bandy b
INNER JOIN kocury k ON k.nr_bandy = b.nr_bandy
INNER JOIN wrogowie_kocurow wk ON wk.pseudo = k.pseudo
GROUP BY nazwa;


/* Zad. 22. Znaleźć koty (wraz z pełnioną funkcją), które posiadają więcej niż jednego wroga. */
SELECT
    funkcja AS "Funkcja",
    k.pseudo AS "Pseudonim kota",
    COUNT(DISTINCT imie_wroga) AS "Liczba wrogow"
FROM kocury k
INNER JOIN wrogowie_kocurow wk ON wk.pseudo = k.PSEUDO
GROUP BY k.pseudo, funkcja
HAVING COUNT(DISTINCT imie_wroga) > 1;


/* Zad. 23. Wyświetlić imiona kotów, które dostają „myszą” premię wraz z ich całkowitym
rocznym spożyciem myszy. Dodatkowo jeśli ich roczna dawka myszy przekracza 864
wyświetlić tekst ’powyzej 864’, jeśli jest równa 864 tekst ’864’, jeśli jest mniejsza od 864
tekst ’poniżej 864’. Wyniki uporządkować malejąco wg rocznej dawki myszy. Do
rozwiązania wykorzystać operator zbiorowy UNION. */
SELECT
    imie,
    (przydzial_myszy + NVL(myszy_extra, 0)) * 12 AS "DAWKA ROCZNA",
    'powyzej 864' AS "DAWKA"
FROM kocury
WHERE myszy_extra IS NOT NULL AND (przydzial_myszy + NVL(myszy_extra, 0)) * 12 > 864

UNION ALL

SELECT
    imie,
    (przydzial_myszy + NVL(myszy_extra, 0)) * 12,
    '864'
FROM kocury
WHERE myszy_extra IS NOT NULL AND (przydzial_myszy + NVL(myszy_extra, 0)) * 12 = 864

UNION ALL

SELECT
    imie,
    (przydzial_myszy + NVL(myszy_extra, 0)) * 12,
    'ponizej 864'
FROM kocury
WHERE myszy_extra IS NOT NULL AND (przydzial_myszy + NVL(myszy_extra, 0)) * 12 < 864

ORDER BY 2 DESC;


/* Zad. 24. Znaleźć bandy, które nie posiadają członków. Wyświetlić ich numery, nazwy i
tereny operowania. Zadanie rozwiązać na dwa sposoby: bez podzapytań i operatorów
zbiorowych oraz wykorzystując operatory zbiorowe. */

-- 24 A
SELECT
    b.nr_bandy AS "NR BANDY",
    nazwa,
    teren
FROM bandy b
LEFT JOIN kocury k ON k.nr_bandy = b.nr_bandy
WHERE k.pseudo IS NULL;

-- 24 B
SELECT
    nr_bandy AS "NR BANDY",
    nazwa,
    teren
FROM bandy

MINUS 

SELECT
    b.nr_bandy AS "NR BANDY",
    nazwa,
    teren
FROM bandy b
INNER JOIN kocury k ON k.nr_bandy = b.nr_bandy
;


/* Zad. 25. Znaleźć koty, których przydział myszy jest nie mniejszy od potrojonego
najwyższego przydziału spośród przydziałów wszystkich MILUŚ operujących w SADZIE.
Nie stosować funkcji MAX. */
SELECT
    imie,
    funkcja,
    przydzial_myszy AS "PRZYDZIAL MYSZY"
FROM kocury
WHERE przydzial_myszy >= 3 * (
    SELECT MAX(przydzial_myszy)
    FROM kocury k
    INNER JOIN bandy b ON b.nr_bandy = k.nr_bandy
    WHERE funkcja = 'MILUSIA' AND teren IN ('SAD', 'CALOSC') )
ORDER BY imie ASC;


/* Zad. 26. Znaleźć funkcje (pomijając SZEFUNIA), z którymi związany jest najwyższy i
najniższy średni całkowity przydział myszy. Nie używać operatorów zbiorowych (UNION,
INTERSECT, MINUS). */
SELECT
    funkcja AS "Funkcja",
    ROUND(AVG(przydzial_myszy + NVL(myszy_extra, 0)), 0) AS "Srednio najw. i najm. myszy"
FROM kocury
GROUP BY funkcja
HAVING funkcja IN (
    (
    SELECT funkcja
    FROM kocury
    GROUP BY funkcja
    HAVING funkcja != 'SZEFUNIO'
    ORDER BY AVG(przydzial_myszy + NVL(myszy_extra, 0)) DESC
    FETCH FIRST 1 ROWS ONLY
    ),
    (
    SELECT funkcja
    FROM kocury
    GROUP BY funkcja
    HAVING funkcja != 'SZEFUNIO'
    ORDER BY AVG(przydzial_myszy + NVL(myszy_extra, 0)) ASC
    FETCH FIRST 1 ROWS ONLY
    )
);


/* Zad. 27. Znaleźć koty zajmujące pierwszych n miejsc pod względem całkowitej liczby
spożywanych myszy (koty o tym samym spożyciu zajmują to samo miejsce!). Zadanie
rozwiązać na cztery sposoby:
a. wykorzystując podzapytanie skorelowane,
b. wykorzystując pseudokolumnę ROWNUM,
c. wykorzystując złączenie relacji kocury z relacją kocury
d. wykorzystując funkcje analityczne. */

-- 27 A
-- dla kazdego kota liczymy ile kotow je wiecej od niego
SELECT 
    pseudo, 
    przydzial_myszy + NVL(myszy_extra, 0) AS "ZJADA"
FROM kocury k
WHERE :n > (
    SELECT COUNT(DISTINCT przydzial_myszy + NVL(myszy_extra, 0)) 
    FROM kocury
    WHERE przydzial_myszy + NVL(myszy_extra, 0) > k.przydzial_myszy + NVL(k.myszy_extra, 0))
ORDER BY 2 DESC;

-- 27 B
-- wybieramy n najwyzszych porcji myszy (distinct) i tylko te koty ktore maja jedna z tych porcji
SELECT 
    pseudo, 
    przydzial_myszy + NVL(myszy_extra, 0) AS "ZJADA"
FROM kocury
WHERE przydzial_myszy + NVL(myszy_extra, 0) IN (
    SELECT *
    FROM (
        SELECT DISTINCT przydzial_myszy + NVL(myszy_extra, 0)
        FROM kocury
        ORDER BY 1 DESC ) 
    WHERE ROWNUM <= :n 
);

-- 27 C
-- laczymy z tymi kotami ktore jedza wiecej i potem liczymy ile ich bylo grupowaniem
SELECT 
    k1.pseudo,
    MAX(k1.przydzial_myszy + NVL(k1.myszy_extra, 0)) AS "ZJADA"
FROM kocury k1 
LEFT JOIN kocury k2 ON k1.przydzial_myszy + NVL(k1.myszy_extra, 0) < k2.przydzial_myszy + NVL(k2.myszy_extra, 0)
GROUP BY k1.pseudo
HAVING COUNT(k2.pseudo) <= :n
ORDER BY 2 DESC;

-- 27 D
-- zapytanie w srodku robi ranking a na zewnatrz ucinamy ranking i zeby mozna bylo skorzystac z "rank"
SELECT  
    pseudo, 
    ZJADA
FROM (
    SELECT  
        pseudo,
        przydzial_myszy + NVL(myszy_extra, 0) AS "ZJADA",
        DENSE_RANK() OVER (ORDER BY przydzial_myszy + NVL(myszy_extra, 0) DESC) AS "rank"
    FROM kocury )
WHERE "rank" <= :n;


/* Zad. 28. Określić lata, dla których liczba wstąpień do stada jest najbliższa (od góry i od dołu)
średniej liczbie wstąpień dla wszystkich lat (średnia z wartości określających liczbę wstąpień
w poszczególnych latach). Nie stosować widoku (perspektywy). */

SELECT 
    TO_CHAR(w_stadku_od, 'YYYY') AS rok, 
    COUNT(*) AS "LICZBA WSTAPIEN"
FROM kocury
GROUP BY TO_CHAR(w_stadku_od, 'YYYY')
HAVING COUNT(*) IN (
    SELECT MAX("LICZBA WSTAPIEN")
    FROM (
        SELECT 
            TO_CHAR(w_stadku_od, 'YYYY') AS rok, 
            COUNT(*) AS "LICZBA WSTAPIEN"
        FROM kocury
        GROUP BY TO_CHAR(w_stadku_od, 'YYYY')
        HAVING COUNT(*) < ( SELECT ROUND(AVG(COUNT(*)), 7) FROM kocury GROUP BY TO_CHAR(w_stadku_od, 'YYYY') )
    )

    UNION ALL

    SELECT MIN("LICZBA WSTAPIEN")
    FROM (
        SELECT 
            TO_CHAR(w_stadku_od, 'YYYY') AS rok, 
            COUNT(*) AS "LICZBA WSTAPIEN"
        FROM kocury
        GROUP BY TO_CHAR(w_stadku_od, 'YYYY')
        HAVING COUNT(*) > ( SELECT ROUND(AVG(COUNT(*)), 7) FROM kocury GROUP BY TO_CHAR(w_stadku_od, 'YYYY') )
    )
)

UNION ALL

SELECT 
    'Srednia',
    (SELECT ROUND(AVG(COUNT(*)), 7) FROM kocury GROUP BY TO_CHAR(w_stadku_od, 'YYYY'))
FROM DUAL

ORDER BY 2;


/* Zad. 29. Dla kocurów (płeć męska), dla których całkowity przydział myszy nie przekracza
średniej w ich bandzie wyznaczyć następujące dane: imię, całkowite spożycie myszy, numer
bandy, średnie całkowite spożycie w bandzie. Nie stosować widoku (perspektywy). Zadanie
rozwiązać na trzy sposoby:
a. ze złączeniem ale bez podzapytań,
b. ze złączeniem i z jedynym podzapytaniem w klauzurze FROM,
c. bez złączeń i z dwoma podzapytaniami: w klauzurach SELECT i WHERE. */

-- 29 A
SELECT 
    k1.imie, 
    MIN(k1.przydzial_myszy + NVL(k1.myszy_extra, 0)) "ZJADA", 
    MIN(k1.nr_bandy), 
    TO_CHAR(AVG(k2.przydzial_myszy + NVL(k2.myszy_extra, 0)), '99.99') "SREDNIA BANDY"
FROM kocury k1 
INNER JOIN kocury k2 ON k1.nr_bandy= k2.nr_bandy
WHERE k1.plec = 'M'
GROUP BY k1.imie
HAVING MIN(k1.przydzial_myszy + NVL(k1.myszy_extra, 0)) < 
    AVG(k2.przydzial_myszy + NVL(k2.myszy_extra, 0))
ORDER BY 4 ASC;

-- 29 B
SELECT 
    imie, 
    przydzial_myszy + NVL(myszy_extra, 0) "ZJADA", 
    k1.nr_bandy, 
    TO_CHAR(AVG, '99.99') "SREDNIA BANDY"
FROM kocury k1 
INNER JOIN (
    SELECT 
        nr_bandy, 
        AVG(przydzial_myszy + NVL(myszy_extra, 0)) "AVG" 
    FROM kocury 
    GROUP BY nr_bandy
    ) k2 ON k1.nr_bandy= k2.nr_bandy AND przydzial_myszy + NVL(myszy_extra, 0) < AVG
WHERE plec = 'M'
ORDER BY 4 ASC;

-- 29 c
SELECT 
    imie, 
    przydzial_myszy + NVL(myszy_extra, 0) "ZJADA", 
    nr_bandy,
    TO_CHAR((
        SELECT AVG(przydzial_myszy + NVL(myszy_extra, 0)) "AVG" 
        FROM kocury k 
        WHERE kocury.nr_bandy = k.nr_bandy
        ), '99.99') "SREDNIA BANDY"
FROM kocury
WHERE plec = 'M' AND przydzial_myszy + NVL(myszy_extra, 0) < (
    SELECT AVG(przydzial_myszy + NVL(myszy_extra, 0)) "AVG" 
    FROM kocury k 
    WHERE kocury.nr_bandy= k.nr_bandy)
ORDER BY 4 ASC;


/* Zad. 30. Wygenerować listę kotów z zaznaczonymi kotami o najwyższym i o najniższym
stażu w swoich bandach. Zastosować operatory zbiorowe. */
SELECT 
    k1.imie, 
    k1.w_stadku_od || ' <---' AS "Wstapil do stadka", 
    'NAJSTARSZY STAZEM W BANDZIE ' || b.nazwa AS " "
FROM kocury k1
LEFT JOIN bandy b on k1.nr_bandy=b.nr_bandy
WHERE k1.w_stadku_od = (
    SELECT MIN(k2.w_stadku_od) 
    FROM kocury k2 
    WHERE k2.nr_bandy = k1.nr_bandy )  

UNION All

SELECT 
    k1.imie, 
    k1.w_stadku_od || ' <---', 
    'NAJMLODSZY STAZEM W BANDZIE ' || b.nazwa
FROM kocury k1
LEFT JOIN bandy b on k1.nr_bandy=b.nr_bandy
WHERE k1.w_stadku_od = (
    SELECT MAX(k2.w_stadku_od) 
    FROM kocury k2 
    WHERE k2.nr_bandy = k1.nr_bandy )  

UNION ALL

SELECT 
    k1.imie, 
    k1.w_stadku_od || ' ', 
    ' '
FROM kocury k1
LEFT JOIN bandy b on k1.nr_bandy=b.nr_bandy
WHERE k1.w_stadku_od != (
    SELECT MIN(k2.w_stadku_od) 
    FROM kocury k2 
    WHERE k2.nr_bandy = k1.nr_bandy )
AND k1.w_stadku_od != (
    SELECT MAX(k2.w_stadku_od) 
    FROM kocury k2 
    WHERE k2.nr_bandy = k1.nr_bandy )
ORDER BY 1;


/* Zad. 31. Zdefiniować widok (perspektywę) wybierający następujące dane: nazwę bandy,
średni, maksymalny i minimalny przydział myszy w bandzie, całkowitą liczbę kotów w
bandzie oraz liczbę kotów pobierających w bandzie przydziały dodatkowe. Posługując się
zdefiniowanym widokiem wybrać następujące dane o kocie, którego pseudonim podawany
jest interaktywnie z klawiatury: pseudonim, imię, funkcja, przydział myszy, minimalny i
maksymalny przydział myszy w jego bandzie oraz datę wstąpienia do stada. */

CREATE OR REPLACE VIEW bandy_info AS
SELECT 
    b.nazwa AS NAZWA_BANDY,
    ROUND(AVG(k.przydzial_myszy), 2) AS SRE_SPOZ,
    MAX(k.przydzial_myszy) AS MAX_SPOZ,
    MIN(k.przydzial_myszy) AS MIN_SPOZ,
    COUNT(k.pseudo) AS KOTY,
    COUNT(CASE WHEN k.myszy_extra > 0 THEN 1 END) AS KOTY_Z_DOD
FROM 
    kocury k
INNER JOIN bandy b ON b.nr_bandy = k.nr_bandy
GROUP BY b.nazwa;

SELECT * 
FROM bandy_info;

SELECT
    pseudo AS PSEUDONIM,
    imie,
    funkcja,
    przydzial_myszy AS ZJADA,
    'OD ' || MIN_SPOZ || ' DO ' || MAX_SPOZ AS "GRANICE SPOZYCIA",
    w_stadku_od AS "LOWI OD"
FROM kocury k
INNER JOIN bandy b ON b.nr_bandy = k.nr_bandy
INNER JOIN bandy_info ON bandy_info.nazwa_bandy = b.nazwa
WHERE pseudo = UPPER(:imie_kota);


/* Zad. 32. Dla kotów o trzech najdłuższym stażach w połączonych bandach CZARNI
RYCERZE i ŁACIACI MYŚLIWI zwiększyć przydział myszy o 10% minimalnego
przydziału w całym stadzie lub o 10 w zależności od tego czy podwyżka dotyczy kota płci
żeńskiej czy kota płci męskiej. Przydział myszy extra dla kotów obu płci zwiększyć o 15%
średniego przydziału extra w bandzie kota. Wyświetlić na ekranie wartości przed i po
podwyżce a następnie wycofać zmiany. */

CREATE OR REPLACE VIEW najstarsze_koty AS
SELECT
    pseudo,
    plec,
    przydzial_myszy,
    NVL(myszy_extra, 0) AS myszy_extra,
    nazwa
FROM kocury
LEFT JOIN bandy on bandy.nr_bandy = kocury.nr_bandy
WHERE pseudo IN (
    SELECT pseudo
    FROM (
        SELECT
            pseudo,
            w_stadku_od,
            nazwa,
            DENSE_RANK() OVER (ORDER BY w_stadku_od ASC) AS ranking
        FROM kocury k
        LEFT JOIN bandy b ON b.nr_bandy = k.nr_bandy
        WHERE nazwa = 'LACIACI MYSLIWI')
    WHERE ranking <= 3

    UNION

    SELECT pseudo
    FROM (
        SELECT
            pseudo,
            w_stadku_od,
            nazwa,
            DENSE_RANK() OVER (ORDER BY w_stadku_od ASC) AS ranking
        FROM kocury k
        LEFT JOIN bandy b ON b.nr_bandy = k.nr_bandy
        WHERE nazwa = 'CZARNI RYCERZE')
    WHERE ranking <= 3
);

SELECT
    pseudo AS "Pseudonim",
    plec AS "Plec",
    przydzial_myszy AS "Myszy przed podw.",
    myszy_extra AS "Extra przed podw."
FROM najstarsze_koty;

UPDATE kocury
SET przydzial_myszy = 
    CASE plec
    WHEN 'M' THEN przydzial_myszy + 10
    ELSE przydzial_myszy + (SELECT MIN(przydzial_myszy) FROM kocury) * 0.1
    END,
    myszy_extra = NVL(myszy_extra, 0) + 0.15 * (
        SELECT AVG(NVL(myszy_extra, 0)) 
        FROM kocury k2 
        WHERE k2.nr_bandy = kocury.nr_bandy
    )
WHERE pseudo IN (SELECT pseudo FROM najstarsze_koty);

SELECT
    pseudo AS "Pseudonim",
    plec AS "Plec",
    przydzial_myszy AS "Myszy po podw.",
    myszy_extra AS "Extra po podw."
FROM najstarsze_koty;

ROLLBACK;


/* Zad. 33. Napisać zapytanie, w ramach którego obliczone zostaną sumy całkowitego spożycia
myszy przez koty sprawujące każdą z funkcji z podziałem na bandy i płcie kotów.
Podsumować przydziały dla każdej z funkcji. Zadanie wykonać na dwa sposoby:
a. z wykorzystaniem funkcji DECODE i SUM (ew. CASE i SUM),
b. z wykorzystaniem tabel przestawnych */

-- 33 A
SELECT *
FROM (
    SELECT 
        TO_CHAR(DECODE(plec, 'D', nazwa, ' ')) AS "NAZWA BANDY",
        TO_CHAR(DECODE(plec, 'D', 'Kotka', 'Kocor')) AS "PLEC",
        TO_CHAR(COUNT(pseudo)) AS "ILE",
        TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k WHERE funkcja = 'SZEFUNIO' AND k.nr_bandy= kocury.nr_bandy AND k.plec = kocury.plec),0)) AS "SZEFUNIO",
        TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k WHERE funkcja = 'BANDZIOR' AND k.nr_bandy= kocury.nr_bandy AND k.plec = kocury.plec),0)) AS "BANDZIOR",
        TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k WHERE funkcja = 'LOWCZY' AND k.nr_bandy= kocury.nr_bandy AND k.plec = kocury.plec),0)) AS "LOWCZY",
        TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k WHERE funkcja = 'LAPACZ' AND k.nr_bandy= kocury.nr_bandy AND k.plec = kocury.plec),0)) AS "LAPACZ",
        TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k WHERE funkcja = 'KOT' AND k.nr_bandy= kocury.nr_bandy AND k.plec = kocury.plec),0)) AS "KOT",
        TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k WHERE funkcja = 'MILUSIA' AND k.nr_bandy= kocury.nr_bandy AND k.plec = kocury.plec),0)) AS "MILUSIA",
        TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k WHERE funkcja = 'DZIELCZY' AND k.nr_bandy= kocury.nr_bandy AND k.plec = kocury.plec),0)) AS "DZIELCZY",
        TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k WHERE k.nr_bandy= kocury.nr_bandy AND k.plec = kocury.plec),0)) AS "SUMA"
    FROM kocury INNER JOIN bandy ON kocury.nr_bandy = bandy.nr_bandy
    GROUP BY nazwa, plec, kocury.nr_bandy
    ORDER BY nazwa
)

UNION ALL

SELECT '--------------', '------', '--------', '---------', '---------', '--------', '--------', '--------', '--------', '--------', '--------' 
FROM dual

UNION ALL

SELECT DISTINCT 
    'ZJADA RAZEM',
    ' ',
    ' ',
    TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k WHERE funkcja = 'SZEFUNIO'), 0)) AS "SZEFUNIO",
    TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k WHERE funkcja = 'BANDZIOR'), 0)) AS "BANDZIOR",
    TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k WHERE funkcja = 'LOWCZY'), 0)) AS "LOWCZY",
    TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k WHERE funkcja = 'LAPACZ'), 0)) AS "LAPACZ",
    TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k WHERE funkcja = 'KOT'), 0)) AS "KOT",
    TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k WHERE funkcja = 'MILUSIA'), 0)) AS "MILUSIA",
    TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k WHERE funkcja = 'DZIELCZY'), 0)) AS "DZIELCZY",
    TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k), 0)) AS "SUMA"
FROM kocury INNER JOIN bandy ON kocury.nr_bandy= bandy.nr_bandy;

-- 33 B
SELECT 
    TO_CHAR(DECODE(plec, 'D', nazwa, ' ')) AS "NAZWA BANDY",
    TO_CHAR(DECODE(plec, 'D', 'Kotka', 'Kocor')) AS "PLEC",
    TO_CHAR(ile) AS "ILE",
    TO_CHAR(NVL(szefunio, 0)) AS "SZEFUNIO",
    TO_CHAR(NVL(bandzior, 0)) AS "BANDZIOR",
    TO_CHAR(NVL(lowczy, 0)) AS "LOWCZY",
    TO_CHAR(NVL(lapacz, 0)) AS "LAPACZ",
    TO_CHAR(NVL(kot, 0)) AS "KOT",
    TO_CHAR(NVL(milusia, 0)) AS "MILUSIA",
    TO_CHAR(NVL(dzielczy, 0)) AS "DZIELCZY",
    TO_CHAR(NVL(suma, 0)) AS "SUMA"
FROM (
    SELECT 
        nazwa, 
        plec, 
        funkcja, 
        przydzial_myszy + NVL(myszy_extra, 0) AS myszy
    FROM kocury INNER JOIN Bandy ON kocury.nr_bandy= Bandy.nr_bandy )
PIVOT (
    SUM(myszy) FOR funkcja IN (
    'SZEFUNIO' AS szefunio, 'BANDZIOR' AS bandzior, 'LOWCZY' AS lowczy, 'LAPACZ' AS lapacz,
    'KOT' AS kot, 'MILUSIA' AS milusia, 'DZIELCZY' AS dzielczy ) )
INNER JOIN (
    SELECT 
        nazwa AS "nazwa2", 
        plec AS "plec2", 
        COUNT(pseudo) AS ile, 
        SUM(przydzial_myszy + NVL(myszy_extra, 0)) AS suma
    FROM kocury k INNER JOIN bandy b ON k.nr_bandy= b.nr_bandy
    GROUP BY nazwa, plec
    ORDER BY nazwa
) ON "nazwa2" = nazwa AND "plec2" = plec


UNION ALL

SELECT '--------------', '------', '--------', '---------', '---------', '--------', '--------', '--------', '--------', '--------', '--------' 
FROM dual

UNION ALL

SELECT  
    'ZJADA RAZEM',
    ' ',
    ' ',
    TO_CHAR(NVL(szefunio, 0)),
    TO_CHAR(NVL(bandzior, 0)),
    TO_CHAR(NVL(lowczy, 0)),
    TO_CHAR(NVL(lapacz, 0)),
    TO_CHAR(NVL(kot, 0)),
    TO_CHAR(NVL(milusia, 0)),
    TO_CHAR(NVL(dzielczy, 0)),
    TO_CHAR(NVL(suma, 0))
FROM (
    SELECT 
        funkcja, 
        przydzial_myszy + NVL(myszy_extra, 0) AS myszy
    FROM kocury 
    INNER JOIN bandy ON kocury.nr_bandy= bandy.nr_bandy ) 
PIVOT (
    SUM(myszy) FOR funkcja IN (
    'SZEFUNIO' AS szefunio, 'BANDZIOR' AS bandzior, 'LOWCZY' AS lowczy, 'LAPACZ' AS lapacz,
    'KOT' AS kot, 'MILUSIA' AS milusia, 'DZIELCZY' AS dzielczy ) ) 
NATURAL JOIN (
  SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) AS suma
  FROM kocury
);


------------------------------------------------------------------------
