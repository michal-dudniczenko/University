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

    SELECT COUNT(*)
    INTO v_znaleziono
    FROM Kocury
    WHERE funkcja = v_funkcja;

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

    SELECT (przydzial_myszy + NVL(myszy_extra, 0)) * 12, imie, w_stadku_od
    INTO v_roczny_przydzial, v_imie, v_data_przystapienia
    FROM Kocury
    WHERE pseudo = v_pseudo;

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


/* Zad. 36. W związku z dużą wydajnością w łowieniu myszy SZEFUNIO postanowił
wynagrodzić swoich podwładnych. Ogłosił więc, że podwyższa indywidualny przydział
myszy każdego kota o 10%, w kolejności od kota o najniższym przydziale począwszy na
kocie o najwyższym przydziale skończywszy. Ustalił też, że proces zwiększania przydziału
myszy należy zakończyć gdy suma przydziałów wszystkich kotów przekroczy liczbę 1050.
Jeśli dla jakiegoś kota przydział myszy po podwyżce przekroczy maksymalną wartość
należną dla pełnionej funkcji (max_myszy z relacji Funkcje), przydział myszy po podwyżce
ma być równy tej wartości. Napisać blok PL/SQL z kursorem, który realizuje to zadanie. Blok
ma działać tak długo, aż suma wszystkich przydziałów rzeczywiście przekroczy 1050 (liczba
„obiegów podwyżkowych” może być większa od 1 a więc i podwyżka może być większa niż
10%). Wyświetlić na ekranie sumę przydziałów myszy po wykonaniu zadania wraz z liczbą
liczbą modyfikacji w relacji Kocury. Na końcu wycofać wszystkie zmiany. */
DECLARE
    CURSOR c_kocury IS
        SELECT pseudo, przydzial_myszy, max_myszy
        FROM Kocury k
        JOIN Funkcje f ON f.funkcja = k.funkcja
        ORDER BY przydzial_myszy ASC
    FOR UPDATE OF przydzial_myszy;

    v_pseudo VARCHAR2(15);
    v_przydzial NUMBER(3);
    v_max_myszy NUMBER(3);
    v_nowy_przydzial NUMBER(3);
    v_suma_przydzialow NUMBER;
    v_modyfikacje NUMBER := 0;

BEGIN
    SELECT SUM(przydzial_myszy) INTO v_suma_przydzialow FROM Kocury;
    DBMS_OUTPUT.PUT_LINE('Suma przydziałów przed modyfikacjami: ' || v_suma_przydzialow);

    <<outer>>LOOP
        FOR rec IN c_kocury LOOP
        
            IF v_suma_przydzialow > 1050 THEN
                EXIT outer;
            END IF;

            v_pseudo := rec.pseudo;
            v_przydzial := rec.przydzial_myszy;
            v_max_myszy := rec.max_myszy;

            IF v_przydzial = v_max_myszy THEN 
                CONTINUE; 
            END IF;

            v_nowy_przydzial := v_przydzial + ROUND(v_przydzial * 0.1, 0);

            IF v_nowy_przydzial > v_max_myszy THEN
                v_nowy_przydzial := v_max_myszy;
            END IF;

			v_suma_przydzialow := v_suma_przydzialow + (v_nowy_przydzial - v_przydzial);

            UPDATE Kocury
            SET przydzial_myszy = v_nowy_przydzial
            WHERE pseudo = v_pseudo;

            v_modyfikacje := v_modyfikacje + 1;
        END LOOP;
    END LOOP;

    DBMS_OUTPUT.PUT_LINE('Calk. przydzial w stadku ' || v_suma_przydzialow || ' Zmian - ' || v_modyfikacje);

EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Wystąpił błąd: ' || SQLERRM);
END;
/

SELECT imie, przydzial_myszy AS "Myszki po podwyzce" FROM Kocury ORDER BY przydzial_myszy DESC ;
ROLLBACK;



/* Zad. 37. Napisać blok, który powoduje wybranie w pętli kursorowej FOR pięciu kotów o
najwyższym całkowitym przydziale myszy. Wynik wyświetlić na ekranie. */
DECLARE
    CURSOR top5_koty IS
        SELECT pseudo, (przydzial_myszy + NVL(myszy_extra, 0)) AS przydzial
        FROM Kocury
        ORDER BY 2 DESC
        FETCH FIRST 5 ROWS ONLY;

    v_rank NUMBER := 1;
BEGIN
    DBMS_OUTPUT.PUT_LINE('Nr  Pseudonim   Zjada');
    DBMS_OUTPUT.PUT_LINE('--------------------');

    FOR rec IN top5_koty LOOP
        DBMS_OUTPUT.PUT_LINE(RPAD(v_rank, 4, ' ') || RPAD(rec.pseudo, 12, ' ') || rec.przydzial);
        v_rank := v_rank + 1;
    END LOOP;
END;
/


/* Zad. 38. Napisać blok, który zrealizuje wersję a. lub wersję b. zad. 19 w sposób uniwersalny
(bez konieczności uwzględniania wiedzy o głębokości drzewa). Daną wejściową ma być
maksymalna liczba wyświetlanych przełożonych.

Zad. 19. Dla kotów pełniących funkcję KOT i MILUSIA wyświetlić w kolejności hierarchii
imiona wszystkich ich szefów.

DECLARE
    v_max_szefow NUMBER := 3;
    v_sql VARCHAR2(1000);
    v_i NUMBER;
    v_cursor INTEGER;
BEGIN
    v_sql := 'SELECT k1.imie AS "Imie", k1.funkcja AS "Funkcja"';

    v_i := 0;
    WHILE v_i < v_max_szefow LOOP
        v_sql := v_sql || 
            ', NVL(k' || (v_i + 2) || '.imie, '' '') AS "Szef ' || (v_i + 1) || '"';
        v_i := v_i + 1;
    END LOOP;

    v_sql := v_sql || ' FROM kocury k1';

    v_i := 0;
    WHILE v_i < v_max_szefow LOOP
        v_sql := v_sql || 
            ' LEFT JOIN kocury k' || (v_i + 2) || 
            ' ON k' || (v_i + 1) || '.szef = k' || (v_i + 2) || '.pseudo';
        v_i := v_i + 1;
    END LOOP;

    v_sql := v_sql || ' WHERE k1.funkcja IN (''KOT'', ''MILUSIA'')';

    v_cursor := DBMS_SQL.OPEN_CURSOR;
    DBMS_SQL.PARSE(v_cursor, v_sql, DBMS_SQL.NATIVE);

    DBMS_SQL.RETURN_RESULT(v_cursor);

    DBMS_OUTPUT.PUT_LINE(v_sql);
END;
/
*/
DECLARE
    liczba_przelozonych     NUMBER := 5;
    szerokosc_kol           NUMBER := 15;
    pseudo_aktualny         kocury.pseudo%TYPE;
    imie_aktualny           kocury.imie%TYPE;
    pseudo_nastepny         kocury.szef%TYPE;
    CURSOR koty IS SELECT pseudo, imie, funkcja
                    FROM kocury
                    WHERE funkcja IN ('MILUSIA', 'KOT');
BEGIN
    DBMS_OUTPUT.PUT(RPAD('IMIE ', szerokosc_kol));
	DBMS_OUTPUT.PUT(RPAD('|  FUNKCJA ', szerokosc_kol));
    FOR i IN 1..liczba_przelozonych LOOP
        DBMS_OUTPUT.PUT(RPAD('|  Szef ' || i, szerokosc_kol));
    END LOOP;
    DBMS_OUTPUT.PUT_LINE('');
    DBMS_OUTPUT.PUT_LINE(RPAD('-', szerokosc_kol * (liczba_przelozonych + 2), '-'));

    FOR kot IN koty LOOP
        DBMS_OUTPUT.PUT(RPAD(KOT.imie, szerokosc_kol));
		DBMS_OUTPUT.PUT(RPAD('|  ' || KOT.funkcja, szerokosc_kol));
        SELECT szef INTO pseudo_nastepny FROM kocury WHERE pseudo = kot.pseudo;
        FOR i IN 1..liczba_przelozonych LOOP
            IF pseudo_nastepny IS NULL THEN
                DBMS_OUTPUT.PUT(RPAD('|  ', szerokosc_kol));
            ELSE
                SELECT K.imie, K.szef
                INTO imie_aktualny, pseudo_nastepny
                FROM kocury K
                WHERE K.pseudo = pseudo_nastepny;
                DBMS_OUTPUT.PUT(RPAD('|  ' || imie_aktualny, szerokosc_kol));
            END IF;
        END LOOP;
        DBMS_OUTPUT.PUT_LINE('');
    END LOOP;
END;


/* Zad. 39. Napisać blok PL/SQL wczytujący trzy parametry reprezentujące nr bandy, nazwę
bandy oraz teren polowań. Skrypt ma uniemożliwiać wprowadzenie istniejących już wartości
parametrów poprzez obsługę odpowiednich wyjątków. Sytuacją wyjątkową jest także
wprowadzenie numeru bandy <=0. W przypadku zaistnienia sytuacji wyjątkowej należy
wyprowadzić na ekran odpowiedni komunikat. W przypadku prawidłowych parametrów
należy utworzyć nową bandę w relacji Bandy. Zmianę należy na końcu wycofać. */
DECLARE
    v_nr_bandy NUMBER(2) := (1);
    v_nazwa VARCHAR2(20) := UPPER('szefostwo');
    v_teren VARCHAR2(15) := UPPER('calosc');

    v_is_wrong NUMBER(1) := 0;
    v_is_error BOOLEAN := false;
    v_error_message VARCHAR2(50) := '';

    invalid_nr EXCEPTION;
    parameter_exists EXCEPTION;
BEGIN
    IF v_nr_bandy <= 0 THEN 
        RAISE invalid_nr;
    END IF;

    SELECT COUNT(*) INTO v_is_wrong FROM Bandy WHERE nr_bandy = v_nr_bandy;
    IF v_is_wrong > 0 THEN
        v_error_message := v_error_message || v_nr_bandy;
        v_is_error := true;
    END IF;

    SELECT COUNT(*) INTO v_is_wrong FROM Bandy WHERE nazwa = v_nazwa;
    IF v_is_wrong > 0 THEN
        IF v_is_error THEN
            v_error_message := v_error_message || ', ';
        END IF;
        v_error_message := v_error_message || v_nazwa;        
        v_is_error := true;
    END IF;

    SELECT COUNT(*) INTO v_is_wrong FROM Bandy WHERE teren = v_teren;
    IF v_is_wrong > 0 THEN
        IF v_is_error THEN
            v_error_message := v_error_message || ', ';
        END IF;
        v_error_message := v_error_message || v_teren;
        v_is_error := true;
    END IF;

    IF v_is_error THEN RAISE parameter_exists;
    END IF;

    INSERT INTO Bandy(nr_bandy, nazwa, teren)
    VALUES (v_nr_bandy, v_nazwa, v_teren);

    DBMS_OUTPUT.PUT_LINE('Banda (' || v_nr_bandy || ', ' || v_nazwa || ', ' || v_teren || ') dodana.');

EXCEPTION
    WHEN invalid_nr THEN DBMS_OUTPUT.PUT_LINE('Numer bandy musi być liczbą dodatnią.');
    WHEN parameter_exists THEN DBMS_OUTPUT.PUT_LINE(v_error_message || ': juz istnieje');
END;
/

SELECT * FROM bandy;
ROLLBACK;


/* Zad. 40. Przerobić blok z zadania 39 na procedurę umieszczoną w bazie danych. */
CREATE OR REPLACE PROCEDURE Dodaj_bande(
    p_nr_bandy NUMBER,
    p_nazwa_bandy VARCHAR2,
    p_teren_bandy VARCHAR2
) AS
    v_nr_bandy NUMBER(2) := p_nr_bandy;
    v_nazwa VARCHAR2(20) := UPPER(p_nazwa_bandy);
    v_teren VARCHAR2(15) := UPPER(p_teren_bandy);

    v_is_wrong NUMBER(1) := 0;
    v_is_error BOOLEAN := false;
    v_error_message VARCHAR2(50) := '';

    invalid_nr EXCEPTION;
    parameter_exists EXCEPTION;
BEGIN
    IF v_nr_bandy <= 0 THEN 
        RAISE invalid_nr;
    END IF;

    SELECT COUNT(*) INTO v_is_wrong FROM Bandy WHERE nr_bandy = v_nr_bandy;
    IF v_is_wrong > 0 THEN
        v_error_message := v_error_message || v_nr_bandy;
        v_is_error := true;
    END IF;

    SELECT COUNT(*) INTO v_is_wrong FROM Bandy WHERE nazwa = v_nazwa;
    IF v_is_wrong > 0 THEN
        IF v_is_error THEN
            v_error_message := v_error_message || ', ';
        END IF;
        v_error_message := v_error_message || v_nazwa;        
        v_is_error := true;
    END IF;

    SELECT COUNT(*) INTO v_is_wrong FROM Bandy WHERE teren = v_teren;
    IF v_is_wrong > 0 THEN
        IF v_is_error THEN
            v_error_message := v_error_message || ', ';
        END IF;
        v_error_message := v_error_message || v_teren;
        v_is_error := true;
    END IF;

    IF v_is_error THEN RAISE parameter_exists;
    END IF;

    INSERT INTO Bandy(nr_bandy, nazwa, teren)
    VALUES (v_nr_bandy, v_nazwa, v_teren);

    DBMS_OUTPUT.PUT_LINE('Banda (' || v_nr_bandy || ', ' || v_nazwa || ', ' || v_teren || ') dodana.');

EXCEPTION
    WHEN invalid_nr THEN DBMS_OUTPUT.PUT_LINE('Numer bandy musi być liczbą dodatnią.');
    WHEN parameter_exists THEN DBMS_OUTPUT.PUT_LINE(v_error_message || ': juz istnieje');
END Dodaj_bande;
/

SELECT * FROM bandy;
EXECUTE Dodaj_bande(10, 'nowa_banda', 'nowy_teren');
SELECT * FROM bandy;
DELETE FROM bandy WHERE nr_bandy = 10;
SELECT * FROM bandy;


/* Zad. 41. Zdefiniować wyzwalacz, który zapewni, że numer nowej bandy będzie zawsze
większy o 1 od najwyższego numeru istniejącej już bandy. Sprawdzić działanie wyzwalacza
wykorzystując procedurę z zadania 40. */
CREATE OR REPLACE TRIGGER trg_nr_nowej_bandy
BEFORE INSERT ON Bandy
FOR EACH ROW
DECLARE
    v_max_nr_bandy NUMBER(2);
BEGIN
    SELECT NVL(MAX(nr_bandy), 0) INTO v_max_nr_bandy FROM Bandy;

    :NEW.nr_bandy := v_max_nr_bandy + 1;
END;

SELECT * FROM bandy;
EXECUTE Dodaj_bande(10, 'nowa_banda', 'nowy_teren');
SELECT * FROM bandy;
DELETE FROM bandy WHERE nr_bandy = 6;
SELECT * FROM bandy;


/* Zad. 42. Milusie postanowiły zadbać o swoje interesy. Wynajęły więc informatyka, aby
zapuścił wirusa w system Tygrysa. Teraz przy każdej próbie zmiany przydziału myszy na
plus (o minusie w ogóle nie może być mowy - próba takiej zmiany ma być blokowana, w
ramach zaproponowanego mechanizmu, wyjątkiem) o wartość mniejszą niż 10% przydziału
myszy Tygrysa żal Miluś ma być utulony podwyżką ich przydziału o tą 10%’ową wartość
oraz podwyżką myszy extra o 5. Tygrys ma być ukarany stratą wspomnianych 10%. Jeśli
jednak podwyżka będzie satysfakcjonowała Milusie (podwyżka o 10% i więcej), przydział
myszy extra Tygrysa ma wzrosnąć o 5. 
Zaproponować dwa rozwiązania zadania, które ominą podstawowe ograniczenie dla
wyzwalacza wierszowego aktywowanego poleceniem DML tzn. brak możliwości odczytu lub
zmiany relacji, na której operacja (polecenie DML) „wyzwala” ten wyzwalacz. W pierwszym
rozwiązaniu (klasycznym) wykorzystać kilku wyzwalaczy i pamięć w postaci specyfikacji
dedykowanego zadaniu pakietu, w drugim wykorzystać wyzwalacz COMPOUND.
Podać przykład funkcjonowania wyzwalaczy a następnie zlikwidować wprowadzone przez
nie zmiany.*/
CREATE OR REPLACE PACKAGE memory AS
    dobre_zmiany NUMBER := 0; 
    zle_zmiany NUMBER := 0;
    przydzial_tygrysa NUMBER := 0;
    zmiana_wyzwalacza BOOLEAN := FALSE;
END;
/

CREATE OR REPLACE TRIGGER przydzial_trygrysa_init
BEFORE UPDATE OF przydzial_myszy ON kocury
BEGIN
    IF NOT memory.zmiana_wyzwalacza THEN
        SELECT przydzial_myszy INTO memory.przydzial_tygrysa 
        FROM kocury WHERE pseudo = 'TYGRYS';
    END IF;
END;
/

CREATE OR REPLACE TRIGGER Przed_Zmiana
BEFORE UPDATE OF przydzial_myszy ON kocury
FOR EACH ROW
DECLARE
    v_roznica NUMBER;
    zmiana_na_minus EXCEPTION;
BEGIN
    IF NOT memory.zmiana_wyzwalacza AND :NEW.pseudo != 'TYGRYS' THEN
        v_roznica := :NEW.przydzial_myszy - :OLD.przydzial_myszy;
    
        IF v_roznica < 0 THEN
            RAISE zmiana_na_minus;
    	ELSIF v_roznica > 0 AND v_roznica < 0.1 * memory.przydzial_tygrysa THEN
            memory.zle_zmiany := memory.zle_zmiany + 1;
        ELSIF v_roznica > 0 THEN
            memory.dobre_zmiany := memory.dobre_zmiany + 1;
    	END IF;
	END IF;
EXCEPTION
    WHEN zmiana_na_minus THEN
    RAISE_APPLICATION_ERROR(-20002, 'Zmniejszenie przydziału myszy jest zabronione!');
END;
/

CREATE OR REPLACE TRIGGER Po_Zmianie
AFTER UPDATE OF przydzial_myszy ON kocury
BEGIN
    IF NOT memory.zmiana_wyzwalacza THEN
        memory.zmiana_wyzwalacza := TRUE;

        WHILE memory.dobre_zmiany > 0
        LOOP
            UPDATE Kocury SET myszy_extra = myszy_extra + 5
            WHERE pseudo = 'TYGRYS';
            memory.dobre_zmiany := memory.dobre_zmiany - 1;
        END LOOP;

        WHILE memory.zle_zmiany > 0
        LOOP
            UPDATE Kocury SET 
                przydzial_myszy = przydzial_myszy + FLOOR(0.1 * memory.przydzial_tygrysa),
                myszy_extra = myszy_extra + 5
            WHERE funkcja = 'MILUSIA';

            UPDATE Kocury SET przydzial_myszy = GREATEST(0, przydzial_myszy - FLOOR(0.1 * memory.przydzial_tygrysa))
            WHERE pseudo = 'TYGRYS';
            memory.zle_zmiany := memory.zle_zmiany - 1;
        END LOOP;

        memory.zmiana_wyzwalacza := FALSE;
    END IF;
END;
/

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

CREATE OR REPLACE PACKAGE anti_recursion AS
    flaga BOOLEAN := FALSE;
END;
/

CREATE OR REPLACE TRIGGER milusie_trg_compound
FOR UPDATE OF przydzial_myszy ON KOCURY
COMPOUND TRIGGER
    dobre_zmiany NUMBER := 0; 
    zle_zmiany NUMBER := 0;
    przydzial_tygrysa NUMBER := 0;

BEFORE STATEMENT IS
BEGIN
    IF NOT anti_recursion.flaga THEN
        SELECT przydzial_myszy INTO przydzial_tygrysa 
        FROM kocury WHERE pseudo = 'TYGRYS';
    END IF;
END BEFORE STATEMENT;

BEFORE EACH ROW IS
DECLARE
    v_roznica NUMBER;
    zmiana_na_minus EXCEPTION;
BEGIN
    IF NOT anti_recursion.flaga AND :NEW.pseudo != 'TYGRYS' THEN
        v_roznica := :NEW.przydzial_myszy - :OLD.przydzial_myszy;
    
        IF v_roznica < 0 THEN
            RAISE zmiana_na_minus;
    	ELSIF v_roznica > 0 AND v_roznica < 0.1 * przydzial_tygrysa THEN
            zle_zmiany := zle_zmiany + 1;
        ELSIF v_roznica > 0 THEN
            dobre_zmiany := dobre_zmiany + 1;
    	END IF;
	END IF;
EXCEPTION
    WHEN zmiana_na_minus THEN
        RAISE_APPLICATION_ERROR(-20002, 'Zmniejszenie przydziału myszy jest zabronione!');
END BEFORE EACH ROW;

AFTER STATEMENT IS
BEGIN
IF NOT anti_recursion.flaga THEN
    anti_recursion.flaga := TRUE;

    WHILE dobre_zmiany > 0
    LOOP
        UPDATE Kocury SET myszy_extra = myszy_extra + 5
        WHERE pseudo = 'TYGRYS';
        dobre_zmiany := dobre_zmiany - 1;
    END LOOP;

    WHILE zle_zmiany > 0
    LOOP
        UPDATE Kocury SET 
            przydzial_myszy = przydzial_myszy + FLOOR((0.1 * przydzial_tygrysa)),
            myszy_extra = myszy_extra + 5
        WHERE funkcja = 'MILUSIA';

        UPDATE Kocury SET przydzial_myszy = GREATEST(0, przydzial_myszy - FLOOR((0.1 * przydzial_tygrysa)))
        WHERE pseudo = 'TYGRYS';
        zle_zmiany := zle_zmiany - 1;
    END LOOP;

    anti_recursion.flaga := FALSE;
END IF;
END AFTER STATEMENT;

END milusie_trg_compound;
/

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

SELECT pseudo, PRZYDZIAL_MYSZY, MYSZY_EXTRA FROM kocury WHERE funkcja = 'MILUSIA' OR pseudo = 'TYGRYS' OR pseudo = 'PLACEK';
BEGIN
    UPDATE kocury SET przydzial_myszy = przydzial_myszy + 15 WHERE pseudo = 'PLACEK';
END;
/
ROLLBACK;

ALTER TRIGGER przydzial_trygrysa_init DISABLE;
ALTER TRIGGER Po_Zmianie DISABLE;
ALTER TRIGGER Przed_Zmiana DISABLE;

ALTER TRIGGER milusie_trg_compound DISABLE;


ALTER TRIGGER przydzial_trygrysa_init ENABLE;
ALTER TRIGGER Po_Zmianie ENABLE;
ALTER TRIGGER Przed_Zmiana ENABLE;

ALTER TRIGGER milusie_trg_compound ENABLE;


/* Zad. 43. Napisać blok, który zrealizuje zad. 33 w sposób uniwersalny (bez konieczności
uwzględniania wiedzy o funkcjach pełnionych przez koty).

Zad. 33. Napisać zapytanie, w ramach którego obliczone zostaną sumy całkowitego spożycia
myszy przez koty sprawujące każdą z funkcji z podziałem na bandy i płcie kotów.
Podsumować przydziały dla każdej z funkcji. Zadanie wykonać na dwa sposoby:
a. z wykorzystaniem funkcji DECODE i SUM (ew. CASE i SUM),
*/
DECLARE
    CURSOR funkcje IS (SELECT funkcja FROM funkcje);
    CURSOR funkcje is (SELECT funkcja from FUNKCJE);
	CURSOR plcie is (SELECT plec FROM kocury GROUP BY plec);
	CURSOR bandy is (SELECT nazwa, nr_bandy FROM bandy);
    ilosc number;    
BEGIN
    DBMS_OUTPUT.PUT(RPAD('NAZWA BANDY', 20) || ' ' || RPAD('PLEC', 10) || ' ' || RPAD('ILE', 5));
    FOR fun IN funkcje LOOP
      DBMS_OUTPUT.PUT(' ' || RPAD(fun.funkcja, 10));
    END LOOP;

    DBMS_OUTPUT.PUT_LINE(LPAD('SUMA', 5));

    DBMS_OUTPUT.PUT('-------------------- ---------- -----');

    FOR fun IN funkcje LOOP
          DBMS_OUTPUT.PUT(' ----------');
    END LOOP;
    DBMS_OUTPUT.PUT_LINE(' -----');

    FOR banda IN bandy LOOP
        FOR ple IN plcie LOOP
            DBMS_OUTPUT.PUT(CASE WHEN ple.plec = 'M' then RPAD(banda.nazwa, 20) else  RPAD(' ', 20) END);
			DBMS_OUTPUT.PUT(' ');
            DBMS_OUTPUT.PUT(CASE WHEN ple.plec = 'M' then RPAD('Kocor', 10) else RPAD('Kotka', 10) END);
			DBMS_OUTPUT.PUT(' ');

            SELECT COUNT(*) INTO ilosc FROM KOCURY where KOCURY.NR_BANDY = banda.NR_BANDY AND KOCURY.PLEC=ple.plec;
            DBMS_OUTPUT.PUT(LPAD(ilosc, 5));

            FOR fun IN funkcje LOOP
                SELECT SUM(PRZYDZIAL_MYSZY + NVL(MYSZY_EXTRA, 0)) INTO ilosc FROM KOCURY K
                WHERE K.PLEC=ple.plec AND K.FUNKCJA=fun.FUNKCJA AND K.NR_BANDY=banda.NR_BANDY;
                DBMS_OUTPUT.PUT(' ' || LPAD(NVL(ilosc, 0) ,10));
            END LOOP;
			DBMS_OUTPUT.PUT(' ');

            SELECT SUM(PRZYDZIAL_MYSZY + NVL(MYSZY_extra, 0)) INTO ilosc FROM KOCURY K where K.NR_BANDY=banda.NR_BANDY AND ple.PLEC=K.PLEC;
            DBMS_OUTPUT.PUT(LPAD(NVL(ilosc, 0) , 5));
            DBMS_OUTPUT.NEW_LINE();
        END LOOP;
    END LOOP;

    DBMS_OUTPUT.PUT('-------------------- ---------- -----');

    FOR fun IN funkcje LOOP
          DBMS_OUTPUT.PUT(' ----------');
    END LOOP;
    DBMS_OUTPUT.PUT_LINE(' -----');


    DBMS_OUTPUT.PUT(RPAD('Zjada razem', 20));
	DBMS_OUTPUT.PUT(' ');
	DBMS_OUTPUT.PUT(RPAD(' ', 16));
        
    FOR fun IN funkcje LOOP
        SELECT SUM(NVL(PRZYDZIAL_MYSZY,0)+NVL(MYSZY_extra,0)) INTO ilosc FROM Kocury K where K.FUNKCJA=fun.FUNKCJA;
        DBMS_OUTPUT.PUT(' ' || LPAD(NVL(ilosc, 0), 10));
    END LOOP;

	DBMS_OUTPUT.PUT(' ');
    SELECT SUM(PRZYDZIAL_MYSZY + NVL(MYSZY_extra,0)) INTO ilosc FROM Kocury;
    DBMS_OUTPUT.PUT(LPAD(ilosc, 5));
    DBMS_OUTPUT.NEW_LINE();
END;
/

/* Zad. 43. Napisać blok, który zrealizuje zad. 33 w sposób uniwersalny (bez konieczności
uwzględniania wiedzy o funkcjach pełnionych przez koty). */

-- dynamiczny SQL

DECLARE
    CURSOR funkcje IS (SELECT funkcja FROM funkcje);
    v_sql VARCHAR2(9999);
BEGIN
    v_sql := q'[
SELECT *
FROM (
    SELECT 
        TO_CHAR(DECODE(plec, 'D', nazwa, ' ')) AS "NAZWA BANDY",
        TO_CHAR(DECODE(plec, 'D', 'Kotka', 'Kocor')) AS "PLEC",
        TO_CHAR(COUNT(pseudo)) AS "ILE",]' || CHR(10);
	FOR rec in funkcje LOOP
		v_sql := v_sql || CHR(9) || CHR(9) || 'TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k WHERE funkcja = '
        	|| '''' || rec.funkcja || '''' || ' AND k.nr_bandy= kocury.nr_bandy AND k.plec = kocury.plec),0)) AS '
        	|| '"' || rec.funkcja || '", ' || CHR(10);
    END LOOP;
	v_sql := v_sql || CHR(9) || CHR(9) || 'TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0))'
        || 'FROM kocury k WHERE k.nr_bandy= kocury.nr_bandy AND k.plec = kocury.plec),0)) AS "SUMA"'
        || CHR(10);
	v_sql := v_sql || CHR(9) || q'[FROM kocury INNER JOIN bandy ON kocury.nr_bandy = bandy.nr_bandy
    GROUP BY nazwa, plec, kocury.nr_bandy
    ORDER BY nazwa
)

UNION ALL

SELECT '--------------', '------', '--------', '---------', '---------', '--------', '--------', '--------', '--------', '--------', '--------', '--------'
FROM dual

UNION ALL

SELECT DISTINCT 
    'ZJADA RAZEM',
    ' ',
    ' ',]' || CHR(10) || CHR(9);
	FOR rec IN funkcje LOOP
        v_sql := v_sql || 'TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k WHERE funkcja = '
        || '''' || rec.funkcja || '''' || '), 0)) AS ' || '"' || rec.funkcja || '",' || CHR(10) || CHR(9);
	END LOOP;	
	v_sql := v_sql || q'[TO_CHAR(NVL((SELECT SUM(przydzial_myszy + NVL(myszy_extra, 0)) FROM kocury k), 0)) AS "SUMA"
FROM kocury INNER JOIN bandy ON kocury.nr_bandy= bandy.nr_bandy;]';
	
    DBMS_OUTPUT.PUT_LINE(v_sql);
	-- EXECUTE IMMEDIATE v_sql;
END;
/


/* Zad. 44. Tygrysa zaniepokoiło niewytłumaczalne obniżenie zapasów "myszowych".
Postanowił więc wprowadzić podatek pogłówny, który zasiliłby spiżarnię. Zarządził więc, że
każdy kot ma obowiązek oddawać 5% (zaokrąglonych w górę) swoich całkowitych
"myszowych" przychodów. Dodatkowo od tego co pozostanie:
- koty nie posiadające podwładnych oddają po dwie myszy za nieudolność w
umizgach o awans,
- koty nie posiadające wrogów oddają po jednej myszy za zbytnią ugodowość,
- koty płacą dodatkowy podatek, którego formę określa wykonawca zadania.
Napisać funkcję, której parametrem jest pseudonim kota, wyznaczającą należny podatek
pogłówny kota. Funkcję tą razem z procedurą z zad. 40 należy umieścić w pakiecie, a
następnie wykorzystać ją do określenia podatku dla wszystkich kotów. */
CREATE OR REPLACE FUNCTION Podatek (p_pseudo VARCHAR2)
    RETURN NUMBER
AS
	v_pseudo VARCHAR2(15) := UPPER(p_pseudo);
    v_przydzial NUMBER;
	v_podstawowy NUMBER;
	v_podwladni NUMBER;
  	v_wrogowie NUMBER;
	v_staz NUMBER;
BEGIN
    SELECT (przydzial_myszy + NVL(myszy_extra, 0)) INTO v_przydzial FROM kocury WHERE pseudo = v_pseudo;
    SELECT COUNT(*) INTO v_podwladni FROM kocury WHERE szef = v_pseudo;
    SELECT COUNT(*) INTO v_wrogowie FROM wrogowie_kocurow WHERE pseudo = v_pseudo;
	SELECT FLOOR(MONTHS_BETWEEN(SYSDATE, w_stadku_od) / 12) INTO v_staz FROM kocury WHERE pseudo = v_pseudo;

	v_podstawowy := CEIL(0.05 * v_przydzial);

    IF v_podwladni > 0 THEN
      v_podwladni := 0;
    ELSE
      v_podwladni := 2;
    END IF;

    IF v_wrogowie > 0 THEN
      v_wrogowie := 0;
    ELSE
      v_wrogowie := 1;
    END IF;

	IF v_staz >= 15 THEN
        v_staz := 0;
	ELSE 
        v_staz := 3;
	END IF;
    
    RETURN LEAST(v_podstawowy + v_podwladni + v_wrogowie + v_staz, v_przydzial);
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        RETURN 0; 
END Podatek;
/

SELECT Podatek('zero') FROM DUAL;

CREATE OR REPLACE PACKAGE koty_pkg AS
    FUNCTION Podatek(p_pseudo VARCHAR2) RETURN NUMBER;
    PROCEDURE Dodaj_bande(p_nr_bandy NUMBER, p_nazwa_bandy VARCHAR2, p_teren_bandy VARCHAR2);
END koty_pkg;
/

CREATE OR REPLACE PACKAGE BODY koty_pkg AS
	FUNCTION Podatek (p_pseudo VARCHAR2)
        RETURN NUMBER
    IS
    	v_pseudo VARCHAR2(15) := UPPER(p_pseudo);
        v_przydzial NUMBER;
    	v_podstawowy NUMBER;
    	v_podwladni NUMBER;
      	v_wrogowie NUMBER;
    	v_staz NUMBER;
    BEGIN
        SELECT (przydzial_myszy + NVL(myszy_extra, 0)) INTO v_przydzial FROM kocury WHERE pseudo = v_pseudo;
        SELECT COUNT(*) INTO v_podwladni FROM kocury WHERE szef = v_pseudo;
        SELECT COUNT(*) INTO v_wrogowie FROM wrogowie_kocurow WHERE pseudo = v_pseudo;
    	SELECT FLOOR(MONTHS_BETWEEN(SYSDATE, w_stadku_od) / 12) INTO v_staz FROM kocury WHERE pseudo = v_pseudo;
    
    	v_podstawowy := CEIL(0.05 * v_przydzial);
    
        IF v_podwladni > 0 THEN
          v_podwladni := 0;
        ELSE
          v_podwladni := 2;
        END IF;
    
        IF v_wrogowie > 0 THEN
          v_wrogowie := 0;
        ELSE
          v_wrogowie := 1;
        END IF;
    
    	IF v_staz >= 15 THEN
            v_staz := 0;
    	ELSE 
            v_staz := 3;
    	END IF;
        
        RETURN LEAST(v_podstawowy + v_podwladni + v_wrogowie + v_staz, v_przydzial);
	EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RETURN 0; 
    END podatek;

	PROCEDURE Dodaj_bande(
        p_nr_bandy NUMBER,
        p_nazwa_bandy VARCHAR2,
        p_teren_bandy VARCHAR2
    ) IS
        v_nr_bandy NUMBER(2) := p_nr_bandy;
        v_nazwa VARCHAR2(20) := UPPER(p_nazwa_bandy);
        v_teren VARCHAR2(15) := UPPER(p_teren_bandy);
    
        v_is_wrong NUMBER(1) := 0;
        v_is_error BOOLEAN := false;
        v_error_message VARCHAR2(50) := '';
    
        invalid_nr EXCEPTION;
        parameter_exists EXCEPTION;
    BEGIN
        IF v_nr_bandy <= 0 THEN 
            RAISE invalid_nr;
        END IF;
    
        SELECT COUNT(*) INTO v_is_wrong FROM Bandy WHERE nr_bandy = v_nr_bandy;
        IF v_is_wrong > 0 THEN
            v_error_message := v_error_message || v_nr_bandy;
            v_is_error := true;
        END IF;
    
        SELECT COUNT(*) INTO v_is_wrong FROM Bandy WHERE nazwa = v_nazwa;
        IF v_is_wrong > 0 THEN
            IF v_is_error THEN
                v_error_message := v_error_message || ', ';
            END IF;
            v_error_message := v_error_message || v_nazwa;        
            v_is_error := true;
        END IF;
    
        SELECT COUNT(*) INTO v_is_wrong FROM Bandy WHERE teren = v_teren;
        IF v_is_wrong > 0 THEN
            IF v_is_error THEN
                v_error_message := v_error_message || ', ';
            END IF;
            v_error_message := v_error_message || v_teren;
            v_is_error := true;
        END IF;
    
        IF v_is_error THEN RAISE parameter_exists;
        END IF;
    
        INSERT INTO Bandy(nr_bandy, nazwa, teren)
        VALUES (v_nr_bandy, v_nazwa, v_teren);
    
        DBMS_OUTPUT.PUT_LINE('Banda (' || v_nr_bandy || ', ' || v_nazwa || ', ' || v_teren || ') dodana.');
    
    EXCEPTION
        WHEN invalid_nr THEN DBMS_OUTPUT.PUT_LINE('Numer bandy musi być liczbą dodatnią.');
        WHEN parameter_exists THEN DBMS_OUTPUT.PUT_LINE(v_error_message || ': juz istnieje');
    END Dodaj_bande;
END koty_pkg;
/

SELECT pseudo, koty_pkg.podatek(pseudo) FROM kocury;


/* Zad. 45. Tygrys zauważył dziwne zmiany wartości swojego prywatnego przydziału myszy
(patrz zadanie 42). Nie niepokoiły go zmiany na plus ale te na minus były, jego zdaniem,
niedopuszczalne. Zmotywował więc jednego ze swoich szpiegów do działania i dzięki temu
odkrył niecne praktyki Miluś (zadanie 42). Polecił więc swojemu informatykowi
skonstruowanie mechanizmu zapisującego w relacji Dodatki_extra (patrz Wykłady - cz.
2) dla każdej z Miluś -10 (minus dziesięć) myszy dodatku extra przy zmianie na plus
któregokolwiek z przydziałów myszy Miluś, wykonanej przez innego operatora niż on sam
(Tygrys). Zaproponować taki mechanizm, w zastępstwie za informatyka Tygrysa. W
rozwiązaniu wykorzystać funkcję LOGIN_USER zwracającą nazwę użytkownika
aktywującego wyzwalacz oraz elementy dynamicznego SQL'a. */
DROP TABLE Dodatki_extra;
CREATE TABLE Dodatki_extra(
    pseudo VARCHAR2(15) CONSTRAINT dodatki_pseudo_fk REFERENCES kocury(pseudo),
    dod_extra NUMBER(3) DEFAULT 0    
);


CREATE OR REPLACE TRIGGER zemsta_trg
    BEFORE UPDATE OF przydzial_myszy
    ON kocury
    FOR EACH ROW
DECLARE
    CURSOR milusie IS SELECT pseudo
                FROM kocury
                WHERE funkcja = 'MILUSIA';
    v_ile NUMBER;
    v_sql VARCHAR2(1000);
BEGIN
    IF LOGIN_USER <> 'TYGRYS' AND :NEW.PRZYDZIAL_MYSZY > :OLD.PRZYDZIAL_MYSZY AND :NEW.FUNKCJA = 'MILUSIA' THEN
    FOR milusia IN milusie
        LOOP
            SELECT COUNT(*) INTO v_ile FROM dodatki_extra WHERE pseudo = milusia.pseudo;
            IF v_ile > 0 THEN
                v_sql:='UPDATE dodatki_extra SET dod_extra = dod_extra - 10 WHERE :mil_ps = pseudo';
            ELSE 
                v_sql:='INSERT INTO dodatki_extra (pseudo, dod_extra) VALUES (:mil_ps, -10)';
            END IF;
            EXECUTE IMMEDIATE v_sql USING milusia.pseudo;
        END LOOP;
    END IF;
END;
/

SELECT * FROM dodatki_extra;
SELECT * FROM kocury WHERE pseudo = 'MALA';
BEGIN
	UPDATE kocury SET przydzial_myszy = przydzial_myszy + 5 WHERE pseudo = 'MALA';
END;
/
SELECT * FROM kocury WHERE pseudo = 'MALA';
SELECT * FROM dodatki_extra;
ROLLBACK;


/* Zad. 46. Napisać wyzwalacz, który uniemożliwi wpisanie kotu przydziału myszy spoza
przedziału (min_myszy, max_myszy) określonego dla każdej funkcji w relacji Funkcje.
Każda próba wykroczenia poza obowiązujący przedział ma być dodatkowo monitorowana w
osobnej relacji (kto, kiedy, jakiemu kotu, jaką operacją). */
CREATE TABLE Logi (
    kto VARCHAR2(30),
    kiedy DATE,
    pseudonim_kota VARCHAR2(15),
    operacja VARCHAR2(30)
);

CREATE OR REPLACE TRIGGER Sprawdz_przydzial
BEFORE INSERT OR UPDATE ON Kocury
FOR EACH ROW
DECLARE
	PRAGMA AUTONOMOUS_TRANSACTION;
    v_min_myszy NUMBER;
    v_max_myszy NUMBER;
	v_operacja VARCHAR2(10);
BEGIN
    SELECT min_myszy, max_myszy 
    INTO v_min_myszy, v_max_myszy
    FROM funkcje
    WHERE funkcja = :NEW.funkcja;

    IF :NEW.przydzial_myszy < v_min_myszy OR :NEW.przydzial_myszy > v_max_myszy THEN

        IF INSERTING THEN
            v_operacja := 'INSERT';
        ELSIF UPDATING THEN
            v_operacja := 'UPDATE';
        END IF;

        INSERT INTO Logi (kto, kiedy, pseudonim_kota, operacja)
        VALUES (SYS_CONTEXT('USERENV', 'IP_ADDRESS'), SYSDATE, :NEW.pseudo, v_operacja);

		COMMIT;

        RAISE_APPLICATION_ERROR(-20001, 
            'Przydział myszy ' || :NEW.przydzial_myszy || 
            ' dla funkcji ' || :NEW.funkcja || 
            ' poza dozwolonym przedziałem (' || v_min_myszy || ', ' || v_max_myszy || ').');
    END IF;
END;
/