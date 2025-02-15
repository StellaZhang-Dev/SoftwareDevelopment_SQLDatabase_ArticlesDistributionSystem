<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ArticlesDistributionSystem.Models.AccountInfo>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Info
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>个人信息</h2>
    <% using (Html.BeginForm())
       { %>

       <%: Html.HiddenFor( m=> m.UserName) %>
       <%: Html.HiddenFor(m => m.GroupName)%>
    <table>
        <tr>
            <td><%: Html.LabelFor(m => m.UserName) %></td>
            <td><%: Model.UserName %></td>
        </tr>
        <tr>
            <td><%: Html.LabelFor(m => m.GroupName) %></td>
            <td><%: Model.GroupName%></td>
        </tr>
        <tr>
            <td><%: Html.LabelFor(m => m.NickName) %></td>
            <td><%: Html.TextBoxFor(m => m.NickName) %></td>
        </tr>
        <tr>
            <td><%: Html.LabelFor(m => m.Email)%></td>
            <td><%: Html.TextBoxFor(m => m.Email) %></td>
        </tr>
        <tr>
            <td><%: Html.LabelFor(m => m.Contact)%></td>
            <td><%: Html.TextBoxFor(m => m.Contact) %></td>
        </tr>
        <tr>
            <td><%: Html.LabelFor(m => m.Description)%></td>
            <td><%: Html.TextBoxFor(m => m.Description) %></td>
        </tr>
    </table>
    <br /><br />
        <input type="submit" value="更新个人信息" />
    <% } %>
</asp:Content>
