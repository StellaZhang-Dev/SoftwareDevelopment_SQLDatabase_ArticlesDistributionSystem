<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ArticlesDistributionSystem.Models.ContentModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	在线创作
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>在线创作</h2>
    <% using (Html.BeginForm()) {  %>
        <%: Html.HiddenFor(m => m.ArticleID) %>
        <%: Html.HiddenFor(m => m.AuthorID) %>
        <%: Html.HiddenFor(m => m.UniqueID) %>
        <%: Html.HiddenFor(m => m.PostTime) %>
        <div class="write_title">
            <%: Html.LabelFor(m => m.Title) %>
            <%: Html.TextBoxFor(m => m.Title)  %>
        </div>
        <div class="write_content">
            <%: Html.LabelFor(m => m.Content)%>
            <%: Html.TextAreaFor(m => m.Content) %>
        </div>

        <input type="submit" value="完成" />

    <% } %>
</asp:Content>
