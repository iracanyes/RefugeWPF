--------------------------------------------------------------------------------------------------------------------
--====================================== Procédure stockée : Animal ============================================
---------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------
-- Procédure stockée : Animal - Ajouter un animal
-----------------------------------------------------------
DROP FUNCTION IF EXISTS create_animal CASCADE;

CREATE OR REPLACE FUNCTION create_animal(
    id VARCHAR(11),
    name TEXT,
    type TEXT,
    gender VARCHAR(1),
    birthDate DATE,
    deathDate DATE,
    isSterilized BOOLEAN,
    dateSterilization DATE,
    particularity TEXT,
    description TEXT,
    out nb_row_affected INT
) AS $$
BEGIN
    INSERT INTO public."Animals" ("Id", "Name", "Type", "Gender", "BirthDate", "DeathDate", "IsSterilized", "DateSterilization", "Particularity", "Description")
    VALUES (id, name, type, gender, birthDate, deathDate, isSterilized, dateSterilization, particularity, description);
    GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
END;
$$ LANGUAGE plpgsql;

/* Requête test :
   SELECT * FROM create_animal(
    '8ce6426b-1caa-46b6-9c2d-8b56c5a02a4b',
    'Zenith',
    'chien',
    'F',
    '2026-02-06',
    NULL,
    FALSE,
    NULL,
    'Territorial
Méchant
',
    'Aime jouer dehors'
   );
 */

-----------------------------------------------------------
-- Procédure stockée : Animal - Récupérer la liste des animaux
-----------------------------------------------------------
DROP FUNCTION IF EXISTS get_animals CASCADE;

CREATE OR REPLACE FUNCTION get_animals()
RETURNS SETOF public."Animals" AS $$
BEGIN
    RETURN QUERY SELECT * FROM public."Animals" ORDER BY "Name";
END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM get_animals();
*/

-----------------------------------------------------------
-- Procédure stockée : Animal - Récupérer des animaux ayant le mêm nom
-----------------------------------------------------------
DROP FUNCTION IF EXISTS get_animal_by_name CASCADE;

CREATE OR REPLACE FUNCTION get_animal_by_name(name TEXT)
RETURNS SETOF public."Animals" AS $$
BEGIN
    RETURN QUERY SELECT *
                 FROM public."Animals"
                 WHERE "Name" = name;
END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM get_animal('Zenith');
*/

-----------------------------------------------------------
-- Procédure stockée : Animal - Récupérer un animal par son ID
-----------------------------------------------------------
DROP FUNCTION IF EXISTS get_animal_by_id CASCADE;

CREATE OR REPLACE FUNCTION get_animal_by_id(id VARCHAR)
RETURNS public."Animals" AS $$
    DECLARE
        animal_ligne public."Animals"%ROWTYPE;
    BEGIN
        SELECT *
        INTO animal_ligne
        FROM public."Animals"
        WHERE "Id" = id;
        RETURN animal_ligne;
    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM get_animal('26010416322');
*/



-----------------------------------------------------------
-- Procédure stockée : Animal - Supprimer un animal
-----------------------------------------------------------
DROP FUNCTION IF EXISTS delete_animal CASCADE;

CREATE OR REPLACE FUNCTION delete_animal(
    id VARCHAR(11),
    out nb_row_affected INT
) AS $$
    BEGIN
        DELETE FROM public."Animals"
        WHERE "Id" = id;
        GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM delete_animal('26010416322');
*/

-----------------------------------------------------------
-- Procédure stockée : Animal - Mettre à jour un animal
-----------------------------------------------------------
DROP FUNCTION IF EXISTS update_animal CASCADE;

CREATE OR REPLACE FUNCTION update_animal(
    id VARCHAR(11),
    name TEXT,
    type TEXT,
    gender VARCHAR(1),
    birthDate DATE,
    deathDate DATE,
    isSterilized BOOLEAN,
    dateSterilization DATE,
    particularity TEXT,
    description TEXT,
    out nb_row_affected INT
) AS $$
    BEGIN
        UPDATE public."Animals"
        SET "Name" = name,
            "Type" = type,
            "Gender" = gender,
            "BirthDate" = birthDate,
            "DeathDate" = deathDate,
            "IsSterilized" = isSterilized,
            "DateSterilization" = dateSterilization,
            "Particularity" = particularity,
            "Description" = description
        WHERE "Id" = id;
        GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM delete_animal('26010416322');
*/



--------------------------------------------------------------------------------------------------------------------
--====================================== Procédure stockée : Color ============================================
---------------------------------------------------------------------------------------------------------------------

-----------------------------------------------------------
-- Procédure stockée : Compatibilité - Récupérer les couleurs des animaux
-----------------------------------------------------------

DROP FUNCTION IF EXISTS get_colors CASCADE;

CREATE OR REPLACE FUNCTION get_colors()
RETURNS SETOF public."Colors" AS $$
    BEGIN
        RETURN QUERY SELECT *
                        FROM public."Colors"
                        ORDER BY "Name";

    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM get_colors();
*/

--------------------------------------------------------------------------------------------------------------------
--====================================== Procédure stockée : AnimalColor ============================================
---------------------------------------------------------------------------------------------------------------------

-----------------------------------------------------------
-- Procédure stockée : AnimalColor - Type
-----------------------------------------------------------
DROP TYPE IF EXISTS AnimalColors CASCADE;

CREATE TYPE AnimalColors AS (
    "Id" UUID,
    "ColorId" UUID,
    "AnimalId" VARCHAR(11),
    "ColorName" TEXT
);

-----------------------------------------------------------
-- Procédure stockée : AnimalColor - Récupérer les couleurs pour un animal
-----------------------------------------------------------

DROP FUNCTION IF EXISTS get_animal_colors CASCADE;

CREATE OR REPLACE FUNCTION get_animal_colors(animalId VARCHAR)
RETURNS SETOF AnimalColors AS $$
    BEGIN
        RETURN QUERY SELECT ac."Id" AS "Id",ac."ColorId",ac."AnimalId", c."Name" AS "ColorName"
                        FROM public."AnimalColors" ac
                        INNER JOIN public."Colors" c
                    ON ac."ColorId" = c."Id"
                        WHERE ac."AnimalId" = animalId;

    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM get_animal_colors('26010416322');
*/

----------------------------------------------------------------------------------------------
-- Procédure stockée : AnimalColor - Ajouter une couleur à un animal
----------------------------------------------------------------------------------------------

DROP FUNCTION IF EXISTS create_animal_color CASCADE;

CREATE OR REPLACE FUNCTION create_animal_color(
    id UUID,
    animalId VARCHAR(11),
    colorId UUID,
    out nb_row_affected INT
) AS $$
    BEGIN
        INSERT INTO public."AnimalColors" ("Id", "AnimalId", "ColorId")
        VALUES (id, animalId, colorId);
        GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM create_animal_color('31d5c448-a108-4c1c-8dec-8da4b9dbb62d','26020597919', '93e6a21e-cf33-4989-81dc-73805c6c6db0');
*/

--------------------------------------------------------------------------------------------------------------------
--====================================== Procédure stockée : Compatibility ============================================
---------------------------------------------------------------------------------------------------------------------

-----------------------------------------------------------
-- Procédure stockée : Compatibilité - Ajouter une compatibilité
-----------------------------------------------------------

DROP FUNCTION IF EXISTS create_compatibility CASCADE;

CREATE OR REPLACE FUNCTION create_compatibility(
    id UUID,
    type TEXT,
    out nb_row_affected INT
) AS $$
    BEGIN
        INSERT INTO public."Compatibilities" ("Id", "Type")
        VALUES (id, type);
        GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM create_compatibility('31d5c448-a108-4c1c-8dec-8da4b9dbb62d','tortue');
*/

-----------------------------------------------------------
-- Procédure stockée : Compatibilité - Récupérer les compatibilités des animaux
-----------------------------------------------------------

DROP FUNCTION IF EXISTS get_compatibilities CASCADE;

CREATE OR REPLACE FUNCTION get_compatibilities()
RETURNS SETOF public."Compatibilities" AS $$
    BEGIN
        RETURN QUERY SELECT *
                        FROM public."Compatibilities"
                        ORDER BY "Type";

    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM get_compatibilities();
*/


--------------------------------------------------------------------------------------------------------------------
--====================================== Procédure stockée : AnimalCompatibility ============================================
---------------------------------------------------------------------------------------------------------------------

 --------------------------------------------------------------------------------------
-- Type : Compatibilité pour un animal
-----------------------------------------------------------------------------------------
DROP TYPE IF EXISTS AnimalCompatibilities CASCADE;

CREATE TYPE AnimalCompatibilities AS (
    "Id" UUID,
    "Value" BOOLEAN,
    "AnimalCompatibilityDescription" TEXT,
    "AnimalId" VARCHAR,
    "CompatibilityId" UUID,
    "Name" TEXT,
    "AnimalType" TEXT,
    "Gender" VARCHAR(1),
    "BirthDate" DATE,
    "DeathDate" DATE,
    "IsSterilized" BOOLEAN,
    "DateSterilization" DATE,
    "Particularity" TEXT,
    "AnimalDescription" TEXT,
    "CompatibilityType" TEXT
);

--------------------------------------------------------------------------------------
-- Procédure stockée : Compatibilité pour un animal - Ajouter une compatibilité pour un animal
-----------------------------------------------------------------------------------------

DROP FUNCTION IF EXISTS create_animal_compatibility CASCADE;

CREATE OR REPLACE FUNCTION create_animal_compatibility(
    id UUID,
    acValue BOOLEAN,
    description TEXT,
    animalId VARCHAR(11),
    compatibilityId UUID,
    out nb_row_affected INT
) AS $$
    BEGIN
        INSERT INTO public."AnimalCompatibilities" ("Id", "Value", "Description", "AnimalId", "CompatibilityId")
        VALUES (id, acValue, description, animalId, compatibilityId);
        GET DIAGNOSTICS nb_row_affected = ROW_COUNT;
    END;
$$ LANGUAGE plpgsql;

/* Requête test;
    SELECT * FROM create_animal_compatibility(
        '31d5c448-a108-4c1c-8dec-8da4b9dbb62d',
        true,
        'S''entend très bien',
        '26020519042',
        'dd51bf9d-741c-42b0-b04a-019a08f91976'
    );
*/


------------------------------------------------------------------------------------------------------------
-- Procédure stockée : Compatibilité pour un animal - Récupérer la liste des compatibilités pour un animal
------------------------------------------------------------------------------------------------------------
DROP FUNCTION IF EXISTS get_animal_compatibilities;

CREATE OR REPLACE FUNCTION get_animal_compatibilities(
    animalId VARCHAR
) RETURNS SETOF AnimalCompatibilities AS $$
    BEGIN
        RETURN QUERY SELECT ac."Id" AS "Id",
                            ac."Value" AS "Value",
                            ac."Description" AS "AnimalCompatibilityDescription",
                            ac."AnimalId" AS "AnimalId",
                            ac."CompatibilityId" AS "CompatibilityId",
                            a."Name" AS "Name",
                            a."Type" AS "AnimalType",
                            a."Gender" AS "Gender",
                            a."BirthDate" AS "BirthDate",
                            a."DeathDate" AS "DeathDate",
                            a."IsSterilized" AS "IsSterilized",
                            a."DateSterilization" AS "DateSterilization",
                            a."Particularity" AS "Particularity",
                            a."Description" AS "Description",
                            c."Type" AS "CompatibilityType"
                     FROM public."AnimalCompatibilities" ac
                     INNER JOIN public."Animals" a
                         ON a."Id" = ac."AnimalId"
                     INNER JOIN public."Compatibilities" c
                         ON c."Id" = ac."CompatibilityId"
                     WHERE ac."AnimalId" = animalId;
    END;
$$ LANGUAGE plpgsql;

