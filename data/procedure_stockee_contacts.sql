--------------------------------------------------------------------------------------------------------------------
--====================================== Procédure stockée : Address ============================================
---------------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------
-- Procédure stockée : Address - Ajouter une adresse
----------------------------------------------------------------------------------------------
DROP FUNCTION IF EXISTS create_address CASCADE;

CREATE OR REPLACE FUNCTION create_address(
    id UUID,
    street TEXT,
    city TEXT,
    state TEXT,
    zipCode TEXT,
    country TEXT,
    out nb_row_affected INT
) AS $$
    BEGIN
        INSERT INTO public."Addresses" ("Id", "Street", "City", "State", "ZipCode", "Country")
        VALUES (id, street, city, state, zipCode, country);
        GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM create_address('31d5c448-a108-4c1c-8dec-8da4b9dbb62d','Rue Jean Charles, 33', 'Huy','Liège', '4000', 'Belgique');
*/

----------------------------------------------------------------------------------------------
-- Procédure stockée : Address - Mettre à jour une adresse
----------------------------------------------------------------------------------------------
DROP FUNCTION IF EXISTS update_address CASCADE;

CREATE OR REPLACE FUNCTION update_address(
    id UUID,
    street TEXT,
    city TEXT,
    state TEXT,
    zipCode TEXT,
    country TEXT,
    out nb_row_affected INT
) AS $$
    BEGIN
        UPDATE public."Addresses"
        SET "Street" =  street,
            "City" = city,
            "State" = state,
            "ZipCode" = zipCode,
            "Country" = country
        WHERE "Id" = id;
        GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    87.12.24-789.45
*/



--------------------------------------------------------------------------------------------------------------------
--====================================== Procédure stockée : Contact ============================================
---------------------------------------------------------------------------------------------------------------------
DROP TYPE IF EXISTS Contacts CASCADE;

CREATE TYPE Contacts AS (
    id UUID,
    firstname TEXT,
    lastname TEXT,
    registryNumber TEXT,
    email TEXT,
    phoneNumber TEXT,
    mobileNumber TEXT,
    addressId UUID,
    street TEXT,
    city TEXT,
    state TEXT,
    zipCode TEXT,
    country TEXT
);

----------------------------------------------------------------------------------------------
-- Procédure stockée : Contact - Ajouter une personne de contact
----------------------------------------------------------------------------------------------
DROP FUNCTION IF EXISTS create_contact CASCADE;

CREATE OR REPLACE FUNCTION create_contact(
    id UUID,
    firstname TEXT,
    lastname TEXT,
    registryNumber TEXT,
    email TEXT,
    phoneNumber TEXT,
    mobileNumber TEXT,
    addressId UUID,
    out nb_row_affected INT
) AS $$
    BEGIN
        INSERT INTO public."Contacts" ("Id", "Firstname", "Lastname", "RegistryNumber", "Email", "PhoneNumber", "MobileNumber", "AddressId")
        VALUES (id, firstname, lastname, registryNumber, email, phoneNumber, mobileNumber, addressId);
        GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM create_contact('31d5c448-a108-4c1c-8dec-8da4b9dbb62d','Terry', 'Porter','98.12.23-456.32', 't.porter@gmail.com', NULL, NULL, 'ca8458c3-8675-419c-8290-45f14ac4631b');
*/

----------------------------------------------------------------------------------------------
-- Procédure stockée : Contact - Récupérer la liste des personnes de contact
----------------------------------------------------------------------------------------------
DROP FUNCTION IF EXISTS get_contacts CASCADE;

CREATE OR REPLACE FUNCTION get_contacts()
RETURNS SETOF Contacts AS $$
    BEGIN
        RETURN QUERY SELECT c."Id" AS "Id",
                           c."Firstname" AS "Firstname",
                           c."Lastname" AS "Lastname",
                           c."RegistryNumber" AS "RegistryNumber",
                           c."Email" AS "Email",
                           c."MobileNumber" AS "MobileNumber",
                           c."PhoneNumber" AS "PhoneNumber",
                           a."Id" AS "AddressId",
                           a."Street" AS "Street",
                           a."City" AS "City",
                           a."State" AS "State" ,
                           a."ZipCode" AS "ZipCode",
                           a."Country" AS "Country"
                    FROM public."Contacts" c
                    INNER JOIN public."Addresses" a
                        ON c."AddressId" = a."Id";
    END;
$$ LANGUAGE plpgsql;

/* Requête test :
    SELECT * FROM get_contacts();
*/

----------------------------------------------------------------------------------------------
-- Procédure stockée : Contact - Récupérer une personne de contact par son numéro de registre national
----------------------------------------------------------------------------------------------
DROP FUNCTION IF EXISTS get_contact_by_registry_number CASCADE;

CREATE OR REPLACE FUNCTION get_contact_by_registry_number(registryNumber TEXT)
RETURNS Contacts AS $$
    DECLARE
        contactRow Contacts;
    BEGIN
        SELECT c."Id" AS "Id",
                           c."Firstname" AS "Firstname",
                           c."Lastname" AS "Lastname",
                           c."RegistryNumber" AS "RegistryNumber",
                           c."Email" AS "Email",
                           c."MobileNumber" AS "MobileNumber",
                           c."PhoneNumber" AS "PhoneNumber",
                           a."Id" AS "AddressId",
                           a."Street" AS "Street",
                           a."City" AS "City",
                           a."State" AS "State" ,
                           a."ZipCode" AS "ZipCode",
                           a."Country" AS "Country"
                    INTO contactRow
                    FROM public."Contacts" c
                    INNER JOIN public."Addresses" a
                        ON c."AddressId" = a."Id"
                    WHERE c."RegistryNumber" = registryNumber;
        RETURN contactRow;
    END;
$$ LANGUAGE plpgsql;

/* Requête test :
    SELECT * FROM get_contact_by_registry_number('78.12.23-789.12');
*/

----------------------------------------------------------------------------------------------
-- Procédure stockée : Contact - Récupérer une personne de contact par son identifiant
----------------------------------------------------------------------------------------------
DROP FUNCTION IF EXISTS get_contact_by_id CASCADE;

CREATE OR REPLACE FUNCTION get_contact_by_id(contactId UUID)
RETURNS Contacts AS $$
    DECLARE
        contactRow Contacts;
    BEGIN
        SELECT c."Id" AS "Id",
               c."Firstname" AS "Firstname",
               c."Lastname" AS "Lastname",
               c."RegistryNumber" AS "RegistryNumber",
               c."Email" AS "Email",
               c."MobileNumber" AS "MobileNumber",
               c."PhoneNumber" AS "PhoneNumber",
               a."Id" AS "AddressId",
               a."Street" AS "Street",
               a."City" AS "City",
               a."State" AS "State" ,
               a."ZipCode" AS "ZipCode",
               a."Country" AS "Country"
        INTO contactRow
        FROM public."Contacts" c
        INNER JOIN public."Addresses" a
            ON c."AddressId" = a."Id"
        WHERE c."Id" = contactId;
        RETURN contactRow;
    END;
$$ LANGUAGE plpgsql;

/* Requête test :
    SELECT * FROM get_contact_by_id('ba0759a9-60d8-4e6d-9adb-6b0303164b5d');
*/

    ----------------------------------------------------------------------------------------------
-- Procédure stockée : Address - Mettre à jour une personne de contact
----------------------------------------------------------------------------------------------
DROP FUNCTION IF EXISTS update_contact CASCADE;

CREATE OR REPLACE FUNCTION update_contact(
    id UUID,
    firstname TEXT,
    lastname TEXT,
    registryNumber TEXT,
    email TEXT,
    phoneNumber TEXT,
    mobileNumber TEXT,
    addressId UUID,
    out nb_row_affected INT
) AS $$
    BEGIN
        UPDATE public."Contacts"
        SET "Firstname" = firstname,
            "Lastname" = lastname,
            "RegistryNumber" = registryNumber,
            "Email" = email,
            "PhoneNumber" = phoneNumber,
            "MobileNumber" = mobileNumber,
            "AddressId" = addressId
        WHERE "Id" = id;
        GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM update_contact('31d5c448-a108-4c1c-8dec-8da4b9dbb62d','Terry', 'Porter','98.12.23-456.32', 'terry.porter@gmail.com', NULL, NULL, 'ca8458c3-8675-419c-8290-45f14ac4631b');
*/

----------------------------------------------------------------------------------------------
-- Procédure stockée : Contact - Supprimer une personne de contact
----------------------------------------------------------------------------------------------
DROP FUNCTION IF EXISTS delete_contact CASCADE;

CREATE OR REPLACE FUNCTION delete_contact(
    id UUID,
    out nb_row_affected INT
) AS $$
    BEGIN
        DELETE FROM public."Contacts"
        WHERE "Id" = id;
        GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM delete_contact('31d5c448-a108-4c1c-8dec-8da4b9dbb62d');
*/

----------------------------------------------------------------------------------------------
-- Procédure stockée : Contact - Vérifie si le contact existe
----------------------------------------------------------------------------------------------
DROP FUNCTION IF EXISTS registry_number_exists;

CREATE OR REPLACE FUNCTION registry_number_exists(
    registryNumber TEXT
) RETURNS BOOLEAN AS $$
    DECLARE
        exists BOOLEAN;
    BEGIN
        SELECT EXISTS(
            SELECT 1
            FROM public."Contacts"
            WHERE "RegistryNumber" = registryNumber
        ) INTO exists;
        RETURN exists;
    END;
$$ LANGUAGE plpgsql;

/* Requête test :
    SELECT * FROM registry_number_exists('89.12.23-456.23');
 */

----------------------------------------------------------------------------------------------
-- Déclencheur  : Role - Ajouter un rôle
----------------------------------------------------------------------------------------------

DROP TRIGGER IF EXISTS contact_upsert_trigger ON public."Contacts";
DROP FUNCTION IF EXISTS contact_upsert_trigger_function;

CREATE OR REPLACE FUNCTION contact_upsert_trigger_function()
RETURNS TRIGGER AS $$
    DECLARE
        registryNumberExists BOOLEAN;
    BEGIN
        IF(char_length(NEW."Firstname") < 2 OR char_length(NEW."Lastname") < 2) THEN
            RAISE EXCEPTION 'Le nom et le prénom doivent avoir au moins 2 caractères!';
        END IF;
        IF(NEW."Email" IS NOT NULL AND NOT ( NEW."Email" ~ '^[A-Za-z0-9.%!#$/?^|~]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$') ) THEN
            RAISE EXCEPTION 'L''email doit avoir un format valide!';
        END IF;
        IF(NEW."RegistryNumber" IS NOT NULL AND NOT ( NEW."RegistryNumber" ~ '^(\d{2})\.(0[1-9]|1[0-2])\.(0[1-9]|[1-2]\d|3[0-1])-(\d{3})\.(\d{2})$') ) THEN
            RAISE EXCEPTION 'Le numéro de registre national doit avoir un format valide "yy.mm.dd-999.99"!';
        END IF;

        IF (NEW."Email" IS NULL AND NEW."PhoneNumber" IS NULL AND NEW."MobileNumber" IS NULL) THEN
            RAISE EXCEPTION 'Au moins un moyen de contact doit être défini (email, N° fixe, N° mobile)!';
        END IF;

        SELECT EXISTS(
            SELECT 1
            FROM public."Contacts"
            WHERE "RegistryNumber" = NEW."RegistryNumber"
        ) INTO registryNumberExists;

        IF registryNumberExists THEN
            RAISE EXCEPTION 'Le numéro de registre national existe déjà en base de donnée!';
        END IF;


        RETURN NEW;
    END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER contact_upsert_trigger
    BEFORE INSERT OR UPDATE
    ON public."Contacts"
    FOR EACH ROW
    EXECUTE PROCEDURE contact_upsert_trigger_function();

--------------------------------------------------------------------------------------------------------------------
--====================================== Procédure stockée : Role ============================================
---------------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------
-- Procédure stockée : Role - Ajouter un rôle
----------------------------------------------------------------------------------------------

DROP FUNCTION IF EXISTS create_role CASCADE;

CREATE OR REPLACE FUNCTION create_role(
    id UUID,
    name TEXT,
    out nb_row_affected INT
) AS $$
    BEGIN
        INSERT INTO public."Roles" ("Id", "Name")
        VALUES (id, name);
        GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM create_role('31d5c448-a108-4c1c-8dec-8da4b9dbb62d','assistant');
*/

----------------------------------------------------------------------------------------------
-- Procédure stockée : Role - Ajouter un rôle
----------------------------------------------------------------------------------------------

DROP FUNCTION IF EXISTS get_roles CASCADE;

CREATE OR REPLACE FUNCTION get_roles()
RETURNS SETOF public."Roles" AS $$
    BEGIN
        RETURN QUERY SELECT * FROM public."Roles";
    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM get_roles();
*/

--------------------------------------------------------------------------------------------------------------------
--====================================== Procédure stockée : ContactRole ============================================
---------------------------------------------------------------------------------------------------------------------
DROP TYPE IF EXISTS ContactRoles CASCADE;

CREATE TYPE ContactRoles AS (
    "Id" UUID,
    "ContactId" UUID,
    "RoleId" UUID,
    "RoleName" TEXT
);
----------------------------------------------------------------------------------------------
-- Procédure stockée : ContactRole - Ajouter un rôle à une personne de contact
----------------------------------------------------------------------------------------------
DROP FUNCTION IF EXISTS create_contact_role CASCADE;

CREATE OR REPLACE FUNCTION create_contact_role(
    id UUID,
    contactId UUID,
    roleId UUID,
    out nb_row_affected INT
) AS $$
    BEGIN
        INSERT INTO public."ContactRoles" ("Id", "ContactId", "RoleId")
        VALUES (id, contactId, roleId);
        GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM create_contact_role('31d5c448-a108-4c1c-8dec-8da4b9dbb62d','31d5c448-a108-4c1c-8dec-8da4b9dbb62d', '69b49a18-9024-44ba-8455-313df000a8f1');
*/

----------------------------------------------------------------------------------------------
-- Procédure stockée : ContactRole - Supprimer un rôle à une personne de contact
----------------------------------------------------------------------------------------------
DROP FUNCTION IF EXISTS delete_contact_role CASCADE;

CREATE OR REPLACE FUNCTION delete_contact_role(
    id UUID,
    out nb_row_affected INT
) AS $$
    BEGIN
        DELETE FROM public."ContactRoles"
        WHERE "Id" = id;
        GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM delete_contact_role('31d5c448-a108-4c1c-8dec-8da4b9dbb62d');
*/

----------------------------------------------------------------------------------------------
-- Procédure stockée : ContactRole - Récupérer la liste des rôles d'une personne de contact
----------------------------------------------------------------------------------------------
DROP FUNCTION IF EXISTS get_contact_roles CASCADE;

CREATE OR REPLACE FUNCTION get_contact_roles(contactId UUID)
RETURNS SETOF ContactRoles AS $$
    BEGIN
        RETURN QUERY SELECT cr."Id" AS "Id",
        cr."ContactId" AS "ContactId",
        cr."RoleId" AS "RoleId",
        r."Name" AS "RoleName"
FROM public."ContactRoles" cr
INNER JOIN public."Roles" r
    ON cr."RoleId" = r."Id"
WHERE cr."ContactId" = contactId;
    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM get_contact_roles('ba0759a9-60d8-4e6d-9adb-6b0303164b5d');
*/
