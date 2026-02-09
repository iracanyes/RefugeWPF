using Microsoft.Extensions.Logging;
using Npgsql;
using RefugeWPF.CoucheMetiers.Exceptions;
using RefugeWPF.CoucheMetiers.Model.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace RefugeWPF.CoucheAccesDB
{
    internal class ContactDataService : AccessDb, IContactDataService
    {
        private static readonly ILogger MyLogger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(nameof(AnimalDataService));
        public ContactDataService()
            : base()
        { }

        /**
         * <summary>
         *  Ajouter une adresse
         * </summary>
         */
        public bool CreateAddress(Address address, NpgsqlTransaction? transaction = null)
        {
            bool result = false;
            NpgsqlCommand? sqlCmd = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    """"
                    SELECT * FROM create_address(:id, :street, :city, :state, :zipCode, :country);
                    """",
                    this.SqlConn

                );

                sqlCmd.Transaction = transaction;

                sqlCmd.Parameters.Add(new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Uuid));
                sqlCmd.Parameters.Add(new NpgsqlParameter("street", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("city", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("state", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("zipCode", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("country", NpgsqlTypes.NpgsqlDbType.Text));

                sqlCmd.Prepare();

                sqlCmd.Parameters["id"].Value = address.Id;
                sqlCmd.Parameters["street"].Value = address.Street;
                sqlCmd.Parameters["city"].Value = address.City;
                sqlCmd.Parameters["state"].Value = address.State;
                sqlCmd.Parameters["zipCode"].Value = address.ZipCode;
                sqlCmd.Parameters["country"].Value = address.Country;

                int nbRowAffected = sqlCmd.ExecuteNonQuery();

                if (nbRowAffected == 0) throw new AccessDbException(sqlCmd.CommandText, $"Unable to create an address instance. Object : \n{address}");

                result = true;

            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        /**
         * <summary>
         *  Ajouter un rôle à une personne de contact
         * </summary>
         */
        public bool CreateContactRole(ContactRole contactRole, NpgsqlTransaction transaction)
        {
            bool result = false;
            NpgsqlCommand? sqlCmd = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    """
                    SELECT * FROM create_contact_role(:id, :contactId, :roleId);
                    """,
                    this.SqlConn,
                    transaction
                );

                sqlCmd.Parameters.Add(new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Uuid));
                sqlCmd.Parameters.Add(new NpgsqlParameter("contactId", NpgsqlTypes.NpgsqlDbType.Uuid));
                sqlCmd.Parameters.Add(new NpgsqlParameter("roleId", NpgsqlTypes.NpgsqlDbType.Uuid));


                sqlCmd.Prepare();

                sqlCmd.Parameters["id"].Value = contactRole.Id;
                sqlCmd.Parameters["contactId"].Value = contactRole.ContactId;
                sqlCmd.Parameters["roleId"].Value = contactRole.RoleId;

                int nbRowAffected = sqlCmd.ExecuteNonQuery();

                if (nbRowAffected == 0) throw new Exception($"Unable to create an ContactRole instance with data : {contactRole}.");

                result = true;

            }
            catch (Exception ex)
            {
                if(transaction != null)
                    transaction.Rollback();

                if (Debugger.IsAttached)
                    Debug.WriteLine($"Unable to create a ContactRole instance. \nMessage :\n {ex.Message}.\nException :\n{ex}");
                
                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, $"Unable to create a ContactRole instance. \nMessage :\n {ex.Message}.\nException :\n{ex}");
                else
                    throw new AccessDbException("Unable to create an sqlCommand for inserting a ContactRole instance", $"Unable to insert a ContactRole instance. \nMessage :\n {ex.Message}.\nException :\n{ex}");
            }

            return result;
            
        }

        /**
         * <summary>
         *  Gère l'ajout d'une personne de contact
         * </summary>
         */
        public Contact HandleCreateContact(Contact contact)
        {
            NpgsqlTransaction transaction = this.SqlConn.BeginTransaction();
            Contact? result = null;

            try
            {
                this.CreateAddress(contact.Address, transaction);

                result = this.CreateContact(contact, transaction);

                foreach(ContactRole cr in contact.ContactRoles)
                {
                    this.CreateContactRole(cr, transaction);
                }


                transaction.Commit();
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();

                Debug.WriteLine($"Erreur lors de la création d'une personne de contact.\nMessage : {ex.Message}");
                throw;
            }

            return result;
        }

        /**
         * <summary>
         *  Ajoute une personne de contact
         * </summary>
         */
        public Contact CreateContact(Contact contact, NpgsqlTransaction? transaction)
        {
            Contact? result = null;
            NpgsqlCommand? sqlCmd = null;

            try
            {
                               

                sqlCmd = new NpgsqlCommand(
                    """
                    SELECT * FROM create_contact(:id, :firstname, :lastname, :registryNumber, :email, :phoneNumber, :mobileNumber, :addressId);
                    """,
                    this.SqlConn,
                    transaction
                );

                sqlCmd.Parameters.Add(new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Uuid));
                sqlCmd.Parameters.Add(new NpgsqlParameter("firstname", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("lastname", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("registryNumber", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("email", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("phoneNumber", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("mobileNumber", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("addressId", NpgsqlTypes.NpgsqlDbType.Uuid));

                sqlCmd.Prepare();

                sqlCmd.Parameters["id"].Value = contact.Id;
                sqlCmd.Parameters["firstname"].Value = contact.Firstname;
                sqlCmd.Parameters["lastname"].Value = contact.Lastname;
                sqlCmd.Parameters["registryNumber"].Value = contact.RegistryNumber;
                sqlCmd.Parameters["email"].Value = contact.Email != null ? contact.Email : DBNull.Value;
                sqlCmd.Parameters["phoneNumber"].Value = contact.PhoneNumber != null ? contact.PhoneNumber : DBNull.Value;
                sqlCmd.Parameters["mobileNumber"].Value = contact.MobileNumber != null ? contact.MobileNumber : DBNull.Value;
                sqlCmd.Parameters["addressId"].Value = contact.AddressId;

                int nbRowAffected = sqlCmd.ExecuteNonQuery();

                if(nbRowAffected == 0) throw new AccessDbException(sqlCmd.CommandText, $"Unable to create a Contact instance in DB!\nObject:\n{contact}");


               

                result = this.GetContactByRegistryNumber(contact.RegistryNumber);
            }
            catch (Exception ex)
            {
                if(Debugger.IsAttached)
                    Debug.WriteLine($"Unable to create a contact info instance.\nObject : \n{contact}\nReason : \n{ex.Message}\nException : \n{ex}");
                
                if(transaction != null && transaction.Connection != null) transaction.Rollback();

                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, $"Unable to create a Contact instance. \nError: {ex.Message}.\nException: {ex}");
                else
                    throw new AccessDbException("SqlCommand is null", $"Unable to create a Contact instance. \nError: {ex.Message}.\nException: {ex}.");


            }

            if (result == null) throw new Exception($"Unable to create a contact info instance.\nObject : \n{contact}");

            return result!;

        }

        /**
         * <summary>
         *  Récupére la liste des personnes de contact
         * </summary>
         */
        public List<Contact> GetContacts()
        {
            List<Contact> result = new List<Contact>();
            NpgsqlCommand? sqlCmd = null;
            NpgsqlDataReader? reader = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    $"""
                    SELECT * FROM get_contacts();
                    """,
                    this.SqlConn
                );

                
                sqlCmd.Prepare();


                reader = sqlCmd.ExecuteReader();

                while (reader.Read())
                {
                    Address address = new Address(
                        new Guid(Convert.ToString(reader["AddressId"])!),
                        Convert.ToString(reader["Street"])!,
                        Convert.ToString(reader["City"])!,
                        Convert.ToString(reader["State"])!,
                        Convert.ToString(reader["ZipCode"])!,
                        Convert.ToString(reader["Country"])!
                    );

                    result.Add( new Contact(
                        new Guid(Convert.ToString(reader["Id"])!),
                        Convert.ToString(reader["Firstname"])!,
                        Convert.ToString(reader["Lastname"])!,
                        Convert.ToString(reader["RegistryNumber"])!,
                        Convert.ToString(reader["Email"])!,
                        Convert.ToString(reader["PhoneNumber"])!,
                        Convert.ToString(reader["MobileNumber"])!,
                        address
                    ));


                }

                reader.Close();

                // Attach contact's roles to contact
                foreach(Contact co in result)
                    this.GetContactRoles(co);


            }
            catch (Exception ex)
            {
                reader?.Close();

                Debug.WriteLine($"Unable to create a contact info instance.\nReason : \n{ex.Message}\nException : \n{ex}");
                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, ex.Message);
                else
                    throw new AccessDbException("sqlCmd is null", $"Unable to create a contact info instance. Message : {ex.Message}\nException : {ex}");

            }

            return result;
        }

        /**
         * <summary>
         *  Récupérer une personne de contact par son identifiant
         * </summary>
         */
        public Contact GetContactById(Guid id)
        {
            Contact? result = null;
            NpgsqlCommand? sqlCmd = null;
            NpgsqlDataReader? reader = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    $"""
                    SELECT * FROM get_contact_by_id(:id);
                    """,
                    this.SqlConn
                );

                sqlCmd.Parameters.Add(new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Uuid));

                sqlCmd.Prepare();

                sqlCmd.Parameters["id"].Value = id;

                reader = sqlCmd.ExecuteReader();

                if (reader.Read())
                {

                    Address address = new Address(
                        new Guid(Convert.ToString(reader["Id"])!),
                        Convert.ToString(reader["Street"])!,
                        Convert.ToString(reader["City"])!,
                        Convert.ToString(reader["State"])!,
                        Convert.ToString(reader["ZipCode"])!,
                        Convert.ToString(reader["Country"])!
                    );

                    result = new Contact(
                        new Guid(Convert.ToString(reader["ContactId"])!),
                        Convert.ToString(reader["Fistname"])!,
                        Convert.ToString(reader["Lastname"])!,
                        Convert.ToString(reader["RegistryNumber"])!,
                        Convert.ToString(reader["Email"])!,
                        Convert.ToString(reader["PhoneNumber"])!,
                        Convert.ToString(reader["MobileNumber"])!,
                        address
                    );
                }

                reader.Close();
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Unable to retrieve a contact instance with ID : {id}.\nReason :\n{ex.Message}\nException : \n{ex}");

                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, ex.Message);
                else
                    throw new AccessDbException($"Unable to retrieve a contact instance with ID : {id}.", ex.Message);
            }

            if (result == null) throw new Exception($"Unable to retrieve a contact instance with ID: {id}");

            return result;

        }

        /**
         * <summary>
         *  Récupérer une personne de contact par son numéro de registre national
         * </summary>
         */
        public Contact GetContactByRegistryNumber(string registryNumber)
        {
            Contact? result = null;
            NpgsqlCommand? sqlCmd = null;
            NpgsqlDataReader? reader = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    $"""
                    SELECT * FROM get_contact_by_registry_number(:registryNumber);
                    """,
                    this.SqlConn
                );

                sqlCmd.Parameters.Add(new NpgsqlParameter("registryNumber", NpgsqlTypes.NpgsqlDbType.Text));

                sqlCmd.Prepare();

                sqlCmd.Parameters["registryNumber"].Value = registryNumber;

                reader = sqlCmd.ExecuteReader();

                if(reader.Read())
                {
                    Address address = new Address(
                        new Guid(Convert.ToString(reader["AddressId"])!),
                        Convert.ToString(reader["Street"])!,
                        Convert.ToString(reader["City"])!,
                        Convert.ToString(reader["State"])!,
                        Convert.ToString(reader["ZipCode"])!,
                        Convert.ToString(reader["Country"])!
                    );

                    result = new Contact(
                        new Guid(Convert.ToString(reader["Id"])!),
                        Convert.ToString(reader["Firstname"])!,
                        Convert.ToString(reader["Lastname"])!,
                        Convert.ToString(reader["RegistryNumber"])!,                        
                        Convert.ToString(reader["Email"])!,
                        Convert.ToString(reader["PhoneNumber"])!,
                        Convert.ToString(reader["MobileNumber"])!,
                        address
                    );

                    
                }

                reader.Close();

                // Attach contact's roles to contact
                this.GetContactRoles(result!);


            }
            catch (Exception ex)
            {
                reader?.Close();

                Debug.WriteLine($"Unable to create a contact info instance.\nRegistryNumber : \n{registryNumber}\nReason : \n{ex.Message}\nException : \n{ex}");
                if(sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, ex.Message);
                else 
                    throw new AccessDbException($"Unable to create a contact info instance.\nRegistryNumber : \n{registryNumber}", ex.Message);

            }

            if (result == null) throw new Exception($"Unable to retrieve a contact info instance with registry number : {registryNumber}");

            return result;
        }


        /**
         * <summary>
         *  Mettre à jour une adresse
         * </summary>
         */
        public bool UpdateAddress(Address address, NpgsqlTransaction? transaction = null)
        {
            bool result = false;
            NpgsqlCommand? sqlCmd = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    """
                    SELECT * FROM update_address(:id, :street, :city,  :state, :zipCode, :country);
                    """,
                    this.SqlConn,
                    transaction
                );

                sqlCmd.Parameters.Add(new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Uuid));
                sqlCmd.Parameters.Add(new NpgsqlParameter("street", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("city", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("state", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("zipCode", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("country", NpgsqlTypes.NpgsqlDbType.Text));

                sqlCmd.Prepare();

                sqlCmd.Parameters["id"].Value = address.Id;
                sqlCmd.Parameters["street"].Value = address.Street;
                sqlCmd.Parameters["city"].Value = address.City;
                sqlCmd.Parameters["state"].Value = address.State;
                sqlCmd.Parameters["zipCode"].Value = address.ZipCode;
                sqlCmd.Parameters["country"].Value = address.Country;


                int nbRowAffected = sqlCmd.ExecuteNonQuery();

                if (nbRowAffected == 0) throw new AccessDbException(sqlCmd.CommandText, $"Unknown error while updating an address instance with info.");

                result = true;


            }
            catch (Exception ex)
            {                

                if(Debugger.IsAttached)
                    Debug.WriteLine($"Unable to update an address instance.\n Exception message: {ex.Message}.\nException : {ex}");

                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, $"Unable to update an address instance.\n Exception message: {ex.Message}.\nException : {ex}");
                else
                    throw new AccessDbException("SqlCommand - UpdateAddress", $"Unable to update an address instance.\n Exception message: {ex.Message}.\nException : {ex}");
            }


            return result;
        }

        /**
         * <summary>
         *  Mettre à jour une personne de contact
         * </summary>
         */
        public Contact UpdateContact(Contact contact, NpgsqlTransaction? transaction = null) {
            Contact? result = null;
            NpgsqlCommand? sqlCmd = null;

            try
            {
                 

                sqlCmd = new NpgsqlCommand(
                    $"""
                    SELECT * FROM update_contact(:id, :firstname, :lastname, :registryNumber, :email, :phoneNumber, :mobileNumber, :addressId);
                    """,
                    this.SqlConn,
                    transaction
                );

                sqlCmd.Parameters.Add(new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Uuid));
                sqlCmd.Parameters.Add(new NpgsqlParameter("firstname", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("lastname", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("registryNumber", NpgsqlTypes.NpgsqlDbType.Text));;
                sqlCmd.Parameters.Add(new NpgsqlParameter("email", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("phoneNumber", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("mobileNumber", NpgsqlTypes.NpgsqlDbType.Text));
                sqlCmd.Parameters.Add(new NpgsqlParameter("addressId", NpgsqlTypes.NpgsqlDbType.Uuid));


                sqlCmd.Prepare();

                sqlCmd.Parameters["id"].Value = contact.Id;
                sqlCmd.Parameters["firstname"].Value = contact.Firstname;
                sqlCmd.Parameters["lastname"].Value = contact.Lastname;
                sqlCmd.Parameters["registryNumber"].Value = contact.RegistryNumber;
                sqlCmd.Parameters["email"].Value = contact.Email != null ? contact.Email : DBNull.Value;
                sqlCmd.Parameters["phoneNumber"].Value = contact.PhoneNumber != null ? contact.PhoneNumber : DBNull.Value;
                sqlCmd.Parameters["mobileNumber"].Value = contact.MobileNumber != null ? contact.MobileNumber : DBNull.Value;
                sqlCmd.Parameters["addressId"].Value = contact.AddressId;

                int nbRowAffected = sqlCmd.ExecuteNonQuery();

                if (nbRowAffected == 0) throw new AccessDbException(sqlCmd.CommandText, $"Unable to update a Contact instance in DB!\nObject:\n{contact}");

                // Commit transaction
                if(transaction != null)
                    transaction.Commit();

                result = this.GetContactByRegistryNumber(contact.RegistryNumber);


            }
            catch (Exception ex)
            {
                if(transaction != null && transaction.Connection != null) transaction.Rollback();

                if(Debugger.IsAttached)
                    Debug.WriteLine($"Unable to update a contact instance with registry number : {contact.RegistryNumber}.\nReason :\n{ex.Message}\nException : \n{ex}");

                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, $"Unable to update a contact instance with registry number : {contact.RegistryNumber}.\nReason :\n{ex.Message}\nException : \n{ex}");
                else
                    throw new AccessDbException("sqlCmd is null", $"Unable to update a contact instance with registry number : {contact.RegistryNumber}.\nReason :\n{ex.Message}\nException : \n{ex}");
            }

            if (result == null) throw new Exception($"Unable to update a contact instance with registry number : {contact.RegistryNumber}");

            return result;
        }

        /**
         * <summary>
         *  Gère la mise à jour d'une personne de contact
         * </summary>
         */
        public Contact HandleUpdateContact(Contact contact)
        {
            Contact? result = null;
            NpgsqlTransaction transaction = this.SqlConn.BeginTransaction();

            try
            {
                // First, update the address
                bool addressUpdated = this.UpdateAddress(contact.Address, transaction);

                if (!addressUpdated) throw new AccessDbException("sqlCmd - updateAddress", $"Unexpected error while updating the address with info {contact.Address}.");

                result = this.UpdateContact(contact, transaction);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors la mise à jour d'un contact.\nMessage : {ex.Message}.\nErreur : {ex} ");
                throw new AccessDbException("HandleUpdateContact", ex.Message);
            }

            return result;

        }

        /**
         * <summary>
         *  Supprimer un rôle d'une personne de contact
         * </summary>
         */
        public bool DeleteContactRole(ContactRole contactRole, NpgsqlTransaction? transaction = null)
        {
            bool result = false;
            NpgsqlCommand? sqlCmd = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    $"""
                    SELECT * FROM delete_contact_role(:id);
                    """,
                    this.SqlConn,
                    transaction
                );

                sqlCmd.Parameters.Add(new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Uuid));

                sqlCmd.Prepare();

                sqlCmd.Parameters["id"].Value = contactRole.Id;

                int nbRowAffected = sqlCmd.ExecuteNonQuery();

                if (nbRowAffected == 0) throw new AccessDbException(sqlCmd.CommandText, $"Error while deleting contact's role info : \n{contactRole.Id}");

                result = true;
            }
            catch (Exception ex)
            {
                if(Debugger.IsAttached)
                    Debug.WriteLine($"Unable to delete a contact's role instance : {contactRole.Id}.\nReason :\n{ex.Message}\nException : \n{ex}");

                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, $"Unable to delete a contact's role instance : {contactRole.Id}.\nReason :\n{ex.Message}\nException : \n{ex}");
                else
                    throw new AccessDbException($"DeleteContact : qlCmd is null", $"Unable to delete a contact's role instance : {contactRole.Id}.\nReason :\n{ex.Message}\nException : \n{ex}");
            }


            return result;
        }

        /**
         * <summary>
         *  Supprimer une personne de contact
         * </summary>
         */
        public bool DeleteContact(Contact contact)
        {
            bool result = false;
            NpgsqlCommand? sqlCmd = null;
            NpgsqlTransaction transaction = this.SqlConn.BeginTransaction();

            try
            {

                // Delete all contact's roles
                foreach (ContactRole contactRole in contact.ContactRoles)
                    this.DeleteContactRole(contactRole, transaction);

                sqlCmd = new NpgsqlCommand(
                    $"""
                    SELECT * FROM delete_contact(:id);
                    """,
                    this.SqlConn, 
                    transaction
                );

                sqlCmd.Parameters.Add(new NpgsqlParameter("id", NpgsqlTypes.NpgsqlDbType.Uuid));
                 
                sqlCmd.Prepare();

                sqlCmd.Parameters["id"].Value = contact.Id;

                int nbRowAffected = sqlCmd.ExecuteNonQuery();

                if (nbRowAffected == 0) throw new AccessDbException(sqlCmd.CommandText, $"Error while deleting contact info : \n{contact}");

                transaction.Commit();

                result = true;
            }
            catch (Exception ex)
            {
                if(transaction != null && transaction.Connection != null) transaction.Rollback();

                if(Debugger.IsAttached)
                    Debug.WriteLine($"Unable to delete a contact instance with registry number : {contact.RegistryNumber}.\nReason :\n{ex.Message}\nException : \n{ex}");

                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, $"Unable to delete a contact instance with registry number : {contact.RegistryNumber}.\nReason :\n{ex.Message}\nException : \n{ex}");
                else
                    throw new AccessDbException($"DeleteContact : sqlCmd is null", $"Unable to delete a contact instance with registry number : {contact.RegistryNumber}.\nReason :\n{ex.Message}\nException : \n{ex}");
            }
            

            return result;
        }

        /**
         * <summary>
         *  Récupérer la liste des rôles
         * </summary>
         */
        public HashSet<Role> GetRoles()
        {
            HashSet<Role> roles = new HashSet<Role>();
            NpgsqlCommand? sqlCmd = null;
            NpgsqlDataReader? reader = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    """
                    SELECT * FROM get_roles();
                    """,
                    this.SqlConn
                );

                sqlCmd.Prepare();

                reader = sqlCmd.ExecuteReader();

                while (reader.Read())
                {
                    roles.Add(new Role(
                       new Guid(Convert.ToString(reader["Id"])!),
                       Convert.ToString(reader["Name"])!
                    ));
                }

                reader.Close();

            }
            catch (Exception ex)
            {
                if (reader != null) reader.Close();

                if (Debugger.IsAttached)
                    Debug.WriteLine($"Error while retrieving roles from DB. Error : {ex.Message}. Exception: {ex}");

                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, $"Error while retrieving roles from DB. Error : {ex.Message}. Exception: {ex}");
                else
                    throw new AccessDbException("sqlCmd is NULL", $"Error while retrieving roles from DB. Error : {ex.Message}. Exception: {ex}");

            }

            return roles;

        }

        /**
         * <summary>
         *  Récupérer la liste des rôles d'une personne de contact
         * </summary>
         */
        public HashSet<ContactRole> GetContactRoles(Contact contact) { 
            NpgsqlCommand? sqlCmd = null;
            NpgsqlDataReader? reader = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    """
                    SELECT * FROM get_contact_roles(:contactId);                
                    """,
                    this.SqlConn
                );

                sqlCmd.Parameters.Add(new NpgsqlParameter("contactId", NpgsqlTypes.NpgsqlDbType.Uuid));

                sqlCmd.Prepare();

                sqlCmd.Parameters["contactId"].Value = contact.Id;

                reader = sqlCmd.ExecuteReader();

                while (reader.Read())
                {
                    Role role = new Role(new Guid(Convert.ToString(reader["RoleId"])!), Convert.ToString(reader["RoleName"])!);

                    contact.ContactRoles.Add(  new ContactRole(new Guid(Convert.ToString(reader["Id"])!), contact, role) );
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);

                if (reader != null) reader.Close();

                if (Debugger.IsAttached)
                    Debug.WriteLine($"Error while retrieving contact's roles from DB. Error : {ex.Message}. Exception: {ex}");

                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, $"Error while retrieving contact's roles from DB. Error : {ex.Message}. Exception: {ex}");
                else
                    throw new AccessDbException("sqlCmd is NULL", $"Error while retrieving contact's roles from DB. Error : {ex.Message}. Exception: {ex}");
            }

            return contact.ContactRoles;

        }

        /**
         * <summary>
         *  Vérifie si un numéro de registre national existe
         * </summary>
         */
        public bool RegistryNumberExists(string registryNumber)
        {
            bool result = false;
            NpgsqlCommand? sqlCmd = null;
            NpgsqlDataReader? reader = null;

            try
            {
                sqlCmd = new NpgsqlCommand(
                    """
                    SELECT EXISTS(
                        SELECT 1
                        FROM public."Contacts"
                        WHERE "RegistryNumber" = :registryNumber
                        LIMIT 1
                    )
                    """,
                    this.SqlConn
                );

                sqlCmd.Parameters.Add(new NpgsqlParameter("registryNumber", NpgsqlTypes.NpgsqlDbType.Text));

                sqlCmd.Prepare();

                sqlCmd.Parameters["registryNumber"].Value = registryNumber;

                reader = sqlCmd.ExecuteReader();

                if (reader.Read())
                {
                    result = Convert.ToBoolean(reader["myExists"]);
                }

                // Close reader 
                reader.Close();


            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);

                if (reader != null) reader.Close();

                if (Debugger.IsAttached)
                    Debug.WriteLine($"Error while retrieving contact's roles from DB. Error : {ex.Message}. Exception: {ex}");

                if (sqlCmd != null)
                    throw new AccessDbException(sqlCmd.CommandText, $"Error while retrieving contact's roles from DB. Error : {ex.Message}. Exception: {ex}");
                else
                    throw new AccessDbException("sqlCmd is NULL", $"Error while retrieving contact's roles from DB. Error : {ex.Message}. Exception: {ex}");
            }

            return result;
        }



    }
}
