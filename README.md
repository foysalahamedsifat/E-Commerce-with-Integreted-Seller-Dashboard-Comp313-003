# E-Commerce-with-Integreted-Seller-Dashboard-Comp313-003

$Architecture
### **🛠 E-Commerce Project Architecture Overview**

## **1️⃣ Presentation Layer (Frontend - React.js)**
- Built using **React.js** and **Bootstrap** for a responsive UI.
- Communicates with the backend using **Axios (REST API)**.
- Handles client-side routing with **React Router**.
- State management via **React Context API** (or Redux if needed).

### **Key Components:**
- **Home Page:** Displays product listings.
- **Product Details Page:** Shows product details and allows adding to cart.
- **Cart Page:** Manages user cart items.
- **Checkout Page:** Processes orders.
- **Authentication Pages:** Login/Register functionality.
- **Admin Dashboard:** products, orders, and analytics.

---

## **2️⃣ Business Logic Layer (Backend - ASP.NET Core 8)**
- Built using **ASP.NET Core 8 Web API**.
- Implements **JWT Authentication** with role-based access control.
- Uses **Entity Framework Core** for database interactions.
- Provides **RESTful API endpoints** for CRUD operations.

### **Core API Endpoints:**
- **Authentication:** `/api/auth/register`, `/api/auth/login`
- **Product Management:** `/api/products`
- **Cart Management:** `/api/cart/add`, `/api/cart/remove`
- **Order Processing:** `/api/orders/place`, `/api/orders/{id}`
- **Admin Analytics:** `/api/admin/analytics`

---

## **3️⃣ Data Layer (Database - SQL Server)**
- **Microsoft SQL Server** is used for data storage.
- Uses **Entity Framework Core (EF Core)** for ORM mapping.
- **Migrations** manage schema updates.

### **Database Tables:**
1. **Users** (Manages authentication and roles)
2. **Products** (Stores product information)
3. **Cart** (Tracks user-selected items before checkout)
4. **Orders** (Stores order information)
5. **OrderDetails** (Line items for orders)
#Database design 
Entities & Relationships
Users (Customers & Admins)

A user can place multiple orders.
A user can have multiple items in their cart.
Products

Products are stored in the catalog.
Products are added to the cart before purchase.
Cart

Users can add multiple products to their cart.
A cart belongs to a single user.
Orders & OrderDetails

An order is placed by a user.
An order can contain multiple products.
Each product in an order has a quantity.

![image](https://github.com/user-attachments/assets/1f20cd22-4122-4356-8699-ea946e96f33b)
---

## **🔄 API Request Flow**
1. **Frontend (React.js)** makes API requests via **Axios**.
2. **Backend (ASP.NET Core 8 Web API)** processes the request and interacts with the **SQL Server database**.
3. **Responses** are sent back to the frontend, updating the UI dynamically.

---
## **🚀 Installation & Setup Guide**

### **1️⃣ Clone the Repository**
```bash
git clone https://github.com/yourusername/ecommerce-app.git
cd ecommerce-app
```

---

## **🖥 Backend Setup (ASP.NET Core 8)**
### **2️⃣ Configure SQL Server Database**
- Ensure you have **SQL Server** installed.
- Update the **connection string** in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=your_server;Database=EcommerceDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### **3️⃣ Run Migrations**
```bash
cd backend
dotnet ef database update
```

### **4️⃣ Run the API Server**
```bash
dotnet run
```
- The API will be available at: **`http://localhost:5000`**

---

## **🌐 Frontend Setup (React.js)**
### **5️⃣ Install Dependencies**
```bash
cd frontend
npm install
```

### **6️⃣ Configure API Base URL**
- Update `src/config.js`:
```javascript
export const API_BASE_URL = "http://localhost:5000/api";
```

### **7️⃣ Start the React App**
```bash
npm start
```
- The React app will run on: **`http://localhost:3000`**

---

## **📉 Features**
- ✅ User Authentication (Register/Login with JWT)
- ✅ Browse Products & Add to Cart
- ✅ Checkout & Place Orders
- ✅ Admin Dashboard (Product Management, Sales Analytics)
- ✅ Secure API with Role-Based Access

---

## **📝 API Documentation**
| Method | Endpoint | Description |
|--------|---------|------------|
| **POST** | `/api/auth/register` | Register a new user |
| **POST** | `/api/auth/login` | Authenticate user |
| **GET** | `/api/products` | Get product list |
| **POST** | `/api/cart/add` | Add item to cart |
| **POST** | `/api/orders/place` | Place an order |
| **GET** | `/api/admin/analytics` | Get sales reports |


