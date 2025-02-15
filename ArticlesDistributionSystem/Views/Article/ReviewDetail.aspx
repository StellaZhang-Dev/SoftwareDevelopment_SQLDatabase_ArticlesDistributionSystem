<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ArticlesDistributionSystem.Models.ArticleJudgingInfo>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	作品评审
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>作品评审</h2>

    <% using(Html.BeginForm()) { %>
    <table>
        <tr>
            <td>审批编号</td>
            <td><%: Model.PublishID %></td>
        </tr>
        <tr>
            <td>作品编号</td>
            <td><%: Model.ArticleUniqueID %></td>
            <td>作者编号</td>
            <td><%: Model.AuthorID %></td>
        </tr>
        <tr>
            <td>作品名称</td>
            <td><%: Model.ArticleTitle %></td>
        </tr>
        <tr>
            <td>作者名称</td>
            <td><%: Model.AuthorName %></td>
        </tr>
        <tr>
            <td>作品简介</td>
            <td><%: Model.ArticleDescription %></td>
        </tr>
    </table>
    <%: Html.HiddenFor(m => m.PublishID) %>
    <input type="radio" name="result" value="1" checked=checked />通过
    <input type="radio" name="result" value="0" />不通过
       <br />
       回复：
       <%: Html.TextAreaFor(m => m.ReplyMessage) %>
       <br />
       <input type="submit" value="提交" />
    <% } %>
</asp:Content>
