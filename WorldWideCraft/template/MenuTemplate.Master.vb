Imports System.Data.SqlClient

Public Class MenuTemplate
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Session("usersesion") Is Nothing Then
                Response.Redirect("index.aspx")
            Else
                Empezar(Me, New EventArgs)
            End If
        End If
    End Sub

    Protected Sub Empezar(ByVal sender As Object, ByVal e As EventArgs)
        Dim connectionString As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("conexion").ToString)
        connectionString.Open()

        Dim commandString As String = ""
        commandString = "	select distinct Usuarios.usuarioid, " &
                        "					Usuarios.nombre " &
                        "			   from Usuarios with(nolock) " &
                        "			  where usuario = '" & Session("usersesion").ToString & "'"

        Dim myDataAdapter As New SqlDataAdapter(commandString, connectionString)
        Dim myDataSet As New DataSet()
        myDataAdapter.Fill(myDataSet, "Usuarios")

        Dim bandhaymantenimiento As Boolean = False
        If myDataSet.Tables("Usuarios").Rows.Count > 0 Then
            If Not IsDBNull(myDataSet.Tables("Usuarios").Rows(0)("usuarioid")) Then
                lblname.Text = myDataSet.Tables("Usuarios").Rows(0)("nombre")
            End If
        End If

        connectionString.Close()
    End Sub

    Protected Sub CerrarSesion(ByVal sender As Object, ByVal e As EventArgs)
        Session.Remove("usersesion")
        Response.Redirect("index.aspx")
    End Sub
End Class