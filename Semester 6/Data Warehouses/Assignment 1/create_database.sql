CREATE TABLE Szpital (
	id INT IDENTITY(1,1),
	nazwa NVARCHAR(100) NOT NULL,
	kraj NVARCHAR(100) NOT NULL,
	kodPocztowy VARCHAR(10) NOT NULL,
	miasto NVARCHAR(100) NOT NULL,
	ulica NVARCHAR(100) NOT NULL,
	nrDomu NVARCHAR(10) NOT NULL,
	nrLokalu NVARCHAR(10),

	CONSTRAINT pk_szpital PRIMARY KEY (id),
);

CREATE TABLE Oddzial (
	id INT IDENTITY(1,1),
	nr INT NOT NULL,
	nazwa NVARCHAR(100) NOT NULL,
	liczbaLozek INT NOT NULL,
	idSzpitala INT NOT NULL,

	CONSTRAINT pk_oddzial PRIMARY KEY (id),
	CONSTRAINT fk_oddzial_szpital FOREIGN KEY (idSzpitala) REFERENCES Szpital(id),
	CONSTRAINT ck_oddzial_nr CHECK (nr > 0),
	CONSTRAINT ck_oddzial_liczbaLozek CHECK (liczbaLozek > 0),
	CONSTRAINT uq_oddzial_nr_szpital UNIQUE (nr, idSzpitala),
);

CREATE TABLE Lekarz (
	id INT IDENTITY(1,1),
	PESEL CHAR(11) NOT NULL,
	plec CHAR(1) NOT NULL,
	imie NVARCHAR(100) NOT NULL,
	nazwisko NVARCHAR(100) NOT NULL,
	dataUrodzenia DATE NOT NULL,
	nrTel VARCHAR(20) NOT NULL,
	email VARCHAR(100) NOT NULL,
	kraj NVARCHAR(100) NOT NULL,
	kodPocztowy VARCHAR(10) NOT NULL,
	miasto NVARCHAR(100) NOT NULL,
	ulica NVARCHAR(100) NOT NULL,
	nrDomu NVARCHAR(10) NOT NULL,
	nrLokalu NVARCHAR(10),
	pwz VARCHAR(10) NOT NULL,
	tytulNaukowy NVARCHAR(100) NOT NULL,
	specjalizacja NVARCHAR(100),

	CONSTRAINT pk_lekarz PRIMARY KEY (id),
	CONSTRAINT ck_lekarz_plec CHECK (plec IN ('k', 'm')),
	CONSTRAINT ck_lekarz_PESEL CHECK (PESEL LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT uq_lekarz_PESEL UNIQUE (PESEL),
	CONSTRAINT uq_lekarz_pwz UNIQUE (pwz),
);

CREATE TABLE Pacjent (
	id INT IDENTITY(1,1),
	PESEL CHAR(11) NOT NULL,
	plec CHAR(1) NOT NULL,
	imie NVARCHAR(100) NOT NULL,
	nazwisko NVARCHAR(100) NOT NULL,
	dataUrodzenia DATE NOT NULL,
	nrTel VARCHAR(20),
	email VARCHAR(100),
	kraj NVARCHAR(100) NOT NULL,
	kodPocztowy VARCHAR(10) NOT NULL,
	miasto NVARCHAR(100) NOT NULL,
	ulica NVARCHAR(100) NOT NULL,
	nrDomu NVARCHAR(10) NOT NULL,
	nrLokalu NVARCHAR(10),

	CONSTRAINT pk_pacjent PRIMARY KEY (id),
	CONSTRAINT ck_pacjent_plec CHECK (plec IN ('k', 'm')),
	CONSTRAINT ck_pacjent_PESEL CHECK (PESEL LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT uq_pacjent_PESEL UNIQUE (PESEL),
);

CREATE TABLE Choroba (
	id INT IDENTITY(1,1),
	kodChoroby VARCHAR(20) NOT NULL,
	nazwa NVARCHAR(100) NOT NULL,
	czyZakazna BIT NOT NULL,

	CONSTRAINT pk_choroba PRIMARY KEY (id),
	CONSTRAINT uq_choroba_kodChoroby UNIQUE (kodChoroby),
);

CREATE TABLE Ordynator (
	id INT IDENTITY(1,1),
	od DATE NOT NULL DEFAULT GETDATE(),
	do DATE,
	idOddzialu INT NOT NULL,
	idLekarza INT NOT NULL,

	CONSTRAINT pk_ordynator PRIMARY KEY (id),
	CONSTRAINT fk_ordynator_oddzial FOREIGN KEY (idOddzialu) REFERENCES Oddzial(id),
	CONSTRAINT fk_ordynator_lekarz FOREIGN KEY (idLekarza) REFERENCES Lekarz(id),
	CONSTRAINT ck_ordynator_od_do CHECK (do IS NULL OR do > od),
);

CREATE TABLE ZatrudnienieSzpital (
	id INT IDENTITY(1,1),
	od DATE NOT NULL DEFAULT GETDATE(),
	do DATE,
	idSzpitala INT NOT NULL,
	idLekarza INT NOT NULL,	

	CONSTRAINT pk_zatrudnienieSzpital PRIMARY KEY (id),
	CONSTRAINT fk_zatrudnienieSzpital_szpital FOREIGN KEY (idSzpitala) REFERENCES Szpital(id),
	CONSTRAINT fk_zatrudnienieSzpital_lekarz FOREIGN KEY (idLekarza) REFERENCES Lekarz(id),
	CONSTRAINT ck_zatrudnienieSzpital_od_do CHECK (do IS NULL OR do > od),
);

CREATE TABLE ZatrudnienieOddzial (
	id INT IDENTITY(1,1),
	od DATE NOT NULL DEFAULT GETDATE(),
	do DATE,
	idOddzialu INT NOT NULL,
	idLekarza INT NOT NULL,	

	CONSTRAINT pk_zatrudnienieOddzial PRIMARY KEY (id),
	CONSTRAINT fk_zatrudnienieOddzial_oddzial FOREIGN KEY (idOddzialu) REFERENCES Oddzial(id),
	CONSTRAINT fk_zatrudnienieOddzial_lekarz FOREIGN KEY (idLekarza) REFERENCES Lekarz(id),
	CONSTRAINT ck_zatrudnienieOddzial_od_do CHECK (do IS NULL OR do > od),
);

CREATE TABLE Przyjecie (
	id INT IDENTITY(1,1),
	od DATE NOT NULL DEFAULT GETDATE(),
	do DATE,
	idOddzialu INT NOT NULL,
	idPacjenta INT NOT NULL,

	CONSTRAINT pk_przyjecie PRIMARY KEY (id),
	CONSTRAINT fk_przyjecie_oddzial FOREIGN KEY (idOddzialu) REFERENCES Oddzial(id),
	CONSTRAINT fk_przyjecie_pacjent FOREIGN KEY (idPacjenta) REFERENCES Pacjent(id),
	CONSTRAINT ck_przyjecie_od_do CHECK (do IS NULL OR do > od),
);

CREATE TABLE Wizyta (
	id INT IDENTITY(1,1),
	dataWizyty DATE NOT NULL,
	godzWizyty TIME NOT NULL,
	gabinet VARCHAR(10) NOT NULL,
	opis NVARCHAR(max) NOT NULL,
	zalecenie NVARCHAR(max) NOT NULL,
	kodSkierowania VARCHAR(100),
	idLekarza INT NOT NULL,
	idPacjenta INT NOT NULL,

	CONSTRAINT pk_wizyta PRIMARY KEY (id),
	CONSTRAINT fk_wizyta_lekarz FOREIGN KEY (idLekarza) REFERENCES Lekarz(id),
	CONSTRAINT fk_wizyta_pacjent FOREIGN KEY (idPacjenta) REFERENCES Pacjent(id),
);

CREATE TABLE Diagnoza (
	id INT IDENTITY(1,1), 
	idWizyty INT NOT NULL,
	idChoroby INT NOT NULL,

	CONSTRAINT pk_diagnoza PRIMARY KEY (id),
	CONSTRAINT fk_diagnoza_wizyta FOREIGN KEY (idWizyty) REFERENCES Wizyta(id),
	CONSTRAINT fk_diagnoza_choroba FOREIGN KEY (idChoroby) REFERENCES Choroba(id),
	CONSTRAINT uq_diagnoza_wizyta_choroba UNIQUE (idWizyty, idChoroby),
);
