# 🛡️ IP Access Manager API

A high-performance RESTful API built with **.NET** to manage and control incoming IP addresses based on their geographical location. Designed using **Clean Architecture** principles to ensure maintainability, scalability, and separation of concerns.

## 🚀 Key Features

* **IP Geolocation Integration:** Resolves incoming IP addresses to their respective countries using the `ipgeolocation.io` external API.
* **Dynamic Access Control:** Block or unblock specific countries from accessing the system.
* **Temporal Blocking:** Temporarily block a country for a specific duration (e.g., 5 minutes) with an automated `BackgroundService` for real-time cleanup.
* **Attempt Auditing:** Comprehensive logging of blocked access attempts, including IP Address, Country Code, Timestamp, and User-Agent.
* **High Performance & Concurrency:** Utilizes a thread-safe `ConcurrentDictionary` for in-memory caching, ensuring ultra-fast lookups and state management without database bottlenecks.
* **Pagination & Search:** Cleanly structured endpoints to retrieve logs and blocked countries with search functionality and pagination support.

## 🏛️ Architecture & Design Patterns

This project strictly adheres to **Clean Architecture** (Domain, Application, Infrastructure, API) to decouple the business logic from external frameworks.
* **Repository Pattern:** Abstracts the data storage logic.
* **Dependency Injection (DI):** Ensures loose coupling and better testability.
* **Singleton Pattern:** Manages the lifecycle of the in-memory data stores.
* **Facade/Adapter Pattern:** Encapsulates the complexity of external HTTP calls.

## 🛠️ Tech Stack

* **Framework:** .NET 8 / C#
* **External API:** `ipgeolocation.io` via `HttpClient`
* **Documentation:** Swagger / OpenAPI

## ⚙️ How to Run Locally

1. **Clone the repository:**

        git clone https://github.com/AmrSameh02/IPAccessManager-API.git
        cd IPAccessManager-API

2. **Configure the API Key:**
   Open `appsettings.json` in the API project and ensure the API key is set. *(Note: A free key is currently left in the config for seamless testing and review purposes).*

        "IpApi": {
          "Key": "YOUR_API_KEY_HERE"
        }

3. **Run the application:**
   Using the .NET CLI:

        dotnet run --project IPAccessManager.API

4. **Explore the API:**
   Navigate to `https://localhost:<port>/swagger` in your browser to interactively test the endpoints.

## 📡 Core Endpoints

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/api/Ip/lookup` | Lookup country details by IP address. |
| `GET` | `/api/Ip/check-block` | Check if the caller's IP is blocked. |
| `POST` | `/api/Countries/block` | Block a specific country permanently. |
| `POST` | `/api/Countries/temporal-block` | Block a country for a specific duration. |
| `GET` | `/api/logs/blocked-attempts` | Retrieve paginated logs of blocked requests. |
