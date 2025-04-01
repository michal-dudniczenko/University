-- wstawienie prawid³owych danych do tabel

-- próba wstawienia numeru <= 0
INSERT INTO Oddzial (nr, nazwa, liczbaLozek, idSzpitala) VALUES
(-1, 'Kardiologia', 50, 1);

-- próba wstawienia oddzia³u o parze (nr, idSzpitala), która ju¿ istnieje w bazie
INSERT INTO Oddzial (nr, nazwa, liczbaLozek, idSzpitala) VALUES
(1, 'Kardiologia', 50, 1);

-- próba wstawienia oddzia³u o liczbie ³ó¿ek <= 0
INSERT INTO Oddzial (nr, nazwa, liczbaLozek, idSzpitala) VALUES
(1, 'Kardiologia', 0, 10);

-- próba wstawienia -powtarza siê PESEL
INSERT INTO Lekarz (PESEL, plec, imie, nazwisko, dataUrodzenia, nrTel, email, kraj, kodPocztowy, miasto, ulica, nrDomu, nrLokalu, pwz, tytulNaukowy, specjalizacja) VALUES
('90010112345', 'm', 'Jan', 'Kowalski', '1990-01-01', '600123456', 'jan.kowalski@example.com', 'Polska', '00-001', 'Warszawa', 'Marsza³kowska', '10', NULL, '1237456', 'Dr n. med.', 'Kardiolog');

-- próba wstawienia -powtarza siê pwz
INSERT INTO Lekarz (PESEL, plec, imie, nazwisko, dataUrodzenia, nrTel, email, kraj, kodPocztowy, miasto, ulica, nrDomu, nrLokalu, pwz, tytulNaukowy, specjalizacja) VALUES
('90010912645', 'm', 'Jan', 'Kowalski', '1990-01-01', '600123456', 'jan.kowalski@example.com', 'Polska', '00-001', 'Warszawa', 'Marsza³kowska', '10', NULL, '123456', 'Dr n. med.', 'Kardiolog');

-- próba wstawienia PESEL w z³ym formacie
INSERT INTO Lekarz (PESEL, plec, imie, nazwisko, dataUrodzenia, nrTel, email, kraj, kodPocztowy, miasto, ulica, nrDomu, nrLokalu, pwz, tytulNaukowy, specjalizacja) VALUES
('123456789', 'm', 'Jan', 'Kowalski', '1990-01-01', '600123456', 'jan.kowalski@example.com', 'Polska', '00-001', 'Warszawa', 'Marsza³kowska', '10', NULL, '9237456', 'Dr n. med.', 'Kardiolog');

-- próba wstawienia p³eæ w z³ym formacie
INSERT INTO Lekarz (PESEL, plec, imie, nazwisko, dataUrodzenia, nrTel, email, kraj, kodPocztowy, miasto, ulica, nrDomu, nrLokalu, pwz, tytulNaukowy, specjalizacja) VALUES
('12345678900', 'x', 'Jan', 'Kowalski', '1990-01-01', '600123456', 'jan.kowalski@example.com', 'Polska', '00-001', 'Warszawa', 'Marsza³kowska', '10', NULL, '9237456', 'Dr n. med.', 'Kardiolog');

-- próba wstawienia - kod choroby siê powtarza
INSERT INTO Choroba (kodChoroby, nazwa, czyZakazna) VALUES
('C001', 'Grypa', 1);

-- próba dodania - data do wczeœniejsza ni¿ data od
INSERT INTO Ordynator (od, do, idOddzialu, idLekarza) VALUES
('2020-01-01', '2019-09-08', 1, 1);

-- próba wstawienia drugi raz tej samej choroby w ramach tej samej wizyty
INSERT INTO Diagnoza (idWizyty, idChoroby) VALUES
(1, 1);