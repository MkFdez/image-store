# Image Store Project 📸

## Features 🌟

- **Secure Authentication**: Utilizes Microsoft Identity for robust user authentication and authorization. 🔐
- **Seamless Payments**: Integrated with Stripe for smooth and secure payment processing. 💳
- **Interactive Dashboard**: User-friendly dashboard for account management and image statistics. 📊
- **Flexible Image Handling**: Upload, download, and manage images in various resolutions. 🖼️
- **Efficient Thumbnails**: Automatically generate thumbnails for faster loading and improved UX. 🖼️
- **Professional Watermarks**: Apply watermarks to images to protect and brand your content. ©️

## Installation 🚀

1. Clone the repository:
   ```sh
   git clone https://github.com/MkFdez/image-store.git
2. Open the Store.sln with Visual Studio(recommended 2019 od higher)

3. Generate the Database from ADO.NET Entity Data Model:

  - Open the solution in Visual Studio.
  - Open the ADO.NET Entity Data Model (*.edmx file).
  - Right-click in the designer and choose "Generate Database from Model".
  - Follow the wizard to set up your database connection and generate the database schema(recommended create a new database with SQL Server Managment Studio and create the connection with that database) 
  - Add the connection string to the [Web.config](Store/Web.config)
4. Run the application

## Technologies Used 🔧
- C# and ASP.NET MVC 5 for server-side development
- HTML, CSS, and jQuery for front-end development
- Microsoft Identity Framework for authentication and authorization
- Stripe for secure payment processing
- SQL Server for the database

## Authentication 🔒
-User authentication and authorization are handled through Microsoft Identity, providing a secure and reliable experience for sign-up, login, and account management.

## Payments 💳
Securely process payments with ease using the Stripe integration. Users can conveniently make purchases using their preferred payment methods.

## Dashboard 📊
The intuitive user dashboard presents essential account details, including image statistics, payment history, and customizable settings.

## Image Management 🖼️
Easily upload, organize, and manage images. Group images into folders for efficient organization and quick access.

## Thumbnails & Watermarks 🖼️
Uploaded images are automatically transformed into thumbnails for optimized loading. Additionally, users can apply watermarks to images to protect intellectual property and enhance branding.

## Contributing 👥
We warmly welcome contributions! To suggest improvements or fixes, submit a pull request. For major changes, please discuss first by opening an issue.

## License 📜
This project is licensed under the [MIT License](LICENSE.md).

Contact 📧
For any questions or suggestions, feel free to contact the project owner: [mirkofs29@outlook.com](mailto:mirkofs29@outlook.com)

GitHub: MkFdez
