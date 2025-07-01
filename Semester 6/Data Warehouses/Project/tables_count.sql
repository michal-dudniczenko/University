SELECT 'BridgeGroup' AS table_name, COUNT(*) AS row_count FROM BridgeGroup
UNION ALL
SELECT 'BridgeWeapon', COUNT(*) FROM BridgeWeapon
UNION ALL
SELECT 'BridgeAttackType', COUNT(*) FROM BridgeAttackType
UNION ALL
SELECT 'BridgeTarget', COUNT(*) FROM BridgeTarget
UNION ALL
SELECT 'FactAttack', COUNT(*) FROM FactAttack
UNION ALL
SELECT 'DimGroup', COUNT(*) FROM DimGroup
UNION ALL
SELECT 'DimWeapon', COUNT(*) FROM DimWeapon
UNION ALL
SELECT 'DimAttackType', COUNT(*) FROM DimAttackType
UNION ALL
SELECT 'DimTarget', COUNT(*) FROM DimTarget
UNION ALL
SELECT 'DimLocation', COUNT(*) FROM DimLocation
UNION ALL
SELECT 'DimDate', COUNT(*) FROM DimDate
UNION ALL
SELECT 'Staging', COUNT(*) FROM Staging
ORDER BY table_name ASC;
