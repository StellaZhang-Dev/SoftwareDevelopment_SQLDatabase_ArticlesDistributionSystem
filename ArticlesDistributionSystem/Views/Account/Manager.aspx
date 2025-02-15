<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ArticlesDistributionSystem.Models.RegisterModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	用户管理
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>用户管理</h2>

    <table>
        <tr>
            <td>用户ID</td>
            <td>用户名</td>
            <td>所属组别</td>
            <td>昵称</td>
            <td>联系方式</td>
            <td>Email</td>
            <td>个人描述</td>
            <td>操作</td>
        </tr>

        <% List<ArticlesDistributionSystem.Models.AccountInfo> allUserInfo = (List<ArticlesDistributionSystem.Models.AccountInfo>)ViewData["AllUserInfo"]; %>
        <% for (int i = 0; i < allUserInfo.Count; i++)
           { %>
        <% ArticlesDistributionSystem.Models.AccountInfo accountInfo = allUserInfo.ElementAt(i); %>
            <tr>
                <td><%: accountInfo.UserID %></td>
                <td><%: accountInfo.UserName %></td>
                <td><%: accountInfo.GroupName %></td>
                <td><%: accountInfo.NickName %></td>
                <td><%: accountInfo.Contact %></td>
                <td><%: accountInfo.Email %></td>
                <td><%: accountInfo.Description %></td>
                <td>
                    <% if (!Page.User.Identity.Name.Equals(accountInfo.UserName))
                       { %>
                    <a href="/Account/ChangeGroup/?user_id=<%: accountInfo.UserID %>&level=<%: ArticlesDistributionSystem.Models.GroupInfo.ExpertLevel %>">设置为评审专家</a>
                    <a href="/Account/ChangeGroup/?user_id=<%: accountInfo.UserID %>&level=<%: ArticlesDistributionSystem.Models.GroupInfo.AdminLevel %>">设置为管理员</a>
                    <a href="/Account/Delete/?user_id=<%: accountInfo.UserID %>">删除</a>
                    <% }
                       else
                       { %>
                       <a href="/Account/Info">更新个人信息</a>
                    <% } %>
                </td>
            </tr>
        <% } %>
    </table>
    <br />
    <hr /><br /><br />
    <% using (Html.BeginForm())
       { %>
        <%: Html.LabelFor(m => m.UserName) %>
        <%: Html.TextBoxFor(m => m.UserName) %>
        <%: Html.LabelFor(m => m.Password)%>
        <%: Html.PasswordFor(m => m.Password) %>
        <input type="hidden" name="level" value="<%: ArticlesDistributionSystem.Models.GroupInfo.AdminLevel %>" />
        <input type="submit" value="添加管理员" />
    <% } %>

    <br />
    <% using (Html.BeginForm())
       { %>
        <%: Html.LabelFor(m => m.UserName) %>
        <%: Html.TextBoxFor(m => m.UserName) %>
        <%: Html.LabelFor(m => m.Password)%>
        <%: Html.PasswordFor(m => m.Password) %>
        <input type="hidden" name="level" value="<%: ArticlesDistributionSystem.Models.GroupInfo.ExpertLevel %>" />
        <input type="submit" value="添加评审专家" />
    <% } %>

</asp:Content>
