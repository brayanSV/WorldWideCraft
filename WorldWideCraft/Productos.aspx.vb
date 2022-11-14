Imports System.Data.SqlClient
Imports System.IO

Public Class Productos
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            lblproductoId.Text = "0"
            hiddenAction.Value = "read"

            Empezar(Me, New EventArgs)
        End If
    End Sub

    Protected Sub Empezar(ByVal sender As Object, ByVal e As EventArgs)
        pnlread.Visible = False
        pnlnewedit.Visible = False
        pnlnohaydatos.Visible = True

        CargarProductos(Me, New EventArgs)
    End Sub

    Protected Sub CargarProductos(ByVal sender As Object, ByVal e As EventArgs)
        Dim connectionString As New SqlConnection(ConfigurationManager.ConnectionStrings("conexion").ToString)
        connectionString.Open()

        Dim commandString As String = ""
        commandString = "	select distinct Productos.ProductoId, " &
                        "					Productos.nombre, " &
                        "					Productos.referencia, " &
                        "					Productos.descripcion, " &
                        "					('~/Files/' + Productos.foto) as rutafoto " &
                        "			   from Productos " &
                        "			  where Productos.estadoregistroId = '1' "

        Dim myDataAdapter As New System.Data.SqlClient.SqlDataAdapter(commandString, connectionString)

        Dim myDataSet As New Data.DataSet()
        myDataAdapter.Fill(myDataSet, "Productos")

        Dim bandhaydatos As Boolean = False
        If myDataSet.Tables("Productos").Rows.Count > 0 Then
            If Not IsDBNull(myDataSet.Tables("Productos").Rows(0)("ProductoId")) Then
                bandhaydatos = True

                gridProductos.DataSource = myDataSet.Tables("Productos").DefaultView
                gridProductos.DataBind()
            End If
        End If

        connectionString.Close()

        If bandhaydatos = True Then
            pnlgridProductos.Visible = True
            pnlnohayProductos.Visible = False
        Else
            pnlgridProductos.Visible = False
            pnlnohayProductos.Visible = True
        End If
    End Sub

    Protected Sub OnPageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gridProductos.PageIndex = e.NewPageIndex
        CargarProductos(Me, New EventArgs)
    End Sub

    Protected Sub gridProductos_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            lblerror.Text = hiddenAction.Value
            Dim linkAccion As LinkButton = CType(e.Row.FindControl("linkAccion"), LinkButton)
            Dim Confirmlinkaccion As AjaxControlToolkit.ConfirmButtonExtender = CType(e.Row.FindControl("Confirmlinkaccion"), AjaxControlToolkit.ConfirmButtonExtender)


            Select Case hiddenAction.Value
                Case "read", "news"
                    linkAccion.Text = "ver más"
                    Confirmlinkaccion.Enabled = False
                Case "edit"
                    linkAccion.Text = "seleccionar"
                    Confirmlinkaccion.Enabled = False
                Case "delete"
                    linkAccion.Text = "eliminar"
                    Confirmlinkaccion.Enabled = True
            End Select
        End If
    End Sub

    Protected Sub Acciongrid(ByVal sender As Object, ByVal e As CommandEventArgs)
        lblproductoId.Text = e.CommandArgument
        Dim args As New CommandEventArgs("", lblproductoId.Text)

        Select Case hiddenAction.Value
            Case "edit"
                EditarProducto(Me, args)
            Case "read"
                VerMasProducto(Me, args)
            Case "delete"
                EliminarProducto(Me, args)
        End Select
    End Sub

    Protected Sub ActionMenu(ByVal sender As Object, ByVal e As CommandEventArgs)
        hiddenAction.Value = e.CommandArgument

        If e.CommandArgument = "news" Then
            NuevoProducto(Me, New EventArgs)
            CargarProductos(Me, New EventArgs)
        Else
            If lblproductoId.Text <> "0" Then
                Dim args As New CommandEventArgs("", lblproductoId.Text)

                Select Case e.CommandArgument
                    Case "edit"
                        EditarProducto(Me, args)
                    Case "read"
                        VerMasProducto(Me, args)
                End Select

                CargarProductos(Me, New EventArgs)
            Else
                Empezar(Me, New EventArgs)
            End If
        End If
    End Sub

    Protected Sub NuevoProducto(ByVal sender As Object, ByVal e As EventArgs)
        pnlread.Visible = False
        pnlnewedit.Visible = True
        pnlnohaydatos.Visible = False
        btneditnew.Text = "Crear Producto"
    End Sub

    Protected Sub NewEditProducto(ByVal sender As Object, ByVal e As EventArgs)
        If Page.IsValid = True And (hiddenAction.Value = "news" Or hiddenAction.Value = "edit") Then
            Dim bandFieldsEmpty As Boolean = True
            Dim folderPath As String = Server.MapPath("~/Files/")

            'Check whether Directory (Folder) exists.
            If Not Directory.Exists(folderPath) Then
                'If Directory (Folder) does not exists Create it.
                Directory.CreateDirectory(folderPath)
            End If

            Try
                FileUpload1.SaveAs(folderPath & Path.GetFileName(FileUpload1.FileName))
            Catch ex As Exception

            End Try

            Dim MyConnection As New SqlConnection(ConfigurationManager.ConnectionStrings("conexion").ToString)
            MyConnection.Open()

            Dim InsertCmd As String = ""
            Dim bandexito As Boolean = True

            Dim myTrans As SqlTransaction
            Dim MyCommand As SqlClient.SqlCommand
            myTrans = MyConnection.BeginTransaction()

            Try
                If hiddenAction.Value = "news" Then
                    InsertCmd = "insert into productos(nombre, referencia, descripcion, foto, estadoregistroId) values (@nombre, @referencia, @descripcion, @foto, @estadoregistroId)"
                    MyCommand = New SqlClient.SqlCommand(InsertCmd, MyConnection)

                    MyCommand.Parameters.Add(New SqlParameter("@nombre", SqlDbType.NVarChar, 250))
                    MyCommand.Parameters("@nombre").Value = txbnombre.Text

                    MyCommand.Parameters.Add(New SqlParameter("@referencia", SqlDbType.NVarChar, 250))
                    MyCommand.Parameters("@referencia").Value = txbreferencia.Text

                    MyCommand.Parameters.Add(New SqlParameter("@descripcion", SqlDbType.NVarChar))
                    MyCommand.Parameters("@descripcion").Value = txbdescripcion.Text

                    MyCommand.Parameters.Add(New SqlParameter("@foto", SqlDbType.NVarChar, 50))
                    MyCommand.Parameters("@foto").Value = FileUpload1.FileName

                    MyCommand.Parameters.Add(New SqlParameter("@estadoregistroId", SqlDbType.Int))
                    MyCommand.Parameters("@estadoregistroId").Value = 1
                Else
                    InsertCmd = "update Productos set nombre=@nombre, referencia=@referencia, descripcion=@descripcion, foto=@foto where ProductoId = '" & lblproductoId.Text & "'"
                    MyCommand = New SqlClient.SqlCommand(InsertCmd, MyConnection)

                    MyCommand.Parameters.Add(New SqlParameter("@nombre", SqlDbType.NVarChar, 250))
                    MyCommand.Parameters("@nombre").Value = txbnombre.Text

                    MyCommand.Parameters.Add(New SqlParameter("@referencia", SqlDbType.NVarChar, 250))
                    MyCommand.Parameters("@referencia").Value = txbreferencia.Text

                    MyCommand.Parameters.Add(New SqlParameter("@descripcion", SqlDbType.NVarChar))
                    MyCommand.Parameters("@descripcion").Value = txbdescripcion.Text

                    MyCommand.Parameters.Add(New SqlParameter("@foto", SqlDbType.NVarChar, 50))
                    MyCommand.Parameters("@foto").Value = FileUpload1.FileName
                End If

                Try
                    MyCommand.Transaction = myTrans
                    MyCommand.ExecuteNonQuery()
                Catch ex As Exception
                    bandexito = False
                    lblerror.Text = "E2: " & ex.Message
                End Try
            Catch ex As Exception
                bandexito = False
                lblerror.Text = "E1: " & ex.Message
            End Try

            If bandexito = True Then
                myTrans.Commit()
            Else
                myTrans.Rollback()
            End If

            MyConnection.Close()

            If bandexito = True Then
                lblproductoId.Text = "0"
                hiddenAction.Value = "read"
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "CallMyFunction", "ClickLectura()", True)

                Empezar(Me, New EventArgs)
            End If
        End If
    End Sub

    Private Function ValidateFileDimensions() As Boolean
        If System.Drawing.Image.FromStream(FileUpload1.PostedFile.InputStream).Width = 250 And System.Drawing.Image.FromStream(FileUpload1.PostedFile.InputStream).Width = 250 Then
            Return True
        End If

        Return False
    End Function

    Protected Sub EditarProducto(ByVal sender As Object, ByVal e As CommandEventArgs)
        pnlread.Visible = False
        pnlnewedit.Visible = True
        pnlnohaydatos.Visible = False
        lblproductoId.Text = e.CommandArgument
        btneditnew.Text = "Editar Producto"

        Dim connectionString As New SqlConnection(ConfigurationManager.ConnectionStrings("conexion").ToString)
        connectionString.Open()

        Dim commandString As String = ""
        commandString = "	select distinct Productos.ProductoId, " &
                        "					Productos.nombre, " &
                        "					Productos.referencia, " &
                        "					Productos.descripcion, " &
                        "					Productos.foto " &
                        "			   from Productos " &
                        "			  where Productos.ProductoId = '" & lblproductoId.Text & "' "

        Dim myDataAdapter As New System.Data.SqlClient.SqlDataAdapter(commandString, connectionString)

        Dim myDataSet As New Data.DataSet()
        myDataAdapter.Fill(myDataSet, "Productos")

        Dim bandhaydatos As Boolean = False
        If myDataSet.Tables("Productos").Rows.Count > 0 Then
            If Not IsDBNull(myDataSet.Tables("Productos").Rows(0)("ProductoId")) Then
                bandhaydatos = True

                txbnombre.Text = myDataSet.Tables("Productos").Rows(0)("nombre")
                txbreferencia.Text = myDataSet.Tables("Productos").Rows(0)("referencia")
                txbdescripcion.Text = myDataSet.Tables("Productos").Rows(0)("descripcion")
                'FileUpload1.PostedFile.FileName = myDataSet.Tables("Productos").Rows(0)("foto")
            End If
        End If

        connectionString.Close()
    End Sub

    Protected Sub EliminarProducto(ByVal sender As Object, ByVal e As CommandEventArgs)
        pnlread.Visible = False
        pnlnewedit.Visible = False
        pnlnohaydatos.Visible = True

        Dim MyConnection As New SqlConnection(ConfigurationManager.ConnectionStrings("conexion").ToString)
        MyConnection.Open()

        Dim InsertCmd As String = ""
        Dim bandexito As Boolean = True

        Dim myTrans As SqlTransaction
        Dim MyCommand As SqlClient.SqlCommand
        myTrans = MyConnection.BeginTransaction()

        Try
            InsertCmd = "update Productos set estadoregistroID=@estadoregistroID where ProductoId = '" & e.CommandArgument & "'"
            MyCommand = New SqlClient.SqlCommand(InsertCmd, MyConnection)

            MyCommand.Parameters.Add(New SqlParameter("@estadoregistroID", SqlDbType.Int))
            MyCommand.Parameters("@estadoregistroID").Value = 3

            Try
                MyCommand.Transaction = myTrans
                MyCommand.ExecuteNonQuery()
            Catch ex As Exception
                bandexito = False
                lblerror.Text = "E2: " & ex.Message
            End Try
        Catch ex As Exception
            bandexito = False
            lblerror.Text = "E1: " & ex.Message
        End Try

        If bandexito = True Then
            myTrans.Commit()
        Else
            myTrans.Rollback()
        End If

        MyConnection.Close()

        If bandexito = True Then
            lblproductoId.Text = "0"
            hiddenAction.Value = "read"

            Empezar(Me, New EventArgs)
        End If
    End Sub

    Protected Sub VerMasProducto(ByVal sender As Object, ByVal e As CommandEventArgs)
        pnlread.Visible = True
        pnlnewedit.Visible = False
        pnlnohaydatos.Visible = False
        lblproductoId.Text = e.CommandArgument

        Dim connectionString As New SqlConnection(ConfigurationManager.ConnectionStrings("conexion").ToString)
        connectionString.Open()

        Dim commandString As String = ""
        commandString = "	select distinct Productos.ProductoId, " &
                        "					Productos.nombre, " &
                        "					Productos.referencia, " &
                        "					Productos.descripcion, " &
                        "					('~/Files/' + Productos.foto) as rutafoto " &
                        "			   from Productos " &
                        "			  where Productos.ProductoId = '" & lblproductoId.Text & "' "

        Dim myDataAdapter As New System.Data.SqlClient.SqlDataAdapter(commandString, connectionString)

        Dim myDataSet As New Data.DataSet()
        myDataAdapter.Fill(myDataSet, "Productos")

        Dim bandhaydatos As Boolean = False
        If myDataSet.Tables("Productos").Rows.Count > 0 Then
            If Not IsDBNull(myDataSet.Tables("Productos").Rows(0)("ProductoId")) Then
                bandhaydatos = True

                txbnombrelect.Text = myDataSet.Tables("Productos").Rows(0)("nombre")
                txbreferencialect.Text = myDataSet.Tables("Productos").Rows(0)("referencia")
                txbdescripcionlect.Text = myDataSet.Tables("Productos").Rows(0)("descripcion")
                imgfotoprodlect.ImageUrl = myDataSet.Tables("Productos").Rows(0)("rutafoto")
            End If
        End If

        connectionString.Close()
    End Sub

    Protected Sub volver(ByVal sender As Object, ByVal e As EventArgs)
        lblproductoId.Text = "0"
        hiddenAction.Value = "read"

        Empezar(Me, New EventArgs)
    End Sub

    Protected Sub UploadFile(sender As Object, e As EventArgs)
        Dim folderPath As String = Server.MapPath("~/Files/")

        'Check whether Directory (Folder) exists.
        If Not Directory.Exists(folderPath) Then
            'If Directory (Folder) does not exists Create it.
            Directory.CreateDirectory(folderPath)
        End If

        'Save the File to the Directory (Folder).
        FileUpload1.SaveAs(folderPath & Path.GetFileName(FileUpload1.FileName))

        'Display the Picture in Image control.
        'Image1.ImageUrl = "~/Files/" & Path.GetFileName(FileUpload1.FileName)
    End Sub

End Class