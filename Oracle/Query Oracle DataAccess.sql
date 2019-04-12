<appSettings>
   
    <add key="CSFConnection" value="Data Source=(DESCRIPTION =(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST = 127.0.0.1)(PORT = 1521)))(CONNECT_DATA =(SERVICE_NAME = xe)));User ID=USER;Password=PASSWORD;" />
  </appSettings>
  
public List<Object> getQuery(string Parametro)
        {
            var _Object = new List<Object>();

            string _consulta = @"SELECT *
                                 FROM TABLA T
                                 WHERE 
                                    T.CAMPO =:PA_CAMPO";

            var parametros = new List<OracleParameter>
            {
                new OracleParameter {
                    ParameterName = "PA_CAMPO",
                    Value = Parametro,
                    OracleDbType = OracleDbType.Varchar2
                },
                new OracleParameter {
                    ParameterName = "PA_CAMPO2",
                    Value = Parametro,
                    OracleDbType = OracleDbType.Varchar2
                },
            };

            /*ejecuta la consulta*/
            var rows = _database.Query(_consulta, parametros);

            foreach (var row in rows)
            {
                Object _ObjectSingle = new Object();
                _ObjectSingle.Campo = row["CAMPO"]; 
                //_ObjectSingle.Campo = string.IsNullOrEmpty(row["CAMPO"]) ? 0 : decimal.Parse(row["MONTO"]);
                _Object.Add(_ObjectSingle);

            }
            return _Object;
        }
		
		
public int execProcedure(string Parametro)
        {
            int result = -200;
            const string commandText = @"PROCEDURE_NAME";

            var parametros = new List<OracleParameter> {
                new OracleParameter
                {
                    ParameterName = "PA_PARAMETRO1",
                    Value = Parametro,
                    OracleDbType = OracleDbType.Varchar2
                },new OracleParameter
                {
                    ParameterName = "PA_PARAMETRO2",
                    Value = Parametro,
                    OracleDbType = OracleDbType.Varchar2
                },new OracleParameter
                {
                    ParameterName = "PA_PARAMETRO3",
                    Value = Parametro,
                    OracleDbType = OracleDbType.Varchar2
                },new OracleParameter
                {
                    ParameterName = "PA_PARAMETRO4",
                    Value = Parametro,
                    OracleDbType = OracleDbType.Int32,
                    Direction = System.Data.ParameterDirection.Output
                }
            };

            result = _database.ExecProcResult(commandText, parametros);
            return result;
        }
		
		
		
public int insertUpdateDeleteQuery(int Parametro1,int Parametro2)
        {
            int Respuesta = -200;
            string commandText = string.Format(
                @"INSERT INTO TABLA (CAMPO1,CAMPO2)
                 VALUES ({0},{1})",Parametro1, Parametro2);
          

            Respuesta = _database.QueryTextValueInt(commandText, null);

            return Respuesta;
        }