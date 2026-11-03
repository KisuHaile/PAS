
# Property Management System (PMS)

## Overview

The **Property Management System (PMS)** is an asset and inventory management application designed to manage organizational properties, track stock, control storage locations, and handle store requisition workflows. The system provides structured processes for receiving items, inspecting them, storing them, and issuing them to staff through controlled approval workflows.

This project follows a **Domain-Driven Design (DDD)** structure and is intended to support scalable enterprise property management.

---

# Project Goals

The main objectives of this system are to:

* Manage organizational **assets and properties**
* Track **inventory stock levels**
* Control **warehouse and shelf storage**
* Process **store requisitions and item issuance**
* Manage **suppliers and receiving processes**
* Support **inspection workflows**
* Provide **role-based access control**
* Maintain **audit trails and activity history**

---

# System Modules

## 1. Authentication & User Management

Handles system access and user permissions.

Features:

* User login and logout
* Role-based access control
* Create and manage users
* Assign roles and permissions
* Activate or deactivate accounts

---

## 2. Location & Storage Management

Defines where properties and inventory items are stored.

Features:

* Create and manage warehouses
* Manage shelf locations
* Define storage hierarchy
* Track item placement in shelves
* Filter items by location

---

## 3. Property (Asset) Management

Manages all organizational properties and assets.

Features:

* Register new properties
* Edit property information
* Track property type and category
* Assign properties to locations
* Link properties to safety boxes
* Search and filter property records
* View detailed property information

Example property fields:

* Tag Number
* Name
* Serial Number
* Property Type
* Purchase Date
* Unit Price
* Quantity
* Location
* Safety Box

---

## 4. Inventory Management

Tracks stock levels and item availability.

Features:

* Maintain inventory stock levels
* Check stock availability
* Record inventory movements
* Generate bin card history
* Monitor minimum stock levels

Key components:

* InventoryStock
* StockLedger
* BinCard

---

## 5. Receiving & Inspection

Handles incoming items from suppliers.

Features:

* Record goods received notes
* Manage supplier information
* Perform item inspection
* Track inspection results
* Release approved items to inventory

Workflow:

1. Supplier delivers items
2. Goods Received Note (GRN) is created
3. Items are inspected
4. Approved items move to stock

---

## 6. Store Requisition & Issuing

Allows staff to request items from the store.

Features:

* Create store requisitions
* Add multiple items to a request
* Approve or reject requests
* Issue approved items
* Generate store issue vouchers

Requisition workflow:

1. Staff submits request
2. Manager approves request
3. Store officer issues items
4. System updates inventory

---

## 7. Workflow & Approval

Controls approval processes for requests.

Features:

* Multi-level approval workflow
* Approver assignment
* Request status tracking
* Approval history

Statuses include:

* Pending
* Approved
* Rejected
* Issued
* Completed

---

## 8. Reporting & Dashboard

Provides system insights and summaries.

Dashboard indicators may include:

* Total properties
* Total warehouses
* Total safety boxes
* Pending requisitions
* Recent system activities
* Low stock alerts

Reports may include:

* Property by location
* Requisition history
* Asset valuation
* Property issuance

---

# System Architecture

The project follows a **Clean Architecture structure**.

```
PMS
│
├── Domain
│   ├── Entities
│   ├── Enums
│   ├── Interfaces
│   ├── ValueObjects
│   └── Common
│
├── Application
│   ├── Services
│   ├── DTOs
│   ├── Commands
│   └── Queries
│
├── Infrastructure
│   ├── Data
│   ├── Repositories
│   └── Persistence
│
└── Presentation
    ├── Controllers
    ├── Views
    └── UI Components
```

---

# Core Domain Entities

Main entities in the system include:

### Catalog

* Category
* ItemMaster

### Storage

* Warehouse
* ShelfLocation
* InventoryStock
* StockLedger

### Receiving

* Supplier
* ReceivingNote
* InspectionLog

### Requisition & Issuing

* ServiceRequest
* SR_Detail
* StoreIssueVoucher

### Property Management

* Property
* PropertyType
* PropertyCategory
* SafetyBox
* SafetyBoxShelf

### User & Staff

* Employee
* UserLogin
* Role
* Permission

---

# Business Rules

Important rules implemented in the domain layer include:

* Requested quantity must be greater than zero
* Issued quantity cannot exceed requested quantity
* Stock must be available before issuing items
* Inspection must pass before items enter inventory
* Each requisition must contain at least one item
* Users can only access features allowed by their role

---

# Technology Stack

The system is designed to be implemented using:

* **.NET 8**
* **ASP.NET Core MVC**
* **Entity Framework Core**
* **SQL Server**
* **GitHub for version control**

---

# Team Collaboration Workflow

Development is managed using **Git and GitHub branching workflow**.

Typical workflow:

1. Fork or clone repository
2. Create feature branch
3. Implement changes
4. Commit and push code
5. Create Pull Request
6. Review and merge to main branch

Example branches:

```
feature/domain-model
feature/requisition-module
feature/inventory-management
feature/user-authentication
```

---

# Future Enhancements

Potential improvements for the system include:

* Advanced reporting dashboards
* Barcode or QR code scanning
* Automated low-stock notifications
* Mobile-friendly interface
* Integration with procurement systems


