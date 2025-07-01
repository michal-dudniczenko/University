-- Usuwanie tabel bridge
DROP TABLE IF EXISTS BridgeGroup;
DROP TABLE IF EXISTS BridgeWeapon;
DROP TABLE IF EXISTS BridgeAttackType;
DROP TABLE IF EXISTS BridgeTarget;

-- Usuwanie tabeli faktów
DROP TABLE IF EXISTS FactAttack;

-- Usuwanie tabel wymiarów
DROP TABLE IF EXISTS DimGroup;
DROP TABLE IF EXISTS DimWeapon;
DROP TABLE IF EXISTS DimAttackType;
DROP TABLE IF EXISTS DimTarget;
DROP TABLE IF EXISTS DimLocation;
DROP TABLE IF EXISTS DimDate;

DROP TABLE IF EXISTS Staging;
