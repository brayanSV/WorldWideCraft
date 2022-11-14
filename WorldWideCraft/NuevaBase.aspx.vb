Imports System.Data.SqlClient
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices

Public Class NuevaBase
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            lblfileId.Text = "0"
            hiddenAction.Value = "read"

            Empezar(Me, New EventArgs)
        End If
    End Sub

    Protected Sub Empezar(ByVal sender As Object, ByVal e As EventArgs)
        pnlread.Visible = False
        pnlnewedit.Visible = False
        pnlnohaydatos.Visible = True

        CargarCargaExcel(Me, New EventArgs)
    End Sub

    Protected Sub CargarCargaExcel(ByVal sender As Object, ByVal e As EventArgs)
        Dim connectionString As New SqlConnection(ConfigurationManager.ConnectionStrings("conexion").ToString)
        connectionString.Open()

        Dim commandString As String = ""
        commandString = "	select distinct filesupload.fileId, " &
                        "					filesupload.filename, " &
                        "					filesupload.fecha, " &
                        "					typefileupload.typefile " &
                        "			   from filesupload " &
                        "		 inner join typefileupload on typefileupload.typefileId = filesupload.typefileId " &
                        "		   order by filesupload.fecha "

        Dim myDataAdapter As New System.Data.SqlClient.SqlDataAdapter(commandString, connectionString)

        Dim myDataSet As New Data.DataSet()
        myDataAdapter.Fill(myDataSet, "filesupload")

        Dim bandhaydatos As Boolean = False
        If myDataSet.Tables("filesupload").Rows.Count > 0 Then
            If Not IsDBNull(myDataSet.Tables("filesupload").Rows(0)("fileId")) Then
                bandhaydatos = True

                Dim fechatxt As New DataColumn("fechatxt")
                myDataSet.Tables("filesupload").Columns.Add(fechatxt)

                For var1 As Integer = 0 To myDataSet.Tables("filesupload").Rows.Count - 1
                    Dim fecha As DateTime = Convert.ToDateTime(myDataSet.Tables("filesupload").Rows(var1)("fecha"))
                    myDataSet.Tables("filesupload").Rows(var1)("fechatxt") = fecha.ToString("dd/MM/yyyy hh:mm tt")
                Next

                gridCargaExcel.DataSource = myDataSet.Tables("filesupload").DefaultView
                gridCargaExcel.DataBind()
            End If
        End If

        connectionString.Close()

        If bandhaydatos = True Then
            pnlGridCargaExcel.Visible = True
            pnlnohayCargaExcel.Visible = False
        Else
            pnlGridCargaExcel.Visible = False
            pnlnohayCargaExcel.Visible = True
        End If
    End Sub

    Protected Sub OnPageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gridCargaExcel.PageIndex = e.NewPageIndex
        CargarCargaExcel(Me, New EventArgs)
    End Sub

    Protected Sub gridCargaExcel_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            lblerror.Text = hiddenAction.Value
            Dim linkAccion As LinkButton = CType(e.Row.FindControl("linkAccion"), LinkButton)

            Select Case hiddenAction.Value
                Case "read", "news"
                    linkAccion.Text = "ver más"
            End Select
        End If
    End Sub

    Protected Sub Acciongrid(ByVal sender As Object, ByVal e As CommandEventArgs)
        lblfileId.Text = e.CommandArgument
        Dim args As New CommandEventArgs("", lblfileId.Text)

        Select Case hiddenAction.Value
            Case "read"
                VerMasProducto(Me, args)
        End Select
    End Sub

    Protected Sub ActionMenu(ByVal sender As Object, ByVal e As CommandEventArgs)
        hiddenAction.Value = e.CommandArgument

        If e.CommandArgument = "news" Then
            NuevoCargaExcel(Me, New EventArgs)
            CargarCargaExcel(Me, New EventArgs)
        Else
            If lblfileId.Text <> "0" Then
                Dim args As New CommandEventArgs("", lblfileId.Text)

                Select Case e.CommandArgument
                    Case "read"
                        VerMasProducto(Me, args)
                End Select

                CargarCargaExcel(Me, New EventArgs)
            Else
                Empezar(Me, New EventArgs)
            End If
        End If
    End Sub

    Protected Sub NuevoCargaExcel(ByVal sender As Object, ByVal e As EventArgs)
        pnlread.Visible = False
        pnlnewedit.Visible = True
        pnlnohaydatos.Visible = False
        btneditnew.Text = "Cargar Base"

        cargarddltipoarchivo(Me, New EventArgs)
    End Sub

    Protected Sub cargarddltipoarchivo(ByVal sender As Object, ByVal e As EventArgs)
        Dim connectionString As New SqlConnection(ConfigurationManager.ConnectionStrings("conexion").ToString)
        connectionString.Open()

        Dim commandString As String = ""
        commandString = "	select distinct typefileupload.typefileId, " &
                        "					typefileupload.typefile " &
                        "			   from typefileupload "

        Dim myDataAdapter As New System.Data.SqlClient.SqlDataAdapter(commandString, connectionString)

        Dim myDataSet As New Data.DataSet()
        myDataAdapter.Fill(myDataSet, "typefileupload")

        If myDataSet.Tables("typefileupload").Rows.Count > 0 Then
            If Not IsDBNull(myDataSet.Tables("typefileupload").Rows(0)("typefileId")) Then
                ddltipoarchivo.DataSource = myDataSet.Tables("typefileupload").DefaultView
                ddltipoarchivo.DataValueField = "typefileId"
                ddltipoarchivo.DataTextField = "typefile"
                ddltipoarchivo.DataBind()
            End If
        End If
    End Sub

    Protected Sub NewEditProducto(ByVal sender As Object, ByVal e As EventArgs)
        If Page.IsValid = True And (hiddenAction.Value = "news" Or hiddenAction.Value = "edit") Then
            Dim bandFieldsEmpty As Boolean = True
            Dim folderPath As String = Server.MapPath("~/FilesExcel/")

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
                InsertCmd = "insert into filesupload(filename, fecha, typefileId) values (@filename, @fecha, @typefileId) SET @fileId = SCOPE_IDENTITY()"
                MyCommand = New SqlClient.SqlCommand(InsertCmd, MyConnection)

                MyCommand.Parameters.Add(New SqlParameter("@filename", SqlDbType.NVarChar, 50))
                MyCommand.Parameters("@filename").Value = Path.GetFileName(FileUpload1.FileName)

                MyCommand.Parameters.Add(New SqlParameter("@fecha", SqlDbType.DateTime))
                MyCommand.Parameters("@fecha").Value = DateTime.Now

                MyCommand.Parameters.Add(New SqlParameter("@typefileId", SqlDbType.Int))
                MyCommand.Parameters("@typefileId").Value = ddltipoarchivo.SelectedItem.Value

                MyCommand.Parameters.Add(New SqlClient.SqlParameter("@fileId", SqlDbType.Int))
                MyCommand.Parameters("@fileId").Direction = ParameterDirection.Output

                Try
                    MyCommand.Transaction = myTrans
                    MyCommand.ExecuteNonQuery()

                    Dim fileId As String = MyCommand.Parameters("@fileId").Value.ToString

                    Try
                        If ddltipoarchivo.SelectedItem.Value = 1 Then
                            MyCommand = New SqlCommand("load_clientes", MyConnection)
                        Else
                            MyCommand = New SqlCommand("load_productos", MyConnection)
                        End If

                        MyCommand.CommandType = CommandType.StoredProcedure

                        MyCommand.Parameters.Add(New SqlParameter("@fileId", SqlDbType.VarChar, 50))
                        MyCommand.Parameters("@fileId").Value = fileId

                        MyCommand.Parameters.Add(New SqlParameter("@nombrearchivo", SqlDbType.VarChar, 500))
                        MyCommand.Parameters("@nombrearchivo").Value = Path.GetFileName(FileUpload1.FileName)

                        Try
                            MyCommand.Transaction = myTrans
                            MyCommand.ExecuteNonQuery()
                        Catch ex As Exception
                            bandexito = False
                            Page.ClientScript.RegisterStartupScript(Page.GetType(), "CallMyFunction", "ErrorSQL('E4: " & ex.Message & "')", True)
                        End Try
                    Catch ex As Exception
                        bandexito = False
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "CallMyFunction", "ErrorSQL('E3: " & ex.Message & "')", True)
                    End Try
                Catch ex As Exception
                    bandexito = False
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "CallMyFunction", "ErrorSQL('E2: " & ex.Message & "')", True)
                End Try
            Catch ex As Exception
                bandexito = False
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "CallMyFunction", "ErrorSQL('E1: " & ex.Message & "')", True)
            End Try

            If bandexito = True Then
                myTrans.Commit()
            Else
                myTrans.Rollback()
            End If

            MyConnection.Close()

            If bandexito = True Then
                lblfileId.Text = "0"
                hiddenAction.Value = "read"
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "CallMyFunction", "ClickLectura()", True)

                Empezar(Me, New EventArgs)
            End If
        End If
    End Sub

    Protected Sub VerMasProducto(ByVal sender As Object, ByVal e As CommandEventArgs)
        pnlread.Visible = True
        pnlnewedit.Visible = False
        pnlnohaydatos.Visible = False
        lblfileId.Text = e.CommandArgument

        Dim connectionString As New SqlConnection(ConfigurationManager.ConnectionStrings("conexion").ToString)
        connectionString.Open()

        Dim commandString As String = ""
        commandString = "	select distinct filesupload.fileId, " &
                        "					typefileupload.typefile, " &
                        "					filesupload.filename, " &
                        "					filesupload.fecha, " &
                        "					filesupload.totalnew, " &
                        "					filesupload.totalupdate, " &
                        "					filesupload.totaldelete " &
                        "			   from filesupload " &
                        "		 inner join typefileupload on typefileupload.typefileId = filesupload.typefileId " &
                        "			  where filesupload.fileId = '" & lblfileId.Text & "' "

        Dim myDataAdapter As New System.Data.SqlClient.SqlDataAdapter(commandString, connectionString)

        Dim myDataSet As New Data.DataSet()
        myDataAdapter.Fill(myDataSet, "filesupload")

        Dim bandhaydatos As Boolean = False
        If myDataSet.Tables("filesupload").Rows.Count > 0 Then
            If Not IsDBNull(myDataSet.Tables("filesupload").Rows(0)("fileId")) Then
                bandhaydatos = True

                txbtypefilelect.Text = myDataSet.Tables("filesupload").Rows(0)("typefile")
                txbdatefilelect.Text = Convert.ToDateTime(myDataSet.Tables("filesupload").Rows(0)("fecha")).ToString("dd/MM/yyyy hh:mm tt")
                txbfilenamelect.Text = myDataSet.Tables("filesupload").Rows(0)("filename")
                txbtotalnewlect.Text = myDataSet.Tables("filesupload").Rows(0)("totalnew")
                txbtotalupdatelect.Text = myDataSet.Tables("filesupload").Rows(0)("totalupdate")
                txbtotaldeletelect.Text = myDataSet.Tables("filesupload").Rows(0)("totaldelete")
            End If
        End If

        connectionString.Close()
    End Sub

    Protected Sub volver(ByVal sender As Object, ByVal e As EventArgs)
        lblfileId.Text = "0"
        hiddenAction.Value = "read"

        Empezar(Me, New EventArgs)
    End Sub
End Class