-- ustawienie formatu dat
ALTER SESSION SET NLS_DATE_FORMAT = 'YYYY-MM-DD';


-- zadanie 1
SELECT 
    IMIE_WROGA AS "WROG", 
    OPIS_INCYDENTU AS "PRZEWINA"
FROM 
    WROGOWIE_KOCUROW
WHERE 
    TO_CHAR(DATA_INCYDENTU, 'YYYY') = 2009;


-- zadanie2
SELECT 
    IMIE, 
    FUNKCJA, 
    W_STADKU_OD AS "Z NAMI OD"
FROM 
    KOCURY
WHERE 
    PLEC = 'D'
    AND W_STADKU_OD BETWEEN '2005-09-01' AND '2007-07-31';


-- zadanie 3
SELECT 
    IMIE_WROGA AS "WROG", 
    GATUNEK, 
    STOPIEN_WROGOSCI AS "STOPIEN WROGOSCI" 
FROM 
    WROGOWIE
WHERE 
    LAPOWKA IS NULL
ORDER BY 
    STOPIEN_WROGOSCI ASC;


-- zadanie 4
SELECT 
    IMIE || ' zwany ' || PSEUDO || ' (fun. ' || FUNKCJA || ') lowi myszki w bandzie ' || NR_BANDY || ' od ' || W_STADKU_OD AS "WSZYSTKO O KOCURACH"
FROM 
    KOCURY
WHERE 
    PLEC = 'M'
ORDER BY 
    W_STADKU_OD DESC, 
    PSEUDO ASC;


-- zadanie 5
SELECT 
    PSEUDO, 
    REGEXP_REPLACE(REGEXP_REPLACE(PSEUDO, 'A', '#', 1, 1), 'L', '%', 1, 1) AS "Po wymianie A na # oraz L na %"
FROM 
    KOCURY
WHERE 
    PSEUDO LIKE '%L%' 
    AND PSEUDO LIKE '%A%';


-- zadanie 6
SELECT 
    IMIE,
    W_STADKU_OD AS "W stadku", 
    ROUND(PRZYDZIAL_MYSZY / 1.1) AS "Zjadal", 
    W_STADKU_OD + INTERVAL '6' MONTH AS "Podwyzka", 
    PRZYDZIAL_MYSZY AS "Zjada"
FROM KOCURY
WHERE 
    W_STADKU_OD + INTERVAL '15' YEAR <= SYSDATE
    AND TO_CHAR(W_STADKU_OD, 'MM') BETWEEN '03' AND '09'
ORDER BY
    PRZYDZIAL_MYSZY DESC;


-- zadanie 7
SELECT
    IMIE,
    PRZYDZIAL_MYSZY * 3 AS "MYSZY KWARTALNIE",
    NVL(MYSZY_EXTRA, 0) * 3 AS "KWARTALNE DODATKI"
FROM
    KOCURY
WHERE
    PRZYDZIAL_MYSZY > 2 * NVL(MYSZY_EXTRA, 0)
    AND PRZYDZIAL_MYSZY >= 55
ORDER BY
    "MYSZY KWARTALNIE" DESC;


-- zadanie 8
SELECT
    IMIE,
    CASE
        WHEN (PRZYDZIAL_MYSZY + NVL(MYSZY_EXTRA, 0)) * 12 > 660 THEN TO_CHAR((PRZYDZIAL_MYSZY + NVL(MYSZY_EXTRA, 0)) * 12)
        WHEN (PRZYDZIAL_MYSZY + NVL(MYSZY_EXTRA, 0)) * 12 = 660 THEN 'Limit'
        ELSE 'Ponizej 660'
    END AS "Zjada rocznie"
FROM
    KOCURY
ORDER BY
    IMIE;


-- zadanie 9.1 - 29 pazdziernik
SELECT
    PSEUDO,
    W_STADKU_OD AS "W STADKU",
    CASE
        WHEN TO_CHAR(W_STADKU_OD, 'DD') <= '15' AND NEXT_DAY(LAST_DAY('2024-10-29') - 7, 'WEDNESDAY') >= '2024-10-29' THEN
            NEXT_DAY(LAST_DAY('2024-10-29') - 7, 'WEDNESDAY')
        ELSE 
            NEXT_DAY(LAST_DAY(ADD_MONTHS('2024-10-29', 1)) - 7, 'WEDNESDAY')
    END AS "WYPLATA"
FROM 
    KOCURY
ORDER BY
    W_STADKU_OD ASC;


-- zadanie 9.2 - 31 pazdziernik
SELECT
    PSEUDO,
    W_STADKU_OD AS "W STADKU",
    CASE
        WHEN TO_CHAR(W_STADKU_OD, 'DD') <= '15' AND (NEXT_DAY(LAST_DAY('2024-10-31') - 7, 'WEDNESDAY') >= '2024-10-31') THEN
            NEXT_DAY(LAST_DAY('2024-10-31') - 7, 'WEDNESDAY')
        ELSE 
            NEXT_DAY(LAST_DAY(ADD_MONTHS('2024-10-31', 1)) - 7, 'WEDNESDAY')
    END AS "WYPLATA"
FROM 
    KOCURY
ORDER BY
    W_STADKU_OD ASC;


-- zadanie 10.1 - pseudo
SELECT 
    CASE 
        WHEN COUNT(PSEUDO) = 1 THEN LPAD(PSEUDO, 15) || ' - Unikalny'
        ELSE LPAD(PSEUDO, 15) || ' - Duplikat'
    END AS "Unikalnosc atr. PSEUDO"
FROM 
    KOCURY
GROUP BY 
    PSEUDO
ORDER BY 
    PSEUDO;


-- zadanie 10.2 - szef
SELECT 
    CASE 
        WHEN COUNT(FUNKCJA) = 1 THEN LPAD(SZEF, 15) || ' - Unikalny'
        ELSE LPAD(SZEF, 15) || ' - Nieunikalny'
    END AS "Unikalnosc atr. SZEF"
FROM 
    KOCURY
WHERE 
    SZEF IS NOT NULL
GROUP BY 
    SZEF
ORDER BY 
    SZEF;

SELECT 
    SZEF,
    COUNT(MYSZY_EXTRA)
FROM
    KOCURY
GROUP BY
    SZEF;



-- zadanie 11
SELECT 
    PSEUDO AS "Pseudonim",
    COUNT(IMIE_WROGA) AS "Liczba wrogow"
FROM 
    WROGOWIE_KOCUROW
GROUP BY 
    PSEUDO
HAVING 
    COUNT(IMIE_WROGA) >= 2
ORDER BY 
    PSEUDO;


-- zadanie 12
SELECT
    'Liczba kotow=' AS "     ",
    COUNT(PSEUDO) AS " ",
    'lowi jako' AS "    ",
    FUNKCJA AS "   ",
    'i zjada max.' AS "      ",
    TO_CHAR(MAX(PRZYDZIAL_MYSZY + NVL(MYSZY_EXTRA, 0)), 'FM999.00') AS "  ",
    'myszy miesiecznie' AS "       "
FROM
    KOCURY
WHERE 
    FUNKCJA != 'SZEFUNIO'
    AND PLEC != 'M'
GROUP BY 
    FUNKCJA
HAVING
    AVG(PRZYDZIAL_MYSZY + NVL(MYSZY_EXTRA, 0)) > 50;


-- zadanie 13
SELECT
    NR_BANDY AS "Nr bandy",
    PLEC AS "Plec",
    MIN(PRZYDZIAL_MYSZY) AS "Minimalny przydzial"
FROM
    KOCURY
GROUP BY
    NR_BANDY,
    PLEC;


-- zadanie 14
SELECT 
    LEVEL AS "Poziom", 
    PSEUDO AS "Pseudonim",
    FUNKCJA AS "Funkcja",
    NR_BANDY AS "Nr bandy"
FROM 
    KOCURY
WHERE 
    PLEC = 'M'
START WITH 
    FUNKCJA = 'BANDZIOR'
CONNECT BY 
    PRIOR PSEUDO = SZEF
ORDER BY 
    NR_BANDY, 
    LEVEL, 
    PSEUDO;


-- zadanie 15
SELECT 
    RPAD('===>', 4 * (LEVEL - 1), '==>') || (LEVEL - 1) || LPAD(IMIE, 20) AS "Hierarchia", 
    NVL(SZEF, 'Sam sobie panem') AS "Pseudo szefa",
    FUNKCJA AS "Funkcja"
FROM 
    KOCURY
WHERE 
    MYSZY_EXTRA IS NOT NULL
START WITH 
    SZEF IS NULL
CONNECT BY 
    PRIOR PSEUDO = SZEF;


-- zadanie 16
SELECT 
    RPAD(' ', 4 * (LEVEL - 1), ' ') || PSEUDO AS "Droga sluzbowa"
FROM 
    Kocury
START WITH 
    W_STADKU_OD + INTERVAL '15' YEAR <= SYSDATE 
    AND plec = 'M'
    AND myszy_extra IS NULL
CONNECT BY 
    PRIOR SZEF = PSEUDO;