<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	审核稿件
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>待审核稿件</h2>
    <% 
        List<ArticlesDistributionSystem.Models.ArticleJudgingInfo> judgingArticleList = 
            (List<ArticlesDistributionSystem.Models.ArticleJudgingInfo>)ViewData["JudgingArticleList"];
        List<ArticlesDistributionSystem.Models.ArticleJudgingInfo> judgedArticleList =
            (List<ArticlesDistributionSystem.Models.ArticleJudgingInfo>)ViewData["JudgedArticleList"]; 
    %>
    
    <% if (judgingArticleList.Count > 0)
       { %>
    <table>
        <tr>
            <td>投稿编号</td>
            <td>投稿名称</td>
            <td>投稿作者</td>
            <td>作品编号</td>
            <td>作品分类</td>
            <td>投稿简介</td>
            <td>投稿时间</td>
            <td>操作</td>
        </tr>
        <% for (int i = 0; i < judgingArticleList.Count; i++)
            { %>
            <% ArticlesDistributionSystem.Models.ArticleJudgingInfo judgingItem = judgingArticleList[i]; %>
            <tr>
                <td><%: judgingItem.PublishID %></td>
                <td><%: judgingItem.ArticleTitle %></td>
                <td><%: judgingItem.AuthorName %></td>
                <td><%: judgingItem.ArticleTypeUniqueID %></td>
                <td><%: judgingItem.ArticleTypeName %></td>
                <td><%: judgingItem.ArticleDescription %></td>
                <td><%: judgingItem.PublishTime %></td>
                <td><a href="/Article/ReviewDetail/?article_id=<%: judgingItem.ArticleID %>">评审</a></td>
            </tr>
        <% } %>
    </table>
    <% } %>
    


    <h2>我审核的稿件</h2>
    <% if (judgedArticleList.Count > 0)
       { %>
    <table>
        <tr>
            <td>投稿编号</td>
            <td>投稿名称</td>
            <td>投稿作者</td>
            <td>作品编号</td>
            <td>作品分类</td>
            <td>投稿简介</td>
            <td>投稿时间</td>
            <td>审批时间</td>
            <td>操作</td>
        </tr>
        <% for (int i = 0; i < judgedArticleList.Count; i++)
            { %>
            <% ArticlesDistributionSystem.Models.ArticleJudgingInfo judgingItem = judgedArticleList[i]; %>
            <tr>
                <td><%: judgingItem.PublishID %></td>
                <td><%: judgingItem.ArticleTitle %></td>
                <td><%: judgingItem.AuthorName %></td>
                <td><%: judgingItem.ArticleTypeUniqueID %></td>
                <td><%: judgingItem.ArticleTypeName %></td>
                <td><%: judgingItem.ArticleDescription %></td>
                <td><%: judgingItem.PublishTime %></td>
                <td><%: judgingItem.ConfirmTime %></td>
                <td><a href="/Article/ReviewDetailPreview/?article_id=<%: judgingItem.ArticleID %>">查看</a></td>
            </tr>
        <% } %>
    </table>
    <% } %>

</asp:Content>
