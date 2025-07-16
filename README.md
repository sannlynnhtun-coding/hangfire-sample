# Hangfire Job Management with ASP.NET Core Minimal API

This project is a practical demonstration of how to manage **Hangfire** recurring jobs using an **ASP.NET Core Minimal API**. It provides a set of RESTful endpoints to create, view, update, and delete jobs. The application uses **LiteDB** as a lightweight, file-based storage solution for Hangfire and integrates the **CronExpressionDescriptor** library to provide human-readable feedback for cron expressions.

A key feature of this sample is the ability to programmatically stop and restart the Hangfire server instance, allowing for full control over background job processing directly from the API.

-----

## ✨ Features

  * **Full Job CRUD:** Endpoints for creating, viewing, updating, and deleting recurring jobs.
  * **Server Control:** Stop and restart all Hangfire background processing services via API calls.
  * **Cron Expression Validation:** An endpoint to test a cron expression and receive a human-readable description (e.g., "Every 5 minutes").
  * **Lightweight Storage:** Uses **LiteDB** for simple, serverless Hangfire data storage.
  * **Built-in Dashboard:** Includes the standard Hangfire Dashboard for visual monitoring.

-----

## 🛠️ Technologies Used

  * **.NET 8**
  * **ASP.NET Core Minimal API**
  * **Hangfire.AspNetCore**
  * **Hangfire.LiteDB**
  * **CronExpressionDescriptor**

-----

## 🚀 Getting Started

1.  **Clone the repository:**
    ```bash
    git clone <your-repository-url>
    ```
2.  **Navigate to the project directory:**
    ```bash
    cd <project-directory>
    ```
3.  **Restore NuGet packages:**
    ```bash
    dotnet restore
    ```
4.  **Run the application:**
    ```bash
    dotnet run
    ```

The API will be available at `http://localhost:5053` (or another port specified in your `launchSettings.json`).

The Hangfire Dashboard is accessible at `http://localhost:5053/hangfire`.

-----

## ⚙️ API Endpoints

You can use the provided `.http` file in Visual Studio or VS Code (with the REST Client extension) to test the endpoints.

| Method | Endpoint                    | Description                                         |
| :----- | :-------------------------- | :-------------------------------------------------- |
| `POST` | `/jobs/create`              | Creates or updates a recurring job.                 |
| `GET`  | `/jobs`                     | Retrieves a list of all recurring jobs.             |
| `PUT`  | `/jobs/update`              | Updates the cron expression for an existing job.    |
| `DELETE`|`/jobs/delete/{jobId}`      | Deletes a recurring job by its ID.                  |
| `GET`  | `/jobs/test-cron`           | Describes a given cron expression.                  |
| `POST` | `/jobs/stop-all`            | Stops the Hangfire server from processing jobs.     |
| `POST` | `/jobs/restart-all`         | Restarts the Hangfire server.                       |