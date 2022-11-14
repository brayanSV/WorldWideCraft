Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.WebSockets

Public Class Informes
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            lblinformeId.Text = "0"
            hiddenAction.Value = "read"

            Empezar(Me, New EventArgs)
        End If
    End Sub

    Protected Sub Empezar(ByVal sender As Object, ByVal e As EventArgs)
        pnlread.Visible = False
        pnlnohaydatos.Visible = True

        CargarInformes(Me, New EventArgs)
    End Sub

    Protected Sub CargarInformes(ByVal sender As Object, ByVal e As EventArgs)
        Dim connectionString As New SqlConnection(ConfigurationManager.ConnectionStrings("conexion").ToString)
        connectionString.Open()

        Dim commandString As String = ""
        commandString = "	select distinct Informes.informeId, " &
                        "					Informes.nombre " &
                        "			   from Informes " &
                        "			  where Informes.estadoregistroId = '1' "

        Dim myDataAdapter As New System.Data.SqlClient.SqlDataAdapter(commandString, connectionString)

        Dim myDataSet As New Data.DataSet()
        myDataAdapter.Fill(myDataSet, "Informes")

        Dim bandhaydatos As Boolean = False
        If myDataSet.Tables("Informes").Rows.Count > 0 Then
            If Not IsDBNull(myDataSet.Tables("Informes").Rows(0)("informeId")) Then
                bandhaydatos = True

                gridInformes.DataSource = myDataSet.Tables("Informes").DefaultView
                gridInformes.DataBind()
            End If
        End If

        connectionString.Close()

        If bandhaydatos = True Then
            pnlgridInformes.Visible = True
            pnlnohayInformes.Visible = False
        Else
            pnlgridInformes.Visible = False
            pnlnohayInformes.Visible = True
        End If
    End Sub

    Protected Sub OnPageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gridInformes.PageIndex = e.NewPageIndex
        CargarInformes(Me, New EventArgs)
    End Sub

    Protected Sub gridInformes_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            lblerror.Text = hiddenAction.Value
            Dim linkAccion As LinkButton = CType(e.Row.FindControl("linkAccion"), LinkButton)
            Dim Confirmlinkaccion As AjaxControlToolkit.ConfirmButtonExtender = CType(e.Row.FindControl("Confirmlinkaccion"), AjaxControlToolkit.ConfirmButtonExtender)


            Select Case hiddenAction.Value
                Case "read"
                    linkAccion.Text = "ver más"
                    Confirmlinkaccion.Enabled = False
            End Select
        End If
    End Sub

    Protected Sub Acciongrid(ByVal sender As Object, ByVal e As CommandEventArgs)
        lblinformeId.Text = e.CommandArgument
        Dim args As New CommandEventArgs("", lblinformeId.Text)

        Select Case hiddenAction.Value
            Case "read"
                VerMasProducto(Me, args)
        End Select
    End Sub

    Protected Sub ActionMenu(ByVal sender As Object, ByVal e As CommandEventArgs)
        hiddenAction.Value = e.CommandArgument

        If lblinformeId.Text <> "0" Then
            Dim args As New CommandEventArgs("", lblinformeId.Text)

            Select Case e.CommandArgument
                Case "read"
                    VerMasProducto(Me, args)
            End Select

            CargarInformes(Me, New EventArgs)
        Else
            Empezar(Me, New EventArgs)
        End If
    End Sub

    Protected Sub VerMasProducto(ByVal sender As Object, ByVal e As CommandEventArgs)
        pnlread.Visible = True
        pnlnohaydatos.Visible = False
        lblinformeId.Text = e.CommandArgument

        Select Case lblinformeId.Text
            Case "1"
                cargarInformeVentas(Me, New EventArgs)
        End Select
    End Sub

    Protected Sub cargarInformeVentas(ByVal sender As Object, ByVal e As EventArgs)
        Dim connectionString As New SqlConnection(ConfigurationManager.ConnectionStrings("conexion").ToString)
        connectionString.Open()

        Dim commandString As String = ""
        commandString = "	select distinct informeuno.producto, " &
                        "					informeuno.referencia, " &
                        "					informeuno.preciounidad, " &
                        "					informeuno.canttotal, " &
                        "					(informeuno.canttotal * informeuno.preciounidad) as total " &
                        "			   from ( " &
                        "						select distinct InformeVentas.producto, " &
                        "										InformeVentas.referencia, " &
                        "										InformeVentas.preciounidad, " &
                        "										sum(InformeVentas.cantidad) as canttotal " &
                        "								   from InformeVentas " &
                        "							   group by InformeVentas.producto, " &
                        "										InformeVentas.referencia, " &
                        "										InformeVentas.preciounidad " &
                        "					) as informeuno "

        Dim myDataAdapter As New System.Data.SqlClient.SqlDataAdapter(commandString, connectionString)

        Dim myDataSet As New Data.DataSet()
        myDataAdapter.Fill(myDataSet, "informeuno")


        Dim colpreciounidtxt As New DataColumn("preciounidadtxt")
        Dim coltotaltxt As New DataColumn("totaltxt")
        Dim colcantidadtxt As New DataColumn("cantidadtxt")

        myDataSet.Tables("informeuno").Columns.Add(colpreciounidtxt)
        myDataSet.Tables("informeuno").Columns.Add(coltotaltxt)
        myDataSet.Tables("informeuno").Columns.Add(colcantidadtxt)

        If myDataSet.Tables("informeuno").Rows.Count > 0 Then
            If Not IsDBNull(myDataSet.Tables("informeuno").Rows(0)("producto")) Then
                For var1 As Integer = 0 To myDataSet.Tables("informeuno").Rows.Count - 1
                    myDataSet.Tables("informeuno").Rows(var1)("preciounidadtxt") = Convert.ToDecimal(myDataSet.Tables("informeuno").Rows(var1)("preciounidad")).ToString("$#,###")
                    myDataSet.Tables("informeuno").Rows(var1)("totaltxt") = Convert.ToDecimal(myDataSet.Tables("informeuno").Rows(var1)("total")).ToString("$#,###")
                    myDataSet.Tables("informeuno").Rows(var1)("cantidadtxt") = Convert.ToDecimal(myDataSet.Tables("informeuno").Rows(var1)("canttotal")).ToString("#,###")
                Next

                gridInformeUno.DataSource = myDataSet.Tables("informeuno").DefaultView
                gridInformeUno.DataBind()
            End If
        End If

        connectionString.Close()
    End Sub

    Protected Sub OnPageIndexChangingInformeUno(sender As Object, e As GridViewPageEventArgs)
        gridInformeUno.PageIndex = e.NewPageIndex
        cargarInformeVentas(Me, New EventArgs)
    End Sub

    Protected Sub volver(ByVal sender As Object, ByVal e As EventArgs)
        lblinformeId.Text = "0"
        hiddenAction.Value = "read"

        Empezar(Me, New EventArgs)
    End Sub
End Class