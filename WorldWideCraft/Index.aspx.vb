Imports System.Data.SqlClient

Public Class Index
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("usersesion") IsNot Nothing Then
            Response.Redirect("inicio.aspx")
        End If
    End Sub

    Protected Sub IniciarSesion(ByVal sender As Object, ByVal e As EventArgs)
        lblerror.Text = ""
        lblerror.Visible = False

        If txbUser.Text.Length = 0 Or txbPassword.Text.Length = 0 Then
            lblerror.Visible = True
            lblerror.Text = "Llene todos los campos para continuar"
        Else
            Dim connectionString As New SqlConnection(ConfigurationManager.ConnectionStrings("conexion").ToString)
            connectionString.Open()

            Dim myTrans As SqlTransaction
            Dim bandexito As Boolean = True
            myTrans = connectionString.BeginTransaction()

            Try
                Dim myCommand As New SqlCommand("sp_validateUser", connectionString)
                myCommand.CommandType = CommandType.StoredProcedure

                myCommand.Parameters.Add(New SqlParameter("@Usuario", SqlDbType.VarChar, 50))
                myCommand.Parameters("@Usuario").Value = txbUser.Text

                myCommand.Parameters.Add(New SqlParameter("@Passw", SqlDbType.VarChar, 500))
                myCommand.Parameters("@Passw").Value = txbPassword.Text

                myCommand.Parameters.Add(New SqlParameter("@Patron", SqlDbType.VarChar, 50))
                myCommand.Parameters("@Patron").Value = "admin"

                myCommand.Parameters.AddWithValue("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                Try
                    myCommand.Transaction = myTrans
                    myCommand.ExecuteNonQuery()

                    Dim iReturnValue As Integer = Convert.ToInt32(myCommand.Parameters("@ReturnValue").Value.ToString())

                    If iReturnValue < 0 Then
                        bandexito = False
                        lblerror.Visible = True
                        lblerror.Text = "Usuario o contraseña incorrecta"
                    End If
                Catch ex As Exception
                    bandexito = False
                    lblexeption.Text = "E2: " & ex.Message
                End Try
            Catch ex As Exception
                bandexito = False
                lblexeption.Text = "E1: " & ex.Message
            End Try

            If bandexito = True Then
                myTrans.Commit()
            Else
                myTrans.Rollback()
            End If

            connectionString.Close()

            If bandexito = True Then
                Session("usersesion") = txbUser.Text
                Response.Redirect("inicio.aspx")
            End If
        End If
    End Sub
End Class