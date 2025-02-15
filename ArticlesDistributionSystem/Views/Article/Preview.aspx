<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ArticlesDistributionSystem.Models.ContentModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	文章预览
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>文章预览</h2>

    <div class="preview_publish_state">
        <span>投稿状态：</span>
        <% if (Model.ArticlePublishState == ArticlesDistributionSystem.Models.PublishState.PublishState_UnPublish)
           { %>
        <span>未投稿</span>
        <% } else if (Model.ArticlePublishState == ArticlesDistributionSystem.Models.PublishState.PublishState_Judging) { %>
        <span>审批中</span>
        <% }
           else if (Model.ArticlePublishState == ArticlesDistributionSystem.Models.PublishState.PublishState_Accept)
           { %>
        <span>审批通过</span>
        <% }
           else
           { %>
        <span>审批未通过</span>
        <% } %>

        <% 
            if (Model.ArticlePublishState == ArticlesDistributionSystem.Models.PublishState.PublishState_Accept ||
                Model.ArticlePublishState == ArticlesDistributionSystem.Models.PublishState.PublishState_Refuse)
            { %>
        <% ArticlesDistributionSystem.Models.ArticleJudgingInfo judgingInfo = 
                   (ArticlesDistributionSystem.Models.ArticleJudgingInfo)ViewData["JudgeInfo"]; %>
        <br><span>[专家回复]<%: judgingInfo.ExpertName %>：<%: judgingInfo.ReplyMessage %></span>
        <% } %>
    </div>

    <div class="preview_title">文章标题：<%: Model.Title %></div>
    <div class="preview_posttime">创建时间：<%: Model.PostTime %></div>
    <div class="preview_content">
    <pre>
    <%: Model.Content %>
    </pre>
    </div>

</asp:Content>
