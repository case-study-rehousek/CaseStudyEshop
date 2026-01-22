
# CaseStudyEshop
Modern E-commerce API demonstration.

## 🚀 Project Overview
This project is a robust RESTful API for an E-commerce platform, built with ASP.NET Core. It serves as a demonstration of modern backend development practices, focusing on scalability, maintainability, and clean code architecture.

### 📦 Product Management API (version 1)
* **Comprehensive Listing:** Endpoint to retrieve all available products in the catalog.
* **Flexible Creation:** Supports creating new products with minimal required data (Name and Image URL) to streamline onboarding.
* **Detailed View:** Fetch complete information about a specific product using its unique identifier (ID).
* **Partial Updates (Stock Management):** Optimized endpoint for granular updates, specifically focused on modifying product stock levels without affecting other properties.

### 📦 Product Management API (version 2)
* **Paginated Listing:** Endpoint to retrieve products using advanced pagination.
* **Flexible Creation:** Supports creating new products with minimal required data (Name and Image URL) to streamline onboarding.
* **Detailed View:** Fetch complete information about a specific product using its unique identifier (ID).
* **High-Performance Stock Update (Async)**:    
    * **Mechanism**: Implements a **Producer-Consumer pattern** using an in-memory queue (`System.Threading.Channels`).
    * **Behavior**: Returns `202 Accepted` immediately. The actual database update is processed by a background worker, ensuring high API responsiveness even under heavy load.
 
## 🛠 Tech Stack
* Framework: ASP.NET Core 8.0 (Web API)
* Database: Entity Framework Core with SQLite
* Architecture: Clean Architecture / Layered Architecture
* Versioning: ASP.NET API Versioning (URL-based)
* Mapping: AutoMapper
* Unit tests: xUnit, Moq
* Documentation: Swagger (Swashbuckle) with XML comments

## ⚙️ Data Model
The Product entity is designed to handle essential e-commerce data:
* Name, Description, and Price
* Product Image URL
* Stock Quantity
* Order (list sorting)

## 🛠 Installation and Setup
Follow these steps to get the project running on your local machine:
1. Clone the repository: git clone https://github.com/case-study-rehousek/CaseStudyEshop
2. Open the solution: Launch the .sln file in Visual Studio 2022.
3. Run the application: Press F5 or click the Start button to build and run the project.

## 🧪 Testing
To run the unit tests and verify the functionality:
1. Open the solution in Visual Studio.
2. Open **Test Explorer**.
3. Click **Run All Tests** (or use the shortcut `Ctrl + R, A`).


## 📋 Author
* **Ladislav Řehoušek**