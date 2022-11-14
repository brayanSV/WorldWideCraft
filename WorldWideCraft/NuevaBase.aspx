<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/template/MenuTemplate.Master" CodeBehind="NuevaBase.aspx.vb" Inherits="WorldWideCraft.NuevaBase" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hiddenAction" runat="server" />
    <asp:Label ID="lblfileId" runat="server" Visible="false"/>
    <asp:Label ID="lblerror" runat="server" Visible="false"/>

    <div id="actionmenu" class="menu-action">
        <asp:LinkButton runat="server" Text="Nuevo" CommandArgument="news" OnCommand="actionmenu"/>
        <asp:LinkButton ID="lectura" runat="server" Text="Lectura" CommandArgument="read" OnCommand="actionmenu" />
    </div>

    <asp:Panel runat="server" ID="pnlGridCargaExcel">
        <div class="content-grid">
            <div class="ancho-page-div">
                <div class="centrar">
                    <asp:GridView 
                        ID="gridCargaExcel" 
                        runat="server" 
                        AutoGenerateColumns="false" 
                        AllowPaging="true" 
                        OnPageIndexChanging="OnPageIndexChanging" 
                        PageSize="10"            
                        OnRowDataBound="gridCargaExcel_RowDataBound">
                        <HeaderStyle CssClass="grid-header" BorderColor="#95B3D7" BorderWidth="1px"/>
                        <RowStyle CssClass="grid-item-light" BorderColor="#95B3D7" BorderWidth="1px"/>
                        <AlternatingRowStyle CssClass="grid-item-shadow" BorderColor="#95B3D7" BorderWidth="1px"/>

                        <Columns>
                            <asp:BoundField ItemStyle-Width="250px" DataField="filename" HeaderText="Nombre Archivo" />
                            <asp:BoundField ItemStyle-Width="250px" DataField="fechatxt" HeaderText="Fecha" />
                            <asp:BoundField ItemStyle-Width="100px" DataField="typefile" HeaderText="Tipo" />
                            <asp:TemplateField ItemStyle-Width="150px" HeaderText="Acción">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="linkAccion" Text="Ver" CommandArgument='<%# Eval("fileId") %>' OnCommand="acciongrid" />                                                                        
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

    <asp:Panel runat="server" ID="pnlnohayCargaExcel">
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
                } else if (status == "read" && links[i].innerHTML == "Lectura") {
                    links[i].classList.add("active");
                }             
            }
        }

        function ClickLectura() {
            var clickButton = document.getElementById("<%= lectura.ClientID %>");
            clickButton.click();   
        }

        function ErrorSQL(s) {
            console.log(s);  
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
                        <asp:Label runat="server" Text="Tipo de carga" />                        

                        <p>                        
                            <asp:DropDownList runat="server" ID="ddltipoarchivo" />
                        </p>
                    </div>
                    
                    <div>
                        <asp:Label runat="server" Text="Nombre de Archivo" />
                        <p>
                            <asp:RequiredFieldValidator ID="valFileUpload1" ControlToValidate="FileUpload1" runat="server" Display="Dynamic" Font-Bold="True" />
                        </p>

                        <p>                                                    
                            <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="false"  />                                                    
                        </p>
                    </div>
                                    
                    <asp:Button CssClass="stylebutton" ID="btneditnew" runat="server" OnClick="NewEditProducto"/>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlread">
            <div class="centrar forms">
                <p>                    
                    <asp:Label runat="server" Text="Tipo de carga" />
                    <asp:TextBox CssClass="styletext" runat="server" id="txbtypefilelect" Enabled="false"/>
                </p>               
                <p>
                    <asp:Label runat="server" Text="Nombre de Archivo" />
                    <asp:TextBox CssClass="styletext" runat="server" id="txbfilenamelect" Enabled="false"/>
                </p>
                <p>
                    <asp:Label runat="server" Text="Fecha Carga" />
                    <asp:TextBox CssClass="styletext" runat="server" id="txbdatefilelect" Enabled="false"/>
                </p>
                <p>
                    <asp:Label runat="server" Text="Total Nuevos" />
                    <asp:TextBox CssClass="styletext" runat="server" id="txbtotalnewlect" Enabled="false" />
                </p>
                <p>
                    <asp:Label runat="server" Text="Total Actualizados" />
                    <asp:TextBox CssClass="styletext" TextMode="Number" runat="server"  id="txbtotalupdatelect" Enabled="false" />
                </p>
                <p>
                    <asp:Label runat="server" Text="Total Eliminados" />
                    <asp:TextBox CssClass="styletext" TextMode="Number" runat="server"  id="txbtotaldeletelect" Enabled="false" />
                </p>

                <asp:Button CssClass="stylebutton" Text="Volver" runat="server" OnClick="volver"/>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
