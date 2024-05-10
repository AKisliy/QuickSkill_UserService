# QuickSkill_UserService
Part of backend of QuickSkill. UserService microservice.
## Description 👨‍💻
UserService is a part of QuickSkill project. This microservice is responsible for:
1. Registration and authorization
2. Handling operations with user information
## Stack🛠️
- .NET
- Entity Framework
- PostgreSQL
- Docker
- RabbitMQ
- JWT
- Refresh token
- Working with email (smtp)
- Store images in cloud (Yandex Disc)
## Main features 💡
### Registration 
User registration includes:
1. Checking uniquenes of email
2. Verification by sending link to user mail

User's account is considered as verified only after he follows the verification link in his inbox. Verification logic is implemented with special token. 

### Authorization 🔐
For authorization purposes _JWT token_ was used. However, this approach has some noticeable disadvantages such as inability to invalidate JWT in case of compromising. 

To make more secure system, _Refresh token_ was added to the project as well.

### Password operations 
#### Storing password in DB
Password is stored in database in the encrypted form. 
#### Forgot password
"Forgot password" functionality is also included. User can recover his password by following link in his inbox (which will be sent after clicking "Forgot password" button). This functionality is also implemented with token. 

### Work with images 🌅
#### Storing images
Actual images are stored in Yandex Disc. DB only contains links to the appropriate images. 
#### Default profile images
After registration user gets a random default profile image from collection of default images, stored in the cloud. 


## Patterns 📝
In this project, mainly two patterns are used:
1. Repository (for data access layer)
2. Mediator (send information about changed data to RabbitMQ)
