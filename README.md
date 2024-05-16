# DynamicFormApp

The dynamic form application is a robust and versatile platform designed to simplify and streamline the creation, management, and submission of customizable forms.

The dynamic form application is a robust and versatile platform designed to simplify and streamline the creation, management, and submission of customizable forms. This application caters to businesses, educational institutions, and any organization that requires a flexible solution for gathering information from users or clients. Here’s a breakdown of the key features and functionalities of the dynamic form application:

## Key Features

1. **Form Customization**: Users can create forms with various input types, such as text fields, checkboxes, radio buttons, dropdown menus, and date selectors. The platform supports the addition of conditional questions that appear based on previous answers, allowing for highly tailored user experiences.

2. **Dynamic Question Management**: Administrators can easily add, modify, or remove questions in real-time, without needing to redistribute or recreate the entire form. This feature supports quick updates and adjustments to the form as requirements change.

3. **User Submissions**: The application provides a user-friendly interface for respondents to fill out and submit forms. Each submission is securely stored, and data can be retrieved or analyzed at any time.

4. **Data Validation and Security**: Built-in validation rules ensure that the data collected meets the required formats and standards. Security measures protect sensitive information both in transit and at rest.

## Technology Stack

The dynamic form application is built using a modern back-end service like of ASP.NET Core for handling business logic, and a NoSQL database such as MongoDB or Azure Cosmos DB for storing form configurations and submissions. This stack ensures scalability, high performance, and ease of maintenance.

## Setting Up the Application

* Start by configuring Cosmos DB. You can either install the Cosmos DB emulator for development purposes or set it up through the Azure Portal. 
* For installing the emulator, download it from [this link](https://aka.ms/cosmosdb-emulator).
* Use "dynamicForm" as the database name. It should contain four containers: "forms", "questions", "questionTypes", and "submissions". Adjust these names in your configuration if they differ.
* After setting up the database and containers, update the URI and Primary Key in your application's configuration settings which can be found in appSettings.json.
* With these configurations in place, your application is ready to run.

## Summary

The dynamic form application stands out by providing an adaptable, secure, and user-friendly platform for creating and managing interactive forms. Its ability to handle real-time modifications and its comprehensive analytics capabilities make it an invaluable tool for any organization looking to enhance its data collection and user engagement strategies.

## Author

[Michael](https://github.com/m-azra3l)

## License

This project is licensed under the [MIT License](LICENSE).
You can make a copy of the project to use for your own purposes.
