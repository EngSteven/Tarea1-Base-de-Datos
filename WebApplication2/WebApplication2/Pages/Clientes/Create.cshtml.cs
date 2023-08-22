using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
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

            
            //comprobar que no este vacio los datos ingresados
            if (clienteInfo.Nombre.Length == 0 || clienteInfo.Precio.Length == 0 ) 
            {
                errorMessage = "Todos los datos son requeridos.";
                return;
            }

            //Comprobar que el nombre solo contenga letras o guines
            if(!clienteInfo.Nombre.All(c => (Char.IsLetter(c) || c == '-')))
            {
                errorMessage = "El nombre solo puede contener letras o guines";
                return;
            }
            //Comprobar que el precio solo contenga numeros o comas
            if(!clienteInfo.Precio.All(c => (c >= '0' && c <= '9') || c == ','))
            {
                errorMessage = "El precio solo puede tener valores numéricos o coma";
                return;
            }

            try
            {
                String connectionString = "Data Source=project0-server.database.windows.net;Initial Catalog=project0-database;Persist Security Info=True;User ID=stevensql;Password=Killua36911-";
                string spName = "dbo.CrearArticulo";
                using (SqlConnection connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();
                    /*String sql = "INSERT INTO Articulo " +
                                 "(Nombre, Precio) VALUES " +
                                 "(@Nombre, @Precio);";*/
                    using (SqlCommand command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@inNombre", clienteInfo.Nombre);
                        command.Parameters.AddWithValue("@inPrecio", SqlMoney.Parse(clienteInfo.Precio));                        
                        SqlParameter resultCodeParam = new SqlParameter("@outResultCode", SqlDbType.Int);
                        resultCodeParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultCodeParam);

                        command.ExecuteNonQuery();

                        int resultCode = (int)command.Parameters["@outResultCode"].Value;
                        
                        if (resultCode == 50001) //codigo generado en el SP que dice si ya un nombre del articulo existe o no
                        {
                            errorMessage = "El nombre del articulo ya existe";
                            return;
                        }


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
            successMessage = "Nuevo articulo añadido correctamente.";

            Response.Redirect("/Index");
        }
    }
}
