<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ArticlesDistributionSystem.Models.ArticleJudgingInfo>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	作品评审信息
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>作品评审信息</h2>
    <table>
        <tr>
            <td>审批编号</td>
            <td><%: Model.PublishID %></td>
            <td>审批时间</td>
            <td><%: Model.ConfirmTime %></td>
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
            <td>审批专家</td>
            <td><%: Model.ExpertName %></td>
        </tr>
        <tr>
            <td>作品简介</td>
            <td><%: Model.ArticleDescription %></td>
        </tr>
        <tr>
            <td>专家回复</td>
            <td><%: Model.ReplyMessage %></td>
        </tr>
    </table>
</asp:Content>
