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


-----------------------------------------------------------
-- Procédure stockée : Ajouter une nouvelle famille d’accueil à un animal
-----------------------------------------------------------

DROP FUNCTION IF EXISTS create_release CASCADE;

CREATE OR REPLACE FUNCTION create_release(
    id UUID,
    reason TEXT,
    dateCreated timestamptz,
    animalId VARCHAR(11),
    contactId UUID,
    out nb_row_affected int
) AS $$
    BEGIN
       INSERT INTO public."Releases" ("Id", "Reason", "DateCreated", "AnimalId", "ContactId")
       VALUES (id, reason, dateCreated, animalId, contactId);
       GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

/* Requête test :
SELECT * FROM create_release(
   '4e063a06-8685-4124-8196-d52c7c2f61cc',
   'famille_accueil',
   '2026-05-02 04:46:36.788960 +00:00',
   '25122434495',
   '6147e2fb-aa24-488d-ac02-7af44403238c'
);

*/

-----------------------------------------------------------
-- Procédure stockée : Ajouter une nouvelle famille d’accueil à un animal
-----------------------------------------------------------

DROP FUNCTION IF EXISTS create_foster_family CASCADE;

CREATE OR REPLACE FUNCTION create_foster_family(
    id UUID,
    dateCreated timestamptz,
    dateStart DATE,
    dateEnd DATE,
    animalId VARCHAR(11),
    contactId UUID,
    out nb_row_affected int
) AS $$
    BEGIN
       INSERT INTO public."FosterFamilies" ("Id", "DateCreated", "DateStart", "DateEnd", "AnimalId", "ContactId")
       VALUES (id, dateCreated, dateStart, dateEnd, animalId, contactId);
       GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

/* Requête test :
SELECT * FROM foster_family(
   '4e063a06-8685-4124-8196-d52c7c2f61cc',
   'famille_accueil',
   '2026-05-02 04:46:36.788960 +00:00',
   '25122434495',
   '6147e2fb-aa24-488d-ac02-7af44403238c'
);

*/

-----------------------------------------------------------
-- Procédure stockée : Récupérer une famille d’accueil
-----------------------------------------------------------
DROP FUNCTION IF EXISTS get_active_foster_family CASCADE;

CREATE OR REPLACE FUNCTION get_active_foster_family(
    animalId VARCHAR(11),
    contactId UUID
) RETURNS SETOF FosterFamilies AS $$
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
                    WHERE ff."DateEnd" IS NULL AND ff."AnimalId" = animalId AND ff."ContactId" = contactId;
END;
$$ LANGUAGE plpgsql;
