using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WebApplication1.Models
{
    public class database
    {
        //  https://www.connectionstrings.com/sql-server/
        public string ConString = @"Server= server_name ;Database= database_name ;Trusted_Connection=True; integrated security=SSPI";

        /// <summary>
        /// inorder to create connection we need to give connection string 
        /// </summary>
        public SqlConnection SqlConnection = new SqlConnection(@"Server= server_name ;Database= database_name ;Trusted_Connection=True; integrated security=SSPI");


        /// <summary>
        /// this is from webconfig web files 
        /// </summary>
        string WebConfigConnectionString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;


        /// <summary>
        ///- this is for returnted type strings such as select statement in sql.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public DataSet command_ExecuteReader(string sql, SqlConnection connection)
        {
            DataSet ds = new DataSet();

            using (connection)
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandTimeout = 600;
            
                try
                {
                    connection.Open();

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                    dataAdapter.Fill(ds);
                }
                catch (Exception)
                {
                    ds = null;
                }
                finally{
                    connection.close();
                }
              

            }

            return ds;
        }


        /// <summary>
        ///this is for non queries such as insert, update, delete this entries does not have a return types
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool command_ExecuteNonQuery(string sql)
        {
            bool control = false;
            try
            {

                if (command_open_Connection())
                {
                    //System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();


                    control = true;

                    using (var connection1 = new SqlConnection(ConString))
                    using (var cmd = new SqlDataAdapter())
                    using (var insertCommand = new SqlCommand(sql))
                    {
                        insertCommand.Connection = connection1;
                        cmd.InsertCommand = insertCommand;

                        connection1.Open();
                        insertCommand.ExecuteNonQuery();

                    }

                }

            }
            catch (Exception ex)
            {
                control = false;
            }
            finally
            {
                command_close_Connection();
            }

         

            return control;
        }



        /// <summary>
        /// this function is for creating a connection between msql server and this project
        /// </summary>
        /// <returns></returns>
        private bool command_open_Connection()
        {
            bool connection_control = false;

            using (SqlConnection connection = new SqlConnection(ConString))
            {
                try
                {
                    connection.Open();
                    connection_control = true;
                }
                catch (Exception)
                {
                    connection_control = false;
                }
            }
            return connection_control;
        }


        /// <summary>
        /// this function is for closing a connection between msql server and this project
        /// </summary>
        private void command_close_Connection()
        {

            using (SqlConnection connection = new SqlConnection(ConString))
            {
                connection.Close();
            }
        }

        /// <summary>
        /// just to know is there a connection or not 
        /// </summary>
        public bool Is_there_a_connection()
        {
            bool bool_cool = false;
            SqlConnection con = null;
            try
            {
                // Creating Connection  
                string ConnectionString = ConString;
                con = new SqlConnection(ConnectionString);
                con.Open();
                Console.WriteLine("Connection Established Successfully");
                bool_cool = true;
            }
            catch (Exception e)
            {
                bool_cool = false;
                Console.WriteLine("OOPs, something went wrong.\n" + e);
            }
            finally
            {   // Closing the connection  
                con.Close();
            }

            return bool_cool;
        }


        /// <summary>
        /// Use this function when you want to know total rows inside the database
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public int command_ExecuteScalar(string sql,SqlConnection connection)
        {
            int TotalRows = 0;
            try
            {
                using (connection)
                {
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    
                    connection.Open();

                    TotalRows = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("OOPs, something went wrong.\n" + e);
            }
            finally
            {
                connection.Close();
            }


            return TotalRows;
        }


        /// <summary>
        /// use this function inorder to read sp to dataset
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public DataSet command_read_sp(string sql,SqlConnection connection)
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (connection)
                {

                    SqlDataAdapter da = new SqlDataAdapter(sql, connection);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    
                    da.Fill(dataSet);
                   
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("OOPs, something went wrong.\n" + e);
            }

            return dataSet;

        }


        /// <summary>
        /// Use this command when you want to insert with sp and unknown parameter size 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="procedureName"></param>
        /// <param name="paramterList"></param>
        /// <returns></returns>
        public static DataSet ExecuteStoredProcedureReturnDataSet(string connectionString, string procedureName, params SqlParameter[] paramterList)
        {
            //Create DataSet Object
            DataSet dataSet = new DataSet();
            //Create the connection object using the connectionString parameter which it received as input parameter
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                //Create the command object
                using (var command = sqlConnection.CreateCommand())
                {
                    //Create the SqlDataAdapter object by passing command object as a parameter to the constructor
                    using (SqlDataAdapter sda = new SqlDataAdapter(command))
                    {
                        //Set the command type as StoredProcedure
                        command.CommandType = CommandType.StoredProcedure;
                        //Set the command text as the procedure name which you received as input parameter
                        command.CommandText = procedureName;
                        //If Parameter list is not null, add the parameterlist into Parameters collection of the command object
                        if (paramterList != null)
                        {
                            command.Parameters.AddRange(paramterList);
                        }

                        //Fill the Dataset
                        sda.Fill(dataSet);
                    }
                }
            }
            //Return the DataSet
            return dataSet;
        }


        /// <summary>
        /// Use this command when you are going to use more than one sql query in a string
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static bool command_multiple_ExecuteNonQuery_with_transaction(string sql, SqlConnection connection)
        {
            bool query = false;
            using (connection)
            {
                //Open the connection
                //The connection needs to be open before we begin a transaction
                connection.Open();

                // Create the transaction object by calling the BeginTransaction method on connection object
                SqlTransaction transaction = connection.BeginTransaction();
                
                try
                {
                    // Associate the first update command with the transaction
                    SqlCommand cmd = new SqlCommand(sql, connection, transaction);
                    cmd.Connection = connection;
                    //Execute the First Update Command
                    cmd.ExecuteNonQuery();

                    // If everythinhg goes well then commit the transaction
                    transaction.Commit();
                    Console.WriteLine("Transaction Committed");
                    query = true;
                }
                catch (Exception EX)
                {
                    // If anything goes wrong, then Rollback the transaction
                    transaction.Rollback();
                    query = false;
                    Console.WriteLine("Transaction Rollback "+EX);
                }
                finally
                {
                    connection.Close();
                }

                return query;
            }
        }

        //// https://dotnettutorials.net/lesson/what-is-ado-net/ 
        /// this link was very helpfull
    }
}