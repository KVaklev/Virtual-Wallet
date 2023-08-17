# Speed Pay

_Welcome to Speed Pay, your all-in-one solution which manages your budget with ease and style. This web application empowers you to take control of your finances, make seamless money transfers, and stay on top of your transactions._

## Project Description

**Speed Pay** isn't just an app... it's your financial companion. Our web application enables users to effortlessly manage their budget, conduct secure transactions, and keep track of their financial journey. The project prioritizes key features such as user authentication, transaction tracking, transfer management, and even introduces optional advanced features like **Email Verification**, **Multiple Cards** and **Currency Support**.

## Features

1. Upon successful login, a **JWT (JSON Web Token)** is generated and securely stored in a cookie, allowing for hassle-free session management across different parts of the application.

2. **Admin capabilities** for user management and content moderation - enjoy seamless control over user accounts. Easily view and search through a list of all users, empowerment to **Block** or **Unblock** users (a blocked user can access most functionalities, excluding the ability to make transactions), has transaction oversight and control: analyze and filter.

3. **Email Verification** – in order for the registration to be completed, the user must verify their email by clicking on a link send to their email by the application.

4. Security measures taken in mind to safeguard sensitive information and ensure a worry-free financial experience. Advanced **Password Hashing** - login credentials are securely stored, providing a strong defense against unauthorized access. Enhanced **Card Number Protection** - hiding mechanism that displays only the last three digits of your credit or debit card. We successfully maintain the confidentiality of your card information, minimizing the risk of exposure.

5. Register **Multiple Credit/Debit Cards**: When adding funds to your wallet, enjoy the convenience of choosing your funding source.Speed Pay allows users to add multiple cards operating with different currencies.

6. **Initial funding of a digital wallet** -  during the registration process each user can easily opt for the currency of his/her own account. Eqach account can be set in a single currency.

7. **Transfers and Transactions** - these are the money movement operations that Speed Pay maintains as follows:

- Transfers - these could be either deposit or withdrawal respectively when a particular user would like to send funds to his account from a selected card or to withdraw funds back to the desired card of       his list with cards. Currency field is dinamically mapped based on selected card's currency. Each transfer amount is exchanged into its destination's currency in case the currencies between the selected card and the account differ.

- Transactions - these cound be either "In" or "Out" transasactions depending on money movement direction. In case a user wants to send money to another user this will be "Out" transaction for the sender and "In" transaction for the receiver of the funds. Here again each transaction is exchanged into the currency of receiver's account.

Crud operations for money transfers and transactions are all covered and each user can easily - create, read, edit and delete those. NB: Once a particular transfer or transaction is either processed or cancelled it can no longer be updated.

All completed transactions and transfers are stored in the history whereas in the dashbo

8. **External API** - when it comes to exchanging funds described above, Speed Pay uses an external API - https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/ - to get real data exchange rates.


9. **Dashboard** - all transfers and transactions that are successfully completed are stored into the history whereas the transfer and transaction views display on top of the completed those that are either deleted or in progress.

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

See the additional Diagram file in our project folder -->Diagram.jpg

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

![N|Solid](https://i.postimg.cc/ryDpRkDj/HomePage.jpg)

### Sign In Page

![N|Solid](https://i.postimg.cc/g0wS1pmM/SignIn.jpg)

### Email received Inbox

![N|Solid](https://i.postimg.cc/VN5N9Q7z/Email-Link.jpg)

### Successful Registration

![N|Solid](https://i.postimg.cc/MKjbDNFb/Successful-Registration.jpg)

### Users Filter Page 

![N|Solid](https://i.postimg.cc/FsYKMQy8/User-Filter.jpg)

### Payment Methods Page

![N|Solid](https://i.postimg.cc/rwLLf6QS/CardList.jpg)

### Transfer

![N|Solid](https://i.postimg.cc/63jwWwFD/Transfer.jpg)

### Transactions Information

![N|Solid](https://i.postimg.cc/bv6NJQtC/Transactions-List.jpg)

### History

![N|Solid](https://i.postimg.cc/bwyhtZ8K/History.jpg)

### Currencies

![N|Solid](https://i.postimg.cc/rFhrWkcw/Currencies.jpg)

### Terms of Use and Privacy Policy 

![N|Solid](https://i.postimg.cc/FRxZXS4r/Terms-Of-Use.jpg)

#### Leadership/Contact us

![N|Solid](https://i.postimg.cc/hvdt7Ltg/Leadership.jpg)
![N|Solid](https://i.postimg.cc/1Rq6LTpW/Get-In-Touch.jpg)

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
> Each user could see a list of his/her own successfully completed transactions and transfers and if the user is admin he could see such a list for each and every user where records are found.
```

> ### Transfer/Transaction page (visible to .........)
```sh

- Each user could see a list of all his/her transactions and transfers with their status, e.g. completed/cancelled/in progress. On the left hand side each user can also see his/her balance.
Each transfer or transcation has a "details" button that displays a summary with the details for a particular operation. If the user is admin he could see such a list for each and every user where records are found.
```

> ### Currency page (visible to .........)
```sh
> Currency section is maintained by the admin and he can easily add/remove/update the status of each currency being used in the application.
```

> ### You Refer Friends (visible to everyone)
```sh
> Provides information about another functionality where a user can enter email of people, not yet registered for the application, and invite them to register. The application sends to that email a registration link. If a registration from that email is completed and verified, both users receive a certain amount in their virtual wallet. Invitations have an expiration time, and a user can take advantage of that feature a limited number of times. 
```
> ### Terms of Use and Privacy Policy (visible to everyone)
```sh
> Provides information about data and privacy.
```

## Credits

This project was created with the assistance of:

| Contacts | Emails |
| ------ | ------ |
| Marina Toncheva | mar.toncheva@gmail.com |
| Kristian Vaklev | kristian.vaklev@yahoo.com |
| Evelina Hristova | ev.hristova@gmail.com |


## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for detail
