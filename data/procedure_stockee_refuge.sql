-----------------------------------------------------------
-- Procédure stockée: Fonctionnalité Refuge
-----------------------------------------------------------

-----------------------------------------------------------
-- Procédure stockée: Type FosterFamilies
-----------------------------------------------------------
DROP TYPE IF EXISTS FosterFamilies CASCADE;

CREATE TYPE FosterFamilies AS (
    "Id" uuid,
    "DateCreated" timestamptz,
    "DateStart" DATE,
    "DateEnd" DATE,
    "AnimalId" VARCHAR,
    "ContactId" UUID,
    "Name" TEXT,
    "Type" TEXT,
    "Gender" VARCHAR(1),
    "BirthDate" DATE,
    "DeathDate" DATE,
    "IsSterilized" BOOLEAN,
    "DateSterilization" DATE,
    "Particularity" TEXT,
    "Description" TEXT,
    "Firstname" TEXT,
    "Lastname" TEXT,
    "RegistryNumber" TEXT,
    "Email" TEXT,
    "PhoneNumber" TEXT,
    "MobileNumber" TEXT,
    "AddressId" UUID,
    "Street" TEXT,
    "City" TEXT,
    "State" TEXT,
    "ZipCode" TEXT,
    "Country" TEXT
);
-----------------------------------------------------------
-- Procédure stockée : Lister les animaux accueillis par une famille d’accueil
-----------------------------------------------------------
DROP FUNCTION IF EXISTS get_foster_families_by_contact;

CREATE OR REPLACE FUNCTION get_foster_families_by_contact(registryNumber varchar)
RETURNS SETOF FosterFamilies AS $$
BEGIN
    RETURN QUERY SELECT ff."Id" AS "Id",
                       ff."DateCreated" AS "DateCreated",
                       ff."DateStart" AS "DateStart",
                       ff."DateEnd" AS "DateEnd",
                       ff."AnimalId" AS "AnimalId",
                       ff."ContactId" AS "ContactId",
                       a."Name" AS "Name",
                       a."Type" AS "Type",
                       a."Gender" AS "Gender",
                       a."BirthDate" AS "BirthDate",
                       a."DeathDate" AS "DeathDate",
                       a."IsSterilized" AS "IsSterilized",
                       a."DateSterilization" AS "DateSterilization",
                       a."Particularity" AS "Particularity",
                       a."Description" AS "Description",
                       c."Firstname" AS "Firstname",
                       c."Lastname" AS "Lastname",
                       c."RegistryNumber" AS "RegistryNumber",
                       c."Email" AS "Email",
                       c."PhoneNumber" AS "PhoneNumber",
                       c."MobileNumber" AS "MobileNumber",
                       c."AddressId" AS "AddressId",
                       add."Street" AS "Street",
                       add."City" AS "City",
                       add."State" AS "State",
                       add."ZipCode" AS "ZipCode",
                       add."Country" AS "Country"
                    FROM public."FosterFamilies" ff
                    INNER JOIN public."Animals" a
                        ON ff."AnimalId" = a."Id"
                    INNER JOIN public."Contacts" c
                        ON ff."ContactId" = c."Id"
                    INNER JOIN public."Addresses" add
                        ON c."AddressId" = add."Id"
                    WHERE c."RegistryNumber" = registryNumber
                    ORDER BY ff."DateCreated" DESC;
END;
$$ LANGUAGE plpgsql;

-- Requête Test: SELECT * FROM get_foster_families_for_contact('78.06.15-654.23');

-----------------------------------------------------------
-- Procédure stockée : Liste des familles d'accueil par où l'animal est passé
-----------------------------------------------------------
DROP FUNCTION IF EXISTS get_foster_families_by_animal;

CREATE OR REPLACE FUNCTION get_foster_families_by_animal(name varchar)
RETURNS SETOF FosterFamilies AS $$
BEGIN
    RETURN QUERY SELECT ff."Id" AS "Id",
                           ff."DateCreated" AS "DateCreated",
                           ff."DateStart" AS "DateStart",
                           ff."DateEnd" AS "DateEnd",
                           ff."AnimalId" AS "AnimalId",
                           ff."ContactId" AS "ContactId",
                           a."Name" AS "Name",
                           a."Type" AS "Type",
                           a."Gender" AS "Gender",
                           a."BirthDate" AS "BirthDate",
                           a."DeathDate" AS "DeathDate",
                           a."IsSterilized" AS "IsSterilized",
                           a."DateSterilization" AS "DateSterilization",
                           a."Particularity" AS "Particularity",
                           a."Description" AS "Description",
                           c."Firstname" AS "Firstname",
                           c."Lastname" AS "Lastname",
                           c."RegistryNumber" AS "RegistryNumber",
                           c."Email" AS "Email",
                           c."PhoneNumber" AS "PhoneNumber",
                           c."MobileNumber" AS "MobileNumber",
                           c."AddressId" AS "AddressId",
                           add."Street" AS "Street",
                           add."City" AS "City",
                           add."State" AS "State",
                           add."ZipCode" AS "ZipCode",
                           add."Country" AS "Country"
                    FROM public."FosterFamilies" ff
                    INNER JOIN public."Animals" a
                        ON ff."AnimalId" = a."Id"
                    INNER JOIN public."Contacts" c
                        ON ff."ContactId" = c."Id"
                    INNER JOIN public."Addresses" add
                        ON c."AddressId" = add."Id"
                    WHERE a."Name" = name
                    ORDER BY ff."DateCreated" DESC;
END;
$$ LANGUAGE plpgsql;

-- Requête Test: SELECT * FROM list_foster_families_by_animal('Johnny');