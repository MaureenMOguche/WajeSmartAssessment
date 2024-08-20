# WajeSmartAssessment

Welcome to the my submission for the WajeSmart Backend Developer Assessment.'

## Before You Begin

To set up and run the application locally, follow these steps:

### Prerequisites

    - You must have docker/docker-compose installed on your PC.

1. **Make Sure Docker Deamon is running**

2. **Clone this repository to your local machine**

3. **Navigate to the root folder of the project**

4. **Run the Application**
   - Open a new terminal in the root directory and run `docker compose up db`
   - This would spin up an instance of a Mssql database.
   - Once the database fully comes up, open another terminal window, navigate to WajeSmartAssessment.Infrastructure, and run
   ```
        dotnet ef database update --connection "Server=localhost,1433;Database=WajeSmart;User Id=sa;Password=Password123@;TrustServerCertificate=True;"
   ```
   - This would update the migrations for the database running in the docker file (and persisted on your local machine)
   - Once migration is complete, run `docker compose up --build -d` to start the application.
   - Confirm that both the application and the database is running. Check this by running `docker ps`.
   - If amongst the running containers, you see `waje.db` and `waje.blogapi`, then you are good to go and can access the application at `http://localhost:5000`
   - If you can only see `waje.db` or none, kindly run `docker compose up --build -d` again.
   - You can now view the application on `http://localhost:5000`.

## Important Notes

- After running the command, two containers will start: `waje.db` and `waje.blogapi`.
- Access the Swagger documentation at [Swagger](http://localhost:5000/swagger/index.html).
- Send requests to the API at http://localhost:5000/api/v1.

# About the Application\*\*

This application is a simple blogging system that allows an admin user to create blogs (by entering blog title and url). Authors (other users) can then register and begin to make posts on the different blogs.

## Feature List

- Admin

  - Login with predefined credentials (username: superadmin, password: Password@123)
  - Create Blogs
  - Disable or Enable Authors from making posts
  - View List of blogs
  - View list of authors

- Authors
  - Register/Login
  - View list of blogs
  - make posts (including media files)
  - Comment on posts
  - Like posts
