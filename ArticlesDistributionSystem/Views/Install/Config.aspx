<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ArticlesDistributionSystem.Models.WebSiteConfigModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	系统安装向导
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>系统安装向导</h2>
    <p>填写好数据库信息，点击确认开始配置安装服务器。</p>
     <% using (Html.BeginForm()) { %>
        <%: Html.ValidationSummary(true, "连接数据库不成功，请重试。") %>
        <div>
            <fieldset>
                <legend>数据库信息</legend>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.DatabaseServer) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.DatabaseServer)%>
                </div>

                <%: Html.LabelFor(m => m.WindowsAuth) %>
                <%: Html.CheckBoxFor(m => m.WindowsAuth)%>
                
                <%: Html.LabelFor(m => m.SQLSERVERAuth) %>
                <%: Html.CheckBoxFor(m => m.SQLSERVERAuth)%>

                <div class="editor-label">
                    <%: Html.LabelFor(m => m.Username) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.Username)%>
                </div>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.Password)%>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.Password)%>
                </div>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.DatabaseName) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.DatabaseName)%>
                </div>

                <p>
                    <input type="submit" value="确定" />
                </p>
            </fieldset>
        </div>
    <% } %>

</asp:Content>
