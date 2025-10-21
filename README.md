💼 Job Application Tracker API

A backend API built with ASP.NET Core Web API to help users manage and monitor the jobs they’ve applied to. It supports user authentication, job tracking, filtering, exporting, and full Swagger documentation.
🧠 Project Brief

You’ve been hired to build a backend API for a job application tracker. This system will help users manage and monitor the jobs they’ve applied to.
📌 Requirements
1. User Authentication

    POST /auth/register → Register a new user

    POST /auth/login → Login and receive JWT token

    All /jobs endpoints are protected and require authentication

2. Job Application CRUD

    POST /jobs → Add a new job application

    GET /jobs → Get all jobs (with pagination & filtering)

    GET /jobs/{id} → Get job by ID

    PUT /jobs/{id} → Update job

    DELETE /jobs/{id} → Delete job

3. Job Model Fields
Field	Type	Description
id	int	Auto-generated unique ID
userId	int	Foreign key to the user
companyName	string	Name of the company
positionTitle	string	Job title
location	string	City or country
status	enum	applied, interview, offer, rejected
appliedDate	date	Date of application
jobLink	string	URL to job posting
salaryExpectation	int	Expected salary
notes	string	Additional notes
resumePath	string	Path or URL to resume
createdAt	datetime	Timestamp of creation
updatedAt	datetime	Timestamp of last update
4. Filtering & Pagination

    Filter by: status, location, appliedDate range

    Pagination: GET /jobs?page=1&limit=10

5. Export Feature

    GET /jobs/export/csv → Export all jobs to CSV

    GET /jobs/export/pdf → Export all jobs to PDF

6. Swagger Documentation

    Swagger UI available at /swagger

    Includes request/response examples and error codes

    Supports JWT token authentication via "Authorize" button

🛠️ Tech Stack

    Backend: ASP.NET Core Web API (C#)

    Database: SQL Server or PostgreSQL

    Authentication: JWT

    Documentation: Swagger

    Deployment: Railway, Render, or Azure

🎯 Bonus Challenges (Optional)

    Role-based access (admin vs user)

    Email notifications when status changes

    Resume file upload

    Dashboard stats (e.g., total applied, interviews, offers)

🧪 Evaluation Criteria

    Clean code and structure

    Proper use of REST principles

    Secure authentication

    Input validation and error handling

    Pagination and filtering logic

    Swagger documentation quality

    Deployment and accessibility

🚀 Setup Instructions
bash