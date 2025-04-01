BEGIN
    EXECUTE IMMEDIATE 'CREATE TABLE Myszy (
        nr_myszy NUMBER,
        lowca VARCHAR2(15),
        zjadacz VARCHAR2(15),
        waga_myszy NUMBER(2),
        data_zlowienia DATE,
        data_wydania DATE
    )';

    -- Dodanie klucza głównego
    EXECUTE IMMEDIATE 'ALTER TABLE Myszy 
        ADD CONSTRAINT myszy_pk PRIMARY KEY (nr_myszy)';

    -- Dodanie kluczy obcych
    EXECUTE IMMEDIATE 'ALTER TABLE Myszy 
        ADD CONSTRAINT myszy_lowca_fk FOREIGN KEY (lowca) REFERENCES Kocury(pseudo)';
    EXECUTE IMMEDIATE 'ALTER TABLE Myszy 
        ADD CONSTRAINT myszy_zjadacz_fk FOREIGN KEY (zjadacz) REFERENCES Kocury(pseudo)';

    -- Dodanie ograniczenia CHECK
    EXECUTE IMMEDIATE 'ALTER TABLE Myszy 
        ADD CONSTRAINT myszy_waga_ch CHECK (waga_myszy BETWEEN 10 AND 50)';
END;
/

SELECT COUNT(*) FROM Myszy;

DECLARE
    start_date DATE := TO_DATE('01-01-2004', 'DD-MM-YYYY'); -- Początek zakresu
    end_date DATE := TO_DATE('31-12-2024', 'DD-MM-YYYY');   -- Koniec zakresu
    current_date DATE := start_date;                       -- Bieżący miesiąc
    suma_myszy NUMBER := 0;
    liczba_kotow NUMBER := 0;
    srednia_myszy NUMBER;
    ostatni_dzien DATE;
    dodatkowe_dla_szefa NUMBER;
    nr_myszy NUMBER := 0;
    dni_w_miesiacu NUMBER;
    ile_myszy_dodano NUMBER := 0;
    update_index NUMBER := 0;
    przydzial_kota NUMBER;
    dzien_ostatniej_srody NUMBER;

    -- Definicje typów kolekcji
    TYPE Myszy_Table_Type IS TABLE OF Myszy%ROWTYPE;
    myszy_collection Myszy_Table_Type := Myszy_Table_Type();

    -- Funkcja pomocnicza do obliczenia ostatniej środy miesiąca
    FUNCTION Last_Wednesday(p_date DATE) RETURN DATE IS
    BEGIN
        RETURN NEXT_DAY(LAST_DAY(p_date) - 7, 'WEDNESDAY');
    END;
BEGIN
    EXECUTE IMMEDIATE 'TRUNCATE TABLE Myszy';

    WHILE current_date <= end_date LOOP

        ostatni_dzien := LAST_DAY(ADD_MONTHS(current_date, -1));
        dni_w_miesiacu := TO_NUMBER(TO_CHAR(LAST_DAY(current_date), 'DD'));
        dzien_ostatniej_srody := TO_NUMBER(TO_CHAR(Last_Wednesday(current_date), 'DD'));

        /* Obliczenie sumy myszy do upolowania, liczby kotow w stadku w danym miesiacu i sredniej */
        FOR rec IN (
            SELECT 
                pseudo, 
                przydzial_myszy, 
                myszy_extra 
            FROM 
                kocury
            WHERE 
                w_stadku_od <= ostatni_dzien -- Wcześniej od danego miesiaca
        ) LOOP
            suma_myszy := suma_myszy + NVL(rec.przydzial_myszy, 0) + NVL(rec.myszy_extra, 0);
            liczba_kotow := liczba_kotow + 1;
        END LOOP;

        srednia_myszy := CEIL(suma_myszy / liczba_kotow);

        dodatkowe_dla_szefa := srednia_myszy * liczba_kotow - suma_myszy;

        /* Utworzenie rekordow myszy do wstawienia */
        FOR rec IN (
            SELECT pseudo FROM kocury WHERE w_stadku_od <= ostatni_dzien
        ) LOOP
            FOR i IN 1..srednia_myszy LOOP
                nr_myszy := nr_myszy + 1;
                ile_myszy_dodano := ile_myszy_dodano + 1;

                -- Tworzenie rekordu myszy
                DECLARE
                    l_myszy Myszy%ROWTYPE;
                BEGIN
                    l_myszy.nr_myszy := nr_myszy;
                    l_myszy.lowca := rec.pseudo;
                    l_myszy.zjadacz := NULL;
                    l_myszy.waga_myszy := FLOOR(DBMS_RANDOM.VALUE(10, 51));
                    l_myszy.data_zlowienia := current_date + FLOOR(DBMS_RANDOM.VALUE(0, dzien_ostatniej_srody));
                    l_myszy.data_wydania := current_date + dzien_ostatniej_srody - 1;

                    -- Dodanie myszy do kolekcji
                    myszy_collection.EXTEND;
                    myszy_collection(myszy_collection.LAST) := l_myszy;
                END;
            END LOOP;
        END LOOP;

        /* Przydzial myszy dla kocurów */
        FOR rec IN (
            SELECT pseudo, przydzial_myszy, myszy_extra FROM kocury WHERE w_stadku_od <= ostatni_dzien
        ) LOOP
            przydzial_kota := NVL(rec.przydzial_myszy, 0) + NVL(rec.myszy_extra, 0);
            IF rec.pseudo = 'TYGRYS' THEN
                przydzial_kota := przydzial_kota + dodatkowe_dla_szefa;
            END IF;
            FOR i IN 1..przydzial_kota LOOP
                update_index := update_index + 1;
                myszy_collection(update_index).zjadacz := rec.pseudo;
            END LOOP;
        END LOOP;

        -- Przesunięcie do następnego miesiąca
        ile_myszy_dodano := 0;
        suma_myszy := 0;
        liczba_kotow := 0;
        current_date := ADD_MONTHS(current_date, 1);
    END LOOP;
    
    FORALL idx IN myszy_collection.FIRST .. myszy_collection.LAST
        INSERT INTO Myszy VALUES myszy_collection(idx);
END;
/

SELECT * FROM Myszy ORDER BY nr_myszy;


CREATE OR REPLACE PROCEDURE DodajMyszy(
    p_lowca VARCHAR2,
    p_data_zlowienia DATE
) AS 
    v_nr_myszy NUMBER;

    TYPE Myszy_Table_Type IS TABLE OF Myszy%ROWTYPE;
    myszy_collection Myszy_Table_Type := Myszy_Table_Type();
BEGIN
    SELECT MAX(nr_myszy) INTO v_nr_myszy FROM Myszy;

    /* zakladam istenienie relacji zlowione_myszy_<pseudo kota>(data_zlowienia, waga_myszy) */
    /* dla kazdej myszki zlowionej przez tego kocura danego dnia */
    FOR rec IN (
        EXECUTE IMMEDIATE 'SELECT waga_myszy FROM zlowione_myszy_' || p_lowca || ' WHERE data_zlowienia = p_data_zlowienia'
    ) LOOP
        v_nr_myszy := v_nr_myszy + 1;

        /* dodaje wpis do kolekcji */
        DECLARE
            l_myszy Myszy%ROWTYPE;
        BEGIN
            l_myszy.nr_myszy := nr_myszy;
            l_myszy.lowca := p_lowca;
            l_myszy.zjadacz := NULL;
            l_myszy.waga_myszy := rec.waga_myszy;
            l_myszy.data_zlowienia := p_data_zlowienia;
            l_myszy.data_wydania := NULL;

            myszy_collection.EXTEND;
            myszy_collection(myszy_collection.LAST) := l_myszy;
        END;
    END LOOP;

    /* wstawienie myszy do tabeli */
    FORALL idx IN myszy_collection.FIRST .. myszy_collection.LAST
        INSERT INTO Myszy VALUES myszy_collection(idx);
END;
/



CREATE OR REPLACE PROCEDURE WydajMyszy(
    p_data_wydania DATE
) AS 
    myszy_do_wydania NUMBER;
    dostepne_myszy NUMBER;
    numer_myszy NUMBER;
    myszy_juz_wydane NUMBER := 0;

    CURSOR myszki IS
        SELECT nr_myszy FROM Myszy WHERE zjadacz IS NULL;
    
    TYPE Myszy_Table_Type IS TABLE OF Myszy%ROWTYPE;
    myszy_collection Myszy_Table_Type := Myszy_Table_Type();
BEGIN
    IF NEXT_DAY(LAST_DAY(p_data_wydania) - 7, 'WEDNESDAY') != TRUNC(p_data_wydania) THEN
        RAISE_APPLICATION_ERROR(-20001, 'Wydawanie myszek jest możliwe tylko w środy');
    END IF;

    SELECT COUNT(*) INTO myszy_juz_wydane FROM Myszy WHERE data_wydania = p_data_wydania;
    IF myszy_juz_wydane > 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Myszy zostały już wydane w tym miesiacu');
    END IF;

    SELECT COUNT(*) INTO dostepne_myszy FROM Myszy WHERE zjadacz IS NULL;

    OPEN myszki;

    /* Po kolei dla wszystkich kocurów od szefa w dół hierarchii */
    FOR rec IN (
        SELECT 
            PSEUDO,
            PRZYDZIAL_MYSZY,
            MYSZY_EXTRA
        FROM 
            KOCURY
        START WITH 
            SZEF IS NULL
        CONNECT BY 
            PRIOR PSEUDO = SZEF
        ORDER BY LEVEL
    ) LOOP
        myszy_do_wydania := NVL(rec.PRZYDZIAL_MYSZY, 0) + NVL(rec.MYSZY_EXTRA, 0);

        /* przydzielamy poszczególne myszki */
        WHILE myszy_do_wydania > 0 AND dostepne_myszy > 0 LOOP
            FETCH myszki INTO numer_myszy;
            EXIT WHEN myszki%NOTFOUND;

            /* zapisujemy myszki do kolekcji */
            DECLARE
                l_myszy Myszy%ROWTYPE;
            BEGIN
                l_myszy.nr_myszy := numer_myszy;
                l_myszy.lowca := NULL;
                l_myszy.zjadacz := rec.pseudo;
                l_myszy.waga_myszy := NULL;
                l_myszy.data_zlowienia := NULL;
                l_myszy.data_wydania := NULL;

                myszy_collection.EXTEND;
                myszy_collection(myszy_collection.LAST) := l_myszy;
            END;

            myszy_do_wydania := myszy_do_wydania - 1;
            dostepne_myszy := dostepne_myszy - 1;
        END LOOP;  
    END LOOP;

    CLOSE myszki;

    /* faktyczne przydzielenie myszek, czyli aktualizacja tabeli Myszy */
    FORALL idx IN myszy_collection.FIRST .. myszy_collection.LAST
        UPDATE Myszy SET zjadacz = myszy_collection(idx).zjadacz, data_wydania = p_data_wydania WHERE nr_myszy = myszy_collection(idx).nr_myszy;
END;
/