# Speed Pay

_Welcome to Speed Pay, your all-in-one solution which manages your budget with ease and style. This web application empowers you to take control of your finances, make seamless money transfers, and stay on top of your transactions._

## Project Description

**Speed Pay** isn't just an app... it's your financial companion. Our web application enables users to effortlessly manage their budget, conduct secure transactions, and keep track of their financial journey. The project prioritizes key features such as user authentication, transaction tracking, transfer management, and even introduces optional advanced features like **Email Verification**, **Multiple Cards** and **Currency Support**.

## Features

1. Upon successful login, a **JWT (JSON Web Token)** is generated and securely stored in a cookie, allowing for hassle-free session management across different parts of the application.

2. **Admin capabilities** for user management and content moderation - enjoy seamless control over user accounts. Easily view and search through a list of all users, empowerment to **Block** or **Unblock** users (a blocked user can access most functionalities, excluding the ability to make transactions), has transaction oversight and control: analyze and filter.

3. **Email Verification** – in order for the registration to be completed, the user must verify their email by clicking on a link send to their email by the application.

4. Security measures taken in mind to safeguard sensitive information and ensure a worry-free financial experience. Advanced **Password Hashing** - login credentials are securely stored, providing a strong defense against unauthorized access. Enhanced **Card Number Protection** - hiding mechanism that displays only the last three digits of your credit or debit card. We successfully maintain the confidentiality of your card information, minimizing the risk of exposure.

5. Register **Multiple Credit/Debit Cards**: When adding funds to your wallet, enjoy the convenience of choosing your funding source.

Maintain various currencies.
Initial deposit to account. Account maintains single currency.
Funds are transfered into accounts currency.
Create, edit, delete transfers and transactions.
Currency is dinamically mapped based on selected cards currencies.
External API to get real data /Currency Exchange Public APIs./
Withdraw money to selected card.
Transaction In and Out functionality (all money movements are transfered either to sender or receivers currency).
All transfers and transactions are reflected into the history.

## Technologies Used

Front-end: HTML, CSS, JavaScript
Back-end: ASP.NET Core, ASP.NET MVC
IDE: Visual Studio 2022
Database: Microsoft SQL Server Management Studio
REST API: ASP.NET Core Web API
Unit Testing: MSTest
Additional nuggets - MailKit, Moq, AutoMapper
Documentation: Swagger

## How to Set Up and Run the Project Locally - follow these steps:

1. Clone the GitLab Repository
2. Install Dependencies and Packages
3. Ensure that you have Microsoft SQL Server Management Studio installed and running on your local machine. 
4. Configure the Connection String - locate the configuration file where the database connection string is stored. It is usually found in a configuration file like appsettings.json. 
	Update the connection string with the appropriate details for your SQL Server database. 
	Provide the server name, database name, connection.=> "DefaultConnection":"Server=....;Database=....;Trusted_Connection=True;"
4. Build and Run the Project - use the appropriate commands to build and run the project.
5. Access the Application - once the project is running locally, you can access the application by opening a web browser and entering the appropriate URL.

## Database Relations



# About the Project 

## Project Hierarchy and Entity Descriptions

| Layer 	  | Class Libraries  | Description                                                                                                |
|-----------------|------------------|------------------------------------------------------------------------------------------------------------|
| 1. Business     | Dto              | Contains Data Transfer Objects used to transfer data between different layers of the application.          |
|       	  | Exceptions       | Includes custom exception classes for handling specific business logic errors or exceptional situations.   |                                                                    
|       	  | Mapper           | Provides mapping functionality to map objects between different layers or models.                          |
|       	  | QueryParameters  | Defines query parameters for filtering, sorting, and pagination in data retrieval operations.              |
|       	  | Services         | Implements business logic and handles the interaction between the presentation layer and data access layer.|                                                        
|      		  | ViewModels       | Contains view models used for presenting data to the user interface.                                       |
|                 |                  |                                                                                                            |
| 2. DataAccess   | Models	     | Contains data models representing the entities stored in the database - User, Category, Post, Comment, etc.|
|		  | Repositories     | Implements the repository pattern to encapsulate data access logic for each model.                         |
|		  |	-Contracts   | - Defines interfaces for the repositories, specifying the available operations.                            |
|		  |	-Context     | - Represents the database context and provides access to the database using an ORM  tool.                  |
|		  |	-Models      | - Contains entity models mapped to database tables.                                                        |
|                 |                  |                                                                                                            |
| 3. Presentation |wwwroot	     | Stores static files such as CSS, JavaScript, and image files used by the presentation layer.               |     
|		  |Controllers       | Handles requests from the client-side and coordinates the flow of data between the layers.                 |
|                 |     -Api         | - Contains controllers that implement the RESTful API endpoints for the application.                       |
|                 |     -MVC         | - Contains controllers that handle server-side rendering of views.                                         |
|		  |Views             | Contains the views responsible for rendering the user interface and presenting data to the end-user.       |
|		  |Helpers           | Provides helper classes or methods that assist in rendering views or performing other related tasks.       |

#### Home Page


### Login/Sign Up Page


### Email Confirmation


### Users Search Page


### Payment Methods Page


### History Page


### Transfer/Transaction Page


### Currencies


### You Refer Friends


### Terms of Use and Privacy Policy 


#### Leadership/Contact us


## Features - MVC

> ### Home page (visible to everyone)
```sh
> The home page displays logo, main navigation bar, top **four** reason to choose using Speed Pay, short explanation of becoming a member in 1-2-3 steps. Кindly remind and prompt for opening a free account.
```

> ### Login/Sign Up page (visible to everyone)
```sh
> You can choose to Log in or Sign up and after that verify your registration by email confirmation.
```
> **In** order to use the features of our virtual wallet, you need an account. 


> ### Users Search page 
```sh
> All users are visible for admins, regular users can see only after filtration by username, email or phone number.
```

> ### History page (visible to )
```sh
> ........................................
```

> ### Transfer/Transaction page (visible to .........)
```sh

- Transfer - ......................

- Transaction - .......................


> ### Currencies page (visible to ........)
```sh
> ................................

```
> ### You Refer Friends (visible to everyone)
```sh
> Provides information about another functionality where a user can enter email of people, not yet registered for the application, and invite them to register. The application sends to that email a registration link. If a registration from that email is completed and verified, both users receive a certain amount in their virtual wallet. Invitations have an expiration time, and a user can take advantage of that feature a limited number of times. 
```
> ### Terms of Use and Privacy Policy (visible to everyone)
```sh
> Provides information about data and privacy.


## Credits

This project was created with the assistance of:

| Contacts | Emails |
| ------ | ------ |
| Marina Toncheva | mar.toncheva@gmail.com |
| Kristian Vaklev | kristian.vaklev@yahoo.com |
| Evelina Hristova | ev.hristova@gmail.com |


## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for detail
