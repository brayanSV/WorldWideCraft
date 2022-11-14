Imports System.Data.Common
Imports System.Data.SqlClient
Imports System.Runtime.CompilerServices
Imports System.Web.WebSockets
Imports Microsoft.VisualBasic.ApplicationServices

Public Class Clientes
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            lblclienteId.Text = "0"
            hiddenAction.Value = "read"

            Empezar(Me, New EventArgs)
        End If
    End Sub

    Protected Sub Empezar(ByVal sender As Object, ByVal e As EventArgs)
        pnlread.Visible = False
        pnlnewedit.Visible = False
        pnlnohaydatos.Visible = True

        CargarClientes(Me, New EventArgs)
    End Sub

    Protected Sub CargarClientes(ByVal sender As Object, ByVal e As EventArgs)
        Dim connectionString As New SqlConnection(ConfigurationManager.ConnectionStrings("conexion").ToString)
        connectionString.Open()

        Dim commandString As String = ""
        commandString = "	select distinct clientes.clienteId, " &
                        "					clientes.nombre, " &
                        "					clientes.nit, " &
                        "					clientes.telefono " &
                        "			   from clientes " &
                        "			  where clientes.estadoregistroId = '1' "

        Dim myDataAdapter As New System.Data.SqlClient.SqlDataAdapter(commandString, connectionString)

        Dim myDataSet As New Data.DataSet()
        myDataAdapter.Fill(myDataSet, "clientes")

        Dim bandhaydatos As Boolean = False
        If myDataSet.Tables("clientes").Rows.Count > 0 Then
            If Not IsDBNull(myDataSet.Tables("clientes").Rows(0)("clienteId")) Then
                bandhaydatos = True

                gridClientes.DataSource = myDataSet.Tables("clientes").DefaultView
                gridClientes.DataBind()
            End If
        End If

        connectionString.Close()

        If bandhaydatos = True Then
            pnlgridclientes.Visible = True
            pnlnohayclientes.Visible = False
        Else
            pnlgridclientes.Visible = False
            pnlnohayclientes.Visible = True
        End If
    End Sub

    Protected Sub OnPageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gridClientes.PageIndex = e.NewPageIndex
        CargarClientes(Me, New EventArgs)
    End Sub

    Protected Sub gridClientes_RowDataBound(sender As Object, e As GridViewRowEventArgs)
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
        lblclienteId.Text = e.CommandArgument
        Dim args As New CommandEventArgs("", lblclienteId.Text)

        Select Case hiddenAction.Value
            Case "edit"
                EditarCliente(Me, args)
            Case "read"
                VerMasCliente(Me, args)
            Case "delete"
                EliminarCliente(Me, args)
        End Select
    End Sub

    Protected Sub ActionMenu(ByVal sender As Object, ByVal e As CommandEventArgs)
        hiddenAction.Value = e.CommandArgument

        If e.CommandArgument = "news" Then
            NuevoCliente(Me, New EventArgs)
            CargarClientes(Me, New EventArgs)
        Else
            If lblclienteId.Text <> "0" Then
                Dim args As New CommandEventArgs("", lblclienteId.Text)

                Select Case e.CommandArgument
                    Case "edit"
                        EditarCliente(Me, args)
                    Case "read"
                        VerMasCliente(Me, args)
                End Select

                CargarClientes(Me, New EventArgs)
            Else
                Empezar(Me, New EventArgs)
            End If
        End If
    End Sub

    Protected Sub NuevoCliente(ByVal sender As Object, ByVal e As EventArgs)
        pnlread.Visible = False
        pnlnewedit.Visible = True
        pnlnohaydatos.Visible = False
        btneditnew.Text = "Crear Cliente"
    End Sub

    Protected Sub NewEditCliente(ByVal sender As Object, ByVal e As EventArgs)
        If Page.IsValid = True And (hiddenAction.Value = "news" Or hiddenAction.Value = "edit") Then
            Dim MyConnection As New SqlConnection(ConfigurationManager.ConnectionStrings("conexion").ToString)
            MyConnection.Open()

            Dim InsertCmd As String = ""
            Dim bandexito As Boolean = True

            Dim myTrans As SqlTransaction
            Dim MyCommand As SqlClient.SqlCommand
            myTrans = MyConnection.BeginTransaction()

            Try
                If hiddenAction.Value = "news" Then
                    InsertCmd = "insert into clientes(nombre, nit, telefono, estadoregistroId) values (@nombre, @nit, @telefono, @estadoregistroId)"
                    MyCommand = New SqlClient.SqlCommand(InsertCmd, MyConnection)

                    MyCommand.Parameters.Add(New SqlParameter("@nombre", SqlDbType.VarChar, 250))
                    MyCommand.Parameters("@nombre").Value = txbnombre.Text

                    MyCommand.Parameters.Add(New SqlParameter("@nit", SqlDbType.VarChar, 250))
                    MyCommand.Parameters("@nit").Value = txbnit.Text

                    MyCommand.Parameters.Add(New SqlParameter("@telefono", SqlDbType.VarChar, 50))
                    MyCommand.Parameters("@telefono").Value = txbtelefono.Text

                    MyCommand.Parameters.Add(New SqlParameter("@estadoregistroId", SqlDbType.Int))
                    MyCommand.Parameters("@estadoregistroId").Value = 1
                Else
                    InsertCmd = "update clientes set nombre=@nombre, nit=@nit, telefono=@telefono where clienteId = '" & lblclienteId.Text & "'"
                    MyCommand = New SqlClient.SqlCommand(InsertCmd, MyConnection)

                    MyCommand.Parameters.Add(New SqlParameter("@nombre", SqlDbType.VarChar, 250))
                    MyCommand.Parameters("@nombre").Value = txbnombre.Text

                    MyCommand.Parameters.Add(New SqlParameter("@nit", SqlDbType.VarChar, 250))
                    MyCommand.Parameters("@nit").Value = txbnit.Text

                    MyCommand.Parameters.Add(New SqlParameter("@telefono", SqlDbType.VarChar, 50))
                    MyCommand.Parameters("@telefono").Value = txbtelefono.Text
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
                lblclienteId.Text = "0"
                hiddenAction.Value = "read"
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "CallMyFunction", "ClickLectura()", True)

                Empezar(Me, New EventArgs)
            End If
        End If
    End Sub

    Protected Sub EditarCliente(ByVal sender As Object, ByVal e As CommandEventArgs)
        pnlread.Visible = False
        pnlnewedit.Visible = True
        pnlnohaydatos.Visible = False
        lblclienteId.Text = e.CommandArgument
        btneditnew.Text = "Editar Cliente"

        Dim connectionString As New SqlConnection(ConfigurationManager.ConnectionStrings("conexion").ToString)
        connectionString.Open()

        Dim commandString As String = ""
        commandString = "	select distinct clientes.clienteId, " &
                        "					clientes.nombre, " &
                        "					clientes.nit, " &
                        "					clientes.telefono " &
                        "			   from clientes " &
                        "			  where clientes.clienteId = '" & lblclienteId.Text & "' "

        Dim myDataAdapter As New System.Data.SqlClient.SqlDataAdapter(commandString, connectionString)

        Dim myDataSet As New Data.DataSet()
        myDataAdapter.Fill(myDataSet, "clientes")

        Dim bandhaydatos As Boolean = False
        If myDataSet.Tables("clientes").Rows.Count > 0 Then
            If Not IsDBNull(myDataSet.Tables("clientes").Rows(0)("clienteId")) Then
                bandhaydatos = True

                txbnombre.Text = myDataSet.Tables("clientes").Rows(0)("nombre")
                txbnit.Text = myDataSet.Tables("clientes").Rows(0)("nit")
                txbtelefono.Text = myDataSet.Tables("clientes").Rows(0)("telefono")
            End If
        End If

        connectionString.Close()
    End Sub

    Protected Sub EliminarCliente(ByVal sender As Object, ByVal e As CommandEventArgs)
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
            InsertCmd = "update clientes set estadoregistroID=@estadoregistroID where clienteId = '" & e.CommandArgument & "'"
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
            lblclienteId.Text = "0"
            hiddenAction.Value = "read"

            Empezar(Me, New EventArgs)
        End If
    End Sub

    Protected Sub VerMasCliente(ByVal sender As Object, ByVal e As CommandEventArgs)
        pnlread.Visible = True
        pnlnewedit.Visible = False
        pnlnohaydatos.Visible = False
        lblclienteId.Text = e.CommandArgument

        Dim connectionString As New SqlConnection(ConfigurationManager.ConnectionStrings("conexion").ToString)
        connectionString.Open()

        Dim commandString As String = ""
        commandString = "	select distinct clientes.clienteId, " &
                        "					clientes.nombre, " &
                        "					clientes.nit, " &
                        "					clientes.telefono " &
                        "			   from clientes " &
                        "			  where clientes.clienteId = '" & lblclienteId.Text & "' "

        Dim myDataAdapter As New System.Data.SqlClient.SqlDataAdapter(commandString, connectionString)

        Dim myDataSet As New Data.DataSet()
        myDataAdapter.Fill(myDataSet, "clientes")

        Dim bandhaydatos As Boolean = False
        If myDataSet.Tables("clientes").Rows.Count > 0 Then
            If Not IsDBNull(myDataSet.Tables("clientes").Rows(0)("clienteId")) Then
                bandhaydatos = True

                txbnombrelect.Text = myDataSet.Tables("clientes").Rows(0)("nombre")
                txbnitlect.Text = myDataSet.Tables("clientes").Rows(0)("nit")
                txbtelefonolect.Text = myDataSet.Tables("clientes").Rows(0)("telefono")
            End If
        End If

        connectionString.Close()
    End Sub

    Protected Sub volver(ByVal sender As Object, ByVal e As EventArgs)
        lblclienteId.Text = "0"
        hiddenAction.Value = "read"

        Empezar(Me, New EventArgs)
    End Sub
End Class