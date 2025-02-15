<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ArticlesDistributionSystem.Models.ContentModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	我的稿件
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>我的稿件</h2>
    <style>
    .my_article td 
    {
        height: 48px;
        font-size: 18px;
    }
    </style>
    <table class="my_article">
        <tr>
            <td>序号</td>
            <td>编号</td>
            <td>文章标题</td>
            <td>创建时间</td>
            <td>作品编号</td>
            <td>投稿状态</td>
            <td>操作</td>
        </tr>
        <% List<ArticlesDistributionSystem.Models.ContentModel> list = (List<ArticlesDistributionSystem.Models.ContentModel>)ViewData["AllContent"]; %>
    <% for (int i = 0; i < list.Count; i++)
       { ArticlesDistributionSystem.Models.ContentModel item = list.ElementAt(i); %>

       <tr>
            <td><%: (i + 1) %></td>
            <td><%: item.ArticleID %></td>
            <td><%: item.Title %></td>
            <td><%: item.PostTime %></td>
            <td><%: item.UniqueID %></td>
            <% if (item.ArticlePublishState == ArticlesDistributionSystem.Models.PublishState.PublishState_UnPublish)
               { %>
               <td>未投稿</td>
            <% }
               else if (item.ArticlePublishState == ArticlesDistributionSystem.Models.PublishState.PublishState_Judging)
               { %>
               <td>评审中</td>
               <%}
               else if (item.ArticlePublishState == ArticlesDistributionSystem.Models.PublishState.PublishState_Accept)
               { %>
               <td>发表通过</td>
               <% }
               else
               { %>
               <td>发表未通过</td>
               <% } %>
            
            <td>
                <a href="/Article/Preview/?article_id=<%: item.ArticleID %>">预览</a>
                <% if (item.ArticlePublishState == ArticlesDistributionSystem.Models.PublishState.PublishState_UnPublish)
                   { %>
                <a href="/Article/Write/?article_id=<%: item.ArticleID %>">修改</a>
                <a href="/Article/Publish/?article_id=<%: item.ArticleID %>">投稿</a>
                <% } %>
                <a href="/Article/Delete/?article_id=<%: item.ArticleID %>">删除</a>
            </td>
       </tr>

    <% } %>
    </table>
    
</asp:Content>
