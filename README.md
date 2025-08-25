# TypicalTechTools_NET8

This project was developed for **Typical Tech Tools**, a client of Uptown IT, as part of an integration and security upgrade. The goal was to migrate the existing file-based website into a **database-driven system** and to implement modern **application security features** (authentication, authorization, encryption, sanitisation).

## Features Implemented

### Database Integration
- Migrated existing product and comment data into a relational database (SQL Server).
- Display of all products from the database.
- Added product management features:
  - Add new products  
  - Update product prices  
  - Store `updated_date` for modified products  

### Comments Functionality
- View all comments for a product.
- Add new comments (with `created_date`).
- Users can edit or delete their **own recent comments**.
- Admin can delete comments at any time.

### Authentication & Authorization
- **Login/Logout system** implemented using ASP.NET Identity.  
- Protected resources accessible **only after login**.  
- **Role-based authorization**:
  - *Admin*: manage products, delete any comments.  
  - *User*: add/edit/delete their own comments, view products.  
  - *Guest*: view products only.  

### Security Features
- **Authentication** (basic + role-based).  
- **Authorization** to restrict sensitive operations.  
- **Encryption/Decryption** of uploaded files (AES), e.g. warranty documents.  
- **Input sanitisation** with HtmlSanitizer to prevent XSS.  
- **SQL Injection prevention** through Entity Framework Core and parameterised queries.  
- Configured **Content-Security-Policy** and environment profiles for safe deployment.

### Testing
- Verified access control (authenticated vs unauthenticated, authorized vs unauthorized).  
- Verified encryption/decryption (file integrity preserved).  
- Attempted SQL Injection (blocked).  
- Attempted script injection in comments (blocked).  

## Tech Stack
- **.NET 8** — ASP.NET Core MVC  
- **Entity Framework Core** — SQL Server  
- **ASP.NET Identity** — Authentication & Authorization  
- **HtmlSanitizer**, CSP — Input/Output protection  
- **AES Encryption** for file handling  

## Getting Started

1. **Clone the repository**
   ```bash
   git clone https://github.com/<your-username>/TypicalTechTools_NET8.git

2. **Database setup**
   - Update `appsettings.Development.json` with your SQL Server connection string:
     ```json
     {
       "ConnectionStrings": {
         "DefaultConnection": "Server=.;Database=TypicalTechTools;Trusted_Connection=True;TrustServerCertificate=True"
       }
     }

3. **Apply migrations & run**
   ```bash
   dotnet ef database update
   dotnet run

4. **Login**
   - Register a new user.  
   - Assign `Admin` role via seeding or database update for admin features.  

## Project Structure
```
/Controllers
/Models
/Views
/DataAccess
/Migrations
/Services
/wwwroot (css, js, images, uploads)
Program.cs
appsettings.json 

