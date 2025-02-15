# Articles Distribution System

## Overview

Articles Distribution System is a manuscript management system developed using ASP.NET MVC. This system aims to streamline the manuscript submission and review process, reducing time and effort for both authors and publishers. It allows authors to submit their articles online, track their submission status, and receive notifications upon acceptance. Editors can efficiently manage submissions, review manuscripts, and approve content for publication.

This project was originally developed as an academic thesis project over a decade ago, using **Visual Studio 2010** and **SQL Server 2005**, with C# as the primary programming language. Over the years, it has undergone multiple minor modifications to improve compatibility with modern environments, including optimizations for macOS.

The system follows a structured and modular development approach, integrating essential components such as user authentication, role-based access control, article management, and administrative tools.

## Features

The system provides four main functional areas:

1. **User Management**
   - User registration and authentication
   - Role-based access control (Admin, Editor, Author)
   - Password recovery via email

2. **Manuscript Submission and Review**
   - Online article submission
   - Real-time tracking of submission status
   - Editorial review process
   - Approval and rejection management

3. **Content Management**
   - Article categorization
   - Search and filter functionalities
   - Secure storage of articles

4. **Communication & Notifications**
   - Internal messaging system
   - Email notifications for submission updates
   - Interactive commenting on articles

## System Architecture

The system is structured into the following components:

- **Controllers**: Manages requests and responses.
- **Models**: Defines data structures and business logic.
- **Views**: Provides the user interface for interaction.
- **Database**: Stores user data, article submissions, and system configurations.

### Directory Structure

```
ArticlesDistributionSystem/
├── Controllers/
│   ├── AccountController.cs
│   ├── ArticleController.cs
│   ├── HomeController.cs
│   ├── InstallController.cs
│
├── Models/
│   ├── AccountModels.cs
│   ├── ContentModel.cs
│   ├── DataBase.cs
│   ├── GroupModels.cs
│   ├── InstallModels.cs
│   ├── PublishModel.cs
│
├── Views/
│   ├── Account/
│   ├── Article/
│   ├── Home/
│   ├── Install/
│   ├── Shared/
│
├── Scripts/
│   ├── jquery-1.4.1.min.js
│   ├── jquery.validate.js
│   ├── MicrosoftAjax.js
│
├── Content/
│   ├── MyCss.css
│   ├── Site.css
│
├── App_Data/
│   ├── ASPNETDB.MDF
│   ├── aspnetdb_log.ldf
│
├── Web.config
├── Web.Debug.config
├── Web.Release.config
├── ArticlesDistributionSystem.sln
```

## Technologies Used

- **Programming Language**: C# (ASP.NET MVC)
- **Frontend**: HTML, CSS, JavaScript, jQuery
- **Database**: SQL Server 2005
- **Development Tools**: Visual Studio 2010

## Installation & Setup

### Prerequisites

- **Visual Studio 2010** or later
- **SQL Server 2005** (or newer)
- **.NET Framework 4.0**

### Steps to Set Up the Project

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/ArticlesDistributionSystem.git
   cd ArticlesDistributionSystem
   ```
2. Open `ArticlesDistributionSystem.sln` in Visual Studio.
3. Configure the database connection in `Web.config`:
   ```xml
   <connectionStrings>
       <add name="DefaultConnection"
            connectionString="Data Source=YOUR_SERVER;Initial Catalog=ASPNETDB;Integrated Security=True"
            providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```
4. Run the project using Visual Studio (`Ctrl + F5`).

## Database Structure

The system contains several key database tables:

- **UserTable**: Stores user authentication details.
- **UserInfoTable**: Stores user profile information.
- **ArticleTable**: Stores submitted articles.
- **PublishStatusTable**: Tracks the publication status of articles.
- **ArticleTypeTable**: Stores article categories.
- **GroupTable**: Manages user roles and permissions.

## Functionality Breakdown

### **1. User Registration & Authentication**
- Users can register using their email and login credentials.
- Authentication is managed using a secure login system.
- Administrators can create and manage user accounts.

### **2. Article Submission & Review**
- Authors can submit articles with titles, content, and category selection.
- Editors review submissions and provide feedback.
- Approved articles are published and accessible to users.

### **3. Manuscript Browsing & Search**
- Users can browse published articles.
- Advanced search filters are available based on article type and keywords.

### **4. Editorial Review Process**
- Editors can approve, reject, or request modifications on submissions.
- Authors are notified of editorial decisions via email.

### **5. User Interaction & Messaging**
- Internal messaging system allows communication between authors and editors.
- Users receive notifications for updates on their submissions.

## Future Enhancements

The system is designed with scalability in mind. Potential improvements include:

- **Integrating AI for automated plagiarism detection.**
- **Implementing REST APIs for external integrations.**
- **Enhancing UI with modern frontend frameworks (React/Vue).**
- **Adding support for multiple file formats (PDF, DOCX) for manuscript submission.**

## License

This project is licensed under the **Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International (CC BY-NC-SA 4.0)** License.

- **You are free to:**
  - Share — Copy and redistribute the material in any medium or format.
  - Adapt — Remix, transform, and build upon the material.

- **Under the following terms:**
  - **Attribution** — You must give appropriate credit and indicate if changes were made.
  - **NonCommercial** — You may not use the material for **commercial purposes**.
  - **ShareAlike** — If you remix, transform, or build upon the material, you must distribute your contributions under the same license.

📜 Full License Text: [Creative Commons BY-NC-SA 4.0](https://creativecommons.org/licenses/by-nc-sa/4.0/)




