# 企業內部設備借用管理系統 - 借用申請模組

## Domain
Entity: DeviceBooking (Rich Domain Model)


Guid Id : key
string DeviceName : 借用設備名稱 (MaxLen 20)
string Applicant : 借用人 (MaxLen 20)
DateTimeOffset ExpectedReturn : 預計歸還時間
DateTimeOffset? ActualReturn : 實際歸還時間(選填，初始是null)

string? Approver : 簽核人 (MaxLen 20)
int Status : 狀態 enum  Pending (審核中) 1、Approved (已借出) 2、Returned (已歸還) 3、Rejected (已拒絕) 4
string Reason? : 拒絕原因 (MaxLen 100)

初始狀態
 Create(name, user, date){

    LendDayChecker(today, ExpectedReturn)  // (預計歸還時間 - 今日申請時間) 不能超過14天 , throw new DomainException("借用時間最多不能超過 14 天。")
    
    return new DeviceBooking{
        Id = Guid.new
        DeviceName = name
        Applicant = user
        ExpectedReturn = date
        ActualReturn = null
        Approver = null
        Status = 1
        Reason = null
    }
 }

 ToApprove(approver){
    if(Status!=Pending){
        throw new DomainException("非審核中申請單無法過審")
    }
    Approver = approver
    Status = 2
 }

  ToReject(approver,reason){
    if(Status!=Pending){
        throw new DomainException("非審核中申請單無法拒絕")
    }
    Approver = approver
    Reason = reason
    Status = 4
    ActualReturn = DateTimeOffset.UtcNow;
 }

 ToReturn(){
        if(Status!=Approved){
        throw new DomainException(非借出單據無法歸還")
    }
    Status = 3
 }


## Controller

HttpGET Get : 獲取所有單據
return List<DeviceBookingDto>

HttpGET(guid:id) GetById([formroute] guid id, query)
這裡使用with id = query.ID 來做CQRS新物件
return DeviceBookingDto

HttpPOST Create(command)
return guid ID , 201 , typeof(GetById) 

HttpPUT(guid:id/approve) Approve([formroute] guid id, command)
這裡使用with id = command.ID 來做CQRS新物件
return guid ID

HttpPUT(guid:id/reject) Reject([formroute] guid id, command)
這裡使用with id = command.ID 來做CQRS新物件
return guid ID

HttpPUT(guid:id/return) Return([formroute] guid id, command)
這裡使用with id = command.ID 來做CQRS新物件
return guid ID


## Application

DeviceBookingDto
{
       Guid Id 
       string DeviceName 
       string Applicant 
       DateTimeOffset ExpectedReturn 
       DateTimeOffset? ActualReturn 
       string? Approver 
       int Status 
       string? Reason 
}

Exception NotFoundException(guid id):Exception("{id} ： 不存在")


GetQuery : IRequest< List<DeviceBookingDto>>

GetQueryHandler 
查出所有的申請單

GetByIdQuery(guid id) : IRequest< DeviceBookingDto>

GetByIdQueryHandler
查出指定Id的申請單 找不到throw new NotFoundException(Id)


CreateCommand(string name, string user, DateTimeOffset date) : IRequest<Guid>

CreateCommandHandler
建立申請單 Create(name, user, date)


ApproveCommand(guid Id, string approver) :IRequest<Guid>

ApproveCommandHandler
ToApprove( approver)

RejectCommand(guid Id, string approver) :IRequest<Guid>

RejectCommandHandler
ToReject(approver, reason)

ReturnCommand(guid Id, string approver) :IRequest<Guid>

ReturnCommandHandler
ToReturn( approver)

要有injection讓DBA.Api去註冊

## Infrastructure

使用MSSQL

Config根據Domain限制去設定column限制
 
要有injection讓DBA.Api去註冊

