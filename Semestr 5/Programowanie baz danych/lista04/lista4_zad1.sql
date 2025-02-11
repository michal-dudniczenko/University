-- ustawienie formatu dat
ALTER SESSION SET NLS_DATE_FORMAT = 'YYYY-MM-DD';

/* Zad. 47. Założyć, że w stadzie kotów pojawił się podział członków stada na elitę i na plebs.
Członek elity posiada prawo do jednego sługi wybranego spośród plebsu. Dodatkowo może
gromadzić myszy na dostępnym dla każdego członka elity koncie. Konto ma zawierać dane o
dacie wprowadzenia na nie pojedynczej myszy i o dacie jej usunięcia. O tym, do kogo należy
mysz ma mówić odniesienie do jej właściciela z elity. Przyjmując te dodatkowe założenia
zdefiniować schemat bazy danych kotów (bez odpowiedników relacji Funkcje, Bandy,
Wrogowie) w postaci relacyjno-obiektowej, gdzie dane dotyczące kotów, elity, plebsu. kont,
incydentów będą określane przez odpowiednie typy obiektowe. Dla każdego z typów
zaproponować i zdefiniować przykładowe metody. Powiązania referencyjne należy
zdefiniować za pomocą typów odniesienia. Tak przygotowany schemat wypełnić danymi z
rzeczywistości kotów (dane do opisu elit, plebsu i kont zaproponować samodzielnie) a
następnie wykonać przykładowe zapytania SQL, operujące na rozszerzonym schemacie bazy,
wykorzystujące referencje (jako realizacje złączeń), podzapytania, grupowanie oraz metody
zdefiniowane w ramach typów. Dla każdego z mechanizmów (referencja, podzapytanie,
grupowanie) należy przedstawić jeden taki przykład. Zrealizować dodatkowo, w ramach
nowego, relacyjno-obiektowego schematu, po dwa wybrane zadania z list nr 2 i 3. */



/* KOCURY */

CREATE OR REPLACE TYPE KocuryT AS OBJECT (
    imie VARCHAR2(15),
    plec VARCHAR2(1),
    pseudo VARCHAR2(15),
    funkcja VARCHAR2(10),
    szef REF KocuryT,
    w_stadku_od DATE,
    przydzial_myszy NUMBER(3),
    myszy_extra NUMBER(3),
    nr_bandy NUMBER(2),
    MEMBER FUNCTION getSumaMyszy RETURN NUMBER,
    MEMBER PROCEDURE setSzef(nowySzef REF KocuryT)
);
/

CREATE OR REPLACE TYPE BODY KocuryT AS
    MEMBER FUNCTION getSumaMyszy RETURN NUMBER IS
    BEGIN
        RETURN NVL(przydzial_myszy, 0) +  NVL(myszy_extra, 0);
    END;

    MEMBER PROCEDURE setSzef(nowySzef REF KocuryT) IS
    BEGIN
        szef := nowySzef;
    END;
END;
/


/* INCYDENTY */

CREATE OR REPLACE TYPE IncydentyT AS OBJECT (
    id NUMBER,
    pseudo REF KocuryT,
    imie_wroga VARCHAR2(15),
    data_incydentu DATE,
    opis_incydentu VARCHAR2(50),
    MEMBER FUNCTION toString RETURN VARCHAR2
);
/


CREATE OR REPLACE TYPE BODY IncydentyT AS
    MEMBER FUNCTION toString RETURN VARCHAR2 IS
        kot KocuryT;
    BEGIN
        SELECT (pseudo) INTO kot FROM DUAL;
        RETURN 'INCYDENT KOTA: ' || kot.imie || ' Z WROGIEM: ' || imie_wroga || ' DNIA: ' || data_incydentu;
    END;
END;
/


/* PLEBS */

CREATE OR REPLACE TYPE PlebsT AS OBJECT (
    pseudo REF KocuryT,
    MEMBER FUNCTION toString RETURN VARCHAR2
);
/


CREATE OR REPLACE TYPE BODY PlebsT AS
    MEMBER FUNCTION toString RETURN VARCHAR2 IS
        kot KocuryT;
    BEGIN
        SELECT (pseudo) INTO kot FROM DUAL;
        RETURN 'IMIE: ' || kot.imie || ' PSEUDO: ' || kot.pseudo || ' FUNKCJA: ' || kot.funkcja;
    END;
END;
/


/* ELITA */

CREATE OR REPLACE TYPE ElitaT AS OBJECT (
    pseudo REF KocuryT,
    sluga REF PlebsT,
    MEMBER FUNCTION getDaneSlugi RETURN VARCHAR2
);
/


CREATE OR REPLACE TYPE BODY ElitaT AS
    MEMBER FUNCTION getDaneSlugi RETURN VARCHAR2 IS
        kot PlebsT;
    BEGIN
        IF sluga IS NOT NULL THEN
            SELECT (sluga) INTO kot FROM DUAL;
            RETURN kot.toString();
        ELSE
            RETURN 'Kot nie ma slugi!';
        END IF;
    END;
END;
/


/* KONTO */

CREATE OR REPLACE TYPE KontoT AS OBJECT (
    id NUMBER,
    data_dodania DATE,
    data_wyplaty DATE,
    pseudo REF ElitaT,
    MEMBER PROCEDURE wyplataMyszy(dataWyplaty DATE DEFAULT SYSDATE)
);
/


CREATE OR REPLACE TYPE BODY KontoT AS
    MEMBER PROCEDURE wyplataMyszy(dataWyplaty DATE DEFAULT SYSDATE) IS
    BEGIN
        data_wyplaty := dataWyplaty;
    END;
END;
/


/* TABELE */

CREATE TABLE KocuryO OF KocuryT (
    CONSTRAINT kocuryO_imie_nn CHECK (imie IS NOT NULL),
    CONSTRAINT kocuryO_plec_ch CHECK (plec IN ('M', 'D')),
    CONSTRAINT kocuryO_pseudo_pk PRIMARY KEY (pseudo)
);

CREATE TABLE IncydentyO OF IncydentyT (
    CONSTRAINT incydentyO_imie_wroga_nn CHECK (imie_wroga IS NOT NULL),
    CONSTRAINT incydentyO_data_nn CHECK (data_incydentu IS NOT NULL),
    CONSTRAINT incydentyO_pk PRIMARY KEY (id)
);

CREATE TABLE PlebsO OF PlebsT;

CREATE TABLE ElitaO OF ElitaT;

CREATE TABLE KontoO OF KontoT (
    CONSTRAINT kontoO_id_pk PRIMARY KEY (id)
);

DROP TYPE KocuryT;
DROP TYPE IncydentyT;
DROP TYPE PlebsT;
DROP TYPE ElitaT;
DROP TYPE KontoT;

DROP TABLE KocuryO;
DROP TABLE IncydentyO;
DROP TABLE PlebsO;
DROP TABLE ElitaO;
DROP TABLE KontoO;



/* -------------------------------------------------------------------------------------------- */

INSERT INTO KocuryO VALUES (KocuryT('MRUCZEK','M','TYGRYS','SZEFUNIO',NULL,'2002-01-01',103,33,1));
INSERT INTO KocuryO VALUES (KocuryT('JACEK','M','PLACEK','LOWCZY',NULL,'2008-12-01',67,NULL,2));
INSERT INTO KocuryO VALUES (KocuryT('BARI','M','RURA','LAPACZ',NULL,'2009-09-01',56,NULL,2));
INSERT INTO KocuryO VALUES (KocuryT('MICKA','D','LOLA','MILUSIA',NULL,'2009-10-14',25,47,1));
INSERT INTO KocuryO VALUES (KocuryT('LUCEK','M','ZERO','KOT',NULL,'2010-03-01',43,NULL,3));
INSERT INTO KocuryO VALUES (KocuryT('SONIA','D','PUSZYSTA','MILUSIA',NULL,'2010-11-18',20,35,3));
INSERT INTO KocuryO VALUES (KocuryT('LATKA','D','UCHO','KOT',NULL,'2011-01-01',40,NULL,4));
INSERT INTO KocuryO VALUES (KocuryT('DUDEK','M','MALY','KOT',NULL,'2011-05-15',40,NULL,4));
INSERT INTO KocuryO VALUES (KocuryT('CHYTRY','M','BOLEK','DZIELCZY',NULL,'2002-05-05',50,NULL,1));
INSERT INTO KocuryO VALUES (KocuryT('KOREK','M','ZOMBI','BANDZIOR',NULL,'2004-03-16',75,13,3));
INSERT INTO KocuryO VALUES (KocuryT('BOLEK','M','LYSY','BANDZIOR',NULL,'2006-08-15',72,21,2));
INSERT INTO KocuryO VALUES (KocuryT('ZUZIA','D','SZYBKA','LOWCZY',NULL,'2006-07-21',65,NULL,2));
INSERT INTO KocuryO VALUES (KocuryT('RUDA','D','MALA','MILUSIA',NULL,'2006-09-17',22,42,1));
INSERT INTO KocuryO VALUES (KocuryT('PUCEK','M','RAFA','LOWCZY',NULL,'2006-10-15',65,NULL,4));
INSERT INTO KocuryO VALUES (KocuryT('PUNIA','D','KURKA','LOWCZY',NULL,'2008-01-01',61,NULL,3));
INSERT INTO KocuryO VALUES (KocuryT('BELA','D','LASKA','MILUSIA',NULL,'2008-02-01',24,28,2));
INSERT INTO KocuryO VALUES (KocuryT('KSAWERY','M','MAN','LAPACZ',NULL,'2008-07-12',51,NULL,4));
INSERT INTO KocuryO VALUES (KocuryT('MELA','D','DAMA','LAPACZ',NULL,'2008-11-01',51,NULL,4));

DECLARE
    CURSOR kocury_cursor IS
        SELECT REFk AS k_ref, k.pseudo
        FROM KocuryO k;

    szef REF KocuryT;
    pseudo_szefa KocuryO.pseudo%TYPE;
    kocur_obj KocuryT;
BEGIN
    FOR kocur IN kocury_cursor LOOP
        SELECT szef INTO pseudo_szefa FROM Kocury WHERE pseudo = kocur.pseudo;
        IF pseudo_szefa IS NOT NULL THEN
            SELECT REFk INTO szef FROM KocuryO k WHERE k.pseudo = pseudo_szefa;

            SELECT (kocur.k_ref) INTO kocur_obj FROM DUAL;

            kocur_obj.setSzef(szef);

            UPDATE KocuryO k
            SET VALUEk = kocur_obj
            WHERE REFk = kocur.k_ref;
        END IF;
    END LOOP;
END;
/

SELECT k.imie,
       k.plec,
       k.pseudo,
       k.funkcja,
       k.w_stadku_od,
       k.przydzial_myszy,
       k.myszy_extra,
       k.nr_bandy,
       (k.szef).pseudo AS szef_pseudo,
       k.getSumaMyszy() AS suma_myszy
FROM KocuryO k;

CREATE OR REPLACE PROCEDURE DodajIncydent(
    id NUMBER,
    pseudoKota VARCHAR2, 
    imieWroga VARCHAR2, 
    dataIncydentu DATE, 
    opisIncydentu VARCHAR2
) AS
    kot REF KocuryT;
BEGIN
    SELECT REFk INTO kot
    FROM KocuryO k
    WHERE k.pseudo = pseudoKota;

    INSERT INTO IncydentyO VALUES (IncydentyT(id, kot, imieWroga, dataIncydentu, opisIncydentu));
END;
/

BEGIN
    DodajIncydent(1,'TYGRYS','KAZIO','2004-10-13','USILOWAL NABIC NA WIDLY');
    DodajIncydent(2,'ZOMBI','SWAWOLNY DYZIO','2005-03-07','WYBIL OKO Z PROCY');
    DodajIncydent(3,'BOLEK','KAZIO','2005-03-29','POSZCZUL BURKIEM');
    DodajIncydent(4,'SZYBKA','GLUPIA ZOSKA','2006-09-12','UZYLA KOTA JAKO SCIERKI');
    DodajIncydent(5,'MALA','CHYTRUSEK','2007-03-07','ZALECAL SIE');
    DodajIncydent(6,'TYGRYS','DZIKI BILL','2007-06-12','USILOWAL POZBAWIC ZYCIA');
    DodajIncydent(7,'BOLEK','DZIKI BILL','2007-11-10','ODGRYZL UCHO');
    DodajIncydent(8,'LASKA','DZIKI BILL','2008-12-12','POGRYZL ZE LEDWO SIE WYLIZALA');
    DodajIncydent(9,'LASKA','KAZIO','2009-01-07','ZLAPAL ZA OGON I ZROBIL WIATRAK');
    DodajIncydent(10,'DAMA','KAZIO','2009-02-07','CHCIAL OBEDRZEC ZE SKORY');
    DodajIncydent(11,'MAN','REKSIO','2009-04-14','WYJATKOWO NIEGRZECZNIE OBSZCZEKAL');
    DodajIncydent(12,'LYSY','BETHOVEN','2009-05-11','NIE PODZIELIL SIE SWOJA KASZA');
    DodajIncydent(13,'RURA','DZIKI BILL','2009-09-03','ODGRYZL OGON');
    DodajIncydent(14,'PLACEK','BAZYLI','2010-07-12','DZIOBIAC UNIEMOZLIWIL PODEBRANIE KURCZAKA');
    DodajIncydent(15,'PUSZYSTA','SMUKLA','2010-11-19','OBRZUCILA SZYSZKAMI');
    DodajIncydent(16,'KURKA','BUREK','2010-12-14','POGONIL');
    DodajIncydent(17,'MALY','CHYTRUSEK','2011-07-13','PODEBRAL PODEBRANE JAJKA');
    DodajIncydent(18,'UCHO','SWAWOLNY DYZIO','2011-07-14','OBRZUCIL KAMIENIAMI');
END;
/

SELECT 
    i.id,
    i.toString()
FROM IncydentyO i;


CREATE OR REPLACE PROCEDURE DodajPlebs(
    pseudoKota VARCHAR2
) AS
    kot REF KocuryT;
BEGIN
    SELECT REFk INTO kot
    FROM KocuryO k
    WHERE k.pseudo = pseudoKota;

    INSERT INTO PlebsO VALUES (PlebsT(kot));
END;
/

BEGIN
    DodajPlebs('PUSZYSTA');
    DodajPlebs('MALA');
    DodajPlebs('LASKA');
    DodajPlebs('UCHO');
    DodajPlebs('MALY');
END;
/

SELECT p.toString() FROM PLEBSO p;


CREATE OR REPLACE PROCEDURE DodajElita(
    pseudoKota VARCHAR2,
    pseudoSlugi VARCHAR2
) AS
    kot REF KocuryT;
    sluga REF PlebsT;
BEGIN
    -- Pobranie referencji na kota
    SELECT REFk INTO kot
    FROM KocuryO k
    WHERE k.pseudo = pseudoKota;

    -- Pobranie referencji na sługę
    SELECT REF(p) INTO sluga
    FROM PlebsO p
    WHERE p.pseudo.PSEUDO = pseudoSlugi;

    -- Wstawienie do tabeli ElitaO
    INSERT INTO ElitaO VALUES (ElitaT(kot, sluga));
END;
/

BEGIN
    DodajElita('TYGRYS','PUSZYSTA');
    DodajElita('ZOMBI','MALA');
    DodajElita('LYSY','LASKA');
    DodajElita('PLACEK','UCHO');
    DodajElita('RAFA','MALY');
    DodajElita('SONIA', 'PUSZYSTA');
    DodajElita('ZERO', 'MALA');
    DodajElita('LOLA', 'PUSZYSTA');
    DodajElita('ZERO', 'MALA');
end;
/

SELECT e.pseudo.pseudo, e.getDaneSlugi() FROM ElitaO e;

CREATE OR REPLACE PROCEDURE DodajKonto(
    id NUMBER,
    dataDodania DATE,
    dataWyplaty DATE DEFAULT NULL,
    pseudoElity VARCHAR2
) AS
    elita REF ElitaT;
BEGIN
    SELECT REF(e) INTO elita
    FROM ElitaO e
    WHERE e.pseudo.PSEUDO = pseudoElity;

    INSERT INTO KontoO VALUES (KontoT(id, dataDodania, dataWyplaty, elita));
END;
/

BEGIN
    DodajKonto(1, '2020-01-01', '2020-01-31', 'TYGRYS');
    DodajKonto(2, '2020-02-01', '2020-02-28', 'ZOMBI');
    DodajKonto(3, '2020-03-01', '2020-03-31', 'LYSY');
    DodajKonto(4, '2020-04-01', '2020-04-30', 'PLACEK');
    DodajKonto(5, '2020-05-01', '2020-05-31', 'RAFA');
    DodajKonto(6, '2020-01-01', NULL, 'ZERO');
    DodajKonto(7, '2020-02-01', NULL, 'ZERO');
    DodajKonto(8, '2020-03-01', NULL, 'ZERO');
    DodajKonto(9, '2020-04-01', NULL, 'LOLA');
    DodajKonto(10, '2020-05-01', NULL, 'RAFA');
    DodajKonto(11, '2020-01-01', NULL, 'TYGRYS');
    DodajKonto(12, '2020-02-01', NULL, 'ZOMBI');
    DodajKonto(13, '2020-03-01', NULL, 'LYSY');
    DodajKonto(14, '2020-04-01', NULL, 'PLACEK');
    DodajKonto(15, '2020-05-01', NULL, 'RAFA');
    DodajKonto(16, '2020-01-01', NULL, 'TYGRYS');
    DodajKonto(17, '2020-02-01', NULL, 'ZOMBI');
    DodajKonto(18, '2020-03-01', NULL, 'LYSY');
    DodajKonto(19, '2020-04-01', NULL, 'PLACEK');
    DodajKonto(20, '2020-05-01', NULL, 'RAFA');
END;
/

SELECT k.id, k.data_dodania, k.data_wyplaty, k.pseudo.pseudo.pseudo FROM KontoO k;


/* PRZYKŁADOWE ZAPYTANIA */

-- Referencja

SELECT 
    k.imie,
    k.pseudo,
    (k.szef).imie AS "imie szefa",
    (k.szef).funkcja AS "funkcja szefa"
FROM KocuryO k;

-- Podzapytanie

SELECT 
    k.imie,
    k.pseudo,
    k.funkcja,
    (SELECT COUNT(*) FROM IncydentyO i WHERE i.pseudo = REFk) AS "liczba incydentow"
FROM KocuryO k
ORDER BY 4 DESC;

-- Grupowanie

SELECT 
    k.funkcja,
    SUM(k.getSumaMyszy()) AS "suma myszy"
FROM KocuryO k
GROUP BY k.funkcja;


/* Zad. 18. Wyświetlić bez stosowania podzapytania imiona i daty przystąpienia do stada
kotów, które przystąpiły do stada przed kotem o imieniu ’JACEK’. Wyniki uporządkować
malejąco wg daty przystąpienia do stadka. */
SELECT 
    k1.imie AS "IMIE",
    k1.w_stadku_od AS "POLUJE OD"
FROM KocuryO k1, KocuryO k2
WHERE k2.imie = 'JACEK' AND k1.w_stadku_od < k2.w_stadku_od
ORDER BY 2 DESC;


/* Zad. 22. Znaleźć koty (wraz z pełnioną funkcją), które posiadają więcej niż jednego wroga. */
SELECT 
    k.funkcja AS "Funkcja",
    k.pseudo AS "Pseudonim kota",
    COUNT(DISTINCT i.imie_wroga) AS "Liczba wrogów"
FROM KocuryO k
INNER JOIN IncydentyO i ON i.pseudo = REF(k)
GROUP BY k.pseudo, k.funkcja
HAVING COUNT(DISTINCT i.imie_wroga) > 1;


/* Zad. 34. Napisać blok PL/SQL, który wybiera z relacji Kocury koty o funkcji podanej z
klawiatury. Jedynym efektem działania bloku ma być komunikat informujący czy znaleziono,
czy też nie, kota pełniącego podaną funkcję (w przypadku znalezienia kota wyświetlić nazwę
odpowiedniej funkcji). */
DECLARE
    v_funkcja       VARCHAR2(10);
    v_znaleziono    NUMBER := 0;
BEGIN
    DBMS_OUTPUT.PUT_LINE('Podaj nazwę funkcji do wyszukania: ');
    v_funkcja := UPPER(:funkcja);

    SELECT COUNT(k.pseudo)
    INTO v_znaleziono
    FROM KocuryO k
    WHERE k.funkcja = v_funkcja;

    IF v_znaleziono > 0 THEN
        DBMS_OUTPUT.PUT_LINE('Znaleziono kota o funkcji: ' || v_funkcja);
    ELSE 
        DBMS_OUTPUT.PUT_LINE('Nie znaleziono kota o funkcji: ' || v_funkcja);
    END IF;
END;
/

/* Zad. 35. Napisać blok PL/SQL, który wyprowadza na ekran następujące informacje o kocie o
pseudonimie wprowadzonym z klawiatury (w zależności od rzeczywistych danych):
- 'calkowity roczny przydzial myszy >700'
- 'imię zawiera litere A'
- 'maj jest miesiacem przystapienia do stada'
- 'nie odpowiada kryteriom'.
Powyższe informacje wymienione są zgodnie z ich hierarchią ważności, należy więc
wyprowadzić pierwszą prawdziwą informację o kocie pomijając sprawdzanie następnych. */
DECLARE
    v_pseudo VARCHAR2(15); 
    v_roczny_przydzial NUMBER;
    v_imie VARCHAR2(15);
    v_data_przystapienia DATE;
BEGIN
    DBMS_OUTPUT.PUT_LINE('Podaj pseudonim kota:');
    v_pseudo := UPPER(:pseudo);

    SELECT k.getSumaMyszy() * 12, k.imie, k.w_stadku_od
    INTO v_roczny_przydzial, v_imie, v_data_przystapienia
    FROM KocuryO k
    WHERE k.pseudo = v_pseudo;

    IF v_roczny_przydzial > 700 THEN
        DBMS_OUTPUT.PUT_LINE('Calkowity roczny przydzial myszy >700');
    ELSIF INSTR(v_imie, 'A') > 0 THEN
        DBMS_OUTPUT.PUT_LINE('Imię zawiera litere A');
    ELSIF EXTRACT(MONTH FROM v_data_przystapienia) = 5 THEN
        DBMS_OUTPUT.PUT_LINE('Maj jest miesiacem przystapienia do stada');
    ELSE
        DBMS_OUTPUT.PUT_LINE('Nie odpowiada kryteriom');
    END IF;

EXCEPTION
    WHEN NO_DATA_FOUND THEN
        DBMS_OUTPUT.PUT_LINE('Nie znaleziono kota o podanym pseudonimie.');
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Wystąpił błąd: ' || SQLERRM);
END;
/


/* znalezc ile myszy na stanie na koncie ma kazdy kot plci meskiej z elity ktory 
do tej pory nie uczestniczyl w zadnym incydencie bez zlaczen po referencjach */

SELECT 
    COUNT(*) AS "Stan konta",
    k.pseudo.pseudo.pseudo AS "Pseudonim kota"
FROM KontoO k
WHERE k.pseudo.pseudo.plec = 'M' AND DATA_WYPLATY IS NULL AND NOT EXISTS (
      SELECT 1
      FROM IncydentyO i
      WHERE i.pseudo.pseudo = k.pseudo.pseudo.pseudo
)
GROUP BY k.pseudo.pseudo.pseudo;
