--------------------------------------------------------------------------------------------------------------------
--====================================== Procédure stockée : Role ============================================
---------------------------------------------------------------------------------------------------------------------








--------------------------------------------------------------------------------------------------------------------
--====================================== Procédure stockée : Admission ============================================
---------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------
-- Procédure stockée : Type Admissions
-----------------------------------------------------------
DROP TYPE IF EXISTS Admissions CASCADE;

CREATE TYPE Admissions AS (
    "Id" uuid,
    "Reason" TEXT,
    "DateCreated" timestamptz,
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
-- Procédure stockée : Admission - Ajouter une entrée
-----------------------------------------------------------
DROP FUNCTION IF EXISTS create_admission;

CREATE OR REPLACE FUNCTION create_admission(
    id UUID,
    reason TEXT,
    dateCreated TIMESTAMPTZ,
    animalId VARCHAR(11),
    contactId UUID,
    out nb_row_affected INT
) AS $$
    BEGIN
        INSERT INTO public."Admissions" ("Id", "Reason", "DateCreated", "ContactId", "AnimalId")
        VALUES (id, reason, dateCreated, contactId, animalId);
        GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

/* Requête test :
    SELECT * FROM create_admission(
       '4e063a06-8685-4124-8196-d52c7c2f61cc',
       'retour_famille_accueil',
       '2026-05-02 04:46:36.788960 +00:00',
       '25122434495',
       '6147e2fb-aa24-488d-ac02-7af44403238c'
    );

*/

    -----------------------------------------------------------
-- Procédure stockée : Lister les admissions (entrées)
-----------------------------------------------------------
DROP FUNCTION IF EXISTS get_admissions CASCADE;

CREATE OR REPLACE FUNCTION get_admissions()
RETURNS SETOF Admissions AS $$
BEGIN
    RETURN QUERY SELECT adm."Id" AS "Id",
                         adm."Reason" AS "Reason",
                           adm."DateCreated" AS "DateCreated",
                           adm."AnimalId" AS "AnimalId",
                           adm."ContactId" AS "ContactId",
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
                        FROM public."Admissions" adm
                        INNER JOIN public."Animals" a
                            ON adm."AnimalId" = a."Id"
                        INNER JOIN public."Contacts" c
                            ON adm."ContactId" = c."Id"
                        INNER JOIN public."Addresses" add
                            ON c."AddressId" = add."Id"
                        ORDER BY adm."DateCreated";
END;
$$ LANGUAGE plpgsql;

-- Requête Test: SELECT * FROM get_admissions();


---------------------------------------------------------------------------------------------------------------------
-- Déclencheur (Trigger) : Admission - Date_entree: Un animal ne peut être entrée plus d’une fois depuis une sortie
---------------------------------------------------------------------------------------------------------------------
DROP TRIGGER IF EXISTS admission_insert_trigger ON public."Admissions";
DROP FUNCTION IF EXISTS admission_insert_trigger_function;

CREATE OR REPLACE FUNCTION admission_insert_trigger_function()
RETURNS TRIGGER AS $$
    DECLARE
        dateLastRelease TIMESTAMPTZ;
        admissionExistsSinceLastRelease BOOLEAN;
    BEGIN
        -- Récupérer la dernière date de sortie de l'animal
        SELECT "DateCreated"
        INTO dateLastRelease
        FROM public."Releases" r
        WHERE r."AnimalId" = NEW."AnimalId" AND r."DateCreated" < NEW."DateCreated"
        ORDER BY r."DateCreated" DESC
        LIMIT 1;

        IF FOUND THEN
            SELECT EXISTS (
                SELECT 1
                FROM public."Admissions" ad
                WHERE ad."AnimalId" = NEW."AnimalId" AND "DateCreated" > dateLastRelease
            ) INTO admissionExistsSinceLastRelease;

            IF admissionExistsSinceLastRelease THEN
                RAISE EXCEPTION 'Une admission existe déjà pour l''animal depuis la dernière sortie';
            END IF;
        END IF;



        RETURN NEW;
    END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER admission_insert_trigger
    BEFORE INSERT
    ON public."Admissions"
    FOR EACH ROW
    EXECUTE PROCEDURE admission_insert_trigger_function();

/** Requête test : Créer une copie d'une admission pour retour de famille d'accueil réalisé
  SELECT * FROM create_admission(
       '4e063a06-8685-4124-8196-d52c7c2f61cc',
       'retour_famille_accueil',
       '2026-08-02 12:46:36.788960 +00:00',
       '25122449541',
       'ba0759a9-60d8-4e6d-9adb-6b0303164b5d'
    );
 */

--------------------------------------------------------------------------------------------------------------------
--====================================== Procédure stockée : Release ============================================
---------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------
-- Procédure stockée : Release - Ajouter une sortie
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

--------------------------------------------------------------------------------------------------------------------
-- Déclencheur (Trigger) : Release (Sortie) - Date_sortie: il n’y a qu’une seule sortie depuis la plus récente date d’entrée de l’animal
--------------------------------------------------------------------------------------------------------------------
DROP TRIGGER IF EXISTS release_insert_trigger ON public."Releases";
DROP FUNCTION IF EXISTS release_insert_trigger_function;

CREATE OR REPLACE FUNCTION release_insert_trigger_function()
RETURNS TRIGGER AS $$
    DECLARE
        dateLastAdmission TIMESTAMPTZ;
        releaseExistsSinceLastAdmission BOOLEAN;
    BEGIN
        -- Récupérer la plus récente date d'entrée de l'animal
        SELECT "DateCreated"
        INTO dateLastAdmission
        FROM public."Admissions"
        WHERE "AnimalId" = NEW."AnimalId" AND "DateCreated" <= CURRENT_DATE
        ORDER BY "DateCreated" DESC
        LIMIT 1;

        IF FOUND THEN
            -- Vérifie s'il existe une sortie depuis la plus récente date d'entrée de l'animal
            SELECT EXISTS(
                SELECT 1
                FROM public."Releases"
                WHERE "AnimalId" = NEW."AnimalId" AND "DateCreated" > dateLastAdmission
            ) INTO releaseExistsSinceLastAdmission;

            IF releaseExistsSinceLastAdmission THEN
                RAISE EXCEPTION 'Une sortie existe déjà depuis la plus récente date d''entrée de l''animal';
            END IF;
        END IF;

        RETURN NEW;
    END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER release_insert_trigger
    BEFORE INSERT
    ON public."Releases"
    FOR EACH ROW
    EXECUTE PROCEDURE release_insert_trigger_function();

/* Requête test :
SELECT * FROM create_release(
   '4e063a06-8685-4124-8196-d52c7c2f61cc',
   'famille_accueil',
   '2026-08-02 12:46:36.788960 +00:00',
   '25122449541',
   '6147e2fb-aa24-488d-ac02-7af44403238c'
);

*/

--------------------------------------------------------------------------------------------------------------------
--====================================== Procédure stockée : Adoption ============================================
---------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------
-- Procédure stockée : Type Adoptions
-----------------------------------------------------------
DROP TYPE IF EXISTS Adoptions CASCADE;

CREATE TYPE Adoptions AS (
    "Id" uuid,
    "Status" TEXT,
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
-- Procédure stockée : Adoption - Lister les adoptions
-----------------------------------------------------------
DROP FUNCTION IF EXISTS get_adoptions;

CREATE OR REPLACE FUNCTION get_adoptions()
RETURNS SETOF Adoptions AS $$
    BEGIN
        RETURN QUERY SELECT ado."Id" AS "Id",
                               ado."Status" AS "Status",
                               ado."DateCreated" AS "DateCreated",
                               ado."DateStart" AS "DateStart",
                               ado."DateEnd" AS "DateEnd",
                               ado."AnimalId" AS "AnimalId",
                               ado."ContactId" AS "ContactId",
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
                        FROM public."Adoptions" ado
                        INNER JOIN public."Animals" a
                            ON ado."AnimalId" = a."Id"
                        INNER JOIN public."Contacts" c
                            ON ado."ContactId" = c."Id"
                        INNER JOIN public."Addresses" add
                            ON c."AddressId" = add."Id"
                        ORDER BY "DateCreated";
    END;
$$ LANGUAGE plpgsql;

-- Requête Test : SELECT * FROM get_adoptions();

-----------------------------------------------------------
-- Procédure stockée : Adoption - Récupérer une adoption par le nom de l'animal et le numéro de registre national du contact
-----------------------------------------------------------
DROP FUNCTION IF EXISTS get_adoption;

CREATE OR REPLACE FUNCTION get_adoption(
    animalId VARCHAR(11),
    contactId UUID
) RETURNS Adoptions AS $$
    DECLARE
        adoptionRow Adoptions;
    BEGIN
        SELECT ado."Id" AS "Id",
               ado."Status" AS "Status",
               ado."DateCreated" AS "DateCreated",
               ado."DateStart" AS "DateStart",
               ado."DateEnd" AS "DateEnd",
               ado."AnimalId" AS "AnimalId",
               ado."ContactId" AS "ContactId",
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
        INTO adoptionRow
        FROM public."Adoptions" ado
        INNER JOIN public."Animals" a
            ON ado."AnimalId" = a."Id"
        INNER JOIN public."Contacts" c
            ON ado."ContactId" = c."Id"
        INNER JOIN public."Addresses" add
            ON c."AddressId" = add."Id"
        WHERE ado."AnimalId" = animalId AND ado."ContactId" = contactId
        ORDER BY "DateCreated" DESC
        LIMIT 1;
        RETURN adoptionRow;
    END;
$$ LANGUAGE plpgsql;

-- Requête Test : SELECT * FROM get_adoption('26020519042','704faabf-70e7-43b8-8bea-ea1323728a2f');

-----------------------------------------------------------
-- Procédure stockée : Adoption - Ajouter une adoption
-----------------------------------------------------------
DROP FUNCTION IF EXISTS create_adoption;

CREATE OR REPLACE FUNCTION create_adoption(
    id UUID,
    status TEXT,
    dateCreated TIMESTAMPTZ,
    dateStart DATE,
    dateEnd DATE,
    animalId VARCHAR(11),
    contactId UUID,
    out nb_row_affected INT
) AS $$
    BEGIN
        INSERT INTO public."Adoptions" ("Id", "Status", "DateCreated", "DateStart", "DateEnd", "AnimalId", "ContactId")
        VALUES (id, status, dateCreated, dateStart, dateEnd, animalId, contactId);
        GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

-- Requête Test : SELECT * FROM create_adoption('21cf7897-130c-41c7-8da2-3da9e465771e', 'demande', '2026-02-05 08:52:16.552702 +00:00', '2026-02-06', NULL, '26020429855', 'ba0759a9-60d8-4e6d-9adb-6b0303164b5d');

-----------------------------------------------------------
-- Procédure stockée : Adoption - Mettre à jour une adoption
-----------------------------------------------------------
DROP FUNCTION IF EXISTS update_adoption;

CREATE OR REPLACE FUNCTION update_adoption(
    id UUID,
    status TEXT,
    dateStart DATE,
    dateEnd DATE,
    out nb_row_affected INT
) AS $$
    BEGIN
        UPDATE public."Adoptions"
        SET "Status" = status,
            "DateStart" = dateStart,
            "DateEnd" = dateEnd
        WHERE "Id" = id;
        GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

-- Requête Test : SELECT * FROM update_adoption('21cf7897-130c-41c7-8da2-3da9e465771e', 'demande', '2026-05-06', NULL);


--------------------------------------------------------------------------------------------------------------------
--====================================== Procédure stockée : Famille d'accueil ============================================
---------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------
-- Procédure stockée : Type FosterFamilies
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
DROP FUNCTION IF EXISTS get_foster_families_by_contact CASCADE;

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
DROP FUNCTION IF EXISTS get_foster_families_by_animal CASCADE;

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
DROP FUNCTION IF EXISTS get_foster_family CASCADE;

CREATE OR REPLACE FUNCTION get_foster_family(
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

-- Requête test : SELECT * FROM get_foster_family('26020145143','6147e2fb-aa24-488d-ac02-7af44403238c');

-----------------------------------------------------------
-- Procédure stockée : Mettre à jour une famille d’accueil pour un animal
-----------------------------------------------------------

DROP FUNCTION IF EXISTS update_foster_family CASCADE;

CREATE OR REPLACE FUNCTION update_foster_family(
    id UUID,
    dateStart DATE,
    dateEnd DATE,
    out nb_row_affected int
) AS $$
    BEGIN
        UPDATE public."FosterFamilies"
        SET "DateStart" = dateStart,
            "DateEnd" = dateEnd
        WHERE "Id" = id;
        GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

-- Requête Test : SELECT * FROM update_foster_family('f64556ea-9a3b-42ea-b10c-0b6ad67425aa', '2026-05-04', NULL);

--------------------------------------------------------------------------------------------------------------------
--====================================== Procédure stockée : Vaccination ============================================
---------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------
-- Procédure stockée : Type Vaccinations
-----------------------------------------------------------
DROP TYPE IF EXISTS Vaccinations CASCADE;

CREATE TYPE Vaccinations AS (
    "Id" uuid,
    "DateCreated" timestamptz,
    "DateVaccination" DATE,
    "Done" BOOLEAN,
    "AnimalId" VARCHAR,
    "VaccineId" UUID,
    "Name" TEXT,
    "Type" TEXT,
    "Gender" VARCHAR(1),
    "BirthDate" DATE,
    "DeathDate" DATE,
    "IsSterilized" BOOLEAN,
    "DateSterilization" DATE,
    "Particularity" TEXT,
    "Description" TEXT,
    "VaccineName" TEXT
);

-----------------------------------------------------------
-- Procédure stockée : Liste des vaccinations réalisées
-----------------------------------------------------------
DROP FUNCTION IF EXISTS get_vaccinations CASCADE;

CREATE OR REPLACE FUNCTION get_vaccinations()
RETURNS SETOF Vaccinations AS $$
BEGIN
    RETURN QUERY SELECT vact."Id" AS "Id",
                           vact."DateCreated" AS "DateCreated",
                           vact."DateVaccination" AS "DateVaccination",
                           vact."Done" AS "Done",
                           vact."AnimalId" AS "AnimalId",
                           vact."VaccineId" AS "VaccineId",
                           a."Name" AS "Name",
                           a."Type" AS "Type",
                           a."Gender" AS "Gender",
                           a."BirthDate" AS "BirthDate",
                           a."DeathDate" AS "DeathDate",
                           a."IsSterilized" AS "IsSterilized",
                           a."DateSterilization" AS "DateSterilization",
                           a."Particularity" AS "Particularity",
                           a."Description" AS "Description",
                           vacc."Name" AS "VaccineName"
                    FROM public."Vaccinations" vact
                    INNER JOIN public."Animals" a
                        ON vact."AnimalId" = a."Id"
                    INNER JOIN public."Vaccines" vacc
                        ON vact."VaccineId" = vacc."Id"
                    ORDER BY vact."DateCreated" DESC;
END;
$$ LANGUAGE plpgsql;

-- Requête Test: SELECT * FROM get_vaccinations();

-----------------------------------------------------------
-- Procédure stockée : Liste des vaccins disponibles
-----------------------------------------------------------
DROP FUNCTION IF EXISTS get_vaccines CASCADE;

CREATE OR REPLACE FUNCTION get_vaccines()
RETURNS SETOF Vaccines AS $$
BEGIN
    RETURN QUERY SELECT v."Id" AS "Id",
                           v."Name" AS "Name"
                    FROM public."Vaccines" v
                    ORDER BY v."Name" DESC;
END;
$$ LANGUAGE plpgsql;

-- Requête Test: SELECT * FROM get_vaccines();

-----------------------------------------------------------
-- Procédure stockée : Ajouter une vaccination pour un animal
-----------------------------------------------------------

DROP FUNCTION IF EXISTS create_vaccination CASCADE;

CREATE OR REPLACE FUNCTION create_vaccination(
    id public."Vaccinations"."Id"%TYPE,
    dateCreated public."Vaccinations"."DateCreated"%TYPE,
    dateVaccination public."Vaccinations"."DateVaccination"%TYPE,
    done public."Vaccinations"."Done"%TYPE,
    animalId public."Vaccinations"."AnimalId"%TYPE,
    vaccineId public."Vaccinations"."VaccineId"%TYPE,
    out nb_row_affected int
) AS $$
    BEGIN
       INSERT INTO public."Vaccinations" ("Id", "DateCreated", "DateVaccination", "Done", "AnimalId", "VaccineId")
       VALUES (id, dateCreated, dateVaccination, done, animalId, vaccineId);
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