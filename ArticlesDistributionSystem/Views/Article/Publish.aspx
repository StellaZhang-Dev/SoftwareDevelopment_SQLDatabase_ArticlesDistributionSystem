<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ArticlesDistributionSystem.Models.PublishModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	在线投稿
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>在线投稿</h2>
    <% using(Html.BeginForm()) { %>
        <%: Html.HiddenFor(m => m.ArticleID) %>
        <%: Html.HiddenFor(m => m.AuthorID) %>
       
        <p>作品名称：<%: Model.ArticleTitle %></p>
        <p>作者名称：<%: Model.AuthorName %></p>
        <div>
            <span>作品类别：</span>
            <select name="article_type">
                <% List<ArticlesDistributionSystem.Models.ArticleType> typeList = Model.ArticleTypeList; %>
                <% for (int i=0; i<typeList.Count; i++) { %>
                    <% ArticlesDistributionSystem.Models.ArticleType articleType = typeList.ElementAt(i);%>
                    <option value="<%: articleType.TypeID%>"><%: articleType.TypeName %></option>
                <% } %>
            </select>
        </div>
        <p>作品简介：</p>
        <%: Html.TextAreaFor(m => m.ArticleDescription) %>
        <br /><br />
        <input type="submit" value="投稿" />
    <% } %>
</asp:Content>
