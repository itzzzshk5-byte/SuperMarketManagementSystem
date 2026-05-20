# Supermarket Management System
## Deployment Guide
## Course: Visual Programming CS-412

---

## System Requirements
- OS: Windows 7, 8, 10, or 11
- RAM: 2 GB minimum
- Storage: 500 MB free space
- .NET Framework: 4.7.2 or higher

---

## Prerequisites (Install These First)

### 1. Visual Studio 2022 Community (Free)
- Download: https://visualstudio.microsoft.com/
- During install select: .NET Desktop Development

### 2. DB Browser for SQLite (Free)
- Download: https://sqlitebrowser.org/dl/
- Used to view and manage database

---

## Installation Steps

### Step 1: Extract Project
- Extract ZIP file to any location
- Example: C:\Projects\SupermarketManagementSystem

### Step 2: Install NuGet Packages
- Open project in Visual Studio
- Go to: Tools → NuGet Package Manager
- Install: System.Data.SQLite.Core
- Install: BCrypt.Net-Next

### Step 3: Database Setup
- Database file Supermarket.db is included
- Copy it to: ProjectFolder\bin\Debug\
- No additional setup needed!

### Step 4: Run Application
- Press F5 in Visual Studio
- OR double-click SuperMarket.exe in bin\Debug

---

## Default Login Credentials
- Username: admin
- Password: admin123

---

## Database Recreation (If Needed)
1. Open DB Browser for SQLite
2. Create new database: Supermarket.db
3. Go to Execute SQL tab
4. Open and run the database.sql file
5. Click Write Changes
6. Copy database to bin\Debug folder

---

## Troubleshooting

Problem: Database not found error
Solution: Copy Supermarket.db to bin\Debug folder

Problem: NuGet packages missing
Solution: Right-click solution → Restore NuGet Packages

Problem: Application does not start
Solution: Install .NET Framework 4.7.2 from Microsoft website

---

## Project Structure
SuperMarket/
├── Forms/
│   ├── LoginForm.cs
│   ├── MarketForm.cs (Dashboard)
│   ├── ProductsForm.cs
│   ├── CustomersForm.cs
│   ├── OrdersForm.cs
│   └── InventoryForm.cs
├── CDBHelper.cs (Database Connection)
├── database.sql (Database Script)
├── DEPLOYMENT.md (This file)
└── REPOSITORY.txt (GitHub Link)