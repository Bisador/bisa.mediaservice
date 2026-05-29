# Media Service

## 📖 overview
A production-oriented media storage service built with ASP.NET Core, PostgreSQL, and MinIO.

This service provides a clean and extensible architecture for storing and serving media files using object storage patterns commonly used in enterprise systems.

The service supports multiple storage providers through abstraction:

* Local filesystem
* MinIO (S3-compatible)

Additional providers can easily be added.
<hr>
 
## 🧱 Features

#### Core Storage:
* Upload ✅
* Download ✅
* Delete ✅
* Metadata persistence
* Validation

#### Consistency & Reliability
* Upload Status
* Compensating transactions
* Retry
* Idempotency
* Orphan cleanup

#### Security
* Authorization
* Private storage
* Presigned URL
* Virus scanning
* MIME sniffing
* File extension validation
* Size quotas

#### Performance
* Streaming upload
* Multipart upload
* CDN support
* Range requests
* Caching

#### Media Processing
* Thumbnail generation
* Image resize
* Video transcoding
* PDF preview
* EXIF extraction

#### Enterprise Features
* Audit logs
* Versioning
* Retention policy
* Legal hold
* Encryption
* Multi-tenancy
* Geo replication

#### Observability
* Structured logging
* Metrics
* Distributed tracing

#### Async Architecture
* Event publishing
* Message broker

#### Data Lifecycle
* Archiving
* Cold storage
* Automatic cleanup

<hr>

###  Architecture

Client<br>
↓<br>
Media API<br>
↓<br>
Application Layer<br>
↓<br>
IFileStorage<br>
├── LocalFileStorage<br>
└── MinioFileStorage<br>

Metadata → PostgreSQL <br>
Binary Files → MinIO

<hr>

## 🚀 Technologies
* ASP.NET Core
* Entity Framework Core
* PostgreSQL
* MinIO
* Docker
* Clean Architecture

<hr>

## 🛠️ Setup

#### Requirements
* .NET 10 SDK
* Docker Desktop
  
#### Start Infrastructure
```bash
docker compose up -d
```
<br>

####  Apply Migrations
```shell
dotnet ef database update
--project src/MediaService.Infrastructure 
--startup-project src/MediaService.Api
```
#### Run API
```shell
dotnet run --project src/MediaService.Api
```

#### MinIO Console
[http://localhost:9001](http://localhost:9001)
  
Credentials are configured in .env.
 
#### Configuration

##### .env
```
POSTGRES_DB=media_db  
POSTGRES_USER=postgres 
POSTGRES_PASSWORD=postgres  

MINIO_ROOT_USER=admin  
MINIO_ROOT_PASSWORD=password123 
 ```
<hr>

### Roadmap

#### Phase 1
* Core storage ✅
* MinIO ✅
* PostgreSQL ✅
* metadata
* validation
* consistency cleanup

#### Phase 2
* Reliability
* status field
* retry
* compensating transaction
* orphan cleanup job

#### Phase 3
* Security
* auth
* virus scan
* presigned URLs
* quotas

#### Phase 4
* Media processing
* thumbnails
* video processing
* async jobs

#### Phase 5
* Observability
* metrics
* tracing
* dashboards

#### Phase 6
* Scale
* CDN
* multipart upload
* replication
* sharding

