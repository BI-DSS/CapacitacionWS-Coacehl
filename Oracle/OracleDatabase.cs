using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace CSFDataAccess.General
{
    public class OracleDatabase : IDisposable
    {
        private OracleConnection _connection;

        // public object ConfigurationManager { get; private set; }

        /// <summary>
        /// Default constructor which uses the "DefaultConnection" connectionString
        /// </summary>
        public OracleDatabase()
            : this("OracleConect")
        {
        }

        /// <summary>
        /// Constructor which takes the connection string name
        /// </summary>
        /// <param name="connectionStringName"></param>
        public OracleDatabase(string connectionStringName)
        {
            var connectionString = connectionStringName;
            if (connectionStringName.Equals("DefaultConnection") || connectionStringName.Equals("OracleConect"))
            {
               
              connectionString=  ConfigurationSettings.AppSettings.Get("CSFConnection");
               // connectionString= ConfigurationManager.ConnectionStrings["DigiBrd"].ConnectionString;
                //connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
              //  connectionString = "Data Source=(DESCRIPTION =(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST = 10.10.6.11)(PORT = 1521)))(CONNECT_DATA =(SERVICE_NAME = devcsf.sagradafamilia.coop)));User ID=FETBI;Password=Fet2018CoopsafaBi;";

                //"SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.10.6.11)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=pruebacsf.sagradafamilia.coop))); uid = FETBI; pwd = Fet2018CoopsafaBi; ";// desencriptar(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString, "Coops4f4Inform4tic4Oficin4Princi").Replace("\0", String.Empty);
                //connectionString= "User Id=FETBI; Password =Fet2018CoopsafaBi; Data Source=10.10.6.11; ";

            }


            _connection = new OracleConnection(connectionString);
        }

        /// <summary>
        /// Executes a non-query Oracle statement
        /// </summary>
        /// <param name="commandText">The Oracle query to execute</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <returns>The count of records affected by the Oracle statement</returns>
        public int Execute(string commandText, IEnumerable parameters)
        {
            int result;

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                command.CommandType = CommandType.StoredProcedure;
                result = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                FuncionesApoyo.LogEnArchivo(ex);
                result = -1;
            }
            finally
            {
                _connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Executes a Oracle query that returns a single scalar value as the result.
        /// </summary>
        /// <param name="commandText">The Oracle query to execute</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <returns></returns>
        public object QueryValue(string commandText, IEnumerable parameters)
        {
            object result;

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
                result = command.Parameters["PA_CODIGO_ERROR"].Value;
            }
            catch (Exception ex)
            {
                FuncionesApoyo.LogEnArchivo(ex);
                result = null;
            }
            finally
            {
                EnsureConnectionClosed();
            }

            return result;
        }


        /// <summary>
        /// Executes a Oracle query that returns a single scalar value as the result.
        /// </summary>
        /// <param name="commandText">The Oracle query to execute</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <returns></returns>
        public object QueryValue_MCA_K_AHORROS(string commandText, IEnumerable parameters)
        {
            object result;

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
                result = command.Parameters["GV_CODIGOERROR"].Value;
            }
            catch (Exception ex)
            {
                FuncionesApoyo.LogEnArchivo(ex);
                result = null;
            }
            finally
            {
                EnsureConnectionClosed();
            }

            return result;
        }


        /// <summary>
        /// Executes a Oracle query that returns a single scalar value as the result.
        /// </summary>
        /// <param name="commandText">The Oracle query to execute</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <returns></returns>
        public int QueryValueInt(string commandText, IEnumerable parameters)
        {
            int result = -200;

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();

                foreach (OracleParameter p in parameters)
                {
                    if (p.Direction == ParameterDirection.ReturnValue)
                    {
                        result = int.Parse(p.Value.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Data);
                FuncionesApoyo.LogEnArchivo(ex);
                return result;
            }
            finally
            {
                EnsureConnectionClosed();
            }

            return result;
        }


        /// <summary>
        /// Executes a Oracle query that returns a single scalar value as the result.
        /// </summary>
        /// <param name="commandText">The Oracle query to execute</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <returns></returns>
        public int ExecProcResult(string commandText, IEnumerable parameters)
        {
            int result = -200;
            string nameOutputParameter = "";

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                foreach (OracleParameter p in parameters)
                {
                    if (p.Direction == ParameterDirection.Output)
                    {
                        nameOutputParameter = p.ParameterName;
                    }
                }

                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
                result = int.Parse(command.Parameters[nameOutputParameter].Value.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Data);
                FuncionesApoyo.LogEnArchivo(ex);
            }
            finally
            {
                EnsureConnectionClosed();
            }

            return result;
        }

        /*Ejecuta un procedimiento que no retorb¿na valores */
        public bool ExecuteProc(string commandText, IEnumerable parameters)
        {
            bool result = false;
            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }
            try
            {

                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
                result = true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Data);
                FuncionesApoyo.LogEnArchivo(ex);
            }
            finally
            {
                EnsureConnectionClosed();
            }

            return result;

        }

        /// <summary>
        /// Executes a SQL query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The Oracle query to execute</param>
        /// <param name="parameters">Parameters to pass to the Oracle query</param>
        /// <returns>A list of a Dictionary of Key, values pairs representing the 
        /// ColumnName and corresponding value</returns>
        public List<Dictionary<string, string>> Query(string commandText, IEnumerable parameters)
        {
            List<Dictionary<string, string>> rows;
            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                command.CommandTimeout = 30;
                using (var reader = command.ExecuteReader())
                {
                    rows = new List<Dictionary<string, string>>();
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, string>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var columnName = reader.GetName(i);
                            var columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i).ToString();
                            row.Add(columnName, columnValue);
                        }
                        rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                FuncionesApoyo.LogEnArchivo(ex);
                rows = null;
            }
            finally
            {
                EnsureConnectionClosed();
            }

            return rows;
        }

        /// <summary>
        /// Obtiene un fichero
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public byte[] QueryFileData(string commandText, IEnumerable parameters)
        {
            //byte[] result = new byte[100];
            byte[] result = null;
            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                command.CommandTimeout = 30;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            result = (byte[])reader.GetValue(0);
                        }
                        else
                        {
                            result = null;
                        }
                    }
                }
            }
            catch (Exception)
            {
                result = null;
            }
            finally
            {
                EnsureConnectionClosed();
            }

            return result;
        }

        /// <summary>
        /// Executes a SQL query that returns a list of rows as the result (especial form).
        /// </summary>
        /// <param name="commandText">The Oracle query to execute</param>
        /// <param name="parameters">Parameters to pass to the Oracle query</param>
        /// <returns>A list of a Dictionary of Key, values pairs representing the 
        /// ColumnName and corresponding value</returns>
        public List<Dictionary<string, string>> QueryEspecial(string commandText, IEnumerable parameters)
        {
            List<Dictionary<string, string>> rows;
            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                using (var reader = command.ExecuteReader())
                {
                    rows = new List<Dictionary<string, string>>();
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, string>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var columnName = reader.GetName(i);
                            var columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i).ToString();
                            row.Add(columnName, columnValue);
                        }
                        rows.Add(row);
                    }
                }
            }
            finally
            {
                EnsureConnectionClosed();
            }

            return rows;
        }

        /// <summary>
        /// Opens a connection if not open
        /// </summary>
        private void EnsureConnectionOpen()
        {
            var retries = 3;
            if (_connection.State == ConnectionState.Open)
            {
                return;
            }
            while (retries >= 0 && _connection.State != ConnectionState.Open)
            {
                _connection.Open();
                retries--;
                Thread.Sleep(30);
            }
        }

        /// <summary>
        /// Closes a connection if open
        /// </summary>
        private void EnsureConnectionClosed()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Creates a OracleCommand with the given parameters
        /// </summary>
        /// <param name="commandText">The Oracle query to execute</param>
        /// <param name="parameters">Parameters to pass to the Oracle query</param>
        /// <returns></returns>
        private OracleCommand CreateCommand(string commandText, IEnumerable parameters)
        {
            var command = _connection.CreateCommand();
            command.BindByName = true;
            command.CommandText = commandText;
            command.Parameters.Clear();
            AddParameters(command, parameters);

            return command;
        }

        /// <summary>
        /// Adds the parameters to a Oracle command
        /// </summary>
        /// <param name="command">The Oracle query to execute</param>
        /// <param name="parameters">Parameters to pass to the Oracle query</param>
        private static void AddParameters(OracleCommand command, IEnumerable parameters)
        {
            if (parameters == null) return;

            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Helper method to return query a string value 
        /// </summary>
        /// <param name="commandText">The Oracle query to execute</param>
        /// <param name="parameters">Parameters to pass to the Oracle query</param>
        /// <returns>The string value resulting from the query</returns>
        public string GetStrValue(string commandText, IEnumerable parameters)
        {
            var value = QueryValue(commandText, parameters) as string;
            return value;
        }

        public void Dispose()
        {
            if (_connection == null) return;

            _connection.Dispose();
            _connection = null;
        }

        static string desencriptar(string cadena, string clave)
        {
            // Convierto la cadena y la clave en arreglos de bytes
            // para poder usarlas en las funciones de encriptacion
            // En este caso la cadena la convierta usando base 64
            // que es la codificacion usada en el metodo encriptar
            byte[] cadenaBytes = Convert.FromBase64String(cadena);
            byte[] claveBytes = Encoding.UTF8.GetBytes(clave);

            // Creo un objeto de la clase Rijndael
            RijndaelManaged rij = new RijndaelManaged();

            // Configuro para que utilice el modo ECB
            rij.Mode = CipherMode.ECB;

            // Configuro para que use encriptacion de 256 bits.
            rij.BlockSize = 256;

            // Declaro que si necesitara mas bytes agregue ceros.
            rij.Padding = PaddingMode.Zeros;

            // Declaro un desencriptador que use mi clave secreta y un vector
            // de inicializacion aleatorio
            ICryptoTransform desencriptador;
            desencriptador = rij.CreateDecryptor(claveBytes, rij.IV);

            // Declaro un stream de memoria para que guarde los datos
            // encriptados
            MemoryStream memStream = new MemoryStream(cadenaBytes);

            // Declaro un stream de cifrado para que pueda leer de aqui
            // la cadena a desencriptar. Esta clase utiliza el desencriptador
            // y el stream de memoria para realizar la desencriptacion
            CryptoStream cifradoStream;
            cifradoStream = new CryptoStream(memStream, desencriptador, CryptoStreamMode.Read);

            // Declaro un lector para que lea desde el stream de cifrado.
            // A medida que vaya leyendo se ira desencriptando.
            StreamReader lectorStream = new StreamReader(cifradoStream);

            // Leo todos los bytes y lo almaceno en una cadena
            string resultado = lectorStream.ReadToEnd();

            // Cierro los dos streams creados
            memStream.Close();
            cifradoStream.Close();

            // Devuelvo la cadena
            return resultado;
        }

        public int getSecuencial(string NombreSecuencial)
        {
            /*inicializador*/
            int result = 0;

            /*query o comandText*/
            string commandText =string.Format( "SELECT {0}.NEXTVAL AS SECUENCIA FROM DUAL",NombreSecuencial);

            /*ejecuta el query*/
            var rows = Query(commandText, null);

            /*valida la consulta*/
            if (rows == null || rows.Count != 1)
                result = 0;
            else
            {
                var row = rows[0];
                result = string.IsNullOrEmpty(row["SECUENCIA"]) ? 0 : int.Parse(row["SECUENCIA"]);
            }

            return result;
            
        }

        public int QueryTextValueInt(string commandText, IEnumerable parameters)
        {
            int result = -200;

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

                if (parameters!=null)
                {
                    foreach (OracleParameter p in parameters)
                    {
                        if (p.Direction == ParameterDirection.ReturnValue)
                        {
                            result = int.Parse(p.Value.ToString());
                        }
                    }
                }                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Data+" "+ex.InnerException);
                FuncionesApoyo.LogEnArchivo(ex);
                return result;
            }
            finally
            {
                EnsureConnectionClosed();
            }

            return result;
        }

        
        public IEnumerable QueryValueList(string commandText, IEnumerable parameters)
        {
     

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Data);
                FuncionesApoyo.LogEnArchivo(ex);
                return parameters;
            }
            finally
            {
                EnsureConnectionClosed();
            }

            return parameters;
        }
    }
}
