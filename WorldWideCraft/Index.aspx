<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Index.aspx.vb" Inherits="WorldWideCraft.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Login</title>
    <link rel='stylesheet' type='text/css' media='screen' href='css/main.css' />
</head>
<body>   
    <form id="form1" runat="server">
        <asp:Label runat="server" ID="lblexeption" />

        <div class="content">
            <div class="content-login">
                <div class="content-item item-form-left">
                    <h2>Login</h2>
                    <asp:Label CssClass="subtitulo" runat="server" Text="Inicie sesion con su usuario y contraseña" />

                    <div>
                        <asp:Label CssClass="label-form" runat="server" Text="Usuario" />
                        <asp:TextBox CssClass="form-textbox" ID="txbUser" runat="server" />                        
                    </div>                    
                    <div>
                        <asp:Label CssClass="label-form" runat="server" Text="Password" />
                        <asp:TextBox CssClass="form-textbox" ID="txbPassword" runat="server" TextMode="Password"/>
                    </div>
                    <div>
                        <asp:Label id="lblerror" Text="prueba" CssClass="message-error" runat="server" Visible="false" />
                    </div>
                    <div>
                        <asp:Button CssClass="button-login" runat="server" Text="Login"  OnClick="IniciarSesion" />
                    </div>
                </div>
                <div class="content-item">
                    <div class="item-title-right">
                        <div>
                            <span class="title-welcome">
                                Welcome Back
                            </span>
                            <br/>
                            <span class="title-company">
                                Worldwide Craft
                            </span>
                        </div>
                    </div>
                    <div class="item-image-right">
                        <img src="img/imagen_logo.png" />
                    </div>
                </div>
            </div>        
        </div>
    </form>
</body>
</html>
