-- wstawienie prawid³owych danych do tabel

INSERT INTO Szpital (nazwa, kraj, kodPocztowy, miasto, ulica, nrDomu, nrLokalu) VALUES
('Szpital Centralny', 'Polska', '00-001', 'Warszawa', 'Marsza³kowska', '10', '2'),
('Szpital Miejski', 'Polska', '30-002', 'Kraków', 'D³uga', '20', NULL),
('Szpital Wojewódzki', 'Polska', '50-003', 'Wroc³aw', 'Œwidnicka', '5', 'B');

INSERT INTO Oddzial (nr, nazwa, liczbaLozek, idSzpitala)VALUES
(1, 'Kardiologia', 50, 1),
(2, 'Neurologia', 40, 1),
(3, 'Ortopedia', 35, 1),
(4, 'Chirurgia', 45, 1),
(5, 'Onkologia', 30, 1),
(6, 'Pediatria', 60, 1),
(7, 'Geriatria', 25, 1),
(1, 'Urologia', 20, 2),
(2, 'Kardiologia', 40, 2),
(1, 'Pulmonologia', 30, 3);

INSERT INTO Lekarz (PESEL, plec, imie, nazwisko, dataUrodzenia, nrTel, email, kraj, kodPocztowy, miasto, ulica, nrDomu, nrLokalu, pwz, tytulNaukowy, specjalizacja) VALUES
('90010112345', 'm', 'Jan', 'Kowalski', '1990-01-01', '600123456', 'jan.kowalski@example.com', 'Polska', '00-001', 'Warszawa', 'Marsza³kowska', '10', NULL, '123456', 'Dr n. med.', 'Kardiolog'),
('85051256789', 'k', 'Anna', 'Nowak', '1985-05-12', '601987654', 'anna.nowak@example.com', 'Polska', '30-002', 'Kraków', 'D³uga', '15', '5', 'PWZ654321', 'Lek.', 'Neurolog'),
('78092311122', 'm', 'Piotr', 'Zieliñski', '1978-09-23', '602345678', 'piotr.zielinski@example.com', 'Polska', '40-003', 'Katowice', 'Sokolska', '8', NULL, '789012', 'Lek.', 'Ortopeda'),
('92030433344', 'k', 'Maria', 'Wiœniewska', '1992-03-04', '603456789', 'maria.wisniewska@example.com', 'Polska', '50-004', 'Wroc³aw', 'Legnicka', '22', '3', '567890', 'Dr hab. n. med.', NULL),
('81071555566', 'm', 'Tomasz', 'Lewandowski', '1981-07-15', '604567890', 'tomasz.lewandowski@example.com', 'Polska', '60-005', 'Poznañ', 'G³ogowska', '30', NULL, '345678', 'Prof. dr hab. n. med.', 'Chirurg plastyczny'),
('95020177788', 'k', 'Karolina', 'D¹browska', '1995-02-01', '605678901', 'karolina.dabrowska@example.com', 'Polska', '70-006', 'Szczecin', 'Niepodleg³oœci', '40', '7', '234567', 'Dr n. med.', 'Dermatolog'),
('87080899900', 'm', 'Andrzej', 'Wójcik', '1987-08-08', '606789012', 'andrzej.wojcik@example.com', 'Polska', '80-007', 'Gdañsk', 'D³uga', '12', NULL, '123789', 'Dr hab. n. med.', 'Onkolog'),
('93061222233', 'k', 'Joanna', 'Kamiñska', '1993-06-12', '607890123', 'joanna.kaminska@example.com', 'Polska', '90-008', '£ódŸ', 'Piotrkowska', '55', '9', '456123', 'Dr n. med.', 'Ginekolog'),
('76042944455', 'm', 'Marek', 'Szymañski', '1976-04-29', '608901234', 'marek.szymanski@example.com', 'Polska', '20-009', 'Lublin', 'Lipowa', '5', NULL, '789345', 'Lek.', NULL),
('89031666677', 'k', 'Barbara', 'Jankowska', '1989-03-16', '609012345', 'barbara.jankowska@example.com', 'Polska', '10-010', 'Bia³ystok', 'Branickiego', '14', '2', '987654', 'Lek.', 'Endokrynolog');

INSERT INTO Pacjent (PESEL, plec, imie, nazwisko, dataUrodzenia, nrTel, email, kraj, kodPocztowy, miasto, ulica, nrDomu, nrLokalu) VALUES
('90010112345', 'm', 'Jan', 'Kowalski', '1990-01-01', '123456789', 'jan.kowalski@example.com', 'Polska', '00-001', 'Warszawa', 'Miodowa', '10', NULL),
('85050554321', 'k', 'Anna', 'Nowak', '1985-05-05', '987654321', 'anna.nowak@example.com', 'Polska', '30-001', 'Kraków', 'D³uga', '20', '5'),
('92030423456', 'm', 'Piotr', 'Wiœniewski', '1992-03-04', '111222333', 'piotr.w@example.com', 'Polska', '80-001', 'Gdañsk', 'Grunwaldzka', '15', NULL),
('89080887654', 'k', 'Maria', 'Lewandowska', '1989-08-08', '444555666', 'maria.l@example.com', 'Polska', '40-001', 'Katowice', 'Koœciuszki', '7', 'C'),
('95020109876', 'm', 'Tomasz', 'D¹browski', '1995-02-01', '777888999', 'tomasz.d@example.com', 'Polska', '20-001', 'Lublin', 'Lipowa', '8', NULL),
('81090934567', 'k', 'Karolina', 'Wójcik', '1981-09-09', '222333444', 'karolina.w@example.com', 'Polska', '60-001', 'Poznañ', 'Œwiêty Marcin', '25', NULL),
('87070776543', 'm', 'Micha³', 'Zieliñski', '1987-07-07', '555666777', 'michal.z@example.com', 'Polska', '90-001', '£ódŸ', 'Piotrkowska', '30', NULL),
('93050556789', 'k', 'Alicja', 'Kamiñska', '1993-05-05', '888999000', 'alicja.k@example.com', 'Polska', '50-001', 'Wroc³aw', 'Kazimierza Wielkiego', '18', NULL),
('94030365432', 'm', 'Pawe³', 'Jankowski', '1994-03-03', '999000111', 'pawel.j@example.com', 'Polska', '70-001', 'Szczecin', 'Jagielloñska', '11', '3'),
('96060643210', 'k', 'Ewa', 'Mazur', '1996-06-06', '000111222', 'ewa.m@example.com', 'Polska', '10-001', 'Olsztyn', 'Dworcowa', '5', NULL);

INSERT INTO Choroba (kodChoroby, nazwa, czyZakazna) VALUES
('C001', 'Grypa', 1),
('C002', 'Zapalenie p³uc', 1),
('C003', 'Nowotwór p³uc', 0),
('C004', 'Choroba wieñcowa', 0),
('C005', 'Cukrzyca', 0),
('C006', 'GruŸlica', 1),
('C007', 'Astma', 0),
('C008', 'Nadciœnienie', 0),
('C009', 'Padaczka', 0),
('C010', 'Alzheimer', 0);

INSERT INTO Ordynator (od, do, idOddzialu, idLekarza) VALUES
('2020-01-01', NULL, 1, 1),
('2019-06-15', '2023-06-30', 2, 2),
('2021-03-01', NULL, 3, 3),
('2018-09-10', '2022-09-09', 4, 6),
('2022-02-20', NULL, 5, 7);

INSERT INTO ZatrudnienieSzpital (od, do, idSzpitala, idLekarza) VALUES
('2015-05-10', NULL, 1, 3),
('2017-08-12', NULL, 2, 4),
('2016-11-20', '2023-11-19', 1, 5),
('2023-11-23', NULL, 2, 5),
('2019-01-01', NULL, 3, 6),
('2018-03-15', NULL, 1, 8),
('2021-09-01', NULL, 1, 9),
('2017-12-05', '2022-12-04', 2, 10),
('2016-06-30', NULL, 1, 1),
('2019-04-22', NULL, 1, 1);

INSERT INTO ZatrudnienieOddzial (od, do, idOddzialu, idLekarza) VALUES
('2009-09-17', NULL, 1, 3),
('2011-12-01', NULL, 1, 4),
('2002-01-27', '2005-05-14', 3, 3),
('1994-10-09', NULL, 4, 3),
('1997-05-24', NULL, 1, 4),
('2004-12-24', '2010-10-06', 5, 5),
('2006-07-06', NULL, 6, 6),
('1995-09-25', NULL, 1, 5),
('1994-08-06', '2003-02-13', 2, 8),
('2016-06-30', NULL, 3, 9);

INSERT INTO Przyjecie (od, do, idOddzialu, idPacjenta) VALUES
('1998-03-31', '2007-12-12', 1, 1),
('2014-10-10', NULL, 1, 2),
('2016-05-27', '2017-12-16', 2, 3),
('2017-08-02', NULL, 3, 3),
('1994-07-04', NULL, 4, 3),
('2003-01-21', NULL, 5, 4),
('2005-03-30', '2007-12-01', 1, 1),
('2019-12-15', NULL, 2, 2),
('1997-01-21', NULL, 1, 3),
('2009-11-18', NULL, 1, 1);

INSERT INTO Wizyta (dataWizyty, godzWizyty, gabinet, opis, zalecenie, kodSkierowania, idLekarza, idPacjenta) VALUES
('2012-04-04', '12:40', '119', 'Przyk³adowy opis wizyty: pacjent z bólem g³owy, potrzeba dalszej diagnostyki.', 'Zalecenie: odpoczynek, unikaæ stresu, w razie pogorszenia zg³osiæ siê do lekarza.', '855456079415529', 4, 9),
('2020-09-19', '07:50', '309f', 'Przyk³adowy opis wizyty: pacjent z bólem g³owy, potrzeba dalszej diagnostyki.', 'Zalecenie: odpoczynek, unikaæ stresu, w razie pogorszenia zg³osiæ siê do lekarza.', '264673967314700', 6, 3),
('2023-03-01', '16:40', '256', 'Przyk³adowy opis wizyty: pacjent z bólem g³owy, potrzeba dalszej diagnostyki.', 'Zalecenie: odpoczynek, unikaæ stresu, w razie pogorszenia zg³osiæ siê do lekarza.', '894280689559176', 3, 5),
('2022-10-15', '16:10', '145a', 'Przyk³adowy opis wizyty: pacjent z bólem g³owy, potrzeba dalszej diagnostyki.', 'Zalecenie: odpoczynek, unikaæ stresu, w razie pogorszenia zg³osiæ siê do lekarza.', '615207712655955', 7, 3),
('1997-04-28', '12:30', '342', 'Przyk³adowy opis wizyty: pacjent z bólem g³owy, potrzeba dalszej diagnostyki.', 'Zalecenie: odpoczynek, unikaæ stresu, w razie pogorszenia zg³osiæ siê do lekarza.', '349152633812567', 6, 6),
('2010-08-23', '08:20', '100', 'Przyk³adowy opis wizyty: pacjent z bólem g³owy, potrzeba dalszej diagnostyki.', 'Zalecenie: odpoczynek, unikaæ stresu, w razie pogorszenia zg³osiæ siê do lekarza.', '707327722829931', 9, 1),
('1992-01-03', '07:00', '38', 'Przyk³adowy opis wizyty: pacjent z bólem g³owy, potrzeba dalszej diagnostyki.', 'Zalecenie: odpoczynek, unikaæ stresu, w razie pogorszenia zg³osiæ siê do lekarza.', '347620754164097', 1, 2),
('2018-07-09', '14:20', '292', 'Przyk³adowy opis wizyty: pacjent z bólem g³owy, potrzeba dalszej diagnostyki.', 'Zalecenie: odpoczynek, unikaæ stresu, w razie pogorszenia zg³osiæ siê do lekarza.', '627508100819951', 4, 10),
('2001-06-07', '13:00', '108b', 'Przyk³adowy opis wizyty: pacjent z bólem g³owy, potrzeba dalszej diagnostyki.', 'Zalecenie: odpoczynek, unikaæ stresu, w razie pogorszenia zg³osiæ siê do lekarza.', '428911343310245', 8, 7),
('2014-06-13', '15:30', '324a', 'Przyk³adowy opis wizyty: pacjent z bólem g³owy, potrzeba dalszej diagnostyki.', 'Zalecenie: odpoczynek, unikaæ stresu, w razie pogorszenia zg³osiæ siê do lekarza.', '355056070968287', 4, 10);

INSERT INTO Diagnoza (idWizyty, idChoroby) VALUES
(1, 1),
(2, 1),
(2, 2),
(2, 3),
(4, 2),
(5, 3),
(5, 1),
(2, 5),
(3, 2),
(1, 9);