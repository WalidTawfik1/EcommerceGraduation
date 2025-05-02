# 🛒 E-Commerce Platform – ASP.NET Core Web API

A scalable, secure, and feature-rich e-commerce back-end system developed as a graduation project using modern web technologies and clean architecture principles. This project simulates a real-world online shopping experience for both customers and administrators, showcasing robust implementation of product management, authentication, payment processing, and more.

---

## 📌 Purpose

To demonstrate the practical application of software engineering concepts, database design, and secure web development practices in building a real-world e-commerce solution for businesses and consumers.

---

## 🧩 Core Features

- 🧑‍💻 **User Authentication**: JWT with cookies, OTP-based account verification, and Google login (OAuth 2.0)
- 🛍️ **Product Catalog**: Categories, subcategories, brands, product details
- 🔍 **Search & Filtering**: Efficient querying with filtering by name, brand, category, etc.
- 🛒 **Cart & Wishlist**: Stored in Redis for high-performance access
- 💳 **Payment Integration**: Paymob payment processing with order tracking and secure callbacks
- ⭐ **Product Reviews**: Users can leave and manage product reviews and ratings
- 🛠️ **Admin Dashboard**: Role-based access for inventory, product, and order management
- ✉️ **Email Notifications**: SMTP-based emails for registration, order confirmation, and password reset
- 🔐 **Security**: HTTPS, input validation, 2FA with OTP, and CORS policy
- 📊 **Scalable Architecture**: Built using Clean Architecture (3-tiered with separation of concerns)

---

## 🏗️ System Architecture

### 1. **Presentation Layer (API)**
- Controllers, DTOs, Middleware, Swagger, Authentication
- Handles all HTTP requests and responses

### 2. **Core Layer**
- Business logic, domain models, interfaces, and services
- Independent of external frameworks and UI

### 3. **Infrastructure Layer**
- Data persistence using EF Core
- Repository + Unit of Work pattern
- Integration with Paymob, Redis, SMTP, and Cloud services

---

## 🧪 Testing

- ✅ Unit Testing: Business logic and services
- 🔄 Integration Testing: API endpoints, authentication, and workflows

---

## 💻 Technologies Used

| Category | Technologies |
|---------|--------------|
| **Backend** | ASP.NET Core 8, Entity Framework Core |
| **Database** | SQL Server |
| **Authentication** | ASP.NET Core Identity, JWT, OTP, Google OAuth |
| **Caching** | Redis |
| **Payment Gateway** | Paymob (X.Paymob.CashIn) |
| **Email** | SMTP with HTML templates |
| **API Tools** | Swagger / OpenAPI |
| **Mapping** | AutoMapper |
| **Others** | DotNetEnv, Newtonsoft.Json, Git |

---

## 🚀 Getting Started

1. Clone the repo
2. Add your configuration in `appsettings.json` and `.env`
3. Run EF Core migrations
4. Launch the project and test using Swagger

---

## 🔗 Links

- [Live Preview (Optional)](https://graduation-project-smarket.vercel.app/)
- [GitHub Repository](https://github.com/WalidTawfik1/EcommerceGraduation)
