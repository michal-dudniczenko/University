-- Wymiary

-- 1. Wymiar daty
CREATE TABLE DimDate (
    date_id INT IDENTITY(1,1) PRIMARY KEY,
    iyear SMALLINT NOT NULL,
    imonth SMALLINT NOT NULL CONSTRAINT chk_imonth_range CHECK (imonth BETWEEN 0 AND 12),
    iday SMALLINT NOT NULL CONSTRAINT chk_iday_range CHECK (iday BETWEEN 0 AND 31),
    CONSTRAINT uq_DimDate UNIQUE (iyear, imonth, iday)
);

-- 2. Wymiar lokalizacji
CREATE TABLE DimLocation (
    location_id INT IDENTITY(1,1) PRIMARY KEY,
    region_txt NVARCHAR(100) NOT NULL,
    country_txt NVARCHAR(100) NOT NULL,
    provstate NVARCHAR(100) NOT NULL,
    city NVARCHAR(100) NOT NULL,
    latitude DECIMAL(8,6) NOT NULL CONSTRAINT chk_latitude_range CHECK (latitude >= -91 AND latitude <= 90),
    longitude DECIMAL(9,6) NOT NULL CONSTRAINT chk_longitude_range CHECK (longitude >= -181 AND longitude <= 180),
    CONSTRAINT uq_DimLocation UNIQUE (region_txt, country_txt, provstate, city, latitude, longitude)
);

-- 3. Wymiar celu
CREATE TABLE DimTarget (
    target_id INT IDENTITY(1,1) PRIMARY KEY,
    targtype_txt NVARCHAR(100) NOT NULL,
    targsubtype_txt NVARCHAR(100) NOT NULL,
    corp NVARCHAR(255) NOT NULL,
    target NVARCHAR(255) NOT NULL,
    natlty_txt NVARCHAR(100) NOT NULL,
    CONSTRAINT uq_DimTarget UNIQUE (targtype_txt, targsubtype_txt, corp, target, natlty_txt)
);

-- 4. Wymiar typu ataku
CREATE TABLE DimAttackType (
    attacktype_id INT IDENTITY(1,1) PRIMARY KEY,
    attacktype_txt NVARCHAR(100) NOT NULL,
    CONSTRAINT uq_DimAttackType UNIQUE (attacktype_txt)
);

-- 5. Wymiar broni
CREATE TABLE DimWeapon (
    weapon_id INT IDENTITY(1,1) PRIMARY KEY,
    weaptype_txt NVARCHAR(100) NOT NULL,
    weapsubtype_txt NVARCHAR(100) NOT NULL,
    CONSTRAINT uq_DimWeapon UNIQUE (weaptype_txt, weapsubtype_txt)
);

-- 6. Wymiar grupy
CREATE TABLE DimGroup (
    group_id INT IDENTITY(1,1) PRIMARY KEY,
    gname NVARCHAR(255) NOT NULL,
    gsubname NVARCHAR(255) NOT NULL,
    CONSTRAINT uq_DimGroup UNIQUE (gname, gsubname)
);



-- Tabela faktów
CREATE TABLE FactAttack (
    [attack_id] INT IDENTITY(1,1) PRIMARY KEY,

    [date_id] INT NOT NULL REFERENCES DimDate(date_id),
    [location_id] INT NOT NULL REFERENCES DimLocation(location_id),

    [eventid] NCHAR(12) NOT NULL CONSTRAINT chk_eventid_format CHECK (
        eventid LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'
    ),
    [nperps] SMALLINT NOT NULL CONSTRAINT chk_nperps_non_negative CHECK (nperps >= -1),
    [nperpcap] SMALLINT NOT NULL CONSTRAINT chk_nperpcap_non_negative CHECK (nperpcap >= -1),
    [nkill] SMALLINT NOT NULL CONSTRAINT chk_nkill_non_negative CHECK (nkill >= -1),
    [nkillter] SMALLINT NOT NULL CONSTRAINT chk_nkillter_non_negative CHECK (nkillter >= -1),
    [nwound] SMALLINT NOT NULL CONSTRAINT chk_nwound_non_negative CHECK (nwound >= -1),
    [nwoundte] SMALLINT NOT NULL CONSTRAINT chk_nwoundte_non_negative CHECK (nwoundte >= -1),
    [propvalue] DECIMAL(20,2) NOT NULL CONSTRAINT chk_propvalue_non_negative CHECK (propvalue >= -1),
    [nhostkid] SMALLINT NOT NULL CONSTRAINT chk_nhostkid_non_negative CHECK (nhostkid >= -1),
    [nreleased] SMALLINT NOT NULL CONSTRAINT chk_nreleased_non_negative CHECK (nreleased >= -1),
    [ransomamt] DECIMAL(20,2) NOT NULL CONSTRAINT chk_ransomamt_non_negative CHECK (ransomamt >= -1),
    [ransompaid] DECIMAL(20,2) NOT NULL CONSTRAINT chk_ransompaid_non_negative CHECK (ransompaid >= -1),

    [extended] SMALLINT NOT NULL CONSTRAINT chk_extended_values CHECK ([extended] IN (-1, 0, 1)),
    [multiple] SMALLINT NOT NULL CONSTRAINT chk_multiple_values CHECK ([multiple] IN (-1, 0, 1)),
    [success] SMALLINT NOT NULL CONSTRAINT chk_success_values CHECK ([success] IN (-1, 0, 1)),
    [suicide] SMALLINT NOT NULL CONSTRAINT chk_suicide_values CHECK ([suicide] IN (-1, 0, 1)),
    [individual] SMALLINT NOT NULL CONSTRAINT chk_individual_values CHECK ([individual] IN (-1, 0, 1)),
    [claimed] SMALLINT NOT NULL CONSTRAINT chk_claimed_values CHECK ([claimed] IN (-1, 0, 1)),
    [property] SMALLINT NOT NULL CONSTRAINT chk_property_values CHECK ([property] IN (-1, 0, 1)),
    [ishostkid] SMALLINT NOT NULL CONSTRAINT chk_ishostkid_values CHECK ([ishostkid] IN (-1, 0, 1)),
    [ransom] SMALLINT NOT NULL CONSTRAINT chk_ransom_values CHECK ([ransom] IN (-1, 0, 1)),
    [INT_ANY] SMALLINT NOT NULL CONSTRAINT chk_INT_ANY_values CHECK ([INT_ANY] IN (-1, 0, 1))

    CONSTRAINT uq_FactAttack_eventid UNIQUE (eventid)
);



-- tabele bridge wiele do wiele
CREATE TABLE BridgeTarget (
    target_id INT NOT NULL REFERENCES DimTarget(target_id),
    attack_id INT NOT NULL REFERENCES FactAttack(attack_id),
    CONSTRAINT pk_BridgeTarget PRIMARY KEY (target_id, attack_id)
);

CREATE TABLE BridgeAttackType (
    attacktype_id INT NOT NULL REFERENCES DimAttackType(attacktype_id),
    attack_id INT NOT NULL REFERENCES FactAttack(attack_id),
    CONSTRAINT pk_BridgeAttackType PRIMARY KEY (attacktype_id, attack_id)
);

CREATE TABLE BridgeWeapon (
    weapon_id INT NOT NULL REFERENCES DimWeapon(weapon_id),
    attack_id INT NOT NULL REFERENCES FactAttack(attack_id),
    CONSTRAINT pk_BridgeWeapon PRIMARY KEY (weapon_id, attack_id)
);

CREATE TABLE BridgeGroup (
    group_id INT NOT NULL REFERENCES DimGroup(group_id),
    attack_id INT NOT NULL REFERENCES FactAttack(attack_id),
    CONSTRAINT pk_BridgeGroup PRIMARY KEY (group_id, attack_id)
);

-- poczekalnia danych
CREATE TABLE Staging (
    -- data
    iyear SMALLINT,
    imonth SMALLINT,
    iday SMALLINT,
    
    -- lokalizacja
    region_txt NVARCHAR(100),
    country_txt NVARCHAR(100),
    provstate NVARCHAR(100),
    city NVARCHAR(100),
    latitude DECIMAL(8,6),
    longitude DECIMAL(9,6),
    
    -- cel
    targtype1_txt NVARCHAR(100),
    targsubtype1_txt NVARCHAR(100),
    corp1 NVARCHAR(255),
    target1 NVARCHAR(255),
    natlty1_txt NVARCHAR(100),

    targtype2_txt NVARCHAR(100),
    targsubtype2_txt NVARCHAR(100),
    corp2 NVARCHAR(255),
    target2 NVARCHAR(255),
    natlty2_txt NVARCHAR(100),

    targtype3_txt NVARCHAR(100),
    targsubtype3_txt NVARCHAR(100),
    corp3 NVARCHAR(255),
    target3 NVARCHAR(255),
    natlty3_txt NVARCHAR(100),
    
    -- typ ataku
    attacktype1_txt NVARCHAR(100),
    attacktype2_txt NVARCHAR(100),
    attacktype3_txt NVARCHAR(100),
    
    -- bron
    weaptype1_txt NVARCHAR(100),
    weapsubtype1_txt NVARCHAR(100),

    weaptype2_txt NVARCHAR(100),
    weapsubtype2_txt NVARCHAR(100),

    weaptype3_txt NVARCHAR(100),
    weapsubtype3_txt NVARCHAR(100),

    weaptype4_txt NVARCHAR(100),
    weapsubtype4_txt NVARCHAR(100),
    
    -- grupa 
    gname NVARCHAR(255),
    gsubname NVARCHAR(255),

    gname2 NVARCHAR(255),
    gsubname2 NVARCHAR(255),

    gname3 NVARCHAR(255),
    gsubname3 NVARCHAR(255),
    
    -- atak terrorystyczny
    eventid NCHAR(12),
    nperps SMALLINT,
    nperpcap SMALLINT,
    nkill SMALLINT,
    nkillter SMALLINT,
    nwound SMALLINT,
    nwoundte SMALLINT,
    propvalue DECIMAL(20,2),
    nhostkid SMALLINT,
    nreleased SMALLINT,
    ransomamt DECIMAL(20,2),
    ransompaid DECIMAL(20,2),
    extended SMALLINT,
    multiple SMALLINT,
    success SMALLINT,
    suicide SMALLINT,
    individual SMALLINT,
    claimed SMALLINT,
    property SMALLINT,
    ishostkid SMALLINT,
    ransom SMALLINT,
    INT_ANY SMALLINT
);
