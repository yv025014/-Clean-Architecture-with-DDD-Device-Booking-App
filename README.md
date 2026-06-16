 # 企業內部設備借用管理系統 / Device Booking Management System

 一套用於企業內部管理設備借用流程的全端應用系統，提供設備申請、審核、歸還的完整生命週期管理。

 A full-stack internal enterprise application for managing the complete lifecycle of device borrowing — from application, approval, to return.

 ---

 ## 技術架構 / Tech Stack

 ### 後端 / Backend
 | 技術 | 說明 |
 |------|------|
 | .NET 8 | 主要執行環境 |
 | ASP.NET Core Web API | RESTful API 框架 |
 | Clean Architecture + DDD | 乾淨架構搭配領域驅動設計 |
 | MediatR v12 | CQRS 模式實作 |
 | FluentValidation | 請求驗證 |
 | EF Core 8 + MSSQL | ORM 與資料庫 |
 | Swagger / OpenAPI | API 文件 |

 ### 前端 / Frontend
 | 技術 | 說明 |
 |------|------|
 | React 19 + TypeScript 6 | UI 框架與型別安全 |
 | Vite 8 | 建置工具 |
 | Mantine v9 | UI 元件庫 |
 | TanStack Query v5 | 伺服器狀態管理 |
 | Axios + Zod v4 | HTTP 客戶端與錯誤解析 |
 | openapi-typescript | 從 OpenAPI Spec 自動生成 TypeScript 型別 |

 ---

 ## 專案結構 / Project Structure

 device-booking-app/
 ├── device-booking-app/          # 後端 .NET 方案 / Backend .NET solution
 │   ├── DBA.Domain/              # 領域實體、列舉、例外 / Domain entities, enums, exceptions
 │   ├── DBA.Application/         # CQRS 指令/查詢、DTO、介面 / Commands, queries, DTOs, interfaces
 │   ├── DBA.Infrastructure/      # EF Core、資料庫遷移 / EF Core, migrations, persistence
 │   └── DBA.Api/                 # 控制器、全域例外處理、Program.cs
 └── DBA.Web/                     # 前端 React 應用 / Frontend React app
     ├── src/
     │   ├── api/                 # openapi-typescript 型別、axios 客戶端
     │   └── features/
     │       └── device-bookings/ # 設備借用功能模組（Form、List、Hooks）
     └── swagger.json             # OpenAPI Spec（用於型別生成）

 ---

 ## 系統功能 / Features

 - **申請設備借用** — 填寫設備名稱、借用人、預計歸還時間
 - **審核申請** — 主管可審核通過或拒絕（附拒絕原因）
 - **歸還設備** — 標記設備已歸還
 - **清單總覽** — 依狀態篩選（全部 / 審核中 / 已借出 / 已結束）

 ---

 ## 商業規則 / Business Rules

 | 規則 | 說明 |
 |------|------|
 | 預計歸還時間需晚於申請當下 | 不可選擇過去或當前時間 |
 | 借用時間上限 14 天 | 超出將拋出 Domain Exception |
 | 狀態機流轉 | Pending → Approved / Rejected；Approved → Returned |

 ---

 ## 事前準備 / Prerequisites

 - [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
 - [Node.js 18+](https://nodejs.org/)
 - MSSQL（本機安裝或 Docker）

 ### 啟動 MSSQL（Docker 範例）/ Start MSSQL via Docker

 ```bash
 docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Qwer1234!" \
   -p 1433:1433 --name mssql \
   -d mcr.microsoft.com/mssql/server:2022-latest

 ---
 快速開始 / Quick Start

 1. 後端 / Backend

 # 進入後端方案目錄
 cd device-booking-app/device-booking-app

 # 設定資料庫連線字串（編輯 DBA.Api/appsettings.json）
 # 預設值 / Default:
 # Server=localhost;Database=DeviceBookingDb;User ID=sa;Password=Qwer1234!;TrustServerCertificate=True;

 # 執行資料庫遷移
 dotnet ef database update --project DBA.Infrastructure --startup-project DBA.Api

 # 啟動 API
 dotnet run --project DBA.Api

 API 啟動後可在以下位置存取 Swagger 文件：
 Swagger UI is available at:

 http://localhost:5238/swagger

 2. 前端 / Frontend

 # 進入前端目錄
 cd device-booking-app/DBA.Web

 # 安裝套件
 npm install

 # （選用）從最新 OpenAPI Spec 重新生成 TypeScript 型別
 # (Optional) Regenerate TypeScript types from OpenAPI spec
 npm run api:sync

 # 啟動開發伺服器（已設定 /api Proxy 至後端）
 npm run dev

 前端開發伺服器位址 / Frontend dev server:

 http://localhost:5173

 ▎ Vite 已設定 /api 路徑自動代理至後端 http://localhost:5238，開發環境無需手動處理 CORS。
 ▎ Vite proxy forwards /api to the backend automatically — no CORS configuration needed in development.

 ---
 API 端點 / API Endpoints

 ┌────────┬──────────────────────────────────┬──────────────────┐
 │ Method │             Endpoint             │       說明       │
 ├────────┼──────────────────────────────────┼──────────────────┤
 │ GET    │ /api/DeviceBookings              │ 取得所有借用申請 │
 ├────────┼──────────────────────────────────┼──────────────────┤
 │ GET    │ /api/DeviceBookings/{id}         │ 取得單筆申請     │
 ├────────┼──────────────────────────────────┼──────────────────┤
 │ POST   │ /api/DeviceBookings              │ 新增借用申請     │
 ├────────┼──────────────────────────────────┼──────────────────┤
 │ PUT    │ /api/DeviceBookings/{id}/approve │ 審核通過         │
 ├────────┼──────────────────────────────────┼──────────────────┤
 │ PUT    │ /api/DeviceBookings/{id}/reject  │ 拒絕申請         │
 ├────────┼──────────────────────────────────┼──────────────────┤
 │ PUT    │ /api/DeviceBookings/{id}/return  │ 歸還設備         │
 └────────┴──────────────────────────────────┴──────────────────┘

 所有錯誤回應均符合 RFC 7807 ProblemDetails (https://www.rfc-editor.org/rfc/rfc7807) 格式。
 All error responses conform to RFC 7807 ProblemDetails format.

 ---
 狀態說明 / Booking Status

 ┌────────┬────────────────────┬──────────────────────┐
 │ 狀態碼 │        名稱        │         說明         │
 ├────────┼────────────────────┼──────────────────────┤
 │ 1      │ Pending（審核中）  │ 申請送出、待審核     │
 ├────────┼────────────────────┼──────────────────────┤
 │ 2      │ Approved（已借出） │ 審核通過、設備借出中 │
 ├────────┼────────────────────┼──────────────────────┤
 │ 3      │ Returned（已歸還） │ 設備已歸還           │
 ├────────┼────────────────────┼──────────────────────┤
 │ 4      │ Rejected（已拒絕） │ 申請遭拒絕           │
 └────────┴────────────────────┴──────────────────────┘

 ---
 開發指令 / Dev Commands

 # 後端建置 / Backend build
 dotnet build device-booking-app/device-booking-app/device-booking-app.sln

 # 前端型別檢查 / Frontend typecheck
 cd DBA.Web && npm run typecheck

 # 前端 Lint
 cd DBA.Web && npm run lint

 # 前端生產建置 / Frontend production build
 cd DBA.Web && npm run build

 # 同步後端 API 型別 / Sync API types from OpenAPI
 cd DBA.Web && npm run api:sync

 ---
