/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2008                    */
/* Created on:     10/06/2025 12:07:11 SA                       */
/*==============================================================*/

CREATE DATABASE GYM
go
use GYM
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('MemberPakage') and o.name = 'FK_MEMBERPA_MEMBERPAK_MEMBER')
alter table MemberPakage
   drop constraint FK_MEMBERPA_MEMBERPAK_MEMBER
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('MemberPakage') and o.name = 'FK_MEMBERPA_MEMBERPAK_PACKAGE')
alter table MemberPakage
   drop constraint FK_MEMBERPA_MEMBERPAK_PACKAGE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('MemberPayment') and o.name = 'FK_MEMBERPA_MEMBERPAY_MEMBER')
alter table MemberPayment
   drop constraint FK_MEMBERPA_MEMBERPAY_MEMBER
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('MemberPayment') and o.name = 'FK_MEMBERPA_MEMBERPAY_PAYMENT')
alter table MemberPayment
   drop constraint FK_MEMBERPA_MEMBERPAY_PAYMENT
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('MemberPayment') and o.name = 'FK_MEMBERPA_MEMBERPAY_STAFF')
alter table MemberPayment
   drop constraint FK_MEMBERPA_MEMBERPAY_STAFF
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TrainingSchedule') and o.name = 'FK_TRAINING_TRAININGS_TRAINER')
alter table TrainingSchedule
   drop constraint FK_TRAINING_TRAININGS_TRAINER
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TrainingSchedule') and o.name = 'FK_TRAINING_TRAININGS_MEMBER')
alter table TrainingSchedule
   drop constraint FK_TRAINING_TRAININGS_MEMBER
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('"User"') and o.name = 'FK_USER_INCLUDE_ROLE')
alter table "User"
   drop constraint FK_USER_INCLUDE_ROLE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('UserNotification') and o.name = 'FK_USERNOTI_USERNOTIF_NOTIFICA')
alter table UserNotification
   drop constraint FK_USERNOTI_USERNOTIF_NOTIFICA
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('UserNotification') and o.name = 'FK_USERNOTI_USERNOTIF_USER')
alter table UserNotification
   drop constraint FK_USERNOTI_USERNOTIF_USER
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Member')
            and   type = 'U')
   drop table Member
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('MemberPakage')
            and   name  = 'MemberPakage2_FK'
            and   indid > 0
            and   indid < 255)
   drop index MemberPakage.MemberPakage2_FK
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('MemberPakage')
            and   name  = 'MemberPakage_FK'
            and   indid > 0
            and   indid < 255)
   drop index MemberPakage.MemberPakage_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('MemberPakage')
            and   type = 'U')
   drop table MemberPakage
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('MemberPayment')
            and   name  = 'MemberPayment3_FK'
            and   indid > 0
            and   indid < 255)
   drop index MemberPayment.MemberPayment3_FK
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('MemberPayment')
            and   name  = 'MemberPayment2_FK'
            and   indid > 0
            and   indid < 255)
   drop index MemberPayment.MemberPayment2_FK
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('MemberPayment')
            and   name  = 'MemberPayment_FK'
            and   indid > 0
            and   indid < 255)
   drop index MemberPayment.MemberPayment_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('MemberPayment')
            and   type = 'U')
   drop table MemberPayment
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Notification')
            and   type = 'U')
   drop table Notification
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Package')
            and   type = 'U')
   drop table Package
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Payment')
            and   type = 'U')
   drop table Payment
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Role')
            and   type = 'U')
   drop table Role
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Staff')
            and   type = 'U')
   drop table Staff
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Trainer')
            and   type = 'U')
   drop table Trainer
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('TrainingSchedule')
            and   name  = 'TrainingSchedule2_FK'
            and   indid > 0
            and   indid < 255)
   drop index TrainingSchedule.TrainingSchedule2_FK
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('TrainingSchedule')
            and   name  = 'TrainingSchedule_FK'
            and   indid > 0
            and   indid < 255)
   drop index TrainingSchedule.TrainingSchedule_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('TrainingSchedule')
            and   type = 'U')
   drop table TrainingSchedule
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('"User"')
            and   name  = 'include_FK'
            and   indid > 0
            and   indid < 255)
   drop index "User".include_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('"User"')
            and   type = 'U')
   drop table "User"
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('UserNotification')
            and   name  = 'UserNotification2_FK'
            and   indid > 0
            and   indid < 255)
   drop index UserNotification.UserNotification2_FK
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('UserNotification')
            and   name  = 'UserNotification_FK'
            and   indid > 0
            and   indid < 255)
   drop index UserNotification.UserNotification_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('UserNotification')
            and   type = 'U')
   drop table UserNotification
go

/*==============================================================*/
/* Table: Member                                                */
/*==============================================================*/
create table Member (
   memberId             int                  not null,
   fullName             text                 null,
   dateOfBirth          datetime             null,
   sex                  bit                  null,
   phone                text                 null,
   address              text                 null,
   createDate           datetime             null,
   constraint PK_MEMBER primary key nonclustered (memberId)
)
go

/*==============================================================*/
/* Table: MemberPakage                                          */
/*==============================================================*/
create table MemberPakage (
   memberId             int                  not null,
   packageId            int                  not null,
   startDate            datetime             null,
   endDate              datetime             null,
   isPaid               bit                  null,
   isActive             bit                  null,
   constraint PK_MEMBERPAKAGE primary key (memberId, packageId)
)
go

/*==============================================================*/
/* Index: MemberPakage_FK                                       */
/*==============================================================*/
create index MemberPakage_FK on MemberPakage (
memberId ASC
)
go

/*==============================================================*/
/* Index: MemberPakage2_FK                                      */
/*==============================================================*/
create index MemberPakage2_FK on MemberPakage (
packageId ASC
)
go

/*==============================================================*/
/* Table: MemberPayment                                         */
/*==============================================================*/
create table MemberPayment (
   memberId             int                  not null,
   paymentId            int                  not null,
   staffId              int                  not null,
   paymentDate          datetime             null,
   constraint PK_MEMBERPAYMENT primary key (memberId, paymentId, staffId)
)
go

/*==============================================================*/
/* Index: MemberPayment_FK                                      */
/*==============================================================*/
create index MemberPayment_FK on MemberPayment (
memberId ASC
)
go

/*==============================================================*/
/* Index: MemberPayment2_FK                                     */
/*==============================================================*/
create index MemberPayment2_FK on MemberPayment (
paymentId ASC
)
go

/*==============================================================*/
/* Index: MemberPayment3_FK                                     */
/*==============================================================*/
create index MemberPayment3_FK on MemberPayment (
staffId ASC
)
go

/*==============================================================*/
/* Table: Notification                                          */
/*==============================================================*/
create table Notification (
   notificationId       int                  not null,
   title                text                 null,
   content              text                 null,
   createdAt            datetime             null,
   sendRole             text                 null,
   constraint PK_NOTIFICATION primary key nonclustered (notificationId)
)
go

/*==============================================================*/
/* Table: Package                                               */
/*==============================================================*/
create table Package (
   packageId            int                  not null,
   name                 text                 null,
   type                 text                 null,
   price                decimal(10,2)        null,
   durationInDays       int                  null,
   description          text                 null,
   constraint PK_PACKAGE primary key nonclustered (packageId)
)
go

/*==============================================================*/
/* Table: Payment                                               */
/*==============================================================*/
create table Payment (
   paymentId            int                  not null,
   amount               decimal(10,2)        null,
   method               text                 null,
   constraint PK_PAYMENT primary key nonclustered (paymentId)
)
go

/*==============================================================*/
/* Table: Role                                                  */
/*==============================================================*/
create table Role (
   roleId               int                  not null,
   roleName             text                 null,
   description          text                 null,
   constraint PK_ROLE primary key nonclustered (roleId)
)
go

/*==============================================================*/
/* Table: Staff                                                 */
/*==============================================================*/
create table Staff (
   staffId              int                  not null,
   fullName             text                 null,
   phone                text                 null,
   email                text                 null,
   workingSince         datetime             null,
   constraint PK_STAFF primary key nonclustered (staffId)
)
go

/*==============================================================*/
/* Table: Trainer                                               */
/*==============================================================*/
create table Trainer (
   trainerId            int                  not null,
   fullName             text                 null,
   phone                text                 null,
   specialty            text                 null,
   scheduleNote         text                 null,
   constraint PK_TRAINER primary key nonclustered (trainerId)
)
go

/*==============================================================*/
/* Table: TrainingSchedule                                      */
/*==============================================================*/
create table TrainingSchedule (
   trainerId            int                  not null,
   memberId             int                  not null,
   trainingDate         datetime             null,
   startTime            datetime             null,
   endTime              datetime             null,
   node                 text                 null,
   constraint PK_TRAININGSCHEDULE primary key (trainerId, memberId)
)
go

/*==============================================================*/
/* Index: TrainingSchedule_FK                                   */
/*==============================================================*/
create index TrainingSchedule_FK on TrainingSchedule (
trainerId ASC
)
go

/*==============================================================*/
/* Index: TrainingSchedule2_FK                                  */
/*==============================================================*/
create index TrainingSchedule2_FK on TrainingSchedule (
memberId ASC
)
go

/*==============================================================*/
/* Table: "User"                                                */
/*==============================================================*/
create table "User" (
   userId               int                  not null,
   roleId               int                  not null,
   userName             text                 null,
   password             text                 null,
   email                text                 null,
   referenceId          int                  null,
   status               char(10)             null,
   isAtive              bit                  null,
   constraint PK_USER primary key nonclustered (userId)
)
go

/*==============================================================*/
/* Index: include_FK                                            */
/*==============================================================*/
create index include_FK on "User" (
roleId ASC
)
go

/*==============================================================*/
/* Table: UserNotification                                      */
/*==============================================================*/
create table UserNotification (
   notificationId       int                  not null,
   userId               int                  not null,
   timeSend             datetime             null,
   seen                 bit                  null,
   constraint PK_USERNOTIFICATION primary key (notificationId, userId)
)
go

/*==============================================================*/
/* Index: UserNotification_FK                                   */
/*==============================================================*/
create index UserNotification_FK on UserNotification (
notificationId ASC
)
go

/*==============================================================*/
/* Index: UserNotification2_FK                                  */
/*==============================================================*/
create index UserNotification2_FK on UserNotification (
userId ASC
)
go

alter table MemberPakage
   add constraint FK_MEMBERPA_MEMBERPAK_MEMBER foreign key (memberId)
      references Member (memberId)
go

alter table MemberPakage
   add constraint FK_MEMBERPA_MEMBERPAK_PACKAGE foreign key (packageId)
      references Package (packageId)
go

alter table MemberPayment
   add constraint FK_MEMBERPA_MEMBERPAY_MEMBER foreign key (memberId)
      references Member (memberId)
go

alter table MemberPayment
   add constraint FK_MEMBERPA_MEMBERPAY_PAYMENT foreign key (paymentId)
      references Payment (paymentId)
go

alter table MemberPayment
   add constraint FK_MEMBERPA_MEMBERPAY_STAFF foreign key (staffId)
      references Staff (staffId)
go

alter table TrainingSchedule
   add constraint FK_TRAINING_TRAININGS_TRAINER foreign key (trainerId)
      references Trainer (trainerId)
go

alter table TrainingSchedule
   add constraint FK_TRAINING_TRAININGS_MEMBER foreign key (memberId)
      references Member (memberId)
go

alter table "User"
   add constraint FK_USER_INCLUDE_ROLE foreign key (roleId)
      references Role (roleId)
go

alter table UserNotification
   add constraint FK_USERNOTI_USERNOTIF_NOTIFICA foreign key (notificationId)
      references Notification (notificationId)
go

alter table UserNotification
   add constraint FK_USERNOTI_USERNOTIF_USER foreign key (userId)
      references "User" (userId)
go

