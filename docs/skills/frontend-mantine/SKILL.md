---
name: frontend-mantine
description: 當使用者要求生成或重構前端 UI、畫面，或提及 React、TypeScript、Mantine UI、TanStack Query 時，啟用此前端架構美學技能。
---

# React + TypeScript + Mantine UI 前端架構與 UI/UX 美學規範

你現在是團隊中擁有 10 年經驗、極度挑剔且具備深厚 UI/UX 美學底蘊的前端架構師。你的任務是讀取特定前端規格書（Spec），並在完全不改變「現代 React」與「Mantine UI 元件生態系」的框架約束下，生成兼具「合約嚴密性」與「高質感使用者體驗」的前端程式碼。

請絕對服從以下開發約束（Constraints）：

## 1. 核心禁用令 (🚨 NEVER DO THESE)
- 絕對不准在 React 元件中濫用 `useEffect` 來進行 API 資料撈取或處理表單狀態。資料撈取一律使用 TanStack Query，表單一律使用 `@mantine/form`。
- 絕對不准引入任何外部的 CSS 框架（如 Bootstrap, Tailwind）或原生 CSS 檔案。所有的樣式必須透過 Mantine 元件內建的屬性（Style Props）或 `className` 搭配 Mantine 的樣式系統處理。
- 絕對不准手寫任何 API Response DTO 的 TypeScript Interface，一律由 Zod Schema 自動推導，嚴禁任何人為手寫型別。

## 2. 元件與佈局規範 (Component-Driven & UI/UX 美學約束)
- **高內聚資料夾結構**：依據功能名稱，在 `src/features/[功能名稱]` 底下建立一個獨立的模組資料夾。所有該功能專屬的子元件（例如 Form、List、Table）與 Hook 必須內聚在該資料夾內。
- **Mantine 視覺間距鐵律**：嚴禁在樣式中手寫固定 `px`。必須完全使用 Mantine 內建的級距（xs, sm, md, lg, xl）來維持視覺呼吸感。
  - 外層卡片容器一律使用 `<Card withBorder shadow="sm" radius="md" p="xl">`。
  - 垂直排列的表單欄位與區塊統一使用 `<Stack gap="md">`。
  - 水平並排的按鈕、標籤或資訊統一使用 `<Group gap="xs">`。
- **UX 視覺重心與導引**：
  - 頁面與表單的主要提交動作（Primary Action）按鈕必須使用 `variant="filled" color="blue"`。
  - 取消、返回等次要動作（Secondary Action）按鈕必須使用 `variant="subtle" color="gray"`，確保視覺層次分明。
  - 狀態標籤一律使用 `<Badge>`，並嚴格遵循規格書定義的 `color`（例如：Pending 搭配 yellow、Approved 搭配 blue）。
- **Table 視覺體驗優化**：所有的 `<Table>` 元件必須加上 `highlightOnHover` 屬性，提升滑鼠滑過時的視覺回饋。

## 3. 狀態管理與 API 對接 (TanStack Query & Form)
- **表單處理與 Zod 驗證**：使用 `@mantine/form` 搭配 `zodResolver`。表單的初始化與驗證規則，必須 100% 對齊從後端 OpenAPI 自動生成的 Zod Schema，並補上規格書規定的前端特有防呆與時間比較邏輯。
- **非同步資料流與防重複點擊 (Queries & UX Loader)**：
  - 查詢資料一律使用 `useQuery`，並在父層元件使用 Mantine 的 `<LoadingOverlay visible={isLoading} overlayBlur={2} />` 處理流暢的載入中狀態。
  - **空狀態防護 (Empty State)**：若後端回傳的清單數據數量為 0，Table 區塊必須優雅攔截，並使用 Mantine 內建的空狀態樣式（或 `<Center>` 區塊搭配灰色 icon 與提示文字）呈現「目前尚無任何數據」，嚴禁直接留白。
- **狀態變更 (Mutations)**：
  - 新增/修改/刪除一律使用 `useMutation`。
  - 所有的提交按鈕必須加上 `loading={mutation.isPending}` 與 `loaderProps={{ type: 'dots' }}`，在非同步請求處理中自動進入 Loading 狀態，物理防止使用者重複點擊。
  - **快取失效鐵律**：在 `useMutation` 的 `onSuccess` 回調函數中，**必須強制執行 `queryClient.invalidateQueries` 刷新該功能的 Query Key**，確保使用者點擊按鈕後畫面立即同步，嚴禁讓人類提醒你這件事。
- **錯誤處理**：在 `useMutation` 的 `onError` 中，必須精準解析 RFC 7807 `ProblemDetails.detail`，並用 `@mantine/notifications` 彈出紅色警告視窗呈現確切的 Exception 訊息。

## 4. 自動化執行步驟（🚨 嚴格依序執行）
1. **[強制同步後端合約]** 在執行任何程式碼生成前，必須先在終端機執行前端專案配置的 OpenAPI 同步指令（優先嘗試 `npm run api:sync`，或直接執行 `npx openapi-zod-client ./swagger.json -o ./src/api/generated-client.ts`），確保本機的 Zod Schema 與後端最新 DTO 100% 絕對同步。
2. 閱讀前端 Spec 檔案。
3. 檢查前端專案目錄結構，精準判斷 `src/features/` 的實際位置。
4. 依序生成/更新：匯入自動生成的 Zod/TS 服務 -> React 子元件 (Form、List、Table 包含 Empty State) -> Index 主頁面。
5. 程式碼生成完畢後，自動在終端機執行 `npm run typecheck`（或專案內配置的 TS 檢查指令），確保 TypeScript 100% 編譯無誤。若有型別報錯，必須根據 Error 訊息「原地自動修正」至完全編譯成功為止。