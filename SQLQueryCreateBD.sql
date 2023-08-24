/****** Object:  Database [project0-database]    Script Date: 23/8/2023 19:09:17 ******/
CREATE DATABASE [project0-database]  (EDITION = 'Basic', SERVICE_OBJECTIVE = 'Basic', MAXSIZE = 2 GB) WITH CATALOG_COLLATION = SQL_Latin1_General_CP1_CI_AS, LEDGER = OFF;
GO
ALTER DATABASE [project0-database] SET COMPATIBILITY_LEVEL = 150
GO
ALTER DATABASE [project0-database] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [project0-database] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [project0-database] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [project0-database] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [project0-database] SET ARITHABORT OFF 
GO
ALTER DATABASE [project0-database] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [project0-database] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [project0-database] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [project0-database] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [project0-database] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [project0-database] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [project0-database] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [project0-database] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [project0-database] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO
ALTER DATABASE [project0-database] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [project0-database] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [project0-database] SET  MULTI_USER 
GO
ALTER DATABASE [project0-database] SET ENCRYPTION ON
GO
ALTER DATABASE [project0-database] SET QUERY_STORE = ON
GO
ALTER DATABASE [project0-database] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 7), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 10, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
/*** The scripts of database scoped configurations in Azure should be executed inside the target database connection. ***/
GO
-- ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 8;
GO
/****** Object:  Table [dbo].[Articulo]    Script Date: 23/8/2023 19:09:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Articulo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](128) NOT NULL,
	[Precio] [money] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Prueba]    Script Date: 23/8/2023 19:09:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Prueba](
	[ID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[CrearArticulo]    Script Date: 23/8/2023 19:09:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CrearArticulo] 
	 @inNombre VARCHAR(128)		-- Nuevo Nombre de articulo
	, @inPrecio MONEY				-- Nuevo Precion
	, @outResultCode INT OUTPUT			-- Codigo de resultado del SP
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	-- Se validan los datos de entrada, pues no estamos seguros que se validaron en capa logica.
	-- Validar que articulo exista.

	BEGIN TRY
		-- Inicia codigo en el cual se captura errores

		DECLARE @LogDescription VARCHAR(2000)='Insertando en tabla Articulo: {Nombre="'
		
		SET @outResultCode=0;  -- Por default codigo error 0 es que no hubo error

		IF EXISTS (SELECT 1 FROM dbo.Articulo A WHERE A.Nombre=@inNombre)
		BEGIN
			 --procesar error
			SET @outResultCode=50001;		-- Articulo no exist
			RETURN;
		END; 
		-- Se hacen otras validaciones ....

		-- se preprocesa lo que luego se actualiza, si es necesario se guarda informacion en variables o en tablas variable

		SET @LogDescription = 
					@LogDescription+@inNombre
					+'", Precio="'
					+CONVERT(VARCHAR, @inPrecio)+'}';


		-- siempre que vamos a actualizar tablas (y son 2 o mas tablas se inicia transaccion de BD)_
		BEGIN TRANSACTION tCrearArticulo 
			
			INSERT [dbo].[Articulo] (
				 [Nombre]
				, [Precio])
			VALUES (
				 @inNombre
				, @inPrecio
			);

		COMMIT TRANSACTION tCrearArticulo

	END TRY
	BEGIN CATCH

		IF @@TRANCOUNT>0  -- error sucedio dentro de la transaccion
		BEGIN
			ROLLBACK TRANSACTION tCrearArticulo; -- se deshacen los cambios realizados
		END;

		SET @outResultCode=50005;
	
	END CATCH

	SET NOCOUNT OFF;
END;
GO
/****** Object:  StoredProcedure [dbo].[ListaDeArticulos2]    Script Date: 23/8/2023 19:09:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[ListaDeArticulos2]
	 @outResultCode INT OUTPUT						--Al ser un SP solo de consulta solo se tendra una variable para errores
	 
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	BEGIN TRY
		SET @outResultCode=0;
		SELECT @outResultCode AS Error;					--Primero siempre la table de errores para comprobar en capa logica.
		SELECT id, Nombre, Precio FROM dbo.Articulo ORDER BY Nombre;		--La tabla con el contenido ordenada alfabeticamente.
		
	END TRY
	BEGIN CATCH										--En caso de cualquier error coloque el codigo 50003
		SET @outResultCode=50003;					--Pase por dataset el error
		SELECT @outResultCode AS Error;
	END CATCH

	SET NOCOUNT OFF;	
END
GO
ALTER DATABASE [project0-database] SET  READ_WRITE 
GO
