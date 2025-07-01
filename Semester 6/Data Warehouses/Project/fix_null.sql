UPDATE Staging
SET
    -- smallint
    iyear = ISNULL(iyear, 0),
    imonth = ISNULL(imonth, 0),
    iday = ISNULL(iday, 0),

    nperps = ISNULL(nperps, -1),
    nperpcap = ISNULL(nperpcap, -1),
    nkill = ISNULL(nkill, -1),
    nkillter = ISNULL(nkillter, -1),
    nwound = ISNULL(nwound, -1),
    nwoundte = ISNULL(nwoundte, -1),
    nhostkid = ISNULL(nhostkid, -1),
    nreleased = ISNULL(nreleased, -1),
    extended = ISNULL(extended, -1),
    multiple = ISNULL(multiple, -1),
    success = ISNULL(success, -1),
    suicide = ISNULL(suicide, -1),
    individual = ISNULL(individual, -1),
    claimed = ISNULL(claimed, -1),
    property = ISNULL(property, -1),
    ishostkid = ISNULL(ishostkid, -1),
    ransom = ISNULL(ransom, -1),
    INT_ANY = ISNULL(INT_ANY, -1),

    -- decimal
    latitude = ISNULL(ROUND(latitude, 6), -91),
    longitude = ISNULL(ROUND(longitude, 6), -181),
    propvalue = ISNULL(ROUND(propvalue, 6), -1),
    ransomamt = ISNULL(ROUND(ransomamt, 6), -1),
    ransompaid = ISNULL(ROUND(ransompaid, 6), -1),

    -- nvarchar
    region_txt = ISNULL(TRIM(region_txt), ''),
    country_txt = ISNULL(TRIM(country_txt), ''),
    provstate = ISNULL(TRIM(provstate), ''),
    city = ISNULL(TRIM(city), ''),

    targtype1_txt = ISNULL(TRIM(targtype1_txt), ''),
    targsubtype1_txt = ISNULL(TRIM(targsubtype1_txt), ''),
    corp1 = ISNULL(TRIM(corp1), ''),
    target1 = ISNULL(TRIM(target1), ''),
    natlty1_txt = ISNULL(TRIM(natlty1_txt), ''),

    targtype2_txt = ISNULL(TRIM(targtype2_txt), ''),
    targsubtype2_txt = ISNULL(TRIM(targsubtype2_txt), ''),
    corp2 = ISNULL(TRIM(corp2), ''),
    target2 = ISNULL(TRIM(target2), ''),
    natlty2_txt = ISNULL(TRIM(natlty2_txt), ''),

    targtype3_txt = ISNULL(TRIM(targtype3_txt), ''),
    targsubtype3_txt = ISNULL(TRIM(targsubtype3_txt), ''),
    corp3 = ISNULL(TRIM(corp3), ''),
    target3 = ISNULL(TRIM(target3), ''),
    natlty3_txt = ISNULL(TRIM(natlty3_txt), ''),

    attacktype1_txt = ISNULL(TRIM(attacktype1_txt), ''),
    attacktype2_txt = ISNULL(TRIM(attacktype2_txt), ''),
    attacktype3_txt = ISNULL(TRIM(attacktype3_txt), ''),

    weaptype1_txt = ISNULL(TRIM(weaptype1_txt), ''),
    weapsubtype1_txt = ISNULL(TRIM(weapsubtype1_txt), ''),

    weaptype2_txt = ISNULL(TRIM(weaptype2_txt), ''),
    weapsubtype2_txt = ISNULL(TRIM(weapsubtype2_txt), ''),

    weaptype3_txt = ISNULL(TRIM(weaptype3_txt), ''),
    weapsubtype3_txt = ISNULL(TRIM(weapsubtype3_txt), ''),

    weaptype4_txt = ISNULL(TRIM(weaptype4_txt), ''),
    weapsubtype4_txt = ISNULL(TRIM(weapsubtype4_txt), ''),

    gname = ISNULL(TRIM(gname), ''),
    gsubname = ISNULL(TRIM(gsubname), ''),

    gname2 = ISNULL(TRIM(gname2), ''),
    gsubname2 = ISNULL(TRIM(gsubname2), ''),

    gname3 = ISNULL(TRIM(gname3), ''),
    gsubname3 = ISNULL(TRIM(gsubname3), '')
;