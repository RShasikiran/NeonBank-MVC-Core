# 🏦 NeonBank-MVC-Core
### ASP.NET Core MVC Banking Web Application

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=flat-square&logo=microsoftsqlserver)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=flat-square&logo=bootstrap)
![EF Core](https://img.shields.io/badge/EF%20Core-10.0-512BD4?style=flat-square)

NeonBank is a full-stack institutional banking web application built with **ASP.NET Core MVC** and **Microsoft SQL Server**. It features secure authentication, fund transfers, loan management, an admin control panel, and a customer support system.

---

## ✨ Features

### 👤 Customer
- Secure registration & cookie-based login
- Personal dashboard with live account balance
- Real-time fund transfers between accounts
- Full transaction history & account statement
- Loan application submission
- Contact support messaging
- Live corporate announcements feed

### 🛡️ Admin
- Admin control panel with all client accounts
- Total deposits & active client statistics
- Loan approval with direct balance credit
- Post announcements visible to all users
- Customer support inbox management

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core MVC (.NET 10) |
| Language | C# 12 |
| Database | Microsoft SQL Server |
| ORM | Entity Framework Core 10 (Code-First) |
| Auth | ASP.NET Cookie Authentication |
| Frontend | Bootstrap 5.3 + Bootstrap Icons |
| JavaScript | jQuery 3.7 |

---

## 📁 Project Structure

```
NeonBank/
├── Controllers/
│   ├── AccountController.cs    # Register, Login, Logout
│   ├── BankingController.cs    # Transfer, Loans, Admin, Support
│   └── HomeController.cs       # Dashboard & Announcements
├── Data/
│   └── ApplicationDbContext.cs
├── Migrations/                 # EF Core Migrations
├── Models/
│   ├── Account.cs
│   ├── Announcement.cs
│   ├── SupportMessage.cs
│   ├── Transaction.cs
│   └── User.cs
├── Views/
│   ├── Account/                # Login, Register
│   ├── Banking/                # Dashboard, Transfer, Loans...
│   ├── Home/                   # Main dashboard (Index)
│   └── Shared/                 # _Layout, Error
├── wwwroot/                    # Static files (CSS, JS)
├── appsettings.json            # DB Connection String
└── Program.cs                  # App bootstrap & middleware
```

---

## 🚀 Getting Started

### Prerequisites
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (with ASP.NET workload)
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or full)
- [SSMS](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) (recommended)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/YOUR_USERNAME/NeonBank.git
   cd NeonBank
   ```

2. **Open in Visual Studio 2022**

3. **Set your connection string** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=NeonBankingDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```
   > Replace `YOUR_SERVER_NAME` with your SQL Server instance (e.g., `.\SQLEXPRESS` or `localhost`)

4. **Apply database migrations** — open Package Manager Console and run:
   ```
   Update-Database
   ```

5. **Run the project** — press `F5` or click the Run button

6. **Register your first account** at the Register page!

### Creating an Admin Account

After registering, open SSMS and run:
```sql
USE NeonBankingDB;
UPDATE Users SET Role = 'Admin' WHERE Email = 'your@email.com';
```
Log back in — the Admin panel will appear in the navbar.

---

## 🗄️ Database Schema

| Table | Key Columns |
|-------|------------|
| `Users` | UserId, FullName, Email, Password, Role |
| `Accounts` | AccountId, AccountNumber, Balance, UserId (FK) |
| `Transactions` | TransactionId, AccountId (FK), Amount, Type, Description, Date |
| `Announcements` | Id, Title, Content, Priority, PostedDate |
| `SupportMessages` | Id, SenderName, Subject, MessageContent, SentDate, IsRead |

**Relationships:**
- `User` (1) ↔ `Account` (1) — one-to-one, cascade delete
- `Account` (1) ↔ `Transactions` (many) — one-to-many, cascade delete

---

## 🔐 Security Notes

> This project is for educational/portfolio purposes. For production:

- [ ] Replace plain-text passwords with **BCrypt** or **ASP.NET Identity**
- [x] HTTPS enforced via `app.UseHttpsRedirection()`
- [x] CSRF protection via `@Html.AntiForgeryToken()` on all forms
- [x] Role-based authorization with `[Authorize(Roles = "Admin")]`
- [x] Input validation with model data annotations

---

## 🤝 Contributing

Pull requests are welcome! For major changes, please open an issue first.

1. Fork the repository
2. Create your branch: `git checkout -b feature/YourFeature`
3. Commit: `git commit -m 'Add YourFeature'`
4. Push: `git push origin feature/YourFeature`
5. Open a Pull Request

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## 🙏 Acknowledgments

- [Bootstrap 5](https://getbootstrap.com/) — UI framework
- [Bootstrap Icons](https://icons.getbootstrap.com/) — Icon library
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) — ORM
- [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet) — Web framework
