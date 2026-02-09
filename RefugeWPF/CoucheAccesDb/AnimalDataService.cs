  using Microsoft.Extensions.Logging;
using Npgsql;
using RefugeWPF.CoucheMetiers.Exceptions;
using RefugeWPF.CoucheMetiers.Helper;
using RefugeWPF.CoucheMetiers.Model.Entities;
using RefugeWPF.CoucheMetiers.Model.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RefugeWPF.CoucheAccesDB
{
    internal class AnimalDataService : AccessDb, IAnimalDataService
    {
        private static readonly ILogger MyLogger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(nameof(AnimalDataService));
        

        public AnimalDataService()
            : base() { }



        /**
         * <summary>
         *  Ajouter un animal
         * </summary>
         */
        public Animal CreateAnimal(Animal animal)
        {
            Animal? result = null;
            NpgsqlCommand? sqlCmd = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    $"""
                    SELECT * FROM create_animal(:id, :name, :type, :gender, :dateBirth, :dateDeath, :isSterilized, :dateSterilization, :particularity, :description );
                    """,
                    this.SqlConn
                );

                sqlCmd.Parameters.Add(new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("name", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("type", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("gender", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("dateBirth", NpgsqlTypes.NpgsqlDbType.Date));
                sqlCmd.Parameters.Add(new NpgsqlParameter("dateDeath", NpgsqlTypes.NpgsqlDbType.Date));
                sqlCmd.Parameters.Add(new NpgsqlParameter("isSterilized", NpgsqlTypes.NpgsqlDbType.Boolean));
                sqlCmd.Parameters.Add(new NpgsqlParameter("dateSterilization", NpgsqlTypes.NpgsqlDbType.Date));
                sqlCmd.Parameters.Add(new NpgsqlParameter("particularity", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("description", NpgsqlTypes.NpgsqlDbType.Text));

                // Prepare the parametized statement
                sqlCmd.Prepare();

                
                // Parameters value
                sqlCmd.Parameters["id"].Value = animal.Id;
                sqlCmd.Parameters["name"].Value = animal.Name;
                sqlCmd.Parameters["type"].Value = animal.Type;
                sqlCmd.Parameters["gender"].Value = animal.Gender;
                sqlCmd.Parameters["dateBirth"].Value = animal.BirthDate;
                sqlCmd.Parameters["dateDeath"].Value = animal.DeathDate != null ? animal.DeathDate : DBNull.Value;
                sqlCmd.Parameters["isSterilized"].Value = animal.IsSterilized;
                sqlCmd.Parameters["dateSterilization"].Value = animal.DateSterilization != null ? animal.DateSterilization : DBNull.Value;
                sqlCmd.Parameters["particularity"].Value = animal.Particularity != null ? animal.Particularity : DBNull.Value;
                sqlCmd.Parameters["description"].Value = animal.Description != null ? animal.Description : DBNull.Value;
                
                // Parametized statement execution
                int createOp = sqlCmd.ExecuteNonQuery();

                if (createOp == 0) throw new Exception("Impossible d'ajouter l'animal!");

                result = GetAnimalById(animal.Id);

            }
            catch (Exception ex)
            {
                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, ex.Message);
                else
                    throw new AccessDbException("Error while creating an animal", ex.Message);
            }

            if (result == null) throw new Exception($"Unable to find newly created animal with name {animal.Name}");

            return result;
        }

        /**
         * <summary>
         *  Récupérer la liste des animaux
         * </summary>
         */
        public List<Animal> GetAnimals()
        {
            List<Animal> result = [];
            NpgsqlCommand? sqlCmd = null;
            NpgsqlDataReader? reader = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    $"""
                    SELECT * FROM get_animals();
                    """,
                    SqlConn
                );

                sqlCmd.Prepare();

                reader = sqlCmd.ExecuteReader();

                while (reader.Read())
                {
                    Debug.WriteLine($"reader - birthDate : {reader["BirthDate"]}");
                    DateOnly? birthDate = reader["BirthDate"] != DBNull.Value ? (DateOnly)reader["BirthDate"] : null;
                    DateOnly? deathDate = reader["DeathDate"] != DBNull.Value ? (DateOnly)reader["DeathDate"] : null;
                    DateOnly? dateSterilization = reader["dateSterilization"] != DBNull.Value ? (DateOnly)reader["dateSterilization"] : null;

                    Animal animal = new Animal(
                        Convert.ToString(reader["Id"])!,
                        Convert.ToString(reader["Name"])!,
                        MyEnumHelper.GetEnumFromDescription<AnimalType>(Convert.ToString(reader["Type"])!),
                        MyEnumHelper.GetEnumFromDescription<GenderType>(Convert.ToString(reader["Gender"])!),
                        birthDate,
                        deathDate,
                        Convert.ToBoolean(reader["IsSterilized"])!,
                        dateSterilization,
                        Convert.ToString(reader["Particularity"])!,
                        Convert.ToString(reader["Description"])!

                    );




                    result.Add(animal);


                }

                reader.Close();

                foreach (Animal a in result)
                {
                    // Retrieve animal's colors from DB
                    this.GetAnimalColors(a);
                }

            }
            catch (Exception ex)
            {
                if (reader != null) reader.Close();

                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, ex.ToString());
                else
                    throw new AccessDbException("Error while retrieving all animals", ex.ToString());
            }

            

            return result;
        }

        /**
         * <summary>
         *  Récupérer les animaux ayant le même nom
         * </summary>
         */
        public List<Animal> GetAnimalByName(string name)
        {
            List<Animal> result = [];
            NpgsqlCommand? sqlCmd = null;
            NpgsqlDataReader? reader = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    $"""
                    SELECT *
                    FROM get_animal_by_name(:name);
                    """,
                    SqlConn
                );

                sqlCmd.Parameters.Add("name", NpgsqlTypes.NpgsqlDbType.Text);

                sqlCmd.Prepare();

                sqlCmd.Parameters["name"].Value = name;

                reader = sqlCmd.ExecuteReader();

                while (reader.Read())
                {
                    Debug.WriteLine($"reader - birthDate : {reader["BirthDate"]}");
                    DateOnly? birthDate = reader["BirthDate"] != DBNull.Value ? (DateOnly) reader["BirthDate"] : null; 
                    DateOnly? deathDate = reader["DeathDate"] != DBNull.Value ? (DateOnly) reader["DeathDate"] : null;
                    DateOnly? dateSterilization = reader["dateSterilization"] != DBNull.Value ? (DateOnly)  reader["dateSterilization"] : null;

                    Animal animal = new Animal(
                        Convert.ToString(reader["Id"])!,
                        Convert.ToString(reader["Name"])!,
                        MyEnumHelper.GetEnumFromDescription<AnimalType>(Convert.ToString(reader["Type"])!),
                        MyEnumHelper.GetEnumFromDescription<GenderType>(Convert.ToString(reader["Gender"])!),
                        birthDate,
                        deathDate,
                        Convert.ToBoolean(reader["IsSterilized"])!,
                        dateSterilization,
                        Convert.ToString(reader["Particularity"])!,
                        Convert.ToString(reader["Description"])!

                    );

                    

                    result.Add( animal );                    


                }

                reader.Close();

                foreach(Animal a in result)
                {
                    // Retrieve animal's colors from DB
                    this.GetAnimalColors(a);
                }

            }
            catch (Exception ex)
            {
                if(reader != null) reader.Close();

                if(sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, ex.ToString());
                else
                    throw new AccessDbException("Error while retrieving an animal", ex.ToString());
            }

            

            return result;
        }

        /**
         * <summary>
         *  Récupérer un animal par son identifiant
         * </summary>
         */
        public Animal? GetAnimalById(string id)
        {
            Animal? result = null;
            NpgsqlCommand? sqlCmd = null;
            NpgsqlDataReader? reader = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    $"""
                    SELECT *
                    FROM get_animal_by_id(:id)
                    """,
                    SqlConn
                );

                sqlCmd.Parameters.Add("id", NpgsqlTypes.NpgsqlDbType.Varchar);

                sqlCmd.Prepare();

                sqlCmd.Parameters["id"].Value = id;

                reader = sqlCmd.ExecuteReader();

                if (reader.Read())
                {
                    Debug.WriteLine($"reader - birthDate : {reader["BirthDate"]}");
                    DateOnly? birthDate = reader["BirthDate"] != DBNull.Value ? (DateOnly)reader["BirthDate"] : null;
                    DateOnly? deathDate = reader["DeathDate"] != DBNull.Value ? (DateOnly)reader["DeathDate"] : null;
                    DateOnly? dateSterilization = reader["dateSterilization"] != DBNull.Value ? (DateOnly)reader["dateSterilization"] : null;


                    result = new Animal(
                        Convert.ToString(reader["Id"])!,
                        Convert.ToString(reader["Name"])!,
                        MyEnumHelper.GetEnumFromDescription<AnimalType>(Convert.ToString(reader["Type"])!),
                        MyEnumHelper.GetEnumFromDescription<GenderType>(Convert.ToString(reader["Gender"])!),
                        birthDate,
                        deathDate,
                        Convert.ToBoolean(reader["IsSterilized"])!,
                        dateSterilization,
                        Convert.ToString(reader["Particularity"])!,
                        Convert.ToString(reader["Description"])!

                    );

                    


                }

                reader.Close();

                if (result == null)
                    throw new AccessDbException(sqlCmd.CommandText, $"Unknown error while retrieving animal.");

                // Retrieve animal's colors from DB
                this.GetAnimalColors(result);
            }
            catch (Exception ex)
            {
                reader?.Close();

                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, ex.ToString());
                else
                    throw new AccessDbException("Error while retrieving an animal", ex.ToString());
            }



            return result;
        }

        /**
         * <summary>
         *  Récupérer les couleurs d'un animal
         * </summary>
         */
        public HashSet<AnimalColor> GetAnimalColors(Animal animal)
        {
            NpgsqlCommand? sqlCmd = null;
            NpgsqlDataReader? reader = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    """
                    SELECT *
                    FROM get_animal_colors(:animalId);
                    """,
                    this.SqlConn
                );

                sqlCmd.Parameters.Add(new NpgsqlParameter("animalId", NpgsqlTypes.NpgsqlDbType.Varchar));

                sqlCmd.Prepare();

                sqlCmd.Parameters["animalId"].Value = animal.Id;

                reader = sqlCmd.ExecuteReader();

                while (reader.Read()) {
                    Color color = new Color(new Guid(Convert.ToString(reader["ColorId"])!), Convert.ToString(reader["ColorName"])!);

                    animal.AnimalColors.Add(
                        new AnimalColor(
                            new Guid(Convert.ToString(reader["Id"])!),
                            animal,
                            new Color(
                                new Guid(Convert.ToString(reader["ColorId"])!),
                                Convert.ToString(reader["ColorName"])!
                            )
                        )
                    );
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                if(reader != null) { reader.Close(); }

                if (Debugger.IsAttached)
                    Debug.WriteLine($"Error while retrieving animal's colors for {animal.Name}. Message: {ex.Message}. Exception : {ex}");

                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, $"Error while retrieving animal's colors for {animal.Name}. Message: {ex.Message}. Exception : {ex}");
                else
                    throw new AccessDbException("sqlCmd.CommandText is null", $"Error while retrieving animal's colors for {animal.Name}. Message: {ex.Message}. Exception : {ex}");

            }

            return animal.AnimalColors;

        }

        /**
         * <summary>
         *  Supprimer un animal
         * </summary>
         */
        public bool RemoveAnimal(Animal animal)
        {
            bool result = false;
            NpgsqlCommand? sqlCmd = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    $"""
                    SELECT * FROM delete_animal(:id);
                    """, 
                    this.SqlConn
                );

                sqlCmd.Parameters.Add(new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Varchar));

                sqlCmd.Prepare();

                sqlCmd.Parameters["id"].Value = animal.Id;

                int response = sqlCmd.ExecuteNonQuery();

                if (response == 0) throw new AccessDbException(sqlCmd.CommandText, $"Error while deleting an animal with ID({animal.Id})");

                result = true;
            }
            catch (Exception ex)
            {
                if(sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, ex.Message);
                else
                    throw new AccessDbException("Error while creating the SQL command instance!", ex.Message);
            }

            return result;
        }

        /**
         * <summary>
         *  Mettre à jour un animal
         * </summary>
         */
        public Animal UpdateAnimal(Animal animal)
        {
            Animal? result = null;
            NpgsqlCommand? sqlCmd = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    $"""
                    SELECT * FROM update_animal(:id, :name,  :type, :gender, :dateBirth,  :dateDeath, :isSterilized, :dateSterilization, :particularity, :description);
                    """,
                    this.SqlConn
                );

                sqlCmd.Parameters.Add(new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("name", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("type", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("gender", NpgsqlTypes.NpgsqlDbType.Varchar));
                sqlCmd.Parameters.Add(new NpgsqlParameter("dateBirth", NpgsqlTypes.NpgsqlDbType.Date));
                sqlCmd.Parameters.Add(new NpgsqlParameter("dateDeath", NpgsqlTypes.NpgsqlDbType.Date));
                sqlCmd.Parameters.Add(new NpgsqlParameter("isSterilized", NpgsqlTypes.NpgsqlDbType.Boolean));
                sqlCmd.Parameters.Add(new NpgsqlParameter("dateSterilization", NpgsqlTypes.NpgsqlDbType.Date));
                sqlCmd.Parameters.Add(new NpgsqlParameter("particularity", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("description", NpgsqlTypes.NpgsqlDbType.Text));

                // Prepare the parametized statement
                sqlCmd.Prepare();


                // Parameters value
                sqlCmd.Parameters["id"].Value = animal.Id;
                sqlCmd.Parameters["name"].Value = animal.Name;
                sqlCmd.Parameters["type"].Value = animal.Type;
                sqlCmd.Parameters["gender"].Value = animal.Gender;
                sqlCmd.Parameters["dateBirth"].Value = animal.BirthDate;
                sqlCmd.Parameters["dateDeath"].Value = animal.DeathDate != null ? animal.DeathDate : DBNull.Value;
                sqlCmd.Parameters["isSterilized"].Value = animal.IsSterilized;
                sqlCmd.Parameters["dateSterilization"].Value = animal.DateSterilization != null ? animal.DateSterilization : DBNull.Value;
                sqlCmd.Parameters["particularity"].Value = animal.Particularity != null ? animal.Particularity : DBNull.Value;
                sqlCmd.Parameters["description"].Value = animal.Description != null ? animal.Description : DBNull.Value;

                // Parametized statement execution
                int nbRowAffected = sqlCmd.ExecuteNonQuery();

                if (nbRowAffected == 0) throw new AccessDbException(sqlCmd.CommandText, $"Unable to update animal with id {animal.Id}! No row affected!");

                result = GetAnimalById(animal.Id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AnimalDataService : Error while updating an animal with name {animal.Name} in DB!\nException :\n{ex.Message}\nException:\n{ex}");

                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, ex.Message);
                else
                    throw new AccessDbException("Error while creating the SQL command instance!", ex.Message);

                
            }

            if (result == null) throw new Exception($"Unknown error while updating animal with name {animal.Name}.");

            return result!;
        }

        /**
         * <summary>
         *  Ajouter une compatibilité
         * </summary>
         */
        public bool CreateCompatibility(Compatibility compatibility, NpgsqlTransaction? transaction = null) {
            bool result = false;
            NpgsqlCommand? sqlCmd = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                     """
                     SELECT * FROM create_compatibility(:id, :type);
                     """,
                     this.SqlConn,
                     transaction
                );

                sqlCmd.Parameters.Add(new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Uuid));
                sqlCmd.Parameters.Add(new NpgsqlParameter("type", NpgsqlTypes.NpgsqlDbType.Text));

                sqlCmd.Prepare();

                sqlCmd.Parameters["id"].Value = compatibility.Id;
                sqlCmd.Parameters["type"].Value = compatibility.Type;

                int nbRowAffected = sqlCmd.ExecuteNonQuery();

                if (nbRowAffected > 0) {
                    throw new AccessDbException(sqlCmd.CommandText, $"Unable to create a compatibility instance with type name : {compatibility.Type}");
                }

                result = true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to create a compatibility instance with type name : {compatibility.Type}.\nException Message: {ex.Message}.\nException : {ex}");
                
                if(transaction != null) transaction.Rollback();
            }

            return result;
        }

        /**
         * <summary>
         *  Ajouter une compatibilité pour un animal
         * </summary>
         */
        public bool CreateAnimalCompatibility(AnimalCompatibility animalCompatibility)
        {
            bool result = false;
            NpgsqlCommand? sqlCmd = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    $"""
                    SELECT * FROM create_animal_compatibility( :id, :value, :description, :animalId, :compatibilityId );
                    """,
                    this.SqlConn
                );

                sqlCmd.Parameters.Add(new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Uuid));
                sqlCmd.Parameters.Add(new NpgsqlParameter("value", NpgsqlTypes.NpgsqlDbType.Boolean));
                sqlCmd.Parameters.Add(new NpgsqlParameter("description", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("animalId", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("compatibilityId", NpgsqlTypes.NpgsqlDbType.Uuid));

                sqlCmd.Prepare();

                sqlCmd.Parameters["id"].Value = animalCompatibility.Id;
                sqlCmd.Parameters["value"].Value = animalCompatibility.Value != null ? animalCompatibility.Value : DBNull.Value;
                sqlCmd.Parameters["description"].Value = animalCompatibility.Description != null ? animalCompatibility.Description : DBNull.Value;
                sqlCmd.Parameters["animalId"].Value = animalCompatibility.AnimalId;
                sqlCmd.Parameters["compatibilityId"].Value = animalCompatibility.CompatibilityId;


                int nbRowAffected = sqlCmd.ExecuteNonQuery();

                if (nbRowAffected == 0) throw new AccessDbException(sqlCmd.CommandText, $"Unable to create a compatibility with an animal in Db.\nObject\n{animalCompatibility}");

                result = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to create a compatibility with an animal.\nReason : {ex.Message}.\nObject :\n{animalCompatibility}\nException:\n{ex}");
                throw;
            }

            return result;
        }

        /**
         * <summary>
         *  Récupérer la liste des compatibilités
         * </summary>
         */
        public HashSet<Compatibility> GetCompatibilities()
        {
            HashSet<Compatibility> result = new HashSet<Compatibility>();
            NpgsqlCommand? sqlCmd = null;
            NpgsqlDataReader? reader = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    """
                    SELECT * FROM get_compatibilities();
                    """,
                    this.SqlConn
                );

                sqlCmd.Prepare();

                reader = sqlCmd.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(new Compatibility(
                        new Guid(Convert.ToString(reader["Id"])!),
                        Convert.ToString(reader["Type"])!
                    )); 
                }

                // Close the reader 
                reader.Close();

            }
            catch (Exception ex)
            {
                if (reader != null) reader.Close();

                if (Debugger.IsAttached)
                    Debug.WriteLine($"Unable to retrieve compatibilities instance! Message: {ex.Message}.\nException : {ex}");

                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, $"Unable to retrieve compatibilities instance! Message: {ex.Message}.\nException : {ex}");
                else
                    throw new AccessDbException("SqlCommand is null!", $"Unable to retrieve compatibilities instance! Message: {ex.Message}.\nException : {ex}");
            }

            return result;

        }

        /**
         * <summary>
         *  Récupérer la liste des couleurs des animaux
         * </summary>
         */
        public HashSet<Color> GetColors()
        {
            HashSet<Color> colors = new HashSet<Color>();
            NpgsqlCommand? sqlCmd = null;
            NpgsqlDataReader? reader = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    """
                    SELECT * FROM get_colors();
                    """,
                    this.SqlConn
                );

                sqlCmd.Prepare();

                reader = sqlCmd.ExecuteReader();

                while (reader.Read()) {
                    colors.Add(new Color(
                        new Guid(Convert.ToString(reader["Id"])!),
                        Convert.ToString(reader["Name"])!
                    ));
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                if(reader != null) reader.Close();

                if (Debugger.IsAttached)
                    Debug.WriteLine($"Error while retrieving animal's colors in database. Message : {ex.Message}. Error : {ex} ");
                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, $"Error while retrieving animal's colors in database. Message : {ex.Message}. Error : {ex} ");
                else
                    throw new AccessDbException("SQL Command is null", $"Error while retrieving animal's colors in database. Message : {ex.Message}. Error : {ex} ");
            }

            return colors;
        }

        /**
         * <summary>
         *  Ajouter une couleur à un animal 
         * </summary>
         */
        public bool CreateAnimalColor(AnimalColor animalColor, NpgsqlTransaction? transaction = null)
        {
            bool result = false;
            NpgsqlCommand? sqlCmd = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    """
                    SELECT * FROM create_animal_color(:id, :animalId, :colorId);
                    """,
                    this.SqlConn,
                    transaction
                );

                sqlCmd.Parameters.Add(new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Uuid));
                sqlCmd.Parameters.Add(new NpgsqlParameter("animalId", NpgsqlTypes.NpgsqlDbType.Varchar));
                sqlCmd.Parameters.Add(new NpgsqlParameter("colorId", NpgsqlTypes.NpgsqlDbType.Uuid));


                sqlCmd.Prepare();

                sqlCmd.Parameters["id"].Value = animalColor.Id;
                sqlCmd.Parameters["animalId"].Value = animalColor.AnimalId;
                sqlCmd.Parameters["colorId"].Value = animalColor.ColorId;

                int nbRowAffected = sqlCmd.ExecuteNonQuery();

                if(nbRowAffected == 0)
                    throw new AccessDbException(sqlCmd.CommandText, $"Error while creating animal's colors in database.");

                result = true;


            }
            catch (Exception ex)
            {

                if (Debugger.IsAttached)
                    Debug.WriteLine($"Error while creating animal's colors in database. Message : {ex.Message}. Error : {ex} ");

                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, $"Error while creating animal's colors in database. Message : {ex.Message}. Error : {ex} ");
                else
                    throw new AccessDbException("SQL Command is null", $"Error while creating animal's colors in database. Message : {ex.Message}. Error : {ex} ");
            }


            return result;
        }


    }
}
