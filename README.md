⏳ Unseal - Distributed Digital Time Capsule Ecosystem

Unseal is a sophisticated social media platform designed as a Digital Time Capsule. It redefines social interaction by blending it with "patience" and "anticipation." Users can lock their memories (photos, videos, voice recordings, or text) into capsules that remain sealed until a specific future date.

🚀 Engineering Excellence & Technical Stack
Unseal is built with the principle that software should not just work—it must be scalable, secure, and flexible.

🏗️ Architectural Foundations
Domain-Driven Design (DDD): Ensuring a sustainable and maintainable codebase by focusing on the core domain logic.
- Repository Pattern: Decoupling the data access layer for better testability and flexibility.

- Tenant & Generic Response Middleware: Standardized API management and multi-tenancy support.

- Dynamic Logic: Leveraging Reflection for high-performance, dynamic filtering and sorting operations.

🔒 Security & Authorization
- Identity Management: Comprehensive Authentication and Authorization flows powered by OpenIddict.

- Data Protection: Sensitive content URLs are encrypted using the Data Protection API before storage.

- Traffic Control: IP and Path-based Rate Limiting to prevent brute-force attacks and redundant traffic.

- Access Control: Granular Permission Management and tailored CORS configurations.

⚡ Performance & Messaging
- Real-time Interaction: SignalR for instant messaging and Server-Sent Events (SSE) for real-time monitoring of newly revealed capsules.

- Asynchronous Tasks: RabbitMQ coupled with Worker Services for background task processing.

- Caching Strategy: Redis and Memory Cache integration for tracking user and rapid data access.

- Database Optimization: Bulk Insert/Update operations to minimize database overhead during heavy writes.

📊 Data & Storage
- Advanced Search: Elastic Search integration for lightning-fast retrieval within large datasets.

- Spatial Data: GeoJSON integration for location-based capsule operations.

- Cloud & Mail: Cloudinary for media asset management and Gmail SMTP for reliable mail operations.

🛠️ Advanced Software Standards
The project serves as a laboratory for complex engineering scenarios, implementing:

- Fluent Validation for robust schema enforcement.

- Global Action Filters for cross-cutting concerns.

- Setting Management for flexible application configuration.

Some pictures of Unseal:

| Swagger - Admin & Auth | Swagger - Capsule & Group & Messages | Swagger - SSE & User |
| :---: | :---: | :---: |
| ![Swagger 1](https://github.com/user-attachments/assets/4019b8e3-bece-4820-bbbe-17dbac79b453) | ![Swagger 2](https://github.com/user-attachments/assets/83d516c9-2d55-4b5b-8346-8d5ad5dc1173) | ![Swagger 3](https://github.com/user-attachments/assets/9c780ec4-5456-44d8-8d66-aa063e8a110e) |


| Mail - Register | Mail - Account Activation | Mail - Account Delete |
| :---: | :---: | :---: |
| ![Swagger 1](https://github.com/user-attachments/assets/20ba51b7-c2cf-48a7-b146-3519a1348eef) | ![Swagger 2](https://github.com/user-attachments/assets/c15c502e-2884-48c2-ad8b-abc21bb208cd) | ![Swagger 3](https://github.com/user-attachments/assets/b6f0a734-2e51-40d3-a530-720046b4e581) |


| App - Login | App - Register | App - Reset Password |
| :---: | :---: | :---: 
| ![App 1](https://github.com/user-attachments/assets/e199510f-e03d-49d6-b193-ebe7c3930db2) | ![App 2](https://github.com/user-attachments/assets/313217d9-2bfe-4cde-9966-c284e70d1ac0) | ![App 3](https://github.com/user-attachments/assets/1502aa85-80d1-4db7-9832-43f10260760c) |

| App - Capsule Detail | App - Explore | App - Profile |
| :---: | :---: |:---: |
| ![App 4](https://github.com/user-attachments/assets/fa805f68-debe-4f1f-9f27-0b091367e3af) | ![App 5](https://github.com/user-attachments/assets/d8dace9e-6f81-499d-be2b-5a62cdd75bc3) | ![App 6](https://github.com/user-attachments/assets/31e9d34b-2d03-4f77-a82d-33ffda3e1c8d) |


| App - Capsule Create | App - User Search | App - Capsule QR |
 | :---: | :---: |:---: |
| ![App 7](https://github.com/user-attachments/assets/3a7554f9-9153-4cda-a656-1285168a2af6) | ![App 8](https://github.com/user-attachments/assets/f0ddc251-e86a-486c-a834-28656bc66861) | ![App 9](https://github.com/user-attachments/assets/c0956d56-b43c-459c-b2aa-5c0e274b52fb) |