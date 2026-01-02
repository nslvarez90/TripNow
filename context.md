Estructura de base de datos

-- Tabla principal de reservas

CREATE TABLE Bookings (

&nbsp;   Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),

&nbsp;   CustomerEmail NVARCHAR(255) NOT NULL,

&nbsp;   TripCountry NVARCHAR(100) NOT NULL,

&nbsp;   Amount DECIMAL(18,2) NOT NULL CHECK (Amount > 0),

&nbsp;   Status NVARCHAR(50) NOT NULL CHECK (Status IN ('PENDING\_RISK\_CHECK', 'APPROVED', 'REJECTED')),

&nbsp;   RiskScore INT NULL,

&nbsp;   Reason NVARCHAR(500) NULL,

&nbsp;   CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

&nbsp;   UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

&nbsp;   CorrelationId UNIQUEIDENTIFIER NOT NULL, -- Para idempotencia y seguimiento

&nbsp;   INDEX IX\_Bookings\_Status (Status),

&nbsp;   INDEX IX\_Bookings\_CreatedAt (CreatedAt DESC),

&nbsp;   INDEX IX\_Bookings\_CorrelationId (CorrelationId)

);



-- Tabla para idempotencia (evitar duplicados)

CREATE TABLE IdempotencyKeys (

&nbsp;   IdempotencyKey NVARCHAR(255) PRIMARY KEY,

&nbsp;   BookingId UNIQUEIDENTIFIER NOT NULL,

&nbsp;   RequestHash NVARCHAR(64) NOT NULL, -- Hash de la solicitud

&nbsp;   CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

&nbsp;   FOREIGN KEY (BookingId) REFERENCES Bookings(Id),

&nbsp;   INDEX IX\_IdempotencyKeys\_CreatedAt (CreatedAt)

);



-- Tabla de logs de evaluación de riesgo (para resiliencia)

CREATE TABLE RiskAssessmentLogs (

&nbsp;   Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),

&nbsp;   BookingId UNIQUEIDENTIFIER NOT NULL,

&nbsp;   ExternalRequest NVARCHAR(MAX) NULL,

&nbsp;   ExternalResponse NVARCHAR(MAX) NULL,

&nbsp;   Status NVARCHAR(50) NOT NULL,

&nbsp;   ErrorMessage NVARCHAR(1000) NULL,

&nbsp;   AttemptNumber INT NOT NULL DEFAULT 1,

&nbsp;   CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

&nbsp;   FOREIGN KEY (BookingId) REFERENCES Bookings(Id),

&nbsp;   INDEX IX\_RiskAssessmentLogs\_BookingId (BookingId)

);



-----------------------------------------Sistema de archivos-------------



TripNow.Backend/

├── 📁 src/

│   ├── 📁 TripNow.Core/                    # Capa de dominio

│   │   ├── 📁 Entities/

│   │   │   ├── Booking.cs

│   │   │   └── ValueObjects/              # Objetos de valor (opcional)

│   │   ├── 📁 Enums/

│   │   │   └── BookingStatus.cs

│   │   ├── 📁 Events/                     # Eventos de dominio

│   │   │   └── BookingCreatedEvent.cs

│   │   ├── 📁 Exceptions/

│   │   │   ├── DomainException.cs

│   │   │   └── BookingExceptions/

│   │   ├── 📁 Interfaces/

│   │   │   ├── 📁 Repositories/

│   │   │   │   └── IBookingRepository.cs

│   │   │   ├── 📁 Services/

│   │   │   │   └── IRiskAssessmentService.cs

│   │   │   └── IUnitOfWork.cs

│   │   └── 📁 Specifications/             # Patrón Specification (opcional)

│   │

│   ├── 📁 TripNow.Application/             # Capa de aplicación

│   │   ├── 📁 Common/

│   │   │   ├── Interfaces/

│   │   │   │   └── IIdempotencyService.cs

│   │   │   └── Behaviors/

│   │   │       └── IdempotencyBehavior.cs # Pipeline behavior para idempotencia

│   │   ├── 📁 Features/

│   │   │   ├── 📁 Bookings/

│   │   │   │   ├── 📁 Commands/

│   │   │   │   │   ├── CreateBooking/

│   │   │   │   │   │   ├── CreateBookingCommand.cs

│   │   │   │   │   │   ├── CreateBookingCommandHandler.cs

│   │   │   │   │   │   ├── CreateBookingCommandValidator.cs

│   │   │   │   │   │   └── CreateBookingCommandResponse.cs

│   │   │   │   │   └── UpdateBookingRisk/

│   │   │   │   │       └── ... (similar estructura)

│   │   │   │   ├── 📁 Queries/

│   │   │   │   │   ├── GetBookingById/

│   │   │   │   │   │   ├── GetBookingByIdQuery.cs

│   │   │   │   │   │   ├── GetBookingByIdQueryHandler.cs

│   │   │   │   │   │   └── GetBookingByIdResponse.cs

│   │   │   │   │   └── GetRecentBookings/

│   │   │   │   │       └── ... (similar estructura)

│   │   │   │   └── 📁 DTOs/

│   │   │   │       └── BookingDto.cs

│   │   │   └── 📁 RiskAssessment/

│   │   │       └── ... (carpetas similares)

│   │   ├── 📁 Mappings/

│   │   │   └── BookingProfile.cs          # AutoMapper profiles

│   │   └── ApplicationServiceRegistration.cs

│   │

│   ├── 📁 TripNow.Infrastructure/          # Capa de infraestructura

│   │   ├── 📁 Data/

│   │   │   ├── ApplicationDbContext.cs

│   │   │   ├── 📁 Configurations/

│   │   │   │   └── BookingConfiguration.cs

│   │   │   ├── 📁 Migrations/              # Entity Framework Migrations

│   │   │   ├── 📁 Repositories/

│   │   │   │   └── BookingRepository.cs

│   │   │   └── UnitOfWork.cs

│   │   ├── 📁 ExternalServices/

│   │   │   ├── 📁 RiskProvider/

│   │   │   │   ├── RiskAssessmentService.cs

│   │   │   │   ├── Models/

│   │   │   │   │   ├── RiskAssessmentRequest.cs

│   │   │   │   │   └── RiskAssessmentResponse.cs

│   │   │   │   └── Resilience/

│   │   │   │       ├── PollyPolicies.cs    # Políticas de resiliencia

│   │   │   │       └── CircuitBreaker.cs

│   │   │   └── 📁 EmailService/            # Para notificaciones futuras

│   │   ├── 📁 BackgroundServices/          # Servicios en segundo plano

│   │   │   ├── RiskAssessmentBackgroundService.cs

│   │   │   └── OutboxPatternProcessor.cs   # Patrón Outbox (opcional)

│   │   ├── 📁 Services/

│   │   │   └── IdempotencyService.cs

│   │   ├── 📁 MessageBroker/               # Para arquitectura serverless/eventos

│   │   │   └── AzureServiceBus/

│   │   └── InfrastructureServiceRegistration.cs

│   │

│   └── 📁 TripNow.API/                     # Capa de presentación

│       ├── 📁 Controllers/

│       │   ├── BookingsController.cs

│       │   └── ApiControllerBase.cs        # Base con helpers comunes

│       ├── 📁 Middleware/

│       │   ├── ExceptionHandlingMiddleware.cs

│       │   ├── IdempotencyMiddleware.cs

│       │   └── RequestLoggingMiddleware.cs

│       ├── 📁 Filters/

│       │   └── ValidateModelAttribute.cs

│       ├── 📁 Extensions/

│       │   ├── ServiceExtensions.cs

│       │   └── SwaggerExtensions.cs

│       ├── appsettings.json

│       ├── appsettings.Development.json

│       ├── Program.cs

│       └── TripNow.API.csproj

│

├── 📁 tests/                               # Tests

│   ├── 📁 TripNow.Core.UnitTests/

│   ├── 📁 TripNow.Application.UnitTests/

│   ├── 📁 TripNow.Infrastructure.UnitTests/

│   └── 📁 TripNow.API.IntegrationTests/

│

├── 📁 docker/                              # Configuración Docker

│   ├── Dockerfile

│   └── docker-compose.yml

├── .gitignore

├── README.md

└── TripNow.sln



----------------------------------------------------Archivos a crear con base del base de datod---------

Capa Core (Dominio):

Booking.cs → src/TripNow.Core/Entities/



BookingStatus.cs → src/TripNow.Core/Enums/



IBookingRepository.cs → src/TripNow.Core/Interfaces/Repositories/



IRiskAssessmentService.cs → src/TripNow.Core/Interfaces/Services/



Capa Application (Casos de Uso):

CreateBookingCommand.cs → src/TripNow.Application/Features/Bookings/Commands/CreateBooking/



GetBookingByIdQuery.cs → src/TripNow.Application/Features/Bookings/Queries/GetBookingById/



BookingDto.cs → src/TripNow.Application/Features/Bookings/DTOs/



IIdempotencyService.cs → src/TripNow.Application/Common/Interfaces/



Capa Infrastructure (Implementaciones):

ApplicationDbContext.cs → src/TripNow.Infrastructure/Data/



BookingRepository.cs → src/TripNow.Infrastructure/Data/Repositories/



RiskAssessmentService.cs → src/TripNow.Infrastructure/ExternalServices/RiskProvider/



RiskAssessmentBackgroundService.cs → src/TripNow.Infrastructure/BackgroundServices/



IdempotencyService.cs → src/TripNow.Infrastructure/Services/



Capa API (Presentación):

BookingsController.cs → src/TripNow.API/Controllers/



ExceptionHandlingMiddleware.cs → src/TripNow.API/Middleware/



Program.cs → src/TripNow.API/



---------------

