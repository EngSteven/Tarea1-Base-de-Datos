using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using WebApplication2.Pages.Clientes;

namespace WebApplication2.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public List<ClienteInfo> listaClientes = new List<ClienteInfo>();
            
        public void OnGet()
        {
            try
            {
                String connectionString = "Data Source=project0-server.database.windows.net;Initial Catalog=project0-database;Persist Security Info=True;User ID=stevensql;Password=Killua36911-";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //String sql = "SELECT * FROM Articulo";
                    using (SqlCommand command = new SqlCommand("dbo.ListaDeArticulos2", connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        DataTable table = new DataTable();

                        //using (SqlDataReader reader = command.ExecuteReader())
                        //{
                        //while (reader.Read())
                        //{
                        //ClienteInfo clienteInfo = new ClienteInfo();
                        //clienteInfo.id = "" + reader.GetInt32(0);
                        //clienteInfo.Nombre = reader.GetString(1);
                        //clienteInfo.Precio = "" + reader.GetSqlMoney(2);
                        //clienteInfo.created_at = reader.GetDateTime(3).ToString();

                        //listaClientes.Add(clienteInfo);

                        //}
                        //}
                        command.CommandType = CommandType.StoredProcedure;
                        SqlParameter resultCodeParam = new SqlParameter("@outResultCode", SqlDbType.Int);
                        resultCodeParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultCodeParam);


                        adapter.SelectCommand = command;
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        //if (dataSet.Tables[1].Rows[0][0].Equals("0"))
                        //{
                         //   Console.WriteLine("Exito!!!!");
                        //}
                        //else { Console.WriteLine("Fracaso!!!!"); }
                        
                        int val = dataSet.Tables.Count;
                        //Console.WriteLine(val+"Prueba1");
                        //object value = dataSet.Tables[0].Rows[0][0];
                        if (val == 2)
                        {
                            foreach (DataRow row in dataSet.Tables[0].Rows)
                            {
                                ClienteInfo clienteInfo = new ClienteInfo();
                                clienteInfo.id = "" + row[0];
                                clienteInfo.Nombre = "" + row[1];
                                clienteInfo.Precio = "" + SqlMoney.Parse(row[2].ToString());

                                listaClientes.Add(clienteInfo);
                            }
                        }
                        else
                        {
                            //Pendiente
                        }



                        //command.ExecuteNonQuery();
                        //int resultCode = (int)command.Parameters["@outResultCode"].Value;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }
    }
    public class ClienteInfo
    {
        public String id;
        public String Nombre;
        public String Precio;
        //public String created_at;

    }
}