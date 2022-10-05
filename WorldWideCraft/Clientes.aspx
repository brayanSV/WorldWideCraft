<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/template/MenuTemplate.Master" CodeBehind="Clientes.aspx.vb" Inherits="WorldWideCraft.Clientes" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="scriptManager" runat="server"></asp:ScriptManager>

    <asp:HiddenField ID="hiddenAction" runat="server" />
    <asp:Label ID="lblclienteId" runat="server" Visible="false"/>
    <asp:Label ID="lblerror" runat="server" Visible="false"/>

    <div id="actionmenu" class="menu-action">
        <asp:LinkButton runat="server" Text="Nuevo" CommandArgument="news" OnCommand="actionmenu"/>
        <asp:LinkButton runat="server" Text="Editar" CommandArgument="edit" OnCommand="actionmenu" />
        <asp:LinkButton runat="server" Text="Eliminar" CommandArgument="delete" OnCommand="actionmenu" />
        <asp:LinkButton ID="lectura" runat="server" Text="Lectura" CommandArgument="read" OnCommand="actionmenu" />
    </div>

    <asp:Panel runat="server" ID="pnlgridclientes">
        <div class="content-grid">
            <div class="ancho-page-div">
                <div class="centrar">
                    <asp:GridView 
                        ID="gridClientes" 
                        runat="server" 
                        AutoGenerateColumns="false" 
                        AllowPaging="true" 
                        OnPageIndexChanging="OnPageIndexChanging" 
                        PageSize="10"            
                        OnRowDataBound="gridClientes_RowDataBound">
                        <HeaderStyle CssClass="grid-header" BorderColor="#95B3D7" BorderWidth="1px"/>
                        <RowStyle CssClass="grid-item-light" BorderColor="#95B3D7" BorderWidth="1px"/>
                        <AlternatingRowStyle CssClass="grid-item-shadow" BorderColor="#95B3D7" BorderWidth="1px"/>

                        <Columns>
                            <asp:BoundField ItemStyle-Width="150px" DataField="nombre" HeaderText="Cliente" />
                            <asp:BoundField ItemStyle-Width="150px" DataField="nit" HeaderText="Nit" />
                            <asp:BoundField ItemStyle-Width="150px" DataField="telefono" HeaderText="Telefono" />
                            <asp:TemplateField ItemStyle-Width="150px" HeaderText="Acción">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="linkAccion" Text="Ver" CommandArgument='<%# Eval("clienteId") %>' OnCommand="acciongrid" />
                                    <cc1:ConfirmButtonExtender runat="server" ID="Confirmlinkaccion" TargetControlID="linkAccion" ConfirmText="Realmente desea eliminar este cliente?" />
                                </ItemTemplate>
                                <ItemStyle Width="100" VerticalAlign="Middle" HorizontalAlign="Center"/>
                                <HeaderStyle Width="100"/>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlnohayclientes">
        <div style="padding: 40px;">
            <div class="ancho-page-div">
                <div class="centrar nohaydatos">
                    Sin información
                </div>
            </div> 
        </div>             
    </asp:Panel>

        

    <script type="text/javascript">
        var status = document.getElementById('<%= hiddenAction.ClientID%>').value;
        var menu = document.getElementById('actionmenu');

        if (menu !== null) {
            var links = menu.querySelectorAll("a");

            for (var i = 0; i < links.length; i++) {
                links[i].classList.remove("active");

                if (status == "new" && links[i].innerHTML == "Nuevo") {
                    links[i].classList.add("active");
                } else if (status == "edit" && links[i].innerHTML == "Editar") {
                    links[i].classList.add("active");
                } else if (status == "delete" && links[i].innerHTML == "Eliminar") {
                    links[i].classList.add("active");
                } else if (status == "read" && links[i].innerHTML == "Lectura") {
                    links[i].classList.add("active");
                }             
            }
        }

        function ClickLectura() {
            var clickButton = document.getElementById("<%= lectura.ClientID %>");
            clickButton.click();   
        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="content-form">
        <asp:Panel runat="server" ID="pnlnohaydatos">
            <div class="ancho-page-div">
                <div class="centrar nohaydatos">
                    Sin información
                </div>
            </div>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlnewedit">
            <div class="ancho-page-div">
                <div class="centrar forms">
                    <p>
                        <asp:Label runat="server" Text="Cliente" />
                        <asp:TextBox CssClass="styletext" runat="server" id="txbnombre"/>
                    </p>
                    <p>
                        <asp:Label runat="server" Text="Nit" />
                        <asp:TextBox CssClass="styletext" runat="server" id="txbnit" />
                    </p>
                    <p>
                        <asp:Label runat="server" Text="Telefono" />
                        <asp:TextBox CssClass="styletext" TextMode="Number" runat="server"  id="txbtelefono" />
                    </p>

                    <asp:Button CssClass="stylebutton" ID="btneditnew" runat="server" OnClick="NewEditCliente"/>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlread">
            lectura
        </asp:Panel>
    </div>
    
</asp:Content>
