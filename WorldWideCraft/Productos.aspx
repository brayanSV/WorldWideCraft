<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/template/MenuTemplate.Master" CodeBehind="Productos.aspx.vb" Inherits="WorldWideCraft.Productos" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="scriptManager" runat="server"></asp:ScriptManager>

    <asp:HiddenField ID="hiddenAction" runat="server" />
    <asp:Label ID="lblproductoId" runat="server" Visible="false"/>
    <asp:Label ID="lblerror" runat="server" Visible="false"/>

    <div id="actionmenu" class="menu-action">
        <asp:LinkButton runat="server" Text="Nuevo" CommandArgument="news" OnCommand="actionmenu"/>
        <asp:LinkButton runat="server" Text="Editar" CommandArgument="edit" OnCommand="actionmenu" />
        <asp:LinkButton runat="server" Text="Eliminar" CommandArgument="delete" OnCommand="actionmenu" />
        <asp:LinkButton ID="lectura" runat="server" Text="Lectura" CommandArgument="read" OnCommand="actionmenu" />
    </div>

    <asp:Panel runat="server" ID="pnlgridProductos">
        <div class="content-grid">
            <div class="ancho-page-div">
                <div class="centrar">
                    <asp:GridView 
                        ID="gridProductos" 
                        runat="server" 
                        AutoGenerateColumns="false" 
                        AllowPaging="true" 
                        OnPageIndexChanging="OnPageIndexChanging" 
                        PageSize="10"            
                        OnRowDataBound="gridProductos_RowDataBound">
                        <HeaderStyle CssClass="grid-header" BorderColor="#95B3D7" BorderWidth="1px"/>
                        <RowStyle CssClass="grid-item-light" BorderColor="#95B3D7" BorderWidth="1px"/>
                        <AlternatingRowStyle CssClass="grid-item-shadow" BorderColor="#95B3D7" BorderWidth="1px"/>

                        <Columns>
                            <asp:TemplateField ItemStyle-Width="250px" HeaderText="Foto">
                                <ItemTemplate>
                                    <asp:Image runat="server" Width="200" Height="200" ImageUrl='<%# Eval("rutafoto") %>'/>
                                </ItemTemplate>
                                <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"/>
                            </asp:TemplateField>
                            <asp:BoundField ItemStyle-Width="150px" DataField="nombre" HeaderText="Producto" />
                            <asp:BoundField ItemStyle-Width="150px" DataField="referencia" HeaderText="Referencia" />
                            <asp:BoundField ItemStyle-Width="150px" DataField="descripcion" HeaderText="Descripción" />
                            <asp:TemplateField ItemStyle-Width="150px" HeaderText="Acción">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="linkAccion" Text="Ver" CommandArgument='<%# Eval("ProductoId") %>' OnCommand="acciongrid" />
                                    <cc1:ConfirmButtonExtender runat="server" ID="Confirmlinkaccion" TargetControlID="linkAccion" ConfirmText="Realmente desea eliminar este Producto?" />
                                    
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

    <asp:Panel runat="server" ID="pnlnohayProductos">
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

                if (status == "news" && links[i].innerHTML == "Nuevo") {
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
                    <div>
                        <asp:Label runat="server" Text="Foto del producto" />
                        <p>
                            <asp:RequiredFieldValidator ID="valFileUpload1" ControlToValidate="FileUpload1" runat="server" Display="Dynamic" Font-Bold="True" />
                        </p>

                        <p>                                                    
                            <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="false"  />                                                    
                        </p>
                    </div>
                    
                    <div>
                        <asp:Label runat="server" Text="Producto" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txbnombre" ErrorMessage="*" ForeColor="Red" />

                        <p>                        
                            <asp:TextBox CssClass="styletext" runat="server" id="txbnombre"/>                        
                        </p>
                    </div>
                    
                    <div>
                        <asp:Label runat="server" Text="Referencia" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txbreferencia" ErrorMessage="*" ForeColor="Red" />

                        <p>                            
                            <asp:TextBox CssClass="styletext" TextMode="Number" runat="server" id="txbreferencia" />                        
                        </p>
                    </div>

                    <div>
                        <asp:Label runat="server" Text="Descripción" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txbdescripcion" ErrorMessage="*" ForeColor="Red" />

                        <p>                            
                            <asp:TextBox CssClass="styletext" runat="server"  id="txbdescripcion" />                        
                        </p>
                    </div>                                        

                    <asp:Button CssClass="stylebutton" ID="btneditnew" runat="server" OnClick="NewEditProducto"/>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlread">
            <div class="centrar forms">
                <div>
                    <asp:Label runat="server" Text="Foto del producto" />

                    <p style="text-align:center;">                    
                        <asp:Image CssClass="image-form-lect" runat="server" Width="250" Height="250" ID="imgfotoprodlect"/>
                    </p>
                </div>                
                <p>
                    <asp:Label runat="server" Text="Producto" />
                    <asp:TextBox CssClass="styletext" runat="server" id="txbnombrelect" Enabled="false"/>
                </p>
                <p>
                    <asp:Label runat="server" Text="Referencia" />
                    <asp:TextBox CssClass="styletext" runat="server" id="txbreferencialect" Enabled="false" />
                </p>
                <p>
                    <asp:Label runat="server" Text="Descripción" />
                    <asp:TextBox CssClass="styletext" TextMode="Number" runat="server"  id="txbdescripcionlect" Enabled="false" />
                </p>

                <asp:Button CssClass="stylebutton" Text="Volver" runat="server" OnClick="volver"/>
            </div>
        </asp:Panel>
    </div>    
</asp:Content>
