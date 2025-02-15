<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%
    bool isLogin = Convert.ToBoolean(Session["isLogin"]);
    int level = 20; //默认，游客权限
    if (isLogin) 
    {
        if (Session["Level"] != null)
        {
            level = Convert.ToInt32(Session["Level"]);
        }
    }
    if (level <= 20)
    { //游客
%>

<li><%: Html.ActionLink("主页", "Index", "Home")%></li>

<% if (level <= 10)
   { //作家  %>

<li><%: Html.ActionLink("个人信息", "Info", "Account")%></li>
<li><%: Html.ActionLink("在线创作", "Write", "Article")%></li>
<li><%: Html.ActionLink("我的稿件", "MyArticle", "Article")%></li>

<% if (level <= 5)
   { //评审专家  %>

<li><%: Html.ActionLink("审批稿件", "Review", "Article")%></li>

<% if (level == 0)
   { //管理员  %>

<li><%: Html.ActionLink("作品分类管理", "Type", "Article")%></li>
<li><%: Html.ActionLink("用户管理", "Manager", "Account")%></li>

<% } //管理员 %>

<% } //评审专家 %>

<% } //作家 %>

<% } //游客 %>
