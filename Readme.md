\# WEBSITE BÁN QUẦN ÁO – ASP.NET CORE MVC



\## 1. Công nghệ sử dụng

\- ASP.NET Core MVC

\- Entity Framework Core

\- SQL Server

\- HTML, CSS, Bootstrap

\- Visual Studio 2022



---



\## 2. Yêu cầu môi trường



Cần cài đặt các phần mềm sau để chạy project:



\- \*\*Visual Studio 2022\*\*

&nbsp; - Chọn workload: \*\*ASP.NET and web development\*\*

\- \*\*.NET SDK\*\* 8

&nbsp; - Tải tại: https://dotnet.microsoft.com/download

\- \*\*SQL Server\*\* (2022)

\- \*\*SQL Server Management Studio (SSMS)\*\*



---



\## 3. Cấu hình Database



\### 4.1. Tạo Database

Mở SQL Server Management Studio và tạo database: ClothingShopDB



\### 4.2. Cấu hình Connection String

Mở file `appsettings.json` và chỉnh:



```json

"ConnectionStrings": {

&nbsp; "DefaultConnection": "Server=.;Database=ClothingShopDB;Trusted\_Connection=True;TrustServerCertificate=True"

}



\## 5. Khởi tạo Database

Mở Package Manager Console trong Visual Studio và chạy: Update-Database



\## 6. Chạy project

Mở file .sln bằng Visual Studio

Restore NuGet Packages

RUN Project



