# 企業內部設備借用管理系統 - 前端 UI 規格書 (合約自動化版)

## 1. 核心技術棧約束 (Tech Stack)
- **Framework**: React (Vite + TypeScript)
- **UI Library**: Mantine UI v7+ (包含 `@mantine/core`, `@mantine/dates`, `@mantine/notifications`)
- **State & API**: TanStack Query v5 (React Query)
- **Form Handling**: `@mantine/form`
- **Contract & Validation**: `Zod` (用於執行時資料安檢與型別推導)

## 2. 前後端合約與自動化生成規範 (Contract Automation)
- **嚴禁人為手寫型別**：禁止手寫任何與後端對接的 TypeScript Interface。
- **單一真理來源 (SSOT)**：一律透過工具（如 `openapi-zod-client`）直接讀取後端編譯產出的 `swagger.json`。
- **自動生成內容**：
  - 由工具自動產出 API 請求的 `Zod Schema`。
  - 透過 `z.infer<typeof Schema>` 自動推導出符合後端 DTO 結構（如 `DeviceBookingDto`）的 TypeScript 強型別。
  - 所有 API 的 `Axios` 請求與 `TanStack Query` 鉤子（Hooks）一律由腳本自動生成，開發時直接匯入使用。

## 3. 頁面佈局與 Mantine UI 元件規範
本功能採單頁面設計（Single Page），依功能切分為上下兩個區塊：

### A. 上半部：申請借用表單 (Booking Form)
- **外層容器**：使用 Mantine `<Card withBorder shadow="sm" radius="md">`。
- **輸入欄位**：
  - `DeviceName`：使用 `<TextInput>`，必填。
  - `ExpectedReturn`：使用 `@mantine/dates` 的 `<DateTimePickerInput>`，必填。
- **送出按鈕**：使用 `<Button>`，點擊時調用自動生成的 Create Mutation。

### B. 下半部：我的申請清單 (Booking List)
- **分頁切換**：使用 Mantine `<Tabs>`，依據單據狀態分流，區分為四個頁籤：
  - 「全部」、「審核中 (Pending)」、「已借出 (Approved) Foro」、「已結束 (Returned/Rejected)」。
- **列表呈現**：使用 Mantine `<Table withTableBorder withColumnBorders>` 呈現數據。欄位依序為：設備名稱、借用人、預計歸還時間、狀態、動作欄。
- **狀態標籤 (Status Badge)**：根據後端 Enum 狀態，顯示對應公認顏色的 Mantine `<Badge>`：
  - `Pending (1)`: `yellow`
  - `Approved (2)`: `blue`
  - `Returned (3)`: `green`
  - `Rejected (4)`: `red`

## 4. 互動行為與表單前端驗證 (@mantine/form)
表單狀態初始化必須綁定 Zod Schema 進行嚴格校驗：
1. `deviceName`：必填，且長度不可超過 20 個字（與後端欄位最大長度限制對齊）。
2. `expectedReturn`：選擇之時間物件必須大於當下時間（`new Date()`），否則阻擋送出並提示錯誤訊息：「預計歸還時間不可早於當下時間」。

### 清單動作與動態彈窗 (Modals)
- 當單據 `Status === 1 (Pending)`：顯示「同意」與「拒絕」按鈕。
  - 點擊「拒絕」時，必須彈出 Mantine `<Modal>`，內含一個 `<TextInput>` 供輸入「拒絕原因（必填，上限 100 字）」，送出時調用 Reject Mutation。
- 當單據 `Status === 2 (Approved)`：顯示「歸還設備」按鈕，點擊調用 Return Mutation。
- 當單據狀態為 `Returned (3)` 或 `Rejected (4)`：隱藏所有動作按鈕。

## 5. 全域異常處理與狀態同步 (Exception & Sync)

### A. 異常捕獲鐵律 (RFC 7807 Problem Details)
- **錯誤呈現**：所有變更狀態的 Mutation（Create, Approve, Reject, Return）在失敗時（`onError`），**必須嚴格解析後端拋出的標準 RFC 7807 `ProblemDetails` JSON 結構**。
- **通知元件**：提取 JSON 中的 `detail` 欄位（例如："借用時間最多不能超過 14 天。"），並調用 `@mantine/notifications` 的 `notifications.show()` 彈出紅色警告視窗。

### B. 資料流快取管理
- **非同步遮罩**：使用 `useQuery` 撈取資料時，外層必須包裹 Mantine `<LoadingOverlay visible={isLoading} overlayBlur={2} />` 處理載入動畫。
- **自動快取重整**：任何變更狀態的 Mutation 執行成功（`onSuccess`）時，必須強制觸發：
  ```typescript
  queryClient.invalidateQueries({ queryKey: ['device-bookings'] })
  ```

## 6. 使用者體驗與動態流向 (UX Refinement & Flow)

### A. 視覺重心與動作按鈕導引 (Visual Hierarchy)
1. **主要提交動作**：上半部的「送出申請」表單提交按鈕、以及簽核彈窗中的「確認拒絕」按鈕，一律使用 `variant="filled" color="blue"`，確保視覺重心聚焦在核心操作。
2. **次要防護動作**：動態彈窗（Modal）中的「取消」按鈕，一律使用 `variant="subtle" color="gray"`，以輕量化視覺降低次要動作的干擾。
3. **列表體驗優化**：`<Table>` 元件必須啟用 `highlightOnHover` 屬性，在使用者滑鼠滑過單據行時給予即時的淡灰色背景反饋。

### B. 非同步防禦與加載狀態 (Optimistic UX & Feedback)
1. **防重複點擊鎖**：所有觸發 Mutation 的按鈕（送出申請、同意、確認拒絕、歸還設備），必須綁定 `loading={mutation.isPending}` 狀態，並在載入時自動套用 `loaderProps={{ type: 'dots' }}`。非同步處理期間按鈕必須維持 Disabled 狀態，物理阻擋重複點擊造成的多重單據送出。
2. **列表骨架屏 (Skeleton Map)**：若初始讀取資料（`isInitialLoading`）時，下半部的清單區塊不留白，必須渲染 Mantine `<Skeleton>` 模擬表格行高結構（至少 5 行），降低使用者等待的焦慮感。

### C. 數據空狀態與流暢反饋 (Empty State & Toast)
1. **Empty State（空狀態防護）**：當後端回傳借用清單數量為 0 時，表格本體應隱藏。改為在中央渲染 Mantine `<Center style={{ height: 200 }}>` 區塊，內嵌一個灰色設備圖示（Theme Icon）與文字提示：`「目前尚無任何借用申請紀錄」`，維持界面的工整。
2. **操作成功提示**：任何變更狀態的 Mutation 成功時，除了重整快取，必須同步發出 `@mantine/notifications` 的綠色成功通知：
   - 建立成功：`「借用申請已成功送出，進入審核流程」`
   - 簽核成功：`「該單據已變更為【已借出】狀態」`
   - 歸還成功：`「設備已順利歸還，單據結案」`