using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace WebApplication2.Pages.Clientes
{
    public class CreateModel : PageModel
    {
        public ClienteInfo clienteInfo = new ClienteInfo();
        public String errorMessage = "";
        public String successMessage = "";

        public void OnGet()
        {
        }

        public void OnPost() 
        { 
            clienteInfo.Nombre = Request.Form["Nombre"];
            clienteInfo.Precio = Request.Form["Precio"];

            

            if (clienteInfo.Nombre.Length == 0 || clienteInfo.Precio.Length == 0 ) 
            {
                errorMessage = "Todos los datos son requeridos.";
                return;
            }
            //Guardar el nuevo cliente
            //Comprobar el formato. 

            try
            {
                String connectionString = "Data Source=project0-server.database.windows.net;Initial Catalog=project0-database;Persist Security Info=True;User ID=stevensql;Password=Killua36911-";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "INSERT INTO Articulo " +
                                 "(Nombre, Precio) VALUES " +
                                 "(@Nombre, @Precio);";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", clienteInfo.Nombre);
                        command.Parameters.AddWithValue("@Precio", SqlMoney.Parse(clienteInfo.Precio)); 
                            
                        command.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            clienteInfo.Nombre = "";
            clienteInfo.Precio = "";
            successMessage = "Nuevo cliente anadido correctamente.";

            Response.Redirect("/Index");
        }
    }
}
