<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ArticlesDistributionSystem.Models.ArticleType>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	作品分类管理
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>作品分类管理</h2>
    <table>
        <tr>
            <td>编号</td>
            <td>作品类别</td>
            <td>类型编号</td>
            <td>操作</td>
        </tr>
        <% List<ArticlesDistributionSystem.Models.ArticleType> allTypes = (List<ArticlesDistributionSystem.Models.ArticleType>)ViewData["AllTypes"]; %>
        <% for (int i = 0; i < allTypes.Count; i++)
           { %>
           <% ArticlesDistributionSystem.Models.ArticleType type = allTypes.ElementAt(i); %>
           <tr>
                <td><%: type.TypeID %></td>
                <td><%: type.TypeName %></td>
                <td><%: type.TypeUniqueID %></td>
                <td><a href="/Article/DelType/?type_id=<%: type.TypeID %>">删除</a></td>
           </tr>
        <% } %>
    </table>

    <br />
    
    <% using(Html.BeginForm()) { %>
        <%: Html.LabelFor(m => m.TypeName) %>
        <%: Html.TextBoxFor(m => m.TypeName) %>
        <input type="submit" value="添加作品类别" />
    <% } %> 
</asp:Content>
